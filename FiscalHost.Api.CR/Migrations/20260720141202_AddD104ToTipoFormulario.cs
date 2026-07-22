using System;
using System.Net;
using FiscalHost.Api.CR.Models.Enums.Audit;
using FiscalHost.Api.CR.Models.Enums.Identity;
using FiscalHost.Api.CR.Models.Enums.Operations;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FiscalHost.Api.CR.Migrations
{
    /// <inheritdoc />
    public partial class AddD104ToTipoFormulario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_auditoria_operacion",
                schema: "fiscalhost_db",
                table: "auditoria_operacion");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "fiscalhost_db",
                table: "auditoria_operacion");

            migrationBuilder.DropColumn(
                name: "Accion",
                schema: "fiscalhost_db",
                table: "auditoria_operacion");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                schema: "fiscalhost_db",
                table: "auditoria_operacion");

            migrationBuilder.DropColumn(
                name: "Entidad",
                schema: "fiscalhost_db",
                table: "auditoria_operacion");

            migrationBuilder.DropColumn(
                name: "EntidadId",
                schema: "fiscalhost_db",
                table: "auditoria_operacion");

            migrationBuilder.RenameTable(
                name: "auditoria_operacion",
                schema: "fiscalhost_db",
                newName: "audit_log",
                newSchema: "fiscalhost_db");

            migrationBuilder.RenameColumn(
                name: "Justificacion",
                schema: "fiscalhost_db",
                table: "audit_log",
                newName: "justificacion");

            migrationBuilder.RenameColumn(
                name: "ValorNuevo",
                schema: "fiscalhost_db",
                table: "audit_log",
                newName: "user_agent");

            migrationBuilder.RenameColumn(
                name: "ValorAnterior",
                schema: "fiscalhost_db",
                table: "audit_log",
                newName: "correo_usuario");

            migrationBuilder.RenameColumn(
                name: "Usuario",
                schema: "fiscalhost_db",
                table: "audit_log",
                newName: "tabla_afectada");

            migrationBuilder.RenameColumn(
                name: "Fecha",
                schema: "fiscalhost_db",
                table: "audit_log",
                newName: "created_at");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:estado_llave.fiscalhost_db", "ACTIVA,EXPIRADA,REVOCADA")
                .Annotation("Npgsql:Enum:fiscalhost_db.canal_notificacion", "CORREO,PLATAFORMA,AMBOS")
                .Annotation("Npgsql:Enum:fiscalhost_db.clasificacion_fiscal", "GRAVADO,EXENTO,GRAVADO_CON_RETENCION")
                .Annotation("Npgsql:Enum:fiscalhost_db.clasificacion_iva", "GRAVADO_13,EXENTO")
                .Annotation("Npgsql:Enum:fiscalhost_db.estado_declaracion", "BORRADOR,CALCULADO,VALIDADO,EXPORTADO,PRESENTADO")
                .Annotation("Npgsql:Enum:fiscalhost_db.estado_importacion", "PENDIENTE,PROCESANDO,COMPLETADO,COMPLETADO_PARCIAL,RECHAZADO")
                .Annotation("Npgsql:Enum:fiscalhost_db.estado_llave", "ACTIVA,EXPIRADA,REVOCADA")
                .Annotation("Npgsql:Enum:fiscalhost_db.estado_notificacion", "PENDIENTE,ENVIADA,ENTREGADA,FALLIDA,LEIDA")
                .Annotation("Npgsql:Enum:fiscalhost_db.estado_obligacion", "VIGENTE,VENCIDA,PAGADA,EN_ARREGLO")
                .Annotation("Npgsql:Enum:fiscalhost_db.estado_ocr", "pendiente,procesado,ilegible,sin_procesar")
                .Annotation("Npgsql:Enum:fiscalhost_db.estado_usuario", "PENDIENTE_ACTIVACION,ACTIVO,INACTIVO,BLOQUEADO,ELIMINADO")
                .Annotation("Npgsql:Enum:fiscalhost_db.estado_validacion", "PENDIENTE,VALIDO,RECHAZADO,DUPLICADO")
                .Annotation("Npgsql:Enum:fiscalhost_db.fuente_ingreso", "NACIONAL,EXTRANJERA")
                .Annotation("Npgsql:Enum:fiscalhost_db.fuente_registro", "IMPORTACION_CSV,MANUAL,RECONSTRUCCION")
                .Annotation("Npgsql:Enum:fiscalhost_db.operacion_auditoria", "insert,update,delete,login,logout,export,reclasificacion,cambio_regimen,cambio_actividad_economica")
                .Annotation("Npgsql:Enum:fiscalhost_db.plataforma_origen", "AIRBNB,BOOKING,VRBO,DIRECTA,OTRA")
                .Annotation("Npgsql:Enum:fiscalhost_db.regimen_tributario", "CAPITAL_INMOBILIARIO,UTILIDADES")
                .Annotation("Npgsql:Enum:fiscalhost_db.rol_usuario", "ANFITRION,CONTADOR,ADMINISTRADOR")
                .Annotation("Npgsql:Enum:fiscalhost_db.tipo_alerta", "VENCIMIENTO_15_DIAS,VENCIMIENTO_10_DIAS,VENCIMIENTO_7_DIAS,VENCIMIENTO_3_DIAS,VENCIMIENTO_1_DIA,VENCIMIENTO_EXPIRADO,LLAVE_POR_VENCER,INFORMATIVA,SEGURIDAD")
                .Annotation("Npgsql:Enum:fiscalhost_db.tipo_formulario", "D150,D116,D176,D104")
                .Annotation("Npgsql:Enum:fiscalhost_db.tipo_identificacion", "FISICA,JURIDICA,DIMEX,NITE")
                .Annotation("Npgsql:Enum:fiscalhost_db.tipo_moneda", "CRC,USD")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.canal_notificacion", "CORREO,PLATAFORMA,AMBOS")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.clasificacion_fiscal", "GRAVADO,EXENTO,GRAVADO_CON_RETENCION")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.estado_declaracion", "BORRADOR,CALCULADO,VALIDADO,EXPORTADO,PRESENTADO")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.estado_importacion", "PENDIENTE,PROCESANDO,COMPLETADO,COMPLETADO_PARCIAL,RECHAZADO")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.estado_llave", "ACTIVA,EXPIRADA,REVOCADA")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.estado_notificacion", "PENDIENTE,ENVIADA,ENTREGADA,FALLIDA,LEIDA")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.estado_obligacion", "VIGENTE,VENCIDA,PAGADA,EN_ARREGLO")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.estado_ocr", "pendiente,procesado,ilegible,sin_procesar")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.estado_usuario", "PENDIENTE_ACTIVACION,ACTIVO,INACTIVO,BLOQUEADO,ELIMINADO")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.estado_validacion", "PENDIENTE,VALIDO,RECHAZADO,DUPLICADO")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.fuente_registro", "IMPORTACION_CSV,MANUAL,RECONSTRUCCION")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.operacion_auditoria", "insert,update,delete,login,logout,export,reclasificacion,cambio_regimen,cambio_actividad_economica")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.plataforma_origen", "AIRBNB,BOOKING,VRBO,DIRECTA,OTRA")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.regimen_tributario", "CAPITAL_INMOBILIARIO,UTILIDADES")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.rol_usuario", "ANFITRION,CONTADOR,ADMINISTRADOR")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.tipo_alerta", "VENCIMIENTO_15_DIAS,VENCIMIENTO_10_DIAS,VENCIMIENTO_7_DIAS,VENCIMIENTO_3_DIAS,VENCIMIENTO_1_DIA,VENCIMIENTO_EXPIRADO,LLAVE_POR_VENCER,INFORMATIVA,SEGURIDAD")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.tipo_formulario", "D150,D116,D176")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.tipo_identificacion", "FISICA,JURIDICA,DIMEX,NITE")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.tipo_moneda", "CRC,USD");

            migrationBuilder.AlterColumn<EstadoLlave>(
                name: "estado",
                schema: "fiscalhost_db",
                table: "llave_criptografica",
                type: "fiscalhost_db.estado_llave",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "audit_id",
                schema: "fiscalhost_db",
                table: "audit_log",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string[]>(
                name: "campos_modificados",
                schema: "fiscalhost_db",
                table: "audit_log",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<IPAddress>(
                name: "ip_origen",
                schema: "fiscalhost_db",
                table: "audit_log",
                type: "inet",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "new_values",
                schema: "fiscalhost_db",
                table: "audit_log",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "old_values",
                schema: "fiscalhost_db",
                table: "audit_log",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<OperacionAuditoria>(
                name: "operacion",
                schema: "fiscalhost_db",
                table: "audit_log",
                type: "fiscalhost_db.operacion_auditoria",
                nullable: false,
                defaultValue: OperacionAuditoria.INSERT);

            migrationBuilder.AddColumn<Guid>(
                name: "registro_id",
                schema: "fiscalhost_db",
                table: "audit_log",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "request_id",
                schema: "fiscalhost_db",
                table: "audit_log",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<RolUsuario>(
                name: "rol_usuario",
                schema: "fiscalhost_db",
                table: "audit_log",
                type: "fiscalhost_db.rol_usuario",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "usuario_id",
                schema: "fiscalhost_db",
                table: "audit_log",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_audit_log",
                schema: "fiscalhost_db",
                table: "audit_log",
                column: "audit_id");

            migrationBuilder.CreateTable(
                name: "clasificacion_ingreso",
                schema: "fiscalhost_db",
                columns: table => new
                {
                    clasificacion_ingreso_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    anfitrion_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    fecha_entrada = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_salida = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dias_estancia = table.Column<int>(type: "integer", nullable: false),
                    monto_bruto = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    fuente_ingreso = table.Column<FuenteIngreso>(type: "fiscalhost_db.fuente_ingreso", nullable: false),
                    tiene_factura_electronica_nacional = table.Column<bool>(type: "boolean", nullable: false),
                    huesped_residente = table.Column<bool>(type: "boolean", nullable: false),
                    clasificacion_iva = table.Column<ClasificacionIva>(type: "fiscalhost_db.clasificacion_iva", nullable: false),
                    monto_iva = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    base_imponible_renta = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    impuesto_renta = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    monto_retencion = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    neto_anfitrion = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    reclasificado_manualmente = table.Column<bool>(type: "boolean", nullable: false),
                    justificacion_manual = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_actualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clasificacion_ingreso", x => x.clasificacion_ingreso_id);
                });

            migrationBuilder.CreateTable(
                name: "auditoria_clasificacion_ingreso",
                schema: "fiscalhost_db",
                columns: table => new
                {
                    auditoria_clasificacion_ingreso_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    clasificacion_ingreso_id = table.Column<int>(type: "integer", nullable: false),
                    usuario_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    valor_anterior = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    valor_nuevo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    justificacion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    fecha_evento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auditoria_clasificacion_ingreso", x => x.auditoria_clasificacion_ingreso_id);
                    table.ForeignKey(
                        name: "FK_auditoria_clasificacion_ingreso_clasificacion_ingreso_clasi~",
                        column: x => x.clasificacion_ingreso_id,
                        principalSchema: "fiscalhost_db",
                        principalTable: "clasificacion_ingreso",
                        principalColumn: "clasificacion_ingreso_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_auditoria_clasificacion_ingreso_clasificacion_ingreso_id",
                schema: "fiscalhost_db",
                table: "auditoria_clasificacion_ingreso",
                column: "clasificacion_ingreso_id");

            migrationBuilder.CreateIndex(
                name: "IX_clasificacion_ingreso_anfitrion_id",
                schema: "fiscalhost_db",
                table: "clasificacion_ingreso",
                column: "anfitrion_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "auditoria_clasificacion_ingreso",
                schema: "fiscalhost_db");

            migrationBuilder.DropTable(
                name: "clasificacion_ingreso",
                schema: "fiscalhost_db");

            migrationBuilder.DropPrimaryKey(
                name: "PK_audit_log",
                schema: "fiscalhost_db",
                table: "audit_log");

            migrationBuilder.DropColumn(
                name: "audit_id",
                schema: "fiscalhost_db",
                table: "audit_log");

            migrationBuilder.DropColumn(
                name: "campos_modificados",
                schema: "fiscalhost_db",
                table: "audit_log");

            migrationBuilder.DropColumn(
                name: "ip_origen",
                schema: "fiscalhost_db",
                table: "audit_log");

            migrationBuilder.DropColumn(
                name: "new_values",
                schema: "fiscalhost_db",
                table: "audit_log");

            migrationBuilder.DropColumn(
                name: "old_values",
                schema: "fiscalhost_db",
                table: "audit_log");

            migrationBuilder.DropColumn(
                name: "operacion",
                schema: "fiscalhost_db",
                table: "audit_log");

            migrationBuilder.DropColumn(
                name: "registro_id",
                schema: "fiscalhost_db",
                table: "audit_log");

            migrationBuilder.DropColumn(
                name: "request_id",
                schema: "fiscalhost_db",
                table: "audit_log");

            migrationBuilder.DropColumn(
                name: "rol_usuario",
                schema: "fiscalhost_db",
                table: "audit_log");

            migrationBuilder.DropColumn(
                name: "usuario_id",
                schema: "fiscalhost_db",
                table: "audit_log");

            migrationBuilder.RenameTable(
                name: "audit_log",
                schema: "fiscalhost_db",
                newName: "auditoria_operacion",
                newSchema: "fiscalhost_db");

            migrationBuilder.RenameColumn(
                name: "justificacion",
                schema: "fiscalhost_db",
                table: "auditoria_operacion",
                newName: "Justificacion");

            migrationBuilder.RenameColumn(
                name: "user_agent",
                schema: "fiscalhost_db",
                table: "auditoria_operacion",
                newName: "ValorNuevo");

            migrationBuilder.RenameColumn(
                name: "tabla_afectada",
                schema: "fiscalhost_db",
                table: "auditoria_operacion",
                newName: "Usuario");

            migrationBuilder.RenameColumn(
                name: "created_at",
                schema: "fiscalhost_db",
                table: "auditoria_operacion",
                newName: "Fecha");

            migrationBuilder.RenameColumn(
                name: "correo_usuario",
                schema: "fiscalhost_db",
                table: "auditoria_operacion",
                newName: "ValorAnterior");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:fiscalhost_db.canal_notificacion", "CORREO,PLATAFORMA,AMBOS")
                .Annotation("Npgsql:Enum:fiscalhost_db.clasificacion_fiscal", "GRAVADO,EXENTO,GRAVADO_CON_RETENCION")
                .Annotation("Npgsql:Enum:fiscalhost_db.estado_declaracion", "BORRADOR,CALCULADO,VALIDADO,EXPORTADO,PRESENTADO")
                .Annotation("Npgsql:Enum:fiscalhost_db.estado_importacion", "PENDIENTE,PROCESANDO,COMPLETADO,COMPLETADO_PARCIAL,RECHAZADO")
                .Annotation("Npgsql:Enum:fiscalhost_db.estado_llave", "ACTIVA,EXPIRADA,REVOCADA")
                .Annotation("Npgsql:Enum:fiscalhost_db.estado_notificacion", "PENDIENTE,ENVIADA,ENTREGADA,FALLIDA,LEIDA")
                .Annotation("Npgsql:Enum:fiscalhost_db.estado_obligacion", "VIGENTE,VENCIDA,PAGADA,EN_ARREGLO")
                .Annotation("Npgsql:Enum:fiscalhost_db.estado_ocr", "pendiente,procesado,ilegible,sin_procesar")
                .Annotation("Npgsql:Enum:fiscalhost_db.estado_usuario", "PENDIENTE_ACTIVACION,ACTIVO,INACTIVO,BLOQUEADO,ELIMINADO")
                .Annotation("Npgsql:Enum:fiscalhost_db.estado_validacion", "PENDIENTE,VALIDO,RECHAZADO,DUPLICADO")
                .Annotation("Npgsql:Enum:fiscalhost_db.fuente_registro", "IMPORTACION_CSV,MANUAL,RECONSTRUCCION")
                .Annotation("Npgsql:Enum:fiscalhost_db.operacion_auditoria", "insert,update,delete,login,logout,export,reclasificacion,cambio_regimen,cambio_actividad_economica")
                .Annotation("Npgsql:Enum:fiscalhost_db.plataforma_origen", "AIRBNB,BOOKING,VRBO,DIRECTA,OTRA")
                .Annotation("Npgsql:Enum:fiscalhost_db.regimen_tributario", "CAPITAL_INMOBILIARIO,UTILIDADES")
                .Annotation("Npgsql:Enum:fiscalhost_db.rol_usuario", "ANFITRION,CONTADOR,ADMINISTRADOR")
                .Annotation("Npgsql:Enum:fiscalhost_db.tipo_alerta", "VENCIMIENTO_15_DIAS,VENCIMIENTO_10_DIAS,VENCIMIENTO_7_DIAS,VENCIMIENTO_3_DIAS,VENCIMIENTO_1_DIA,VENCIMIENTO_EXPIRADO,LLAVE_POR_VENCER,INFORMATIVA,SEGURIDAD")
                .Annotation("Npgsql:Enum:fiscalhost_db.tipo_formulario", "D150,D116,D176")
                .Annotation("Npgsql:Enum:fiscalhost_db.tipo_identificacion", "FISICA,JURIDICA,DIMEX,NITE")
                .Annotation("Npgsql:Enum:fiscalhost_db.tipo_moneda", "CRC,USD")
                .OldAnnotation("Npgsql:Enum:estado_llave.fiscalhost_db", "ACTIVA,EXPIRADA,REVOCADA")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.canal_notificacion", "CORREO,PLATAFORMA,AMBOS")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.clasificacion_fiscal", "GRAVADO,EXENTO,GRAVADO_CON_RETENCION")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.clasificacion_iva", "GRAVADO_13,EXENTO")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.estado_declaracion", "BORRADOR,CALCULADO,VALIDADO,EXPORTADO,PRESENTADO")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.estado_importacion", "PENDIENTE,PROCESANDO,COMPLETADO,COMPLETADO_PARCIAL,RECHAZADO")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.estado_llave", "ACTIVA,EXPIRADA,REVOCADA")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.estado_notificacion", "PENDIENTE,ENVIADA,ENTREGADA,FALLIDA,LEIDA")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.estado_obligacion", "VIGENTE,VENCIDA,PAGADA,EN_ARREGLO")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.estado_ocr", "pendiente,procesado,ilegible,sin_procesar")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.estado_usuario", "PENDIENTE_ACTIVACION,ACTIVO,INACTIVO,BLOQUEADO,ELIMINADO")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.estado_validacion", "PENDIENTE,VALIDO,RECHAZADO,DUPLICADO")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.fuente_ingreso", "NACIONAL,EXTRANJERA")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.fuente_registro", "IMPORTACION_CSV,MANUAL,RECONSTRUCCION")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.operacion_auditoria", "insert,update,delete,login,logout,export,reclasificacion,cambio_regimen,cambio_actividad_economica")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.plataforma_origen", "AIRBNB,BOOKING,VRBO,DIRECTA,OTRA")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.regimen_tributario", "CAPITAL_INMOBILIARIO,UTILIDADES")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.rol_usuario", "ANFITRION,CONTADOR,ADMINISTRADOR")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.tipo_alerta", "VENCIMIENTO_15_DIAS,VENCIMIENTO_10_DIAS,VENCIMIENTO_7_DIAS,VENCIMIENTO_3_DIAS,VENCIMIENTO_1_DIA,VENCIMIENTO_EXPIRADO,LLAVE_POR_VENCER,INFORMATIVA,SEGURIDAD")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.tipo_formulario", "D150,D116,D176,D104")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.tipo_identificacion", "FISICA,JURIDICA,DIMEX,NITE")
                .OldAnnotation("Npgsql:Enum:fiscalhost_db.tipo_moneda", "CRC,USD");

            migrationBuilder.AlterColumn<string>(
                name: "estado",
                schema: "fiscalhost_db",
                table: "llave_criptografica",
                type: "text",
                nullable: false,
                oldClrType: typeof(EstadoLlave),
                oldType: "fiscalhost_db.estado_llave");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                schema: "fiscalhost_db",
                table: "auditoria_operacion",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "Accion",
                schema: "fiscalhost_db",
                table: "auditoria_operacion",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                schema: "fiscalhost_db",
                table: "auditoria_operacion",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Entidad",
                schema: "fiscalhost_db",
                table: "auditoria_operacion",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "EntidadId",
                schema: "fiscalhost_db",
                table: "auditoria_operacion",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_auditoria_operacion",
                schema: "fiscalhost_db",
                table: "auditoria_operacion",
                column: "Id");
        }
    }
}
