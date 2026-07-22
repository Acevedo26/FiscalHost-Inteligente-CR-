using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PDFtoImage;
using Tesseract;
using System.Text.Json;
using System.Linq;

namespace FiscalHost.Api.CR.Services;

// ========================================================================
// Interfaz para el servicio de Extracción de Metadatos (OCR)
// ========================================================================
public interface IOcrService
{
    Task<OcrResult> ExtractMetadataAsync(Stream fileStream, string contentType);
}

// Objeto para devolver el resultado estructurado de la lectura del OCR
public class OcrResult
{
    public bool Exitoso { get; set; }
    public string? Proveedor { get; set; }
    public string? NumeroFactura { get; set; }
    public DateOnly? FechaEmision { get; set; }
    public decimal? MontoTotal { get; set; }
    public string TextoExtraidoRaw { get; set; } = string.Empty;
}

// ========================================================================
// Implementación del Servicio OCR utilizando Tesseract.
// Si el archivo falla o es ilegible, se activa el "Fallback" (Exitoso = false)
// para que el sistema lo almacene pero requiera digitación manual.
// ========================================================================
public class OcrService : IOcrService
{
    // Ruta al directorio tessdata que contiene los modelos de idioma
    private readonly string _tessDataPath;

    public OcrService()
    {
        // Se asume que la carpeta 'tessdata' está en el root de ejecución
        _tessDataPath = Path.Combine(AppContext.BaseDirectory, "tessdata");
    }

    public async Task<OcrResult> ExtractMetadataAsync(Stream fileStream, string contentType)
    {
        var result = new OcrResult();

        try
        {
            string extractedText = string.Empty;

            // 1. Validar el tipo de archivo y convertir PDF a Imagen si es necesario
            if (contentType.Contains("pdf", StringComparison.OrdinalIgnoreCase))
            {
                var pages = Conversion.ToImages(fileStream).ToList();
                var sb = new System.Text.StringBuilder();
                foreach (var page in pages)
                {
                    using var ms2 = new MemoryStream();
                    using var data = page.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100);
                    data.SaveTo(ms2);
                    ms2.Position = 0;
                    sb.AppendLine(PerformOcrOnImageStream(ms2));
                }
                extractedText = sb.ToString();
            }
            else if (contentType.Contains("image", StringComparison.OrdinalIgnoreCase))
            {
                // Es una imagen directa
                extractedText = PerformOcrOnImageStream(fileStream);
            }
            else
            {
                // Tipo no soportado
                result.Exitoso = false;
                return result;
            }

            result.TextoExtraidoRaw = extractedText;

            // 2. Extracción de metadatos mediante Expresiones Regulares
            // Lógica de negocio: Parsear campos requeridos de la factura

            // Buscar Proveedor (se asume que la primera línea puede ser el proveedor, o se busca una línea específica)
            // Esto es heurístico, pero para facturas electrónicas de CR suele ser la primera línea con texto largo
            var lines = extractedText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length > 0)
            {
                result.Proveedor = lines[0].Trim();
            }

            // Buscar Número de Factura — soporta formato CR: "Consecutivo: 001..." o "Factura N° 12345"
            // Buscar Número de Factura — el consecutivo aparece después de "Clave:\n\n" en tiquetes CR
            var facturaRegex = new Regex(
                @"Consecutivo\s*:[\s\S]*?Clave\s*:[\s\S]*?\n\s*([\d]{10,50})" +
                @"|(?:Consecutivo|Factura|N[°º])\s*[:#-]?\s*[\r\n\s]*([\d]{10,50})",
                RegexOptions.IgnoreCase);
            var facturaMatch = facturaRegex.Match(extractedText);
            if (facturaMatch.Success)
                result.NumeroFactura = facturaMatch.Groups[1].Success
                    ? facturaMatch.Groups[1].Value
                    : facturaMatch.Groups[2].Value;

            // Buscar Fecha — soporta dd/MM/yyyy, yyyy-MM-dd y "14 de marzo de 2026"
            var fechaRegex = new Regex(
                @"\b(\d{2}[/-]\d{2}[/-]\d{4})|" +
                @"(\d{1,2}\s+de\s+(?:enero|febrero|marzo|abril|mayo|junio|julio|agosto|septiembre|octubre|noviembre|diciembre)\s+de\s+\d{4})",
                RegexOptions.IgnoreCase);
            var fechaMatch = fechaRegex.Match(extractedText);
            if (fechaMatch.Success)
            {
                var fechaStr = fechaMatch.Value;
                if (DateTime.TryParse(fechaStr, new System.Globalization.CultureInfo("es-CR"), System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                    result.FechaEmision = DateOnly.FromDateTime(parsedDate);
            }

            // Buscar Monto Total — prioriza "Total comprobante" (formato tiquete CR), luego otros patrones
            var montoRegex = new Regex(
                @"Total\s+comprobante[\s\S]*?([1-9][\d]{0,2}(?:\.[\d]{3})*,[\d]{2})",
                RegexOptions.IgnoreCase);
            var montoMatch = montoRegex.Match(extractedText);
            if (montoMatch.Success)
            {
                var rawMonto = montoMatch.Groups[1].Value.Replace(".", "").Replace(",", ".");
                if (decimal.TryParse(rawMonto, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out decimal parsedMonto) && parsedMonto > 0)
                    result.MontoTotal = parsedMonto;
            }

            // Fallback parcial: si falta solo el monto, se retornan los demás datos para completar manualmente
            result.Exitoso = !string.IsNullOrEmpty(result.NumeroFactura)
                && result.FechaEmision.HasValue
                && result.MontoTotal.HasValue
                && result.MontoTotal > 0;

            return result;
        }
        catch (Exception ex)
        {
            // En caso de que falle el OCR, activamos el "Fallback".
            // El documento se guardará, pero no se considerará para crédito fiscal automáticamente.
            Console.WriteLine($"Error en OCR: {ex.Message}");
            result.Exitoso = false;
            return result;
        }
    }

    /// <summary>
    /// Metodo privado que encapsula el uso de Tesseract.
    /// Lee una imagen y devuelve el texto plano extraído.
    /// </summary>
    private string PerformOcrOnImageStream(Stream imageStream)
    {
        // Nota: Asegurarse de que exista el directorio tessdata con eng.traineddata o spa.traineddata
        if (!Directory.Exists(_tessDataPath))
        {
            // Si no existe, devolvemos un texto que simula la falla del OCR.
            throw new DirectoryNotFoundException("No se encontró el directorio tessdata.");
        }

        // Tesseract requiere que inicialicemos el motor ("spa" para español o "eng" para inglés)
        using var engine = new TesseractEngine(_tessDataPath, "spa", EngineMode.Default);
        
        using var ms = new MemoryStream();
        imageStream.CopyTo(ms);
        byte[] imageBytes = ms.ToArray();

        // Cargamos la imagen a Pix
        using var pix = Pix.LoadFromMemory(imageBytes);
        using var page = engine.Process(pix);
        
        return page.GetText();
    }
}
