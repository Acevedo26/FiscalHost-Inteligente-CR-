using NpgsqlTypes;

namespace FiscalHost.Api.CR.Models.Enums.Audit;

// ========================================================================
// Enum: Define un catálogo estricto de opciones válidas para la base de datos,
// lo cual evita errores tipográficos al impedir que se guarde un estado
// no contemplado.
// ========================================================================


public enum EstadoLlave
{
    [PgName("ACTIVA")]
    ACTIVA,
    [PgName("EXPIRADA")]
    EXPIRADA,
    [PgName("REVOCADA")]
    REVOCADA
}


