using NpgsqlTypes;

namespace FiscalHost.Api.CR.Models.Enums.Operations;

// ========================================================================
// Enum: Define un catálogo estricto de opciones válidas para la base de datos,
// lo cual evita errores tipográficos al impedir que se guarde un estado
// no contemplado.
// ========================================================================


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


