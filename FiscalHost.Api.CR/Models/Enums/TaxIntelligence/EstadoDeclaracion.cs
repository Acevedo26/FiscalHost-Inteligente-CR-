using NpgsqlTypes;

namespace FiscalHost.Api.CR.Models.Enums.TaxIntelligence;

public enum EstadoDeclaracion
{
    [PgName("BORRADOR")]
    BORRADOR,
    [PgName("CALCULADO")]
    CALCULADO,
    [PgName("VALIDADO")]
    VALIDADO,
    [PgName("EXPORTADO")]
    EXPORTADO,
    [PgName("PRESENTADO")]
    PRESENTADO
}

