using NpgsqlTypes;

namespace FiscalHost.Api.CR.Models.Enums.Operations;

// ========================================================================
// Enum: Define un catálogo estricto de opciones válidas para la base de datos,
// lo cual evita errores tipográficos al impedir que se guarde un estado
// no contemplado.
// ========================================================================


public enum ClasificacionFiscal
{
    [PgName("GRAVADO")]
    GRAVADO,
    [PgName("EXENTO")]
    EXENTO,
    [PgName("GRAVADO_CON_RETENCION")]
    GRAVADO_CON_RETENCION
}


