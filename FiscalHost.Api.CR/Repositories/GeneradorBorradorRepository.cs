using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Models.Entities.Operations;

namespace FiscalHost.Api.CR.Repositories;

public interface IGeneradorBorradorRepository
{
    Task<int> ContarReservasSinClasificarAsync(Guid usuarioId, short anio, short? mes = null);
    Task<int> ContarGastosPendientesAsync(Guid usuarioId, short anio, short? mes = null);
    Task<List<Reserva>> ObtenerReservasAsync(Guid usuarioId, short anio, short? mes = null);
    Task<List<Gasto>> ObtenerGastosAsync(Guid usuarioId, short anio, short? mes = null);
}

public class GeneradorBorradorRepository(AppDbContext context) : IGeneradorBorradorRepository
{
    public async Task<int> ContarReservasSinClasificarAsync(Guid usuarioId, short anio, short? mes = null)
    {
        var query = context.Reservas.Where(r => r.UsuarioId == usuarioId && r.PeriodoFiscalAnio == anio);
        if (mes.HasValue)
        {
            query = query.Where(r => r.PeriodoFiscalMes == mes.Value);
        }
        
        // Asumimos que "Sin clasificar" es cuando Estado o metadata indique pendiente, o cuando ClasificacionFiscal no ha sido validada.
        // Nos basamos en Estado == "PENDIENTE" o ClasificacionFiscal == null si aplica.
        // Segun Reserva.cs Estado es string, asumamos que "PENDIENTE" es el valor para no clasificadas.
        return await query.CountAsync(r => r.Estado == "PENDIENTE");
    }

    public async Task<int> ContarGastosPendientesAsync(Guid usuarioId, short anio, short? mes = null)
    {
        var query = context.Gastos.Where(g => g.UsuarioId == usuarioId && g.PeriodoFiscalAnio == anio);
        if (mes.HasValue)
        {
            query = query.Where(g => g.PeriodoFiscalMes == mes.Value);
        }

        return await query.CountAsync(g => g.EstadoValidacion == Models.Enums.Operations.EstadoValidacion.PENDIENTE);
    }

    public async Task<List<Reserva>> ObtenerReservasAsync(Guid usuarioId, short anio, short? mes = null)
    {
        var query = context.Reservas.Where(r => r.UsuarioId == usuarioId && r.PeriodoFiscalAnio == anio);
        if (mes.HasValue)
        {
            query = query.Where(r => r.PeriodoFiscalMes == mes.Value);
        }
        return await query.ToListAsync();
    }

    public async Task<List<Gasto>> ObtenerGastosAsync(Guid usuarioId, short anio, short? mes = null)
    {
        var query = context.Gastos.Where(g => g.UsuarioId == usuarioId && g.PeriodoFiscalAnio == anio);
        if (mes.HasValue)
        {
            query = query.Where(g => g.PeriodoFiscalMes == mes.Value);
        }
        return await query.ToListAsync();
    }
}
