using System;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.DTOs.Operations.Responses;

public class ImportacionMasivaDto
{
    public Guid ImportacionId { get; set; }
    public Guid UsuarioId { get; set; }
    public string TipoImportacion { get; set; } = string.Empty;
    public PlataformaOrigen? PlataformaOrigen { get; set; }
    public string ArchivoUrl { get; set; } = string.Empty;
    public string NombreArchivoOriginal { get; set; } = string.Empty;
    public string? PlantillaUtilizada { get; set; }
    public long? TamanioBytes { get; set; }
    public EstadoImportacion Estado { get; set; }
    public int TotalRegistros { get; set; }
    public int RegistrosExitosos { get; set; }
    public int RegistrosConError { get; set; }
    public string? ReporteErroresUrl { get; set; }
    public string DetalleErrores { get; set; } = "{}";
    public DateTimeOffset? FechaInicioProcesamiento { get; set; }
    public DateTimeOffset? FechaFinProcesamiento { get; set; }
}
