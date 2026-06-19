using NpgsqlTypes;

namespace FiscalHost.Api.CR.Models.Enums.TaxIntelligence;

public enum TipoFormulario
{
    [PgName("D150")]
    D150,
    [PgName("D116")]
    D116,
    [PgName("D176")]
    D176
}

