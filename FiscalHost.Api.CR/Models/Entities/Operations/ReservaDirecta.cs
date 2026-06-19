namespace FiscalHost.Api.CR.Models.Entities.Operations;

public class ReservaDirecta
{
    public int Id { get; set; }

    public string AnfitrionId { get; set; } = string.Empty;

    public DateTime FechaReserva { get; set; }

    public decimal Monto { get; set; }

    public string Huesped { get; set; } = string.Empty;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
