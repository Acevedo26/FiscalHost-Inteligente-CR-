namespace FiscalHost.Api.CR.Models.Enums.Identity;

// ========================================================================
// Enum: Define un catálogo estricto de opciones válidas para la base de datos,
// lo cual evita errores tipográficos al impedir que se guarde un estado
// no contemplado.
// ========================================================================


public enum EstadoConfiguracion
{
    Activa = 1,
    Inactiva = 2,
    PendienteValidacion = 3
}


