using NpgsqlTypes;

namespace FiscalHost.Api.CR.Models.Enums.Operations;

// ========================================================================
// Enum: Define un catálogo estricto de opciones válidas para la base de datos,
// lo cual evita errores tipográficos al impedir que se guarde un estado
// no contemplado.
// ========================================================================


public enum EstadoValidacion
{
    [PgName("PENDIENTE")]
    PENDIENTE,
    [PgName("VALIDO")]
    VALIDO,
    [PgName("RECHAZADO")]
    RECHAZADO,
    [PgName("DUPLICADO")]
    DUPLICADO
}


