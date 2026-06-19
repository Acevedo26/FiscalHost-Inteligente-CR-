using NpgsqlTypes;

namespace FiscalHost.Api.CR.Models.Enums.Identity;

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

