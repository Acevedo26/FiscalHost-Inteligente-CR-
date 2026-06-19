using NpgsqlTypes;

namespace FiscalHost.Api.CR.Models.Enums.Communication;

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

