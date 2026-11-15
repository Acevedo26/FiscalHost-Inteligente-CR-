using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace FiscalHost.Api.CR.Models.DTOs.Operations;

public class ImportacionRequest
{
    public IFormFile Archivo { get; set; } = default!;
}