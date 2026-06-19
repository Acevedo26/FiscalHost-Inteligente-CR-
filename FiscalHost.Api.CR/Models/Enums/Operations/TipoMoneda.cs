using NpgsqlTypes;

namespace FiscalHost.Api.CR.Models.Enums.Operations;

public enum TipoMoneda
{
    [PgName("CRC")]
    CRC,
    [PgName("USD")]
    USD
}

