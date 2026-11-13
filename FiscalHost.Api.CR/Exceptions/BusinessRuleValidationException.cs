namespace FiscalHost.Api.CR.Exceptions;

// Clase capturar y transportar errores específicos 
public class BusinessRuleValidationException : Exception
{
    public object? ErrorDetails { get; }
    public BusinessRuleValidationException(string message) : base(message)
    {
    }

    public BusinessRuleValidationException(string message, object details) : base(message)
    {
        ErrorDetails = details;
    }
}
