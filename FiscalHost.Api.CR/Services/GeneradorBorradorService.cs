using System;
using System.Linq;
using System.Threading.Tasks;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;
using FiscalHost.Api.CR.Models.Enums.Operations;
using FiscalHost.Api.CR.Repositories;

namespace FiscalHost.Api.CR.Services;

public interface IGeneradorBorradorService
{
    Task<BorradorD104Dto> GenerarD104Async(Guid usuarioId, short anio, short mes);
    Task<BorradorD125Dto> GenerarD125Async(Guid usuarioId, short anio, bool regimenUtilidades = false);
    BorradorD176Dto GenerarD176(decimal impuestoPrincipal, DateOnly fechaVencimientoOriginal, DateOnly fechaActual);
}

public class GeneradorBorradorService(IGeneradorBorradorRepository repository) : IGeneradorBorradorService
{
    private async Task ValidarIntegridadDatosAsync(Guid usuarioId, short anio, short? mes = null)
    {
        int reservasPendientes = await repository.ContarReservasSinClasificarAsync(usuarioId, anio, mes);
        int gastosPendientes = await repository.ContarGastosPendientesAsync(usuarioId, anio, mes);
        int totalPendientes = reservasPendientes + gastosPendientes;

        if (totalPendientes > 0)
        {
            throw new InvalidOperationException($"No se puede generar el borrador porque hay {totalPendientes} registros (reservas o gastos) sin clasificar. Complete la información.");
        }
    }

    public async Task<BorradorD104Dto> GenerarD104Async(Guid usuarioId, short anio, short mes)
    {
        await ValidarIntegridadDatosAsync(usuarioId, anio, mes);

        var reservas = await repository.ObtenerReservasAsync(usuarioId, anio, mes);
        var gastos = await repository.ObtenerGastosAsync(usuarioId, anio, mes);

        // Paso 1: IVA Cobrado
        decimal totalIngresosGravados = reservas
            .Where(r => r.ClasificacionFiscal == ClasificacionFiscal.GRAVADO || r.ClasificacionFiscal == ClasificacionFiscal.GRAVADO_CON_RETENCION)
            .Sum(r => r.MontoBruto);

        decimal ivaCobrado = Math.Round(totalIngresosGravados * 0.13m, 2);

        // Paso 2: IVA Pagado
        decimal ivaCreditoFiscal = gastos
            .Where(g => g.EstadoValidacion == EstadoValidacion.VALIDO && g.EsCreditoFiscalValido)
            .Sum(g => g.MontoIvaSoportado);

        // Paso 3: Liquidación
        decimal ivaNeto = ivaCobrado - ivaCreditoFiscal;
        bool esSaldoAFavor = ivaNeto < 0;

        return new BorradorD104Dto
        {
            TotalIngresosGravados = totalIngresosGravados,
            IvaCobrado = ivaCobrado,
            IvaCreditoFiscal = ivaCreditoFiscal,
            IvaNeto = ivaNeto,
            EsSaldoAFavor = esSaldoAFavor,
            MensajeValidacion = "Borrador generado exitosamente. Listo para OVi."
        };
    }

    public async Task<BorradorD125Dto> GenerarD125Async(Guid usuarioId, short anio, bool regimenUtilidades = false)
    {
        await ValidarIntegridadDatosAsync(usuarioId, anio);

        var reservas = await repository.ObtenerReservasAsync(usuarioId, anio);
        var gastos = await repository.ObtenerGastosAsync(usuarioId, anio);

        decimal ingresoBrutoAnual = reservas.Sum(r => r.MontoBruto);
        decimal baseImponible;

        if (regimenUtilidades)
        {
            decimal gastosDeducibles = gastos
                .Where(g => g.EstadoValidacion == EstadoValidacion.VALIDO && g.EsDeducibleRenta)
                .Sum(g => g.MontoTotal); // Usamos MontoTotal o MontoNeto según necesidad, asumiremos MontoTotal (gasto real)
            
            baseImponible = ingresoBrutoAnual - gastosDeducibles;
        }
        else
        {
            // Capital Inmobiliario por defecto (15% deducción fija, por lo que la base es 85%)
            baseImponible = ingresoBrutoAnual * 0.85m;
        }

        if (baseImponible < 0) baseImponible = 0;

        decimal impuestoRenta = Math.Round(baseImponible * 0.15m, 2);
        decimal retencionesExtranjeras = reservas.Sum(r => r.RetencionExtranjera);
        decimal impuestoNeto = impuestoRenta - retencionesExtranjeras;

        return new BorradorD125Dto
        {
            IngresoBrutoAnual = ingresoBrutoAnual,
            BaseImponible = Math.Round(baseImponible, 2),
            ImpuestoRenta = impuestoRenta,
            RetencionesExtranjeras = retencionesExtranjeras,
            ImpuestoNeto = impuestoNeto,
            MensajeValidacion = "Borrador generado exitosamente. Listo para OVi."
        };
    }

    public BorradorD176Dto GenerarD176(decimal impuestoPrincipal, DateOnly fechaVencimientoOriginal, DateOnly fechaActual)
    {
        if (fechaActual <= fechaVencimientoOriginal)
        {
            return new BorradorD176Dto
            {
                ImpuestoPrincipal = impuestoPrincipal,
                MultaBase = 0,
                MultaReducida = 0,
                InteresesMora = 0,
                TotalAPagar = impuestoPrincipal,
                MensajeValidacion = "El pago se realiza en tiempo. No hay multas ni intereses."
            };
        }

        // Cálculos de mora y multa
        int mesesDeAtraso = (fechaActual.Year - fechaVencimientoOriginal.Year) * 12 + fechaActual.Month - fechaVencimientoOriginal.Month;
        if (fechaActual.Day < fechaVencimientoOriginal.Day) mesesDeAtraso--; // Ajuste de meses completos
        
        if (mesesDeAtraso < 0) mesesDeAtraso = 0;
        // Si hay al menos un día de atraso pero no cumple el mes, se cobra como un mes o fracción
        if (mesesDeAtraso == 0 && fechaActual > fechaVencimientoOriginal) mesesDeAtraso = 1;

        // Multa por omisión/atraso
        decimal multaBase = mesesDeAtraso * 231100m;
        decimal topeMulta = 1386600m; // 3 salarios base (3 * 462,200)
        
        if (multaBase > topeMulta) multaBase = topeMulta;

        // Reducción del 80% (el usuario paga solo 20%)
        decimal multaReducida = Math.Round(multaBase * 0.20m, 2);

        // Intereses 1% mensual (proporcional diario)
        // Días de atraso
        int diasAtraso = fechaActual.DayNumber - fechaVencimientoOriginal.DayNumber;
        
        // Asumiendo meses de 30 días o año de 365 días
        decimal tasaDiaria = 0.01m / 30m; // 1% mensual
        decimal interesesMora = Math.Round(impuestoPrincipal * tasaDiaria * diasAtraso, 2);

        // Tope de intereses: no exceder el 20% del monto principal
        decimal topeIntereses = impuestoPrincipal * 0.20m;
        if (interesesMora > topeIntereses)
        {
            interesesMora = topeIntereses;
        }

        return new BorradorD176Dto
        {
            ImpuestoPrincipal = impuestoPrincipal,
            MultaBase = multaBase,
            MultaReducida = multaReducida,
            InteresesMora = interesesMora,
            TotalAPagar = impuestoPrincipal + multaReducida + interesesMora,
            MensajeValidacion = "Cálculos de sanciones aplicados con reducción del Art. 88."
        };
    }
}
