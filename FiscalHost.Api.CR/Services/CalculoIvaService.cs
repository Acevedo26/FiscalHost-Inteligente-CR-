using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence;
using FiscalHost.Api.CR.Models.Entities.TaxIntelligence;
using FiscalHost.Api.CR.Models.Enums.TaxIntelligence;
using System.Text.Json;

namespace FiscalHost.Api.CR.Services;

public class CalculoIvaService : ICalculoIvaService
{
    private readonly AppDbContext _context;

    public CalculoIvaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CalculoIvaResponseDto> CalcularIvaDevengadoAsync(Guid usuarioId, short anio, short mes)
    {
        // 1. Validar registros incompletos
        // Reservas en revisión o pendientes
        var reservasIncompletas = await _context.Reservas
            .Where(r => r.UsuarioId == usuarioId && r.PeriodoFiscalAnio == anio && r.PeriodoFiscalMes == mes)
            .Where(r => r.Estado == "EN_REVISION" || r.Estado == "PENDIENTE")
            .AnyAsync();

        // Gastos pendientes de validación
        var gastosIncompletos = await _context.Gastos
            .Where(g => g.UsuarioId == usuarioId && g.PeriodoFiscalAnio == anio && g.PeriodoFiscalMes == mes)
            .Where(g => g.EstadoValidacion == FiscalHost.Api.CR.Models.Enums.Operations.EstadoValidacion.PENDIENTE)
            .AnyAsync();

        if (reservasIncompletas || gastosIncompletos)
        {
            throw new InvalidOperationException("Existen registros incompletos o pendientes de validación para el periodo seleccionado. No se puede generar el cálculo.");
        }

        // 2. Obtener periodo fiscal
        var periodoFiscal = await _context.PeriodosFiscales
            .FirstOrDefaultAsync(p => p.Anio == anio && p.Mes == mes);

        if (periodoFiscal == null)
        {
            throw new InvalidOperationException("No se encontró el periodo fiscal especificado.");
        }

        // 3. Cálculos de Ingresos y Débito
        // Se consideran las reservas confirmadas/completadas
        var reservas = await _context.Reservas
            .Where(r => r.UsuarioId == usuarioId && r.PeriodoFiscalAnio == anio && r.PeriodoFiscalMes == mes)
            .Where(r => r.Estado != "EN_REVISION" && r.Estado != "PENDIENTE" && r.Estado != "RECHAZADO" && r.Estado != "CANCELADO")
            .ToListAsync();

        decimal totalIngresosBrutos = reservas.Sum(r => r.MontoColones);
        decimal totalIngresosGravados = reservas.Sum(r => r.MontoGravado);
        decimal totalIngresosExentos = reservas.Sum(r => r.MontoExento);
        decimal debitoFiscal = reservas.Sum(r => r.MontoIvaCalculado);

        // 4. Cálculo de Crédito
        var gastos = await _context.Gastos
            .Where(g => g.UsuarioId == usuarioId && g.PeriodoFiscalAnio == anio && g.PeriodoFiscalMes == mes)
            .Where(g => g.EstadoValidacion == FiscalHost.Api.CR.Models.Enums.Operations.EstadoValidacion.VALIDO && g.EsCreditoFiscalValido)
            .ToListAsync();

        decimal creditoFiscal = gastos.Sum(g => g.MontoIvaSoportado);

        // 5. Saldo a favor anterior
        decimal saldoFavorAnterior = 0;
        
        var fechaMesAnterior = new DateTime(anio, mes, 1).AddMonths(-1);
        short anioAnterior = (short)fechaMesAnterior.Year;
        short mesAnterior = (short)fechaMesAnterior.Month;

        var calculoAnterior = await _context.CalculosFiscales
            .Where(c => c.UsuarioId == usuarioId && 
                        c.TipoFormulario == TipoFormulario.D104 &&
                        c.Periodo.Anio == anioAnterior &&
                        c.Periodo.Mes == mesAnterior)
            .OrderByDescending(c => c.CreatedAt)
            .FirstOrDefaultAsync();

        if (calculoAnterior != null)
        {
            saldoFavorAnterior = calculoAnterior.SaldoFavorResultante;
        }

        // 6. IVA Neto y Saldo a Favor Nuevo
        decimal ivaNeto = debitoFiscal - creditoFiscal - saldoFavorAnterior;
        decimal saldoFavorResultante = 0;
        decimal montoTotalAPagar = 0;

        if (ivaNeto < 0)
        {
            saldoFavorResultante = Math.Abs(ivaNeto);
            ivaNeto = 0;
        }
        else
        {
            montoTotalAPagar = ivaNeto;
        }

        // 7. Guardar en BD
        var calculoExistente = await _context.CalculosFiscales
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId && c.PeriodoId == periodoFiscal.PeriodoId && c.TipoFormulario == TipoFormulario.D104);

        var detalle = new 
        {
            ReservasProcesadas = reservas.Count,
            GastosProcesados = gastos.Count,
            CalculadoEn = DateTimeOffset.UtcNow
        };

        if (calculoExistente != null)
        {
            calculoExistente.TotalIngresosBrutos = totalIngresosBrutos;
            calculoExistente.TotalIngresosGravados = totalIngresosGravados;
            calculoExistente.TotalIngresosExentos = totalIngresosExentos;
            calculoExistente.DebitoFiscal = debitoFiscal;
            calculoExistente.CreditoFiscal = creditoFiscal;
            calculoExistente.SaldoFavorAnterior = saldoFavorAnterior;
            calculoExistente.IvaNeto = ivaNeto;
            calculoExistente.SaldoFavorResultante = saldoFavorResultante;
            calculoExistente.MontoTotalAPagar = montoTotalAPagar;
            calculoExistente.Estado = EstadoDeclaracion.CALCULADO;
            calculoExistente.UpdatedAt = DateTimeOffset.UtcNow;
            calculoExistente.DetalleCalculo = JsonSerializer.Serialize(detalle);
            calculoExistente.BorradorGenerado = true;
        }
        else
        {
            var nuevoCalculo = new CalculoFiscal
            {
                CalculoId = Guid.NewGuid(),
                UsuarioId = usuarioId,
                PeriodoId = periodoFiscal.PeriodoId,
                TipoFormulario = TipoFormulario.D104,
                Estado = EstadoDeclaracion.CALCULADO,
                TotalIngresosBrutos = totalIngresosBrutos,
                TotalIngresosGravados = totalIngresosGravados,
                TotalIngresosExentos = totalIngresosExentos,
                DebitoFiscal = debitoFiscal,
                CreditoFiscal = creditoFiscal,
                SaldoFavorAnterior = saldoFavorAnterior,
                IvaNeto = ivaNeto,
                SaldoFavorResultante = saldoFavorResultante,
                MontoTotalAPagar = montoTotalAPagar,
                BorradorGenerado = true,
                FechaGeneracionBorrador = DateTimeOffset.UtcNow,
                DetalleCalculo = JsonSerializer.Serialize(detalle)
            };
            
            _context.CalculosFiscales.Add(nuevoCalculo);
            calculoExistente = nuevoCalculo;
        }

        await _context.SaveChangesAsync();

        return new CalculoIvaResponseDto
        {
            CalculoId = calculoExistente.CalculoId,
            PeriodoId = calculoExistente.PeriodoId,
            TotalIngresosBrutos = calculoExistente.TotalIngresosBrutos,
            TotalIngresosGravados = calculoExistente.TotalIngresosGravados,
            TotalIngresosExentos = calculoExistente.TotalIngresosExentos,
            DebitoFiscal = calculoExistente.DebitoFiscal,
            CreditoFiscal = calculoExistente.CreditoFiscal,
            SaldoFavorAnterior = calculoExistente.SaldoFavorAnterior,
            IvaNeto = calculoExistente.IvaNeto,
            SaldoFavorResultante = calculoExistente.SaldoFavorResultante,
            MontoTotalAPagar = calculoExistente.MontoTotalAPagar,
            Estado = calculoExistente.Estado.ToString(),
            DetalleCalculo = calculoExistente.DetalleCalculo,
            CreatedAt = calculoExistente.CreatedAt
        };
    }
}
