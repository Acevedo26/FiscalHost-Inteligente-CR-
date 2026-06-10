using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IConfiguracionTributariaRepository, ConfiguracionTributariaRepository>();
builder.Services.AddScoped<IActividadEconomicaRepository, ActividadEconomicaRepository>();
builder.Services.AddScoped<IConfiguracionTributariaService, ConfiguracionTributariaService>();
builder.Services.AddScoped<ILlaveCriptograficaRepository, LlaveCriptograficaRepository>();
builder.Services.AddScoped<ILlaveCriptograficaService, LlaveCriptograficaService>();
builder.Services.AddScoped<INotificacionService, NotificacionService>();
builder.Services.AddScoped<IOperacionManualRepository, OperacionManualRepository>();
builder.Services.AddScoped<IOperacionManualService, OperacionManualService>();

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
