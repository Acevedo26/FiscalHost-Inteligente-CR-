using NpgsqlTypes;

namespace FiscalHost.Api.CR.Models.Enums.TaxIntelligence;

// ========================================================================
// Enum: Define un catálogo estricto de opciones válidas para la base de datos,
// lo cual evita errores tipográficos al impedir que se guarde un estado
// no contemplado.
// ========================================================================


public enum TipoFormulario
{
    [PgName("D104")]
    D104,
    [PgName("D125")]
    D125,
    [PgName("D150")]
    D150,
    [PgName("D116")]
    D116,
    [PgName("D176")]
    D176,
	[PgName("D125")]
	D125
    D176,
    [PgName("D104")]
    D104
}


