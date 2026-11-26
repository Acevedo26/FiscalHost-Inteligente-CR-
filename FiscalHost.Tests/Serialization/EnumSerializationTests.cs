using System.Text.Json;
using System.Text.Json.Serialization;
using FiscalHost.Api.CR.Models.Enums.Operations;
using Xunit;

namespace FiscalHost.Tests.Serialization;

public class EnumSerializationTests
{
    // Espeja las opciones JSON de la API: Program.cs registra JsonStringEnumConverter
    // globalmente vía AddJsonOptions, por lo que los enums se serializan como string.
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    [Fact]
    public void ClasificacionFiscal_SerializesAsString_NotInteger()
    {
        var dto = new { ClasificacionFiscal = ClasificacionFiscal.GRAVADO };
        var json = JsonSerializer.Serialize(dto, _options);

        // Debe serializar como string "GRAVADO", no como entero 0
        Assert.Contains("\"GRAVADO\"", json);
        Assert.DoesNotContain(": 0", json); // No debe ser el entero 0
    }

    [Fact]
    public void PlataformaOrigen_SerializesAsString_NotInteger()
    {
        var dto = new { PlataformaOrigen = PlataformaOrigen.AIRBNB };
        var json = JsonSerializer.Serialize(dto, _options);

        Assert.Contains("\"AIRBNB\"", json);
        Assert.DoesNotContain(": 0", json);
    }

    [Fact]
    public void TipoMoneda_SerializesAsString_NotInteger()
    {
        var dto = new { Moneda = TipoMoneda.CRC };
        var json = JsonSerializer.Serialize(dto, _options);

        Assert.Contains("\"CRC\"", json);
        Assert.DoesNotContain(": 0", json);
    }

    [Fact]
    public void AllEnumValues_SerializeAsStrings()
    {
        foreach (var value in Enum.GetValues<ClasificacionFiscal>())
        {
            var json = JsonSerializer.Serialize(new { Value = value }, _options);
            Assert.Contains($"\"{value}\"", json);
            Assert.DoesNotMatch(@": \d+", json); // No dígitos solos
        }

        foreach (var value in Enum.GetValues<PlataformaOrigen>())
        {
            var json = JsonSerializer.Serialize(new { Value = value }, _options);
            Assert.Contains($"\"{value}\"", json);
            Assert.DoesNotMatch(@": \d+", json);
        }

        foreach (var value in Enum.GetValues<TipoMoneda>())
        {
            var json = JsonSerializer.Serialize(new { Value = value }, _options);
            Assert.Contains($"\"{value}\"", json);
            Assert.DoesNotMatch(@": \d+", json);
        }
    }

    [Fact]
    public void Deserialization_FromString_Works()
    {
        var json = """{"clasificacionFiscal": "GRAVADO"}""";
        var dto = JsonSerializer.Deserialize<EnumContainer>(json, _options);
        Assert.NotNull(dto);
        Assert.Equal(ClasificacionFiscal.GRAVADO, dto.ClasificacionFiscal);
    }

    [Fact]
    public void Deserialization_FromPgName_Works()
    {
        // Los valores en BD usan PgName, pero la API usa nombres de enum C#
        // Este test verifica que la deserialización funciona con nombres C#
        var json = """{"plataformaOrigen": "BOOKING"}""";
        var dto = JsonSerializer.Deserialize<EnumContainer>(json, _options);
        Assert.NotNull(dto);
        Assert.Equal(PlataformaOrigen.BOOKING, dto.PlataformaOrigen);
    }

    private record EnumContainer(
        ClasificacionFiscal? ClasificacionFiscal = null,
        PlataformaOrigen? PlataformaOrigen = null,
        TipoMoneda? Moneda = null
    );
}