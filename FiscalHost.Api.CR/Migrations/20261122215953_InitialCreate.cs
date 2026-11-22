using System;
using System.Net;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FiscalHost.Api.CR.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "fiscalhost_db");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:estado_llave.fiscalhost_db", "ACTIVA,EXPIRADA,REVOCADA");

            migrationBuilder.CreateTable(
                name: "audit_log",
                schema: "fiscalhost_db",
                columns: table => new
                {
                    audit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    correo_usuario = table.Column<string>(type: "text", nullable: true),
                    rol_usuario = table.Column<int>(type: "integer", nullable: true),
                    operacion = table.Column<int>(type: "integer", nullable: false),
                    tabla_afectada = table.Column<string>(type: "text", nullable: false),
                    registro_id = table.Column<Guid>(type: "uuid", nullable: true),
                    old_values = table.Column<string>(type: "jsonb", nullable: true),
                    new_values = table.Column<string>(type: "jsonb", nullable: true),
                    campos_modificados = table.Column<string[]>(type: "text[]", nullable: true),
                    justificacion = table.Column<string>(type: "text", nullable: true),
                    ip_origen = table.Column<IPAddress>(type: "inet", nullable: true),
                    user_agent = table.Column<string>(type: "text", nullable: true),
                    request_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_log", x => x.audit_id);
                });

            migrationBuilder.CreateTable(
                name: "catalogo_actividad_economica",
                schema: "fiscalhost_db",
                columns: table => new
                {
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: false),
                    seccion = table.Column<string>(type: "text", nullable: true),
                    tarifa_iva = table.Column<decimal>(type: "numeric", nullable: false),
                    vigente = table.Column<bool>(type: "boolean", nullable: false),
                    fecha_vigencia_desde = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_vigencia_hasta = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_catalogo_actividad_economica", x => x.codigo);
                });

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
                    fuente_ingreso = table.Column<int>(type: "integer", nullable: false),
                    tiene_factura_electronica_nacional = table.Column<bool>(type: "boolean", nullable: false),
                    huesped_residente = table.Column<bool>(type: "boolean", nullable: false),
                    clasificacion_iva = table.Column<int>(type: "integer", nullable: false),
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
                name: "gasto_operativo",
                schema: "fiscalhost_db",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AnfitrionId = table.Column<string>(type: "text", nullable: false),
                    Proveedor = table.Column<string>(type: "text", nullable: false),
                    NumeroFactura = table.Column<string>(type: "text", nullable: false),
                    Monto = table.Column<decimal>(type: "numeric", nullable: false),
                    ComprobanteUrl = table.Column<string>(type: "text", nullable: true),
                    FechaGasto = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gasto_operativo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "periodo_fiscal",
                schema: "fiscalhost_db",
                columns: table => new
                {
                    periodo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    anio = table.Column<short>(type: "smallint", nullable: false),
                    mes = table.Column<short>(type: "smallint", nullable: false),
                    tipo_formulario = table.Column<int>(type: "integer", nullable: false),
                    fecha_inicio_periodo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_fin_periodo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_vencimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tarifa_iva = table.Column<decimal>(type: "numeric", nullable: false),
                    tarifa_renta_capital = table.Column<decimal>(type: "numeric", nullable: false),
                    deduccion_plana_capital = table.Column<decimal>(type: "numeric", nullable: false),
                    tasa_interes_mora_anual = table.Column<decimal>(type: "numeric", nullable: true),
                    salario_base_vigente = table.Column<decimal>(type: "numeric", nullable: true),
                    normativa_aplicable = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_periodo_fiscal", x => x.periodo_id);
                });

            migrationBuilder.CreateTable(
                name: "reserva_directa",
                schema: "fiscalhost_db",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AnfitrionId = table.Column<string>(type: "text", nullable: false),
                    FechaReserva = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Monto = table.Column<decimal>(type: "numeric", nullable: false),
                    Huesped = table.Column<string>(type: "text", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reserva_directa", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "usuario",
                schema: "fiscalhost_db",
                columns: table => new
                {
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo_identificacion = table.Column<int>(type: "integer", nullable: false),
                    numero_identificacion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    nombre_completo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    razon_social = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    correo_electronico = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    contrasena_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    rol_principal = table.Column<int>(type: "integer", nullable: false),
                    es_usuario_nuevo = table.Column<bool>(type: "boolean", nullable: false),
                    correo_verificado = table.Column<bool>(type: "boolean", nullable: false),
                    preferencias_notificacion = table.Column<string>(type: "jsonb", nullable: false),
                    fecha_activacion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ultimo_acceso = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuario", x => x.usuario_id);
                });

            migrationBuilder.CreateTable(
                name: "configuracion_tributaria",
                schema: "fiscalhost_db",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AnfitrionId = table.Column<string>(type: "text", nullable: false),
                    CodigoActividad = table.Column<string>(type: "text", nullable: false),
                    ActividadEconomicaCodigo = table.Column<string>(type: "character varying(20)", nullable: true),
                    TribuCr = table.Column<string>(type: "text", nullable: false),
                    DireccionFiscal = table.Column<string>(type: "text", nullable: false),
                    Nise = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_configuracion_tributaria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_configuracion_tributaria_catalogo_actividad_economica_Activ~",
                        column: x => x.ActividadEconomicaCodigo,
                        principalSchema: "fiscalhost_db",
                        principalTable: "catalogo_actividad_economica",
                        principalColumn: "codigo");
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

            migrationBuilder.CreateTable(
                name: "acceso_contador",
                schema: "fiscalhost_db",
                columns: table => new
                {
                    acceso_id = table.Column<Guid>(type: "uuid", nullable: false),
                    anfitrion_id = table.Column<Guid>(type: "uuid", nullable: false),
                    contador_id = table.Column<Guid>(type: "uuid", nullable: true),
                    correo_contador = table.Column<string>(type: "text", nullable: false),
                    permisos = table.Column<string>(type: "jsonb", nullable: false),
                    fecha_invitacion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    fecha_aceptacion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    fecha_expiracion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    fecha_revocacion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    estado = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_acceso_contador", x => x.acceso_id);
                    table.ForeignKey(
                        name: "FK_acceso_contador_usuario_anfitrion_id",
                        column: x => x.anfitrion_id,
                        principalSchema: "fiscalhost_db",
                        principalTable: "usuario",
                        principalColumn: "usuario_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_acceso_contador_usuario_contador_id",
                        column: x => x.contador_id,
                        principalSchema: "fiscalhost_db",
                        principalTable: "usuario",
                        principalColumn: "usuario_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "calculo_fiscal",
                schema: "fiscalhost_db",
                columns: table => new
                {
                    calculo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    periodo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo_formulario = table.Column<int>(type: "integer", nullable: false),
                    regimen_aplicado = table.Column<int>(type: "integer", nullable: true),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    total_ingresos_brutos = table.Column<decimal>(type: "numeric", nullable: false),
                    total_ingresos_gravados = table.Column<decimal>(type: "numeric", nullable: false),
                    total_ingresos_exentos = table.Column<decimal>(type: "numeric", nullable: false),
                    debito_fiscal = table.Column<decimal>(type: "numeric", nullable: false),
                    credito_fiscal = table.Column<decimal>(type: "numeric", nullable: false),
                    iva_neto = table.Column<decimal>(type: "numeric", nullable: false),
                    saldo_favor_anterior = table.Column<decimal>(type: "numeric", nullable: false),
                    saldo_favor_resultante = table.Column<decimal>(type: "numeric", nullable: false),
                    renta_bruta = table.Column<decimal>(type: "numeric", nullable: true),
                    deduccion_aplicada = table.Column<decimal>(type: "numeric", nullable: true),
                    renta_neta = table.Column<decimal>(type: "numeric", nullable: true),
                    impuesto_renta = table.Column<decimal>(type: "numeric", nullable: true),
                    retenciones_acreditadas = table.Column<decimal>(type: "numeric", nullable: true),
                    monto_total_a_pagar = table.Column<decimal>(type: "numeric", nullable: false),
                    detalle_calculo = table.Column<string>(type: "jsonb", nullable: false),
                    borrador_generado = table.Column<bool>(type: "boolean", nullable: false),
                    fecha_generacion_borrador = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_calculo_fiscal", x => x.calculo_id);
                    table.ForeignKey(
                        name: "FK_calculo_fiscal_periodo_fiscal_periodo_id",
                        column: x => x.periodo_id,
                        principalSchema: "fiscalhost_db",
                        principalTable: "periodo_fiscal",
                        principalColumn: "periodo_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_calculo_fiscal_usuario_usuario_id",
                        column: x => x.usuario_id,
                        principalSchema: "fiscalhost_db",
                        principalTable: "usuario",
                        principalColumn: "usuario_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contenido_educativo",
                schema: "fiscalhost_db",
                columns: table => new
                {
                    contenido_id = table.Column<Guid>(type: "uuid", nullable: false),
                    titulo = table.Column<string>(type: "text", nullable: false),
                    slug = table.Column<string>(type: "text", nullable: false),
                    categoria = table.Column<string>(type: "text", nullable: false),
                    tipo = table.Column<string>(type: "text", nullable: false),
                    contenido_markdown = table.Column<string>(type: "text", nullable: false),
                    contenido_html = table.Column<string>(type: "text", nullable: true),
                    es_tutorial_primer_uso = table.Column<bool>(type: "boolean", nullable: false),
                    orden_display = table.Column<int>(type: "integer", nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    publicado = table.Column<bool>(type: "boolean", nullable: false),
                    autor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    published_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contenido_educativo", x => x.contenido_id);
                    table.ForeignKey(
                        name: "FK_contenido_educativo_usuario_autor_id",
                        column: x => x.autor_id,
                        principalSchema: "fiscalhost_db",
                        principalTable: "usuario",
                        principalColumn: "usuario_id");
                });

            migrationBuilder.CreateTable(
                name: "importacion_masiva",
                schema: "fiscalhost_db",
                columns: table => new
                {
                    importacion_id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo_importacion = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    plataforma_origen = table.Column<int>(type: "integer", nullable: true),
                    archivo_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    nombre_archivo_original = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    plantilla_utilizada = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    tamanio_bytes = table.Column<long>(type: "bigint", nullable: true),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    total_registros = table.Column<int>(type: "integer", nullable: true),
                    registros_exitosos = table.Column<int>(type: "integer", nullable: true),
                    registros_con_error = table.Column<int>(type: "integer", nullable: true),
                    reporte_errores_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    detalle_errores = table.Column<string>(type: "jsonb", nullable: false),
                    fecha_carga = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    fecha_inicio_procesamiento = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    fecha_fin_procesamiento = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_importacion_masiva", x => x.importacion_id);
                    table.ForeignKey(
                        name: "FK_importacion_masiva_usuario_usuario_id",
                        column: x => x.usuario_id,
                        principalSchema: "fiscalhost_db",
                        principalTable: "usuario",
                        principalColumn: "usuario_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "llave_criptografica",
                schema: "fiscalhost_db",
                columns: table => new
                {
                    llave_id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre_archivo = table.Column<string>(type: "text", nullable: false),
                    ruta_blob_storage = table.Column<string>(type: "text", nullable: false),
                    hash_integridad = table.Column<string>(type: "text", nullable: false),
                    referencia_key_vault = table.Column<string>(type: "text", nullable: true),
                    huella_digital_certificado = table.Column<string>(type: "text", nullable: true),
                    fecha_emision_certificado = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    fecha_expiracion_certificado = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    emisor_certificado = table.Column<string>(type: "text", nullable: true),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    fecha_carga = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ultima_actualizacion_contrasena = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_llave_criptografica", x => x.llave_id);
                    table.ForeignKey(
                        name: "FK_llave_criptografica_usuario_usuario_id",
                        column: x => x.usuario_id,
                        principalSchema: "fiscalhost_db",
                        principalTable: "usuario",
                        principalColumn: "usuario_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "perfil_tributario",
                schema: "fiscalhost_db",
                columns: table => new
                {
                    perfil_id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    codigo_actividad_economica = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    descripcion_actividad = table.Column<string>(type: "text", nullable: false),
                    tribu_cr = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    direccion_fiscal = table.Column<string>(type: "text", nullable: true),
                    nise = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    es_domicilio_validado = table.Column<bool>(type: "boolean", nullable: false),
                    regimen_tributario = table.Column<int>(type: "integer", nullable: false),
                    fecha_inicio_actividad = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    fecha_inscripcion_hacienda = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    datos_complementarios = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_perfil_tributario", x => x.perfil_id);
                    table.ForeignKey(
                        name: "FK_perfil_tributario_catalogo_actividad_economica_codigo_activ~",
                        column: x => x.codigo_actividad_economica,
                        principalSchema: "fiscalhost_db",
                        principalTable: "catalogo_actividad_economica",
                        principalColumn: "codigo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_perfil_tributario_usuario_usuario_id",
                        column: x => x.usuario_id,
                        principalSchema: "fiscalhost_db",
                        principalTable: "usuario",
                        principalColumn: "usuario_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "propiedad",
                schema: "fiscalhost_db",
                columns: table => new
                {
                    propiedad_id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    direccion = table.Column<string>(type: "text", nullable: true),
                    provincia = table.Column<string>(type: "text", nullable: true),
                    canton = table.Column<string>(type: "text", nullable: true),
                    distrito = table.Column<string>(type: "text", nullable: true),
                    numero_finca = table.Column<string>(type: "text", nullable: true),
                    valor_fiscal = table.Column<decimal>(type: "numeric", nullable: true),
                    tipo_moneda_valor = table.Column<int>(type: "integer", nullable: true),
                    activa = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_propiedad", x => x.propiedad_id);
                    table.ForeignKey(
                        name: "FK_propiedad_usuario_usuario_id",
                        column: x => x.usuario_id,
                        principalSchema: "fiscalhost_db",
                        principalTable: "usuario",
                        principalColumn: "usuario_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reconstruccion_historica",
                schema: "fiscalhost_db",
                columns: table => new
                {
                    reconstruccion_id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre_archivo_original = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    tamanio_bytes = table.Column<long>(type: "bigint", nullable: false),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    total_registros = table.Column<int>(type: "integer", nullable: false),
                    registros_exitosos = table.Column<int>(type: "integer", nullable: false),
                    registros_con_error = table.Column<int>(type: "integer", nullable: false),
                    detalle_errores = table.Column<string>(type: "jsonb", nullable: false),
                    reporte_errores_url = table.Column<string>(type: "text", nullable: true),
                    fecha_inicio_procesamiento = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    fecha_fin_procesamiento = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reconstruccion_historica", x => x.reconstruccion_id);
                    table.ForeignKey(
                        name: "FK_reconstruccion_historica_usuario_usuario_id",
                        column: x => x.usuario_id,
                        principalSchema: "fiscalhost_db",
                        principalTable: "usuario",
                        principalColumn: "usuario_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "simulacion_fiscal",
                schema: "fiscalhost_db",
                columns: table => new
                {
                    simulacion_id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "text", nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    periodo_base_anio = table.Column<short>(type: "smallint", nullable: false),
                    periodo_base_mes = table.Column<short>(type: "smallint", nullable: true),
                    parametros_entrada = table.Column<string>(type: "jsonb", nullable: false),
                    resultados = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_simulacion_fiscal", x => x.simulacion_id);
                    table.ForeignKey(
                        name: "FK_simulacion_fiscal_usuario_usuario_id",
                        column: x => x.usuario_id,
                        principalSchema: "fiscalhost_db",
                        principalTable: "usuario",
                        principalColumn: "usuario_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "auditoria_configuracion",
                schema: "fiscalhost_db",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ConfiguracionTributariaId = table.Column<int>(type: "integer", nullable: false),
                    Campo = table.Column<string>(type: "text", nullable: false),
                    ValorAnterior = table.Column<string>(type: "text", nullable: false),
                    ValorNuevo = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    FechaEvento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auditoria_configuracion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_auditoria_configuracion_configuracion_tributaria_Configurac~",
                        column: x => x.ConfiguracionTributariaId,
                        principalSchema: "fiscalhost_db",
                        principalTable: "configuracion_tributaria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "exportacion",
                schema: "fiscalhost_db",
                columns: table => new
                {
                    exportacion_id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    calculo_id = table.Column<Guid>(type: "uuid", nullable: true),
                    CalculoFiscalCalculoId = table.Column<Guid>(type: "uuid", nullable: true),
                    formato = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    tipo_contenido = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    archivo_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    nombre_archivo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    tamanio_bytes = table.Column<long>(type: "bigint", nullable: true),
                    esta_protegido = table.Column<bool>(type: "boolean", nullable: false),
                    expira_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exportacion", x => x.exportacion_id);
                    table.ForeignKey(
                        name: "FK_exportacion_calculo_fiscal_CalculoFiscalCalculoId",
                        column: x => x.CalculoFiscalCalculoId,
                        principalSchema: "fiscalhost_db",
                        principalTable: "calculo_fiscal",
                        principalColumn: "calculo_id");
                    table.ForeignKey(
                        name: "FK_exportacion_usuario_usuario_id",
                        column: x => x.usuario_id,
                        principalSchema: "fiscalhost_db",
                        principalTable: "usuario",
                        principalColumn: "usuario_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "obligacion_tributaria",
                schema: "fiscalhost_db",
                columns: table => new
                {
                    obligacion_id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    calculo_id = table.Column<Guid>(type: "uuid", nullable: true),
                    periodo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo_formulario = table.Column<int>(type: "integer", nullable: false),
                    descripcion = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    monto_capital = table.Column<decimal>(type: "numeric", nullable: false),
                    monto_multa = table.Column<decimal>(type: "numeric", nullable: false),
                    monto_intereses_acumulados = table.Column<decimal>(type: "numeric", nullable: false),
                    monto_total_actualizado = table.Column<decimal>(type: "numeric", nullable: false),
                    fecha_vencimiento = table.Column<DateOnly>(type: "date", nullable: false),
                    fecha_pago = table.Column<DateOnly>(type: "date", nullable: true),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    tasa_interes_aplicada = table.Column<decimal>(type: "numeric", nullable: true),
                    fecha_ultimo_calculo_interes = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    historial_intereses = table.Column<string>(type: "jsonb", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_obligacion_tributaria", x => x.obligacion_id);
                    table.ForeignKey(
                        name: "FK_obligacion_tributaria_calculo_fiscal_calculo_id",
                        column: x => x.calculo_id,
                        principalSchema: "fiscalhost_db",
                        principalTable: "calculo_fiscal",
                        principalColumn: "calculo_id");
                    table.ForeignKey(
                        name: "FK_obligacion_tributaria_periodo_fiscal_periodo_id",
                        column: x => x.periodo_id,
                        principalSchema: "fiscalhost_db",
                        principalTable: "periodo_fiscal",
                        principalColumn: "periodo_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_obligacion_tributaria_usuario_usuario_id",
                        column: x => x.usuario_id,
                        principalSchema: "fiscalhost_db",
                        principalTable: "usuario",
                        principalColumn: "usuario_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "auditoria_llave",
                schema: "fiscalhost_db",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LlaveCriptograficaId = table.Column<int>(type: "integer", nullable: false),
                    LlaveCriptograficaLlaveId = table.Column<Guid>(type: "uuid", nullable: false),
                    Accion = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    FechaEvento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auditoria_llave", x => x.Id);
                    table.ForeignKey(
                        name: "FK_auditoria_llave_llave_criptografica_LlaveCriptograficaLlave~",
                        column: x => x.LlaveCriptograficaLlaveId,
                        principalSchema: "fiscalhost_db",
                        principalTable: "llave_criptografica",
                        principalColumn: "llave_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gasto",
                schema: "fiscalhost_db",
                columns: table => new
                {
                    gasto_id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    propiedad_id = table.Column<Guid>(type: "uuid", nullable: true),
                    proveedor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    numero_factura = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    clave_numerica_hacienda = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    fecha_emision = table.Column<DateOnly>(type: "date", nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    monto_total = table.Column<decimal>(type: "numeric", nullable: false),
                    monto_iva_soportado = table.Column<decimal>(type: "numeric", nullable: false),
                    monto_neto = table.Column<decimal>(type: "numeric", nullable: true),
                    moneda = table.Column<int>(type: "integer", nullable: false),
                    tipo_cambio = table.Column<decimal>(type: "numeric", nullable: true),
                    monto_colones = table.Column<decimal>(type: "numeric", nullable: false),
                    tipo_gasto = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    es_deducible_renta = table.Column<bool>(type: "boolean", nullable: false),
                    es_credito_fiscal_valido = table.Column<bool>(type: "boolean", nullable: false),
                    evidencia_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    evidencia_nombre_archivo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    evidencia_tipo_mime = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    evidencia_tamanio_bytes = table.Column<long>(type: "bigint", nullable: true),
                    estado_ocr = table.Column<int>(type: "integer", nullable: true),
                    datos_extraidos_ocr = table.Column<string>(type: "jsonb", nullable: false),
                    hash_unico_comprobante = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    estado_validacion = table.Column<int>(type: "integer", nullable: false),
                    observaciones_validacion = table.Column<string>(type: "text", nullable: true),
                    periodo_fiscal_anio = table.Column<short>(type: "smallint", nullable: false),
                    periodo_fiscal_mes = table.Column<short>(type: "smallint", nullable: false),
                    fuente_registro = table.Column<int>(type: "integer", nullable: false),
                    fecha_registro = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gasto", x => x.gasto_id);
                    table.ForeignKey(
                        name: "FK_gasto_propiedad_propiedad_id",
                        column: x => x.propiedad_id,
                        principalSchema: "fiscalhost_db",
                        principalTable: "propiedad",
                        principalColumn: "propiedad_id");
                    table.ForeignKey(
                        name: "FK_gasto_usuario_usuario_id",
                        column: x => x.usuario_id,
                        principalSchema: "fiscalhost_db",
                        principalTable: "usuario",
                        principalColumn: "usuario_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reserva",
                schema: "fiscalhost_db",
                columns: table => new
                {
                    reserva_id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    propiedad_id = table.Column<Guid>(type: "uuid", nullable: true),
                    importacion_id = table.Column<Guid>(type: "uuid", nullable: true),
                    fecha_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_fin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    nombre_huesped = table.Column<string>(type: "text", nullable: true),
                    identificacion_huesped = table.Column<string>(type: "text", nullable: true),
                    pais_origen_huesped = table.Column<string>(type: "text", nullable: true),
                    monto_bruto = table.Column<decimal>(type: "numeric", nullable: false),
                    moneda = table.Column<int>(type: "integer", nullable: false),
                    tipo_cambio = table.Column<decimal>(type: "numeric", nullable: false),
                    monto_colones = table.Column<decimal>(type: "numeric", nullable: false),
                    clasificacion_fiscal = table.Column<int>(type: "integer", nullable: false),
                    monto_gravado = table.Column<decimal>(type: "numeric", nullable: false),
                    monto_exento = table.Column<decimal>(type: "numeric", nullable: false),
                    monto_iva_calculado = table.Column<decimal>(type: "numeric", nullable: false),
                    retencion_extranjera = table.Column<decimal>(type: "numeric", nullable: false),
                    plataforma_origen = table.Column<int>(type: "integer", nullable: false),
                    fuente_registro = table.Column<int>(type: "integer", nullable: false),
                    referencia_plataforma = table.Column<string>(type: "text", nullable: true),
                    fue_reclasificada = table.Column<bool>(type: "boolean", nullable: false),
                    justificacion_reclasificacion = table.Column<string>(type: "text", nullable: true),
                    fecha_reclasificacion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    usuario_reclasificacion_id = table.Column<Guid>(type: "uuid", nullable: true),
                    periodo_fiscal_anio = table.Column<short>(type: "smallint", nullable: false),
                    periodo_fiscal_mes = table.Column<short>(type: "smallint", nullable: false),
                    estado = table.Column<string>(type: "text", nullable: false),
                    metadata = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reserva", x => x.reserva_id);
                    table.ForeignKey(
                        name: "FK_reserva_importacion_masiva_importacion_id",
                        column: x => x.importacion_id,
                        principalSchema: "fiscalhost_db",
                        principalTable: "importacion_masiva",
                        principalColumn: "importacion_id");
                    table.ForeignKey(
                        name: "FK_reserva_propiedad_propiedad_id",
                        column: x => x.propiedad_id,
                        principalSchema: "fiscalhost_db",
                        principalTable: "propiedad",
                        principalColumn: "propiedad_id");
                    table.ForeignKey(
                        name: "FK_reserva_usuario_usuario_id",
                        column: x => x.usuario_id,
                        principalSchema: "fiscalhost_db",
                        principalTable: "usuario",
                        principalColumn: "usuario_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "alerta",
                schema: "fiscalhost_db",
                columns: table => new
                {
                    alerta_id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    obligacion_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tipo_alerta = table.Column<int>(type: "integer", nullable: false),
                    titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    mensaje = table.Column<string>(type: "text", nullable: false),
                    prioridad = table.Column<short>(type: "smallint", nullable: false),
                    monto_estimado = table.Column<decimal>(type: "numeric", nullable: true),
                    canal = table.Column<int>(type: "integer", nullable: false),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    accion_sugerida = table.Column<string>(type: "jsonb", nullable: false),
                    fecha_programada = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    fecha_envio = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    fecha_lectura = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    error_envio = table.Column<string>(type: "text", nullable: true),
                    intentos_envio = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alerta", x => x.alerta_id);
                    table.ForeignKey(
                        name: "FK_alerta_obligacion_tributaria_obligacion_id",
                        column: x => x.obligacion_id,
                        principalSchema: "fiscalhost_db",
                        principalTable: "obligacion_tributaria",
                        principalColumn: "obligacion_id");
                    table.ForeignKey(
                        name: "FK_alerta_usuario_usuario_id",
                        column: x => x.usuario_id,
                        principalSchema: "fiscalhost_db",
                        principalTable: "usuario",
                        principalColumn: "usuario_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sancion_autoliquidacion",
                schema: "fiscalhost_db",
                columns: table => new
                {
                    sancion_id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    obligacion_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tipo_sancion = table.Column<string>(type: "text", nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: false),
                    fecha_omision = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    monto_base_adeudado = table.Column<decimal>(type: "numeric", nullable: false),
                    multa_base_calculada = table.Column<decimal>(type: "numeric", nullable: false),
                    porcentaje_reduccion = table.Column<decimal>(type: "numeric", nullable: false),
                    monto_reduccion = table.Column<decimal>(type: "numeric", nullable: false),
                    multa_reducida = table.Column<decimal>(type: "numeric", nullable: false),
                    intereses_acumulados = table.Column<decimal>(type: "numeric", nullable: false),
                    monto_total_pagar = table.Column<decimal>(type: "numeric", nullable: false),
                    detalle_calculo = table.Column<string>(type: "jsonb", nullable: false),
                    estado = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sancion_autoliquidacion", x => x.sancion_id);
                    table.ForeignKey(
                        name: "FK_sancion_autoliquidacion_obligacion_tributaria_obligacion_id",
                        column: x => x.obligacion_id,
                        principalSchema: "fiscalhost_db",
                        principalTable: "obligacion_tributaria",
                        principalColumn: "obligacion_id");
                    table.ForeignKey(
                        name: "FK_sancion_autoliquidacion_usuario_usuario_id",
                        column: x => x.usuario_id,
                        principalSchema: "fiscalhost_db",
                        principalTable: "usuario",
                        principalColumn: "usuario_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_acceso_contador_anfitrion_id",
                schema: "fiscalhost_db",
                table: "acceso_contador",
                column: "anfitrion_id");

            migrationBuilder.CreateIndex(
                name: "IX_acceso_contador_contador_id",
                schema: "fiscalhost_db",
                table: "acceso_contador",
                column: "contador_id");

            migrationBuilder.CreateIndex(
                name: "IX_alerta_obligacion_id",
                schema: "fiscalhost_db",
                table: "alerta",
                column: "obligacion_id");

            migrationBuilder.CreateIndex(
                name: "IX_alerta_usuario_id",
                schema: "fiscalhost_db",
                table: "alerta",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_auditoria_clasificacion_ingreso_clasificacion_ingreso_id",
                schema: "fiscalhost_db",
                table: "auditoria_clasificacion_ingreso",
                column: "clasificacion_ingreso_id");

            migrationBuilder.CreateIndex(
                name: "IX_auditoria_configuracion_ConfiguracionTributariaId",
                schema: "fiscalhost_db",
                table: "auditoria_configuracion",
                column: "ConfiguracionTributariaId");

            migrationBuilder.CreateIndex(
                name: "IX_auditoria_llave_LlaveCriptograficaLlaveId",
                schema: "fiscalhost_db",
                table: "auditoria_llave",
                column: "LlaveCriptograficaLlaveId");

            migrationBuilder.CreateIndex(
                name: "IX_calculo_fiscal_periodo_id",
                schema: "fiscalhost_db",
                table: "calculo_fiscal",
                column: "periodo_id");

            migrationBuilder.CreateIndex(
                name: "IX_calculo_fiscal_usuario_id",
                schema: "fiscalhost_db",
                table: "calculo_fiscal",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_catalogo_actividad_economica_codigo",
                schema: "fiscalhost_db",
                table: "catalogo_actividad_economica",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_clasificacion_ingreso_anfitrion_id",
                schema: "fiscalhost_db",
                table: "clasificacion_ingreso",
                column: "anfitrion_id");

            migrationBuilder.CreateIndex(
                name: "IX_configuracion_tributaria_ActividadEconomicaCodigo",
                schema: "fiscalhost_db",
                table: "configuracion_tributaria",
                column: "ActividadEconomicaCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_contenido_educativo_autor_id",
                schema: "fiscalhost_db",
                table: "contenido_educativo",
                column: "autor_id");

            migrationBuilder.CreateIndex(
                name: "IX_exportacion_CalculoFiscalCalculoId",
                schema: "fiscalhost_db",
                table: "exportacion",
                column: "CalculoFiscalCalculoId");

            migrationBuilder.CreateIndex(
                name: "IX_exportacion_usuario_id",
                schema: "fiscalhost_db",
                table: "exportacion",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_gasto_propiedad_id",
                schema: "fiscalhost_db",
                table: "gasto",
                column: "propiedad_id");

            migrationBuilder.CreateIndex(
                name: "IX_gasto_usuario_id",
                schema: "fiscalhost_db",
                table: "gasto",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_importacion_masiva_usuario_id",
                schema: "fiscalhost_db",
                table: "importacion_masiva",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_llave_criptografica_usuario_id",
                schema: "fiscalhost_db",
                table: "llave_criptografica",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_obligacion_tributaria_calculo_id",
                schema: "fiscalhost_db",
                table: "obligacion_tributaria",
                column: "calculo_id");

            migrationBuilder.CreateIndex(
                name: "IX_obligacion_tributaria_periodo_id",
                schema: "fiscalhost_db",
                table: "obligacion_tributaria",
                column: "periodo_id");

            migrationBuilder.CreateIndex(
                name: "IX_obligacion_tributaria_usuario_id",
                schema: "fiscalhost_db",
                table: "obligacion_tributaria",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_perfil_tributario_codigo_actividad_economica",
                schema: "fiscalhost_db",
                table: "perfil_tributario",
                column: "codigo_actividad_economica");

            migrationBuilder.CreateIndex(
                name: "IX_perfil_tributario_usuario_id",
                schema: "fiscalhost_db",
                table: "perfil_tributario",
                column: "usuario_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_propiedad_usuario_id",
                schema: "fiscalhost_db",
                table: "propiedad",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_reconstruccion_historica_usuario_id",
                schema: "fiscalhost_db",
                table: "reconstruccion_historica",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_reserva_importacion_id",
                schema: "fiscalhost_db",
                table: "reserva",
                column: "importacion_id");

            migrationBuilder.CreateIndex(
                name: "IX_reserva_propiedad_id",
                schema: "fiscalhost_db",
                table: "reserva",
                column: "propiedad_id");

            migrationBuilder.CreateIndex(
                name: "IX_reserva_usuario_id",
                schema: "fiscalhost_db",
                table: "reserva",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_sancion_autoliquidacion_obligacion_id",
                schema: "fiscalhost_db",
                table: "sancion_autoliquidacion",
                column: "obligacion_id");

            migrationBuilder.CreateIndex(
                name: "IX_sancion_autoliquidacion_usuario_id",
                schema: "fiscalhost_db",
                table: "sancion_autoliquidacion",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_simulacion_fiscal_usuario_id",
                schema: "fiscalhost_db",
                table: "simulacion_fiscal",
                column: "usuario_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "acceso_contador",
                schema: "fiscalhost_db");

            migrationBuilder.DropTable(
                name: "alerta",
                schema: "fiscalhost_db");

            migrationBuilder.DropTable(
                name: "audit_log",
                schema: "fiscalhost_db");

            migrationBuilder.DropTable(
                name: "auditoria_clasificacion_ingreso",
                schema: "fiscalhost_db");

            migrationBuilder.DropTable(
                name: "auditoria_configuracion",
                schema: "fiscalhost_db");

            migrationBuilder.DropTable(
                name: "auditoria_llave",
                schema: "fiscalhost_db");

            migrationBuilder.DropTable(
                name: "contenido_educativo",
                schema: "fiscalhost_db");

            migrationBuilder.DropTable(
                name: "exportacion",
                schema: "fiscalhost_db");

            migrationBuilder.DropTable(
                name: "gasto",
                schema: "fiscalhost_db");

            migrationBuilder.DropTable(
                name: "gasto_operativo",
                schema: "fiscalhost_db");

            migrationBuilder.DropTable(
                name: "perfil_tributario",
                schema: "fiscalhost_db");

            migrationBuilder.DropTable(
                name: "reconstruccion_historica",
                schema: "fiscalhost_db");

            migrationBuilder.DropTable(
                name: "reserva",
                schema: "fiscalhost_db");

            migrationBuilder.DropTable(
                name: "reserva_directa",
                schema: "fiscalhost_db");

            migrationBuilder.DropTable(
                name: "sancion_autoliquidacion",
                schema: "fiscalhost_db");

            migrationBuilder.DropTable(
                name: "simulacion_fiscal",
                schema: "fiscalhost_db");

            migrationBuilder.DropTable(
                name: "clasificacion_ingreso",
                schema: "fiscalhost_db");

            migrationBuilder.DropTable(
                name: "configuracion_tributaria",
                schema: "fiscalhost_db");

            migrationBuilder.DropTable(
                name: "llave_criptografica",
                schema: "fiscalhost_db");

            migrationBuilder.DropTable(
                name: "importacion_masiva",
                schema: "fiscalhost_db");

            migrationBuilder.DropTable(
                name: "propiedad",
                schema: "fiscalhost_db");

            migrationBuilder.DropTable(
                name: "obligacion_tributaria",
                schema: "fiscalhost_db");

            migrationBuilder.DropTable(
                name: "catalogo_actividad_economica",
                schema: "fiscalhost_db");

            migrationBuilder.DropTable(
                name: "calculo_fiscal",
                schema: "fiscalhost_db");

            migrationBuilder.DropTable(
                name: "periodo_fiscal",
                schema: "fiscalhost_db");

            migrationBuilder.DropTable(
                name: "usuario",
                schema: "fiscalhost_db");
        }
    }
}
