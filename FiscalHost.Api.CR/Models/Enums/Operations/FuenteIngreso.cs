using NpgsqlTypes;

namespace FiscalHost.Api.CR.Models.Enums.Operations;

public enum FuenteIngreso
{
    [PgName("NACIONAL")]
    Nacional = 1,

    [PgName("EXTRANJERA")]
    Extranjera = 2
}
