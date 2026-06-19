using NpgsqlTypes;

namespace FiscalHost.Api.CR.Models.Enums.Operations;

public enum FuenteRegistro
{
    [PgName("IMPORTACION_CSV")]
    IMPORTACION_CSV,
    [PgName("MANUAL")]
    MANUAL,
    [PgName("RECONSTRUCCION")]
    RECONSTRUCCION
}

