using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Extensions;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var dataSource = new NpgsqlDataSourceBuilder(
    builder.Configuration.GetConnectionString("DefaultConnection"))
    .MapFiscalHostEnums()
    .Build();

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(dataSource));

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IConfiguracionTributariaRepository, ConfiguracionTributariaRepository>();
builder.Services.AddScoped<IActividadEconomicaRepository, ActividadEconomicaRepository>();
builder.Services.AddScoped<IConfiguracionTributariaService, ConfiguracionTributariaService>();
builder.Services.AddScoped<ILlaveCriptograficaRepository, LlaveCriptograficaRepository>();
builder.Services.AddScoped<ILlaveCriptograficaService, LlaveCriptograficaService>();
builder.Services.AddScoped<INotificacionService, NotificacionService>();
builder.Services.AddScoped<IOperacionManualRepository, OperacionManualRepository>();
builder.Services.AddScoped<IOperacionManualService, OperacionManualService>();
builder.Services.AddScoped<IClasificacionIngresoRepository, ClasificacionIngresoRepository>();
builder.Services.AddScoped<IClasificacionIngresoService, ClasificacionIngresoService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
