using NpgsqlTypes;

namespace FiscalHost.Api.CR.Models.Enums.Identity;

// ========================================================================
// Enum: Define un catálogo estricto de opciones válidas para la base de datos,
// lo cual evita errores tipográficos al impedir que se guarde un estado
// no contemplado.
// ========================================================================


public enum EstadoUsuario
{
    [PgName("PENDIENTE_ACTIVACION")]
    PENDIENTE_ACTIVACION,
    [PgName("ACTIVO")]
    ACTIVO,
    [PgName("INACTIVO")]
    INACTIVO,
    [PgName("BLOQUEADO")]
    BLOQUEADO,
    [PgName("ELIMINADO")]
    ELIMINADO
}


