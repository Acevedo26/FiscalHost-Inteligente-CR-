using NpgsqlTypes;

namespace FiscalHost.Api.CR.Models.Enums.TaxIntelligence;

// ========================================================================
// Enum: Define un catálogo estricto de opciones válidas para la base de datos,
// lo cual evita errores tipográficos al impedir que se guarde un estado
// no contemplado.
// ========================================================================


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


