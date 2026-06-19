using NpgsqlTypes;

namespace FiscalHost.Api.CR.Models.Enums.Audit;

public enum EstadoLlave
{
    [PgName("ACTIVA")]
    ACTIVA,
    [PgName("EXPIRADA")]
    EXPIRADA,
    [PgName("REVOCADA")]
    REVOCADA
}

