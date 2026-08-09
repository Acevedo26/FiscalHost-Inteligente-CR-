using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Models.DTOs.Dashboard;

namespace FiscalHost.Api.CR.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardResponseDto> GetDashboardDataAsync(Guid usuarioId, DateTime fechaInicio, DateTime fechaFin)
    {
        var response = new DashboardResponseDto();

        // Npgsql requiere que los DateTimes sean UTC al consultar columnas 'timestamp with time zone'
        var fechaInicioUtc = fechaInicio.Kind == DateTimeKind.Unspecified 
            ? DateTime.SpecifyKind(fechaInicio, DateTimeKind.Utc) 
            : fechaInicio.ToUniversalTime();

        var fechaFinUtc = fechaFin.Kind == DateTimeKind.Unspecified 
            ? DateTime.SpecifyKind(fechaFin, DateTimeKind.Utc) 
            : fechaFin.ToUniversalTime();

        // 1. Fetch data with LINQ
        var reservas = await _context.Reservas
            .Where(r => r.UsuarioId == usuarioId && r.FechaInicio >= fechaInicioUtc && r.FechaInicio <= fechaFinUtc && r.Estado != "Cancelada")
            .ToListAsync();

        var gastos = await _context.Gastos
            .Where(g => g.UsuarioId == usuarioId && 
                        g.FechaEmision >= DateOnly.FromDateTime(fechaInicio) && 
                        g.FechaEmision <= DateOnly.FromDateTime(fechaFin) && 
                        g.DeletedAt == null)
            .ToListAsync();

        if (!reservas.Any() && !gastos.Any())
        {
            response.TieneDatos = false;
            response.RiesgoFiscal.NivelRiesgo = "Alto";
            response.RiesgoFiscal.Factores.Add("No hay datos de ingresos ni gastos registrados en este período.");
            return response;
        }

        response.TieneDatos = true;

        // Metricas globales
        decimal ingresosBrutos = reservas.Sum(r => r.MontoColones);
        decimal ivaRecaudado = reservas.Sum(r => r.MontoIvaCalculado);
        decimal ivaSoportadoValido = gastos.Where(g => g.EsCreditoFiscalValido).Sum(g => g.MontoIvaSoportado);
        decimal gastosTotales = gastos.Sum(g => g.MontoColones);

        // Impuestos estimados = IVA Neto + Estimado Renta (15% sobre utilidad asumiendo régimen tradicional básico)
        decimal ivaNeto = Math.Max(0, ivaRecaudado - ivaSoportadoValido);
        decimal utilidad = Math.Max(0, ingresosBrutos - gastosTotales);
        decimal rentaEstimada = utilidad * 0.15m;
        
        response.Metricas.IngresosBrutos = ingresosBrutos;
        response.Metricas.ImpuestosEstimados = ivaNeto + rentaEstimada;
        response.Metricas.IngresosNetos = ingresosBrutos - response.Metricas.ImpuestosEstimados - gastosTotales;

        // Evaluar Riesgo
        if (ingresosBrutos > 0 && gastosTotales == 0)
        {
            response.RiesgoFiscal.NivelRiesgo = "Alto";
            response.RiesgoFiscal.Factores.Add("Se han registrado ingresos pero ningún gasto. Esto puede generar un pago de impuestos irrealmente alto o indicar omisión de datos.");
        }
        else if (gastos.Any() && gastos.Count(g => !g.EsCreditoFiscalValido) > gastos.Count * 0.5)
        {
            response.RiesgoFiscal.NivelRiesgo = "Medio";
            response.RiesgoFiscal.Factores.Add("Más del 50% de los gastos no son créditos fiscales válidos. Revise la justificación de sus gastos.");
        }
        else if (gastosTotales > ingresosBrutos * 1.5m && ingresosBrutos > 0)
        {
            response.RiesgoFiscal.NivelRiesgo = "Medio";
            response.RiesgoFiscal.Factores.Add("Sus gastos declarados superan significativamente sus ingresos, lo cual puede ser objeto de revisión por Hacienda.");
        }
        else
        {
            response.RiesgoFiscal.NivelRiesgo = "Bajo";
            response.RiesgoFiscal.Factores.Add("Sus proporciones de ingresos y gastos parecen normales y bien equilibradas.");
        }

        // Evolución mensual usando LINQ
        int months = 1 + (fechaFin.Month - fechaInicio.Month) + 12 * (fechaFin.Year - fechaInicio.Year);
        if (months > 0)
        {
            var periodos = Enumerable.Range(0, months)
                .Select(offset => fechaInicio.AddMonths(offset))
                .Select(d => new { d.Year, d.Month })
                .ToList();

            foreach (var periodo in periodos)
            {
                var reservasMes = reservas.Where(r => r.FechaInicio.Year == periodo.Year && r.FechaInicio.Month == periodo.Month).ToList();
                var gastosMes = gastos.Where(g => g.FechaEmision.Year == periodo.Year && g.FechaEmision.Month == periodo.Month).ToList();

                decimal mesIngresos = reservasMes.Sum(r => r.MontoColones);
                decimal mesIvaRecaudado = reservasMes.Sum(r => r.MontoIvaCalculado);
                decimal mesIvaSoportado = gastosMes.Where(g => g.EsCreditoFiscalValido).Sum(g => g.MontoIvaSoportado);
                decimal mesGastos = gastosMes.Sum(g => g.MontoColones);

                decimal mesIvaNeto = Math.Max(0, mesIvaRecaudado - mesIvaSoportado);
                decimal mesUtilidad = Math.Max(0, mesIngresos - mesGastos);
                decimal mesRenta = mesUtilidad * 0.15m;
                decimal mesImpuestos = mesIvaNeto + mesRenta;
                
                response.EvolucionMensual.Add(new DashboardEvolutionDto
                {
                    Anio = periodo.Year,
                    Mes = periodo.Month,
                    IngresosBrutos = mesIngresos,
                    ImpuestosEstimados = mesImpuestos,
                    IngresosNetos = mesIngresos - mesImpuestos - mesGastos
                });
            }
        }

        return response;
    }
}
