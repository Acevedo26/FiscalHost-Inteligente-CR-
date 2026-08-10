using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FiscalHost.Api.CR.Models.DTOs.Communication.Responses;

namespace FiscalHost.Api.CR.Services;

public interface IAlertaService
{
	Task GenerarAlertasVencimientoAsync(DateOnly fechaActual);
	Task EnviarAlertasPendientesAsync(DateTimeOffset fechaCorte);
	Task<AlertaDto?> MarcarComoLeidaAsync(Guid alertaId);
	Task<IEnumerable<AlertaDto>> ListarPorUsuarioAsync(Guid usuarioId, bool soloNoLeidas);
}