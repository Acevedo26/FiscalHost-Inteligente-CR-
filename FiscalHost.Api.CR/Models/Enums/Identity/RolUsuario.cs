using NpgsqlTypes;

namespace FiscalHost.Api.CR.Models.Enums.Identity;

// ========================================================================
// Enum: Define un catálogo estricto de opciones válidas para la base de datos,
// lo cual evita errores tipográficos al impedir que se guarde un estado
// no contemplado.
// ========================================================================


public enum RolUsuario
{
    [PgName("ANFITRION")]
    ANFITRION,
    [PgName("CONTADOR")]
    CONTADOR,
    [PgName("ADMINISTRADOR")]
    ADMINISTRADOR
}


