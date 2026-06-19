using NpgsqlTypes;

namespace FiscalHost.Api.CR.Models.Enums.TaxIntelligence;

public enum EstadoObligacion
{
    [PgName("VIGENTE")]
    VIGENTE,
    [PgName("VENCIDA")]
    VENCIDA,
    [PgName("PAGADA")]
    PAGADA,
    [PgName("EN_ARREGLO")]
    EN_ARREGLO
}

