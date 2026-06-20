using NpgsqlTypes;

namespace FiscalHost.Api.CR.Models.Enums.TaxIntelligence;

// ========================================================================
// Enum: Define un catálogo estricto de opciones válidas para la base de datos,
// lo cual evita errores tipográficos al impedir que se guarde un estado
// no contemplado.
// ========================================================================


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


