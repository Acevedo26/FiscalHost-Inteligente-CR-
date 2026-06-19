using NpgsqlTypes;

namespace FiscalHost.Api.CR.Models.Enums.Operations;

public enum ClasificacionFiscal
{
    [PgName("GRAVADO")]
    GRAVADO,
    [PgName("EXENTO")]
    EXENTO,
    [PgName("GRAVADO_CON_RETENCION")]
    GRAVADO_CON_RETENCION
}

