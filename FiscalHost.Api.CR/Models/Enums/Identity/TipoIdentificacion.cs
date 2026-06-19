using NpgsqlTypes;

namespace FiscalHost.Api.CR.Models.Enums.Identity;

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

