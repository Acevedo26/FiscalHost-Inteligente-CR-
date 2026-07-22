using NpgsqlTypes;

namespace FiscalHost.Api.CR.Models.Enums.Operations;

public enum ClasificacionIva
{
    [PgName("GRAVADO_13")]
    Gravado13 = 1,

    [PgName("EXENTO")]
    Exento = 2
}
