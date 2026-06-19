using NpgsqlTypes;

namespace FiscalHost.Api.CR.Models.Enums.TaxIntelligence;

public enum RegimenTributario
{
    [PgName("CAPITAL_INMOBILIARIO")]
    CAPITAL_INMOBILIARIO,
    [PgName("UTILIDADES")]
    UTILIDADES
}

