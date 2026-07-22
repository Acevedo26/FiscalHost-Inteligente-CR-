using NpgsqlTypes;

namespace FiscalHost.Api.CR.Models.Enums.Communication;

// ========================================================================
// Enum: Define un catálogo estricto de opciones válidas para la base de datos,
// lo cual evita errores tipográficos al impedir que se guarde un estado
// no contemplado.
// ========================================================================


public enum EstadoNotificacion
{
    [PgName("PENDIENTE")]
    PENDIENTE,
    [PgName("ENVIADA")]
    ENVIADA,
    [PgName("ENTREGADA")]
    ENTREGADA,
    [PgName("FALLIDA")]
    FALLIDA,
    [PgName("LEIDA")]
    LEIDA
}


