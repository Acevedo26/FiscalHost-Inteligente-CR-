using FiscalHost.Api.CR.Models.Enums;
using Npgsql;

namespace FiscalHost.Api.CR.Extensions;

public static class NpgsqlEnumExtensions
{
    private const string Schema = "fiscalhost_db";

    public static NpgsqlDataSourceBuilder MapFiscalHostEnums(this NpgsqlDataSourceBuilder builder)
    {
        var t = new Npgsql.NameTranslation.NpgsqlNullNameTranslator();
        builder.MapEnum<TipoIdentificacion>($"{Schema}.tipo_identificacion", nameTranslator: t);
        builder.MapEnum<EstadoUsuario>($"{Schema}.estado_usuario", nameTranslator: t);
        builder.MapEnum<RolUsuario>($"{Schema}.rol_usuario", nameTranslator: t);
        builder.MapEnum<RegimenTributario>($"{Schema}.regimen_tributario", nameTranslator: t);
        builder.MapEnum<ClasificacionFiscal>($"{Schema}.clasificacion_fiscal", nameTranslator: t);
        builder.MapEnum<PlataformaOrigen>($"{Schema}.plataforma_origen", nameTranslator: t);
        builder.MapEnum<FuenteRegistro>($"{Schema}.fuente_registro", nameTranslator: t);
        builder.MapEnum<EstadoOcr>($"{Schema}.estado_ocr", nameTranslator: t);
        builder.MapEnum<EstadoValidacion>($"{Schema}.estado_validacion", nameTranslator: t);
        builder.MapEnum<EstadoImportacion>($"{Schema}.estado_importacion", nameTranslator: t);
        builder.MapEnum<TipoFormulario>($"{Schema}.tipo_formulario", nameTranslator: t);
        builder.MapEnum<EstadoDeclaracion>($"{Schema}.estado_declaracion", nameTranslator: t);
        builder.MapEnum<EstadoObligacion>($"{Schema}.estado_obligacion", nameTranslator: t);
        builder.MapEnum<TipoAlerta>($"{Schema}.tipo_alerta", nameTranslator: t);
        builder.MapEnum<CanalNotificacion>($"{Schema}.canal_notificacion", nameTranslator: t);
        builder.MapEnum<EstadoNotificacion>($"{Schema}.estado_notificacion", nameTranslator: t);
        builder.MapEnum<EstadoLlave>($"{Schema}.estado_llave", nameTranslator: t);
        builder.MapEnum<OperacionAuditoria>($"{Schema}.operacion_auditoria", nameTranslator: t);
        builder.MapEnum<TipoMoneda>($"{Schema}.tipo_moneda", nameTranslator: t);
        return builder;
    }
}
