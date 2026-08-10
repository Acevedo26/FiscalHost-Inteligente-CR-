using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;
using FiscalHost.Api.CR.Models.Entities.TaxIntelligence;
using FiscalHost.Api.CR.Repositories;

namespace FiscalHost.Api.CR.Services;

public class SimulacionFiscalService : ISimulacionFiscalService
{
    private readonly ISimulacionFiscalRepository _repository;

    public SimulacionFiscalService(ISimulacionFiscalRepository repository)
    {
        _repository = repository;
    }

    public async Task<SimulacionFiscalResponseDto> CrearSimulacionAsync(Guid usuarioId, CreateSimulacionFiscalRequest request)
    {
        if (request.Parametros.IngresosEstimados < 0 || request.Parametros.GastosProyectados < 0)
        {
            throw new ArgumentException("Los ingresos y gastos no pueden ser negativos.");
        }

        var resultados = CalcularResultados(request.Parametros);

        var simulacion = new SimulacionFiscal
        {
            SimulacionId = Guid.NewGuid(),
            UsuarioId = usuarioId,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            PeriodoBaseAnio = request.PeriodoBaseAnio,
            PeriodoBaseMes = request.PeriodoBaseMes,
            ParametrosEntrada = JsonSerializer.Serialize(request.Parametros),
            Resultados = JsonSerializer.Serialize(resultados)
        };

        await _repository.CreateAsync(simulacion);

        return MapearADto(simulacion);
    }

    public async Task<SimulacionFiscalResponseDto> ObtenerSimulacionAsync(Guid simulacionId, Guid usuarioId)
    {
        var simulacion = await _repository.GetByIdAsync(simulacionId, usuarioId);
        if (simulacion == null)
            throw new KeyNotFoundException("Simulación no encontrada.");

        return MapearADto(simulacion);
    }

    public async Task<IEnumerable<SimulacionFiscalResponseDto>> ListarSimulacionesAsync(Guid usuarioId)
    {
        var simulaciones = await _repository.GetAllByUsuarioIdAsync(usuarioId);
        return simulaciones.Select(MapearADto);
    }

    public async Task<ComparacionSimulacionesResponseDto> CompararSimulacionesAsync(Guid usuarioId, List<Guid> simulacionIds)
    {
        if (simulacionIds == null || !simulacionIds.Any())
            throw new ArgumentException("Debe proporcionar al menos un ID de simulación para comparar.");

        if (simulacionIds.Count > 3)
            throw new ArgumentException("Solo se permite comparar un máximo de 3 escenarios simultáneamente.");

        var response = new ComparacionSimulacionesResponseDto();
        foreach (var id in simulacionIds)
        {
            var simulacion = await _repository.GetByIdAsync(id, usuarioId);
            if (simulacion != null)
            {
                response.Simulaciones.Add(MapearADto(simulacion));
            }
            else
            {
                response.Advertencias.Add($"Simulación con ID {id} no encontrada.");
            }
        }

        return response;
    }

    public async Task<byte[]> ExportarSimulacionCsvAsync(Guid simulacionId, Guid usuarioId)
    {
        var simulacion = await ObtenerSimulacionAsync(simulacionId, usuarioId);

        var sb = new StringBuilder();
        sb.AppendLine("Campo,Valor");
        sb.AppendLine($"Nombre,{simulacion.Nombre}");
        sb.AppendLine($"Descripción,{simulacion.Descripcion}");
        sb.AppendLine($"Periodo,{simulacion.PeriodoBaseAnio}-{simulacion.PeriodoBaseMes}");
        sb.AppendLine($"Ingresos Estimados,{simulacion.Parametros.IngresosEstimados}");
        sb.AppendLine($"Gastos Proyectados,{simulacion.Parametros.GastosProyectados}");
        sb.AppendLine($"IVA Estimado,{simulacion.Resultados.IvaEstimado}");
        sb.AppendLine($"Renta Estimada,{simulacion.Resultados.RentaEstimada}");
        sb.AppendLine($"Total Impuestos,{simulacion.Resultados.TotalImpuestosEstimados}");
        sb.AppendLine($"Ahorro Fiscal Esperado,{simulacion.Resultados.AhorroFiscalEsperado}");

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    public async Task EliminarSimulacionAsync(Guid simulacionId, Guid usuarioId)
    {
        var simulacion = await _repository.GetByIdAsync(simulacionId, usuarioId);
        if (simulacion == null)
            throw new KeyNotFoundException("Simulación no encontrada.");

        await _repository.DeleteAsync(simulacion);
    }

    private SimulacionResultadosDto CalcularResultados(SimulacionParametrosDto parametros)
    {
        // Reglas de negocio simplificadas para la simulación
        decimal ivaEstimado = (parametros.IngresosEstimados - parametros.GastosProyectados) * 0.13m;
        if (ivaEstimado < 0) ivaEstimado = 0;

        decimal utilidad = parametros.IngresosEstimados - parametros.GastosProyectados;
        decimal rentaEstimada = 0;
        
        if (utilidad > 0)
        {
            rentaEstimada = utilidad * 0.15m; // Tasa simplificada del 15%
        }

        decimal ahorroFiscalEsperado = parametros.GastosProyectados * (0.13m + 0.15m); // Ahorro sobre IVA y Renta por gastos

        return new SimulacionResultadosDto
        {
            IvaEstimado = ivaEstimado,
            RentaEstimada = rentaEstimada,
            TotalImpuestosEstimados = ivaEstimado + rentaEstimada,
            AhorroFiscalEsperado = ahorroFiscalEsperado
        };
    }

    private SimulacionFiscalResponseDto MapearADto(SimulacionFiscal simulacion)
    {
        return new SimulacionFiscalResponseDto
        {
            SimulacionId = simulacion.SimulacionId,
            UsuarioId = simulacion.UsuarioId,
            Nombre = simulacion.Nombre,
            Descripcion = simulacion.Descripcion,
            PeriodoBaseAnio = simulacion.PeriodoBaseAnio,
            PeriodoBaseMes = simulacion.PeriodoBaseMes,
            Parametros = string.IsNullOrEmpty(simulacion.ParametrosEntrada) 
                ? new SimulacionParametrosDto() 
                : JsonSerializer.Deserialize<SimulacionParametrosDto>(simulacion.ParametrosEntrada) ?? new SimulacionParametrosDto(),
            Resultados = string.IsNullOrEmpty(simulacion.Resultados) 
                ? new SimulacionResultadosDto() 
                : JsonSerializer.Deserialize<SimulacionResultadosDto>(simulacion.Resultados) ?? new SimulacionResultadosDto()
        };
    }
}
