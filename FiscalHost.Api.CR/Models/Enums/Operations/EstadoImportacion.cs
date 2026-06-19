using NpgsqlTypes;

namespace FiscalHost.Api.CR.Models.Enums.Operations;

public enum EstadoImportacion
{
    [PgName("PENDIENTE")]
    PENDIENTE,
    [PgName("PROCESANDO")]
    PROCESANDO,
    [PgName("COMPLETADO")]
    COMPLETADO,
    [PgName("COMPLETADO_PARCIAL")]
    COMPLETADO_PARCIAL,
    [PgName("RECHAZADO")]
    RECHAZADO
}

