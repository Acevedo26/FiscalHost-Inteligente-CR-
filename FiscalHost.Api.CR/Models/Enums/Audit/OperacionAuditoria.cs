namespace FiscalHost.Api.CR.Models.Enums.Audit;

// ========================================================================
// Enum: Define un catálogo estricto de opciones válidas para la base de datos,
// lo cual evita errores tipográficos al impedir que se guarde un estado
// no contemplado.
// ========================================================================


public enum OperacionAuditoria
{
    INSERT,
    UPDATE,
    DELETE,
    LOGIN,
    LOGOUT,
    EXPORT,
    RECLASIFICACION,
    CAMBIO_REGIMEN,
    CAMBIO_ACTIVIDAD_ECONOMICA
}


