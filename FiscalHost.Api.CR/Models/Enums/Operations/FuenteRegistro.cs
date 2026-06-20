using NpgsqlTypes;

namespace FiscalHost.Api.CR.Models.Enums.Operations;

// ========================================================================
// Enum: Define un catálogo estricto de opciones válidas para la base de datos,
// lo cual evita errores tipográficos al impedir que se guarde un estado
// no contemplado.
// ========================================================================


public enum FuenteRegistro
{
    [PgName("IMPORTACION_CSV")]
    IMPORTACION_CSV,
    [PgName("MANUAL")]
    MANUAL,
    [PgName("RECONSTRUCCION")]
    RECONSTRUCCION
}


