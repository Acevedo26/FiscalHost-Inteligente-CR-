using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Extensions;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var dataSource = new NpgsqlDataSourceBuilder(
    builder.Configuration.GetConnectionString("DefaultConnection"))
    .MapFiscalHostEnums()
    .Build();

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(dataSource, o => o.MigrationsHistoryTable("__EFMigrationsHistory", "fiscalhost_db")));

builder.Services.Configure<FiscalHost.Api.CR.Models.TaxSettings>(builder.Configuration.GetSection("TaxSettings"));

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
builder.Services.AddScoped<IBlobStorageService, LocalStorageService>();
builder.Services.AddScoped<IOcrService, OcrService>();
builder.Services.AddScoped<ICalculoRentaCapitalRepository, CalculoRentaCapitalRepository>();
builder.Services.AddScoped<ICalculoRentaCapitalService, CalculoRentaCapitalService>();
builder.Services.AddScoped<IObligacionTributariaRepository, ObligacionTributariaRepository>();
builder.Services.AddScoped<IObligacionTributariaService, ObligacionTributariaService>();
builder.Services.AddScoped<IGeneradorBorradorRepository, GeneradorBorradorRepository>();
builder.Services.AddScoped<IGeneradorBorradorService, GeneradorBorradorService>();
builder.Services.AddScoped<IExportacionHaciendaRepository, ExportacionHaciendaRepository>();
builder.Services.AddScoped<IExportacionHaciendaService, ExportacionHaciendaService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddHostedService<MoraBackgroundService>();
builder.Services.AddScoped<IImportacionMasivaService, ImportacionMasivaService>();
builder.Services.AddScoped<IImportacionMasivaRepository, ImportacionMasivaRepository>();
builder.Services.AddScoped<ICalculoIvaService, CalculoIvaService>();
builder.Services.AddScoped<ISancionAutoliquidacionRepository, SancionAutoliquidacionRepository>();
builder.Services.AddScoped<ISancionAutoliquidacionService, SancionAutoliquidacionService>();
builder.Services.AddScoped<IContenidoEducativoRepository, ContenidoEducativoRepository>();
builder.Services.AddScoped<IContenidoEducativoService, ContenidoEducativoService>();
builder.Services.AddScoped<IAlertaRepository, AlertaRepository>();
builder.Services.AddScoped<IAlertaService, AlertaService>();
builder.Services.AddScoped<IAuditoriaInalterableRepository, AuditoriaInalterableRepository>();
builder.Services.AddScoped<IAuditoriaInalterableService, AuditoriaInalterableService>();
builder.Services.AddScoped<IAccesoContadorRepository, AccesoContadorRepository>();
builder.Services.AddScoped<IAccesoContadorService, AccesoContadorService>();
builder.Services.AddHostedService<AlertaBackgroundService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is not set"))),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT en el formato: Bearer {token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
