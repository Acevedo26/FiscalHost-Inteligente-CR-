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
                // PDFtoImage nos permite obtener las páginas del PDF como imágenes.
                // Usamos la primera página asumiendo que contiene los datos de la factura
                var pages = Conversion.ToImages(fileStream).ToList();
                if (pages.Any())
                {
                    using var firstPageMemoryStream = new MemoryStream();
                    // Guardamos la primera página como imagen en un MemoryStream
                    using var data = pages.First().Encode(SkiaSharp.SKEncodedImageFormat.Png, 100);
                    data.SaveTo(firstPageMemoryStream);
                    firstPageMemoryStream.Position = 0;
                    extractedText = PerformOcrOnImageStream(firstPageMemoryStream);
                }
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

            // Buscar Número de Factura (ej. Factura N° 12345 o similar)
            var facturaRegex = new Regex(@"(?:Factura|Consecutivo|N[°º])\s*[:#-]?\s*(\d{5,20})", RegexOptions.IgnoreCase);
            var facturaMatch = facturaRegex.Match(extractedText);
            if (facturaMatch.Success)
            {
                result.NumeroFactura = facturaMatch.Groups[1].Value;
            }

            // Buscar Fecha (formatos comunes: dd/MM/yyyy, yyyy-MM-dd)
            var fechaRegex = new Regex(@"\b(\d{2}[/-]\d{2}[/-]\d{4})\b");
            var fechaMatch = fechaRegex.Match(extractedText);
            if (fechaMatch.Success && DateTime.TryParse(fechaMatch.Groups[1].Value, out DateTime parsedDate))
            {
                result.FechaEmision = DateOnly.FromDateTime(parsedDate);
            }

            // Buscar Monto Total (ej. Total: 15000.00)
            var montoRegex = new Regex(@"(?:Total|Monto Total)\s*[:\$₡]?\s*([\d,]+\.?\d{0,2})", RegexOptions.IgnoreCase);
            var montoMatch = montoRegex.Match(extractedText);
            if (montoMatch.Success)
            {
                // Limpiar posibles separadores de miles
                string cleanMonto = montoMatch.Groups[1].Value.Replace(",", "");
                if (decimal.TryParse(cleanMonto, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal parsedMonto))
                {
                    result.MontoTotal = parsedMonto;
                }
            }

            // 3. Fallback: Si no se extrajeron correctamente los campos mínimos, se considera fallido
            // para que advierta al usuario (El usuario ingresa manual)
            if (string.IsNullOrEmpty(result.Proveedor) || string.IsNullOrEmpty(result.NumeroFactura) || !result.MontoTotal.HasValue || !result.FechaEmision.HasValue)
            {
                result.Exitoso = false;
            }
            else
            {
                result.Exitoso = true;
            }

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
