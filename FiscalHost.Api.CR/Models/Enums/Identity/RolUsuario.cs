using NpgsqlTypes;

namespace FiscalHost.Api.CR.Models.Enums.Identity;

public enum RolUsuario
{
    [PgName("ANFITRION")]
    ANFITRION,
    [PgName("CONTADOR")]
    CONTADOR,
    [PgName("ADMINISTRADOR")]
    ADMINISTRADOR
}

