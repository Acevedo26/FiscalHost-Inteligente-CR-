using NpgsqlTypes;

namespace FiscalHost.Api.CR.Models.Enums.Communication;

public enum CanalNotificacion
{
    [PgName("CORREO")]
    CORREO,
    [PgName("PLATAFORMA")]
    PLATAFORMA,
    [PgName("AMBOS")]
    AMBOS
}

