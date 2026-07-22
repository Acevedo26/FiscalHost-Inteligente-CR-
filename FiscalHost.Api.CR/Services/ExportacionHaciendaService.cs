using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;
using FiscalHost.Api.CR.Models.Entities.Operations;
using FiscalHost.Api.CR.Models.Entities.TaxIntelligence;
using FiscalHost.Api.CR.Repositories;

namespace FiscalHost.Api.CR.Services;

public interface IExportacionHaciendaService
{
    Task<ExportacionHaciendaResponse> ExportarAsync(ExportacionHaciendaRequest request);
}

public class ExportacionHaciendaService(
    IExportacionHaciendaRepository repository)
    : IExportacionHaciendaService
{
    public async Task<ExportacionHaciendaResponse> ExportarAsync(ExportacionHaciendaRequest request)
    {
        var formato = request.Formato.Trim().ToUpperInvariant();
        var tipoContenido = request.TipoContenido.Trim().ToUpperInvariant();

        var error = ValidarRequest(request, formato, tipoContenido);
        if (error is not null)
            return Error(error, formato, tipoContenido);

        var reservas = await repository.GetReservasAsync(request.UsuarioId, request.AnioFiscal, request.Mes);
        var gastos = await repository.GetGastosAsync(request.UsuarioId, request.AnioFiscal, request.Mes);
        CalculoFiscal? calculo = null;

        if (request.CalculoId.HasValue)
            calculo = await repository.GetCalculoAsync(request.UsuarioId, request.CalculoId.Value);

        if (!reservas.Any() && !gastos.Any() && calculo is null)
            return Error("No existen datos para exportar en el periodo solicitado.", formato, tipoContenido);

        var contenido = formato == "XML"
            ? GenerarXml(request, reservas, gastos, calculo)
            : GenerarCsv(reservas, gastos);

        var bytes = Encoding.UTF8.GetBytes(contenido);
        var extension = formato.ToLowerInvariant();
        var tipoMime = formato == "XML" ? "application/xml" : "text/csv";

        if (request.ProtegerConContrasena)
        {
            bytes = Proteger(bytes, request.Contrasena!);
            extension += ".protected";
            tipoMime = "application/octet-stream";
        }

        var nombreArchivo = ConstruirNombreArchivo(request, formato, extension);
        var exportacion = new Exportacion
        {
            ExportacionId = Guid.NewGuid(),
            UsuarioId = request.UsuarioId,
            CalculoId = request.CalculoId,
            Formato = formato,
            TipoContenido = tipoContenido,
            ArchivoUrl = $"exports/{nombreArchivo}",
            NombreArchivo = nombreArchivo,
            TamanioBytes = bytes.Length,
            EstaProtegido = request.ProtegerConContrasena,
            ExpiraAt = DateTimeOffset.UtcNow.AddDays(7)
        };

        await repository.AddExportacionAsync(exportacion);
        await repository.SaveChangesAsync();

        return new ExportacionHaciendaResponse
        {
            Success = true,
            Mensaje = request.ProtegerConContrasena
                ? "Exportacion generada y protegida con contraseña."
                : "Exportacion generada correctamente.",
            ExportacionId = exportacion.ExportacionId,
            NombreArchivo = nombreArchivo,
            Formato = formato,
            TipoContenido = tipoContenido,
            TipoMime = tipoMime,
            TamanioBytes = bytes.Length,
            EstaProtegido = request.ProtegerConContrasena,
            ContenidoBase64 = Convert.ToBase64String(bytes)
        };
    }

    private static string? ValidarRequest(
        ExportacionHaciendaRequest request,
        string formato,
        string tipoContenido)
    {
        if (request.UsuarioId == Guid.Empty)
            return "El usuario es obligatorio.";

        if (request.AnioFiscal < 2019 || request.AnioFiscal > DateTime.UtcNow.Year)
            return "El año fiscal solicitado no es valido.";

        if (request.Mes is < 1 or > 12)
            return "El mes solicitado no es valido.";

        if (formato is not "XML" and not "CSV")
            return "Formato no soportado. Use XML o CSV.";

        if (tipoContenido is not "DECLARACION" and not "MOVIMIENTOS")
            return "Tipo de contenido no soportado. Use DECLARACION o MOVIMIENTOS.";

        if (request.ProtegerConContrasena && string.IsNullOrWhiteSpace(request.Contrasena))
            return "La contraseña es obligatoria para proteger el archivo.";

        return null;
    }

    private static string GenerarXml(
        ExportacionHaciendaRequest request,
        List<Reserva> reservas,
        List<Gasto> gastos,
        CalculoFiscal? calculo)
    {
        var doc = new XDocument(
            new XElement("DeclaracionHacienda",
                new XAttribute("anio", request.AnioFiscal),
                request.Mes.HasValue ? new XAttribute("mes", request.Mes.Value) : null,
                new XElement("UsuarioId", request.UsuarioId),
                new XElement("Resumen",
                    new XElement("TotalIngresos", reservas.Sum(MontoReserva)),
                    new XElement("TotalGastos", gastos.Sum(g => g.MontoColones)),
                    new XElement("TotalIvaDebito", calculo?.DebitoFiscal ?? reservas.Sum(r => r.MontoIvaCalculado)),
                    new XElement("TotalCreditoFiscal", calculo?.CreditoFiscal ?? gastos.Where(g => g.EsCreditoFiscalValido).Sum(g => g.MontoIvaSoportado)),
                    new XElement("TotalAPagar", calculo?.MontoTotalAPagar ?? 0)),
                new XElement("Ingresos",
                    reservas.Select(r => new XElement("Ingreso",
                        new XElement("ReservaId", r.ReservaId),
                        new XElement("FechaInicio", r.FechaInicio.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                        new XElement("FechaFin", r.FechaFin.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                        new XElement("MontoBruto", MontoReserva(r)),
                        new XElement("MontoGravado", r.MontoGravado),
                        new XElement("MontoExento", r.MontoExento),
                        new XElement("IvaCalculado", r.MontoIvaCalculado),
                        new XElement("RetencionExtranjera", r.RetencionExtranjera)))),
                new XElement("Gastos",
                    gastos.Select(g => new XElement("Gasto",
                        new XElement("GastoId", g.GastoId),
                        new XElement("FechaEmision", g.FechaEmision.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                        new XElement("Proveedor", g.Proveedor),
                        new XElement("NumeroFactura", g.NumeroFactura),
                        new XElement("MontoTotal", g.MontoColones),
                        new XElement("IvaSoportado", g.MontoIvaSoportado),
                        new XElement("CreditoFiscalValido", g.EsCreditoFiscalValido))))));

        return doc.ToString();
    }

    private static string GenerarCsv(List<Reserva> reservas, List<Gasto> gastos)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Tipo,Fecha,Referencia,Descripcion,MontoBruto,MontoGravado,MontoExento,IVA,Retencion");

        foreach (var reserva in reservas)
        {
            sb.AppendLine(string.Join(",",
                "INGRESO",
                reserva.FechaInicio.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                EscapeCsv(reserva.ReferenciaPlataforma ?? reserva.ReservaId.ToString()),
                EscapeCsv(reserva.PlataformaOrigen.ToString()),
                MontoReserva(reserva).ToString(CultureInfo.InvariantCulture),
                reserva.MontoGravado.ToString(CultureInfo.InvariantCulture),
                reserva.MontoExento.ToString(CultureInfo.InvariantCulture),
                reserva.MontoIvaCalculado.ToString(CultureInfo.InvariantCulture),
                reserva.RetencionExtranjera.ToString(CultureInfo.InvariantCulture)));
        }

        foreach (var gasto in gastos)
        {
            sb.AppendLine(string.Join(",",
                "GASTO",
                gasto.FechaEmision.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                EscapeCsv(gasto.NumeroFactura ?? gasto.GastoId.ToString()),
                EscapeCsv(gasto.Proveedor),
                gasto.MontoColones.ToString(CultureInfo.InvariantCulture),
                "0",
                "0",
                gasto.MontoIvaSoportado.ToString(CultureInfo.InvariantCulture),
                "0"));
        }

        return sb.ToString();
    }

    private static byte[] Proteger(byte[] bytes, string contrasena)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        using var deriveBytes = new Rfc2898DeriveBytes(
            contrasena,
            salt,
            100_000,
            HashAlgorithmName.SHA256);

        using var aes = Aes.Create();
        aes.Key = deriveBytes.GetBytes(32);
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        var cifrado = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);

        return [.. salt, .. aes.IV, .. cifrado];
    }

    private static string ConstruirNombreArchivo(
        ExportacionHaciendaRequest request,
        string formato,
        string extension)
    {
        var periodo = request.Mes.HasValue
            ? $"{request.AnioFiscal}_{request.Mes.Value:00}"
            : request.AnioFiscal.ToString(CultureInfo.InvariantCulture);

        return $"hacienda_{request.UsuarioId:N}_{periodo}_{formato.ToLowerInvariant()}.{extension}";
    }

    private static decimal MontoReserva(Reserva reserva)
    {
        return reserva.MontoColones > 0 ? reserva.MontoColones : reserva.MontoBruto;
    }

    private static string EscapeCsv(string value)
    {
        return $"\"{value.Replace("\"", "\"\"")}\"";
    }

    private static ExportacionHaciendaResponse Error(
        string mensaje,
        string formato,
        string tipoContenido) => new()
    {
        Success = false,
        Mensaje = mensaje,
        Formato = formato,
        TipoContenido = tipoContenido
    };
}
