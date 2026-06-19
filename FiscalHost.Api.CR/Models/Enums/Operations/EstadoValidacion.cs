using NpgsqlTypes;

namespace FiscalHost.Api.CR.Models.Enums.Operations;

public enum EstadoValidacion
{
    [PgName("PENDIENTE")]
    PENDIENTE,
    [PgName("VALIDO")]
    VALIDO,
    [PgName("RECHAZADO")]
    RECHAZADO,
    [PgName("DUPLICADO")]
    DUPLICADO
}

