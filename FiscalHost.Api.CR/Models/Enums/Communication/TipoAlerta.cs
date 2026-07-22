using NpgsqlTypes;

namespace FiscalHost.Api.CR.Models.Enums.Communication;

// ========================================================================
// Enum: Define un catálogo estricto de opciones válidas para la base de datos,
// lo cual evita errores tipográficos al impedir que se guarde un estado
// no contemplado.
// ========================================================================


public enum TipoAlerta
{
    [PgName("VENCIMIENTO_15_DIAS")]
    VENCIMIENTO_15_DIAS,
    [PgName("VENCIMIENTO_10_DIAS")]
    VENCIMIENTO_10_DIAS,
    [PgName("VENCIMIENTO_7_DIAS")]
    VENCIMIENTO_7_DIAS,
    [PgName("VENCIMIENTO_3_DIAS")]
    VENCIMIENTO_3_DIAS,
    [PgName("VENCIMIENTO_1_DIA")]
    VENCIMIENTO_1_DIA,
    [PgName("VENCIMIENTO_EXPIRADO")]
    VENCIMIENTO_EXPIRADO,
    [PgName("LLAVE_POR_VENCER")]
    LLAVE_POR_VENCER,
    [PgName("INFORMATIVA")]
    INFORMATIVA,
    [PgName("SEGURIDAD")]
    SEGURIDAD
}


