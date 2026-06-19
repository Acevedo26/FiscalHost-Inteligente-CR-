using NpgsqlTypes;

namespace FiscalHost.Api.CR.Models.Enums.Operations;

public enum PlataformaOrigen
{
    [PgName("AIRBNB")]
    AIRBNB,
    [PgName("BOOKING")]
    BOOKING,
    [PgName("VRBO")]
    VRBO,
    [PgName("DIRECTA")]
    DIRECTA,
    [PgName("OTRA")]
    OTRA
}

