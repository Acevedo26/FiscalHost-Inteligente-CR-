using NpgsqlTypes;

namespace FiscalHost.Api.CR.Models.Enums.Identity;

// ========================================================================
// Enum: Define un catálogo estricto de opciones válidas para la base de datos,
// lo cual evita errores tipográficos al impedir que se guarde un estado
// no contemplado.
// ========================================================================


public enum TipoIdentificacion
{
    [PgName("FISICA")]
    FISICA,
    [PgName("JURIDICA")]
    JURIDICA,
    [PgName("DIMEX")]
    DIMEX,
    [PgName("NITE")]
    NITE
}


