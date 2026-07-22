DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'fiscalhost_db') THEN
        CREATE SCHEMA fiscalhost_db;
    END IF;
END $EF$;
CREATE TABLE IF NOT EXISTS fiscalhost_db."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'fiscalhost_db') THEN
        CREATE SCHEMA fiscalhost_db;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'fiscalhost_db') THEN
        CREATE SCHEMA fiscalhost_db;
    END IF;
END $EF$;

CREATE TYPE fiscalhost_db.canal_notificacion AS ENUM ('CORREO', 'PLATAFORMA', 'AMBOS');
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'fiscalhost_db') THEN
        CREATE SCHEMA fiscalhost_db;
    END IF;
END $EF$;

CREATE TYPE fiscalhost_db.clasificacion_fiscal AS ENUM ('GRAVADO', 'EXENTO', 'GRAVADO_CON_RETENCION');
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'fiscalhost_db') THEN
        CREATE SCHEMA fiscalhost_db;
    END IF;
END $EF$;

CREATE TYPE fiscalhost_db.estado_declaracion AS ENUM ('BORRADOR', 'CALCULADO', 'VALIDADO', 'EXPORTADO', 'PRESENTADO');
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'fiscalhost_db') THEN
        CREATE SCHEMA fiscalhost_db;
    END IF;
END $EF$;

CREATE TYPE fiscalhost_db.estado_importacion AS ENUM ('PENDIENTE', 'PROCESANDO', 'COMPLETADO', 'COMPLETADO_PARCIAL', 'RECHAZADO');
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'fiscalhost_db') THEN
        CREATE SCHEMA fiscalhost_db;
    END IF;
END $EF$;

CREATE TYPE fiscalhost_db.estado_llave AS ENUM ('ACTIVA', 'EXPIRADA', 'REVOCADA');
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'fiscalhost_db') THEN
        CREATE SCHEMA fiscalhost_db;
    END IF;
END $EF$;

CREATE TYPE fiscalhost_db.estado_notificacion AS ENUM ('PENDIENTE', 'ENVIADA', 'ENTREGADA', 'FALLIDA', 'LEIDA');
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'fiscalhost_db') THEN
        CREATE SCHEMA fiscalhost_db;
    END IF;
END $EF$;

CREATE TYPE fiscalhost_db.estado_obligacion AS ENUM ('VIGENTE', 'VENCIDA', 'PAGADA', 'EN_ARREGLO');
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'fiscalhost_db') THEN
        CREATE SCHEMA fiscalhost_db;
    END IF;
END $EF$;

CREATE TYPE fiscalhost_db.estado_ocr AS ENUM ('pendiente', 'procesado', 'ilegible', 'sin_procesar');
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'fiscalhost_db') THEN
        CREATE SCHEMA fiscalhost_db;
    END IF;
END $EF$;

CREATE TYPE fiscalhost_db.estado_usuario AS ENUM ('PENDIENTE_ACTIVACION', 'ACTIVO', 'INACTIVO', 'BLOQUEADO', 'ELIMINADO');
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'fiscalhost_db') THEN
        CREATE SCHEMA fiscalhost_db;
    END IF;
END $EF$;

CREATE TYPE fiscalhost_db.estado_validacion AS ENUM ('PENDIENTE', 'VALIDO', 'RECHAZADO', 'DUPLICADO');
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'fiscalhost_db') THEN
        CREATE SCHEMA fiscalhost_db;
    END IF;
END $EF$;

CREATE TYPE fiscalhost_db.fuente_registro AS ENUM ('IMPORTACION_CSV', 'MANUAL', 'RECONSTRUCCION');
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'fiscalhost_db') THEN
        CREATE SCHEMA fiscalhost_db;
    END IF;
END $EF$;

CREATE TYPE fiscalhost_db.operacion_auditoria AS ENUM ('insert', 'update', 'delete', 'login', 'logout', 'export', 'reclasificacion', 'cambio_regimen', 'cambio_actividad_economica');
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'fiscalhost_db') THEN
        CREATE SCHEMA fiscalhost_db;
    END IF;
END $EF$;

CREATE TYPE fiscalhost_db.plataforma_origen AS ENUM ('AIRBNB', 'BOOKING', 'VRBO', 'DIRECTA', 'OTRA');
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'fiscalhost_db') THEN
        CREATE SCHEMA fiscalhost_db;
    END IF;
END $EF$;

CREATE TYPE fiscalhost_db.regimen_tributario AS ENUM ('CAPITAL_INMOBILIARIO', 'UTILIDADES');
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'fiscalhost_db') THEN
        CREATE SCHEMA fiscalhost_db;
    END IF;
END $EF$;

CREATE TYPE fiscalhost_db.rol_usuario AS ENUM ('ANFITRION', 'CONTADOR', 'ADMINISTRADOR');
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'fiscalhost_db') THEN
        CREATE SCHEMA fiscalhost_db;
    END IF;
END $EF$;

CREATE TYPE fiscalhost_db.tipo_alerta AS ENUM ('VENCIMIENTO_15_DIAS', 'VENCIMIENTO_10_DIAS', 'VENCIMIENTO_7_DIAS', 'VENCIMIENTO_3_DIAS', 'VENCIMIENTO_1_DIA', 'VENCIMIENTO_EXPIRADO', 'LLAVE_POR_VENCER', 'INFORMATIVA', 'SEGURIDAD');
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'fiscalhost_db') THEN
        CREATE SCHEMA fiscalhost_db;
    END IF;
END $EF$;

CREATE TYPE fiscalhost_db.tipo_formulario AS ENUM ('D150', 'D116', 'D176');
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'fiscalhost_db') THEN
        CREATE SCHEMA fiscalhost_db;
    END IF;
END $EF$;

CREATE TYPE fiscalhost_db.tipo_identificacion AS ENUM ('FISICA', 'JURIDICA', 'DIMEX', 'NITE');
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'fiscalhost_db') THEN
        CREATE SCHEMA fiscalhost_db;
    END IF;
END $EF$;

CREATE TYPE fiscalhost_db.tipo_moneda AS ENUM ('CRC', 'USD');

CREATE TABLE fiscalhost_db.auditoria_operacion (
    "Id" integer GENERATED BY DEFAULT AS IDENTITY,
    "Entidad" text NOT NULL,
    "EntidadId" integer NOT NULL,
    "Usuario" text NOT NULL,
    "Accion" text NOT NULL,
    "Descripcion" text NOT NULL,
    "ValorAnterior" text,
    "ValorNuevo" text,
    "Justificacion" text,
    "Fecha" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_auditoria_operacion" PRIMARY KEY ("Id")
);

CREATE TABLE fiscalhost_db.catalogo_actividad_economica (
    codigo character varying(20) NOT NULL,
    descripcion text NOT NULL,
    seccion text,
    tarifa_iva numeric NOT NULL,
    vigente boolean NOT NULL,
    fecha_vigencia_desde timestamp with time zone NOT NULL,
    fecha_vigencia_hasta timestamp with time zone,
    CONSTRAINT "PK_catalogo_actividad_economica" PRIMARY KEY (codigo)
);

CREATE TABLE fiscalhost_db.gasto_operativo (
    "Id" integer GENERATED BY DEFAULT AS IDENTITY,
    "AnfitrionId" text NOT NULL,
    "Proveedor" text NOT NULL,
    "NumeroFactura" text NOT NULL,
    "Monto" numeric NOT NULL,
    "ComprobanteUrl" text,
    "FechaGasto" timestamp with time zone NOT NULL,
    "FechaCreacion" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_gasto_operativo" PRIMARY KEY ("Id")
);

CREATE TABLE fiscalhost_db.periodo_fiscal (
    periodo_id uuid NOT NULL,
    anio smallint NOT NULL,
    mes smallint NOT NULL,
    tipo_formulario fiscalhost_db.tipo_formulario NOT NULL,
    fecha_inicio_periodo timestamp with time zone NOT NULL,
    fecha_fin_periodo timestamp with time zone NOT NULL,
    fecha_vencimiento timestamp with time zone NOT NULL,
    tarifa_iva numeric NOT NULL,
    tarifa_renta_capital numeric NOT NULL,
    deduccion_plana_capital numeric NOT NULL,
    tasa_interes_mora_anual numeric,
    normativa_aplicable text NOT NULL,
    CONSTRAINT "PK_periodo_fiscal" PRIMARY KEY (periodo_id)
);

CREATE TABLE fiscalhost_db.reserva_directa (
    "Id" integer GENERATED BY DEFAULT AS IDENTITY,
    "AnfitrionId" text NOT NULL,
    "FechaReserva" timestamp with time zone NOT NULL,
    "Monto" numeric NOT NULL,
    "Huesped" text NOT NULL,
    "FechaCreacion" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_reserva_directa" PRIMARY KEY ("Id")
);

CREATE TABLE fiscalhost_db.usuario (
    usuario_id uuid NOT NULL,
    tipo_identificacion fiscalhost_db.tipo_identificacion NOT NULL,
    numero_identificacion character varying(50) NOT NULL,
    nombre_completo character varying(200) NOT NULL,
    razon_social character varying(200),
    correo_electronico character varying(150) NOT NULL,
    contrasena_hash character varying(255) NOT NULL,
    estado fiscalhost_db.estado_usuario NOT NULL,
    rol_principal fiscalhost_db.rol_usuario NOT NULL,
    es_usuario_nuevo boolean NOT NULL,
    correo_verificado boolean NOT NULL,
    preferencias_notificacion jsonb NOT NULL,
    fecha_activacion timestamp with time zone,
    ultimo_acceso timestamp with time zone,
    CONSTRAINT "PK_usuario" PRIMARY KEY (usuario_id)
);

CREATE TABLE fiscalhost_db."ConfiguracionTributaria" (
    "Id" integer GENERATED BY DEFAULT AS IDENTITY,
    "AnfitrionId" text NOT NULL,
    "CodigoActividad" text NOT NULL,
    "ActividadEconomicaCodigo" character varying(20),
    "TribuCr" text NOT NULL,
    "DireccionFiscal" text NOT NULL,
    "Nise" text NOT NULL,
    "Estado" integer NOT NULL,
    "FechaCreacion" timestamp with time zone NOT NULL,
    "FechaActualizacion" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_ConfiguracionTributaria" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ConfiguracionTributaria_catalogo_actividad_economica_Activi~" FOREIGN KEY ("ActividadEconomicaCodigo") REFERENCES fiscalhost_db.catalogo_actividad_economica (codigo)
);

CREATE TABLE fiscalhost_db.acceso_contador (
    acceso_id uuid NOT NULL,
    anfitrion_id uuid NOT NULL,
    contador_id uuid,
    correo_contador text NOT NULL,
    permisos jsonb NOT NULL,
    fecha_invitacion timestamp with time zone NOT NULL,
    fecha_aceptacion timestamp with time zone,
    fecha_expiracion timestamp with time zone,
    fecha_revocacion timestamp with time zone,
    estado text NOT NULL,
    CONSTRAINT "PK_acceso_contador" PRIMARY KEY (acceso_id),
    CONSTRAINT "FK_acceso_contador_usuario_anfitrion_id" FOREIGN KEY (anfitrion_id) REFERENCES fiscalhost_db.usuario (usuario_id) ON DELETE RESTRICT,
    CONSTRAINT "FK_acceso_contador_usuario_contador_id" FOREIGN KEY (contador_id) REFERENCES fiscalhost_db.usuario (usuario_id) ON DELETE RESTRICT
);

CREATE TABLE fiscalhost_db.calculo_fiscal (
    calculo_id uuid NOT NULL,
    usuario_id uuid NOT NULL,
    periodo_id uuid NOT NULL,
    tipo_formulario fiscalhost_db.tipo_formulario NOT NULL,
    regimen_aplicado fiscalhost_db.regimen_tributario,
    estado fiscalhost_db.estado_declaracion NOT NULL,
    total_ingresos_brutos numeric NOT NULL,
    total_ingresos_gravados numeric NOT NULL,
    total_ingresos_exentos numeric NOT NULL,
    debito_fiscal numeric NOT NULL,
    credito_fiscal numeric NOT NULL,
    iva_neto numeric NOT NULL,
    saldo_favor_anterior numeric NOT NULL,
    saldo_favor_resultante numeric NOT NULL,
    renta_bruta numeric,
    deduccion_aplicada numeric,
    renta_neta numeric,
    impuesto_renta numeric,
    retenciones_acreditadas numeric,
    monto_total_a_pagar numeric NOT NULL,
    detalle_calculo jsonb NOT NULL,
    borrador_generado boolean NOT NULL,
    fecha_generacion_borrador timestamp with time zone,
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone NOT NULL,
    CONSTRAINT "PK_calculo_fiscal" PRIMARY KEY (calculo_id),
    CONSTRAINT "FK_calculo_fiscal_periodo_fiscal_periodo_id" FOREIGN KEY (periodo_id) REFERENCES fiscalhost_db.periodo_fiscal (periodo_id) ON DELETE CASCADE,
    CONSTRAINT "FK_calculo_fiscal_usuario_usuario_id" FOREIGN KEY (usuario_id) REFERENCES fiscalhost_db.usuario (usuario_id) ON DELETE CASCADE
);

CREATE TABLE fiscalhost_db.contenido_educativo (
    contenido_id uuid NOT NULL,
    titulo text NOT NULL,
    slug text NOT NULL,
    categoria text NOT NULL,
    tipo text NOT NULL,
    contenido_markdown text NOT NULL,
    contenido_html text,
    es_tutorial_primer_uso boolean NOT NULL,
    orden_display integer NOT NULL,
    version integer NOT NULL,
    publicado boolean NOT NULL,
    autor_id uuid,
    published_at timestamp with time zone,
    CONSTRAINT "PK_contenido_educativo" PRIMARY KEY (contenido_id),
    CONSTRAINT "FK_contenido_educativo_usuario_autor_id" FOREIGN KEY (autor_id) REFERENCES fiscalhost_db.usuario (usuario_id)
);

CREATE TABLE fiscalhost_db.importacion_masiva (
    importacion_id uuid NOT NULL,
    usuario_id uuid NOT NULL,
    tipo_importacion character varying(30) NOT NULL,
    plataforma_origen fiscalhost_db.plataforma_origen,
    archivo_url character varying(500) NOT NULL,
    nombre_archivo_original character varying(255) NOT NULL,
    plantilla_utilizada character varying(50),
    tamanio_bytes bigint,
    estado fiscalhost_db.estado_importacion NOT NULL,
    total_registros integer,
    registros_exitosos integer,
    registros_con_error integer,
    reporte_errores_url character varying(500),
    detalle_errores jsonb NOT NULL,
    fecha_carga timestamp with time zone NOT NULL,
    fecha_inicio_procesamiento timestamp with time zone,
    fecha_fin_procesamiento timestamp with time zone,
    CONSTRAINT "PK_importacion_masiva" PRIMARY KEY (importacion_id),
    CONSTRAINT "FK_importacion_masiva_usuario_usuario_id" FOREIGN KEY (usuario_id) REFERENCES fiscalhost_db.usuario (usuario_id) ON DELETE CASCADE
);

CREATE TABLE fiscalhost_db.llave_criptografica (
    llave_id uuid NOT NULL,
    usuario_id uuid NOT NULL,
    nombre_archivo text NOT NULL,
    ruta_blob_storage text NOT NULL,
    hash_integridad text NOT NULL,
    referencia_key_vault text,
    huella_digital_certificado text,
    fecha_emision_certificado timestamp with time zone,
    fecha_expiracion_certificado timestamp with time zone,
    emisor_certificado text,
    estado text NOT NULL,
    fecha_carga timestamp with time zone NOT NULL,
    ultima_actualizacion_contrasena timestamp with time zone,
    CONSTRAINT "PK_llave_criptografica" PRIMARY KEY (llave_id),
    CONSTRAINT "FK_llave_criptografica_usuario_usuario_id" FOREIGN KEY (usuario_id) REFERENCES fiscalhost_db.usuario (usuario_id) ON DELETE CASCADE
);

CREATE TABLE fiscalhost_db.perfil_tributario (
    perfil_id uuid NOT NULL,
    usuario_id uuid NOT NULL,
    codigo_actividad_economica character varying(20) NOT NULL,
    descripcion_actividad text NOT NULL,
    tribu_cr character varying(50),
    direccion_fiscal text,
    nise character varying(50),
    es_domicilio_validado boolean NOT NULL,
    regimen_tributario fiscalhost_db.regimen_tributario NOT NULL,
    fecha_inicio_actividad timestamp with time zone,
    fecha_inscripcion_hacienda timestamp with time zone,
    datos_complementarios jsonb NOT NULL,
    CONSTRAINT "PK_perfil_tributario" PRIMARY KEY (perfil_id),
    CONSTRAINT "FK_perfil_tributario_catalogo_actividad_economica_codigo_activ~" FOREIGN KEY (codigo_actividad_economica) REFERENCES fiscalhost_db.catalogo_actividad_economica (codigo) ON DELETE CASCADE,
    CONSTRAINT "FK_perfil_tributario_usuario_usuario_id" FOREIGN KEY (usuario_id) REFERENCES fiscalhost_db.usuario (usuario_id) ON DELETE CASCADE
);

CREATE TABLE fiscalhost_db.propiedad (
    propiedad_id uuid NOT NULL,
    usuario_id uuid NOT NULL,
    nombre character varying(200) NOT NULL,
    direccion text,
    provincia text,
    canton text,
    distrito text,
    numero_finca text,
    valor_fiscal numeric,
    tipo_moneda_valor fiscalhost_db.tipo_moneda,
    activa boolean NOT NULL,
    CONSTRAINT "PK_propiedad" PRIMARY KEY (propiedad_id),
    CONSTRAINT "FK_propiedad_usuario_usuario_id" FOREIGN KEY (usuario_id) REFERENCES fiscalhost_db.usuario (usuario_id) ON DELETE CASCADE
);

CREATE TABLE fiscalhost_db.simulacion_fiscal (
    simulacion_id uuid NOT NULL,
    usuario_id uuid NOT NULL,
    nombre text NOT NULL,
    descripcion text,
    periodo_base_anio smallint NOT NULL,
    periodo_base_mes smallint,
    parametros_entrada jsonb NOT NULL,
    resultados jsonb NOT NULL,
    CONSTRAINT "PK_simulacion_fiscal" PRIMARY KEY (simulacion_id),
    CONSTRAINT "FK_simulacion_fiscal_usuario_usuario_id" FOREIGN KEY (usuario_id) REFERENCES fiscalhost_db.usuario (usuario_id) ON DELETE CASCADE
);

CREATE TABLE fiscalhost_db.auditoria_configuracion (
    "Id" integer GENERATED BY DEFAULT AS IDENTITY,
    "ConfiguracionTributariaId" integer NOT NULL,
    "Campo" text NOT NULL,
    "ValorAnterior" text NOT NULL,
    "ValorNuevo" text NOT NULL,
    "Descripcion" text NOT NULL,
    "FechaEvento" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_auditoria_configuracion" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_auditoria_configuracion_ConfiguracionTributaria_Configuraci~" FOREIGN KEY ("ConfiguracionTributariaId") REFERENCES fiscalhost_db."ConfiguracionTributaria" ("Id") ON DELETE CASCADE
);

CREATE TABLE fiscalhost_db.exportacion (
    exportacion_id uuid NOT NULL,
    usuario_id uuid NOT NULL,
    calculo_id uuid,
    "CalculoFiscalCalculoId" uuid,
    formato character varying(10) NOT NULL,
    tipo_contenido character varying(50) NOT NULL,
    archivo_url character varying(500) NOT NULL,
    nombre_archivo character varying(255) NOT NULL,
    tamanio_bytes bigint,
    esta_protegido boolean NOT NULL,
    expira_at timestamp with time zone,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT "PK_exportacion" PRIMARY KEY (exportacion_id),
    CONSTRAINT "FK_exportacion_calculo_fiscal_CalculoFiscalCalculoId" FOREIGN KEY ("CalculoFiscalCalculoId") REFERENCES fiscalhost_db.calculo_fiscal (calculo_id),
    CONSTRAINT "FK_exportacion_usuario_usuario_id" FOREIGN KEY (usuario_id) REFERENCES fiscalhost_db.usuario (usuario_id) ON DELETE CASCADE
);

CREATE TABLE fiscalhost_db.obligacion_tributaria (
    obligacion_id uuid NOT NULL,
    usuario_id uuid NOT NULL,
    calculo_id uuid,
    "CalculoFiscalCalculoId" uuid,
    periodo_id uuid NOT NULL,
    tipo_formulario fiscalhost_db.tipo_formulario NOT NULL,
    descripcion character varying(300) NOT NULL,
    monto_capital numeric NOT NULL,
    monto_multa numeric NOT NULL,
    monto_intereses_acumulados numeric NOT NULL,
    monto_total_actualizado numeric NOT NULL,
    fecha_vencimiento date NOT NULL,
    fecha_pago date,
    estado fiscalhost_db.estado_obligacion NOT NULL,
    tasa_interes_aplicada numeric,
    fecha_ultimo_calculo_interes timestamp with time zone,
    historial_intereses jsonb NOT NULL,
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone NOT NULL,
    CONSTRAINT "PK_obligacion_tributaria" PRIMARY KEY (obligacion_id),
    CONSTRAINT "FK_obligacion_tributaria_calculo_fiscal_CalculoFiscalCalculoId" FOREIGN KEY ("CalculoFiscalCalculoId") REFERENCES fiscalhost_db.calculo_fiscal (calculo_id),
    CONSTRAINT "FK_obligacion_tributaria_periodo_fiscal_periodo_id" FOREIGN KEY (periodo_id) REFERENCES fiscalhost_db.periodo_fiscal (periodo_id) ON DELETE CASCADE,
    CONSTRAINT "FK_obligacion_tributaria_usuario_usuario_id" FOREIGN KEY (usuario_id) REFERENCES fiscalhost_db.usuario (usuario_id) ON DELETE CASCADE
);

CREATE TABLE fiscalhost_db.auditoria_llave (
    "Id" integer GENERATED BY DEFAULT AS IDENTITY,
    "LlaveCriptograficaId" integer NOT NULL,
    "LlaveCriptograficaLlaveId" uuid NOT NULL,
    "Accion" text NOT NULL,
    "Descripcion" text NOT NULL,
    "FechaEvento" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_auditoria_llave" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_auditoria_llave_llave_criptografica_LlaveCriptograficaLlave~" FOREIGN KEY ("LlaveCriptograficaLlaveId") REFERENCES fiscalhost_db.llave_criptografica (llave_id) ON DELETE CASCADE
);

CREATE TABLE fiscalhost_db.gasto (
    gasto_id uuid NOT NULL,
    usuario_id uuid NOT NULL,
    propiedad_id uuid,
    proveedor character varying(200) NOT NULL,
    numero_factura character varying(100),
    clave_numerica_hacienda character varying(50),
    fecha_emision date NOT NULL,
    descripcion text,
    monto_total numeric NOT NULL,
    monto_iva_soportado numeric NOT NULL,
    monto_neto numeric,
    moneda fiscalhost_db.tipo_moneda NOT NULL,
    tipo_cambio numeric,
    monto_colones numeric NOT NULL,
    tipo_gasto character varying(50) NOT NULL,
    es_deducible_renta boolean NOT NULL,
    es_credito_fiscal_valido boolean NOT NULL,
    evidencia_url character varying(500),
    evidencia_nombre_archivo character varying(255),
    evidencia_tipo_mime character varying(50),
    evidencia_tamanio_bytes bigint,
    estado_ocr fiscalhost_db.estado_ocr,
    datos_extraidos_ocr jsonb NOT NULL,
    hash_unico_comprobante character varying(64),
    estado_validacion fiscalhost_db.estado_validacion NOT NULL,
    observaciones_validacion text,
    periodo_fiscal_anio smallint NOT NULL,
    periodo_fiscal_mes smallint NOT NULL,
    fuente_registro fiscalhost_db.fuente_registro NOT NULL,
    fecha_registro timestamp with time zone NOT NULL,
    updated_at timestamp with time zone NOT NULL,
    deleted_at timestamp with time zone,
    CONSTRAINT "PK_gasto" PRIMARY KEY (gasto_id),
    CONSTRAINT "FK_gasto_propiedad_propiedad_id" FOREIGN KEY (propiedad_id) REFERENCES fiscalhost_db.propiedad (propiedad_id),
    CONSTRAINT "FK_gasto_usuario_usuario_id" FOREIGN KEY (usuario_id) REFERENCES fiscalhost_db.usuario (usuario_id) ON DELETE CASCADE
);

CREATE TABLE fiscalhost_db.reserva (
    reserva_id uuid NOT NULL,
    usuario_id uuid NOT NULL,
    propiedad_id uuid,
    importacion_id uuid,
    fecha_inicio timestamp with time zone NOT NULL,
    fecha_fin timestamp with time zone NOT NULL,
    nombre_huesped text,
    identificacion_huesped text,
    pais_origen_huesped text,
    monto_bruto numeric NOT NULL,
    moneda fiscalhost_db.tipo_moneda NOT NULL,
    tipo_cambio numeric NOT NULL,
    monto_colones numeric NOT NULL,
    clasificacion_fiscal fiscalhost_db.clasificacion_fiscal NOT NULL,
    monto_gravado numeric NOT NULL,
    monto_exento numeric NOT NULL,
    monto_iva_calculado numeric NOT NULL,
    retencion_extranjera numeric NOT NULL,
    plataforma_origen fiscalhost_db.plataforma_origen NOT NULL,
    fuente_registro fiscalhost_db.fuente_registro NOT NULL,
    referencia_plataforma text,
    fue_reclasificada boolean NOT NULL,
    justificacion_reclasificacion text,
    fecha_reclasificacion timestamp with time zone,
    usuario_reclasificacion_id uuid,
    periodo_fiscal_anio smallint NOT NULL,
    periodo_fiscal_mes smallint NOT NULL,
    estado text NOT NULL,
    metadata jsonb NOT NULL,
    CONSTRAINT "PK_reserva" PRIMARY KEY (reserva_id),
    CONSTRAINT "FK_reserva_importacion_masiva_importacion_id" FOREIGN KEY (importacion_id) REFERENCES fiscalhost_db.importacion_masiva (importacion_id),
    CONSTRAINT "FK_reserva_propiedad_propiedad_id" FOREIGN KEY (propiedad_id) REFERENCES fiscalhost_db.propiedad (propiedad_id),
    CONSTRAINT "FK_reserva_usuario_usuario_id" FOREIGN KEY (usuario_id) REFERENCES fiscalhost_db.usuario (usuario_id) ON DELETE CASCADE
);

CREATE TABLE fiscalhost_db.alerta (
    alerta_id uuid NOT NULL,
    usuario_id uuid NOT NULL,
    obligacion_id uuid,
    tipo_alerta fiscalhost_db.tipo_alerta NOT NULL,
    titulo character varying(200) NOT NULL,
    mensaje text NOT NULL,
    prioridad smallint NOT NULL,
    monto_estimado numeric,
    canal fiscalhost_db.canal_notificacion NOT NULL,
    estado fiscalhost_db.estado_notificacion NOT NULL,
    accion_sugerida jsonb NOT NULL,
    fecha_programada timestamp with time zone NOT NULL,
    fecha_envio timestamp with time zone,
    fecha_lectura timestamp with time zone,
    error_envio text,
    intentos_envio smallint NOT NULL,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT "PK_alerta" PRIMARY KEY (alerta_id),
    CONSTRAINT "FK_alerta_obligacion_tributaria_obligacion_id" FOREIGN KEY (obligacion_id) REFERENCES fiscalhost_db.obligacion_tributaria (obligacion_id),
    CONSTRAINT "FK_alerta_usuario_usuario_id" FOREIGN KEY (usuario_id) REFERENCES fiscalhost_db.usuario (usuario_id) ON DELETE CASCADE
);

CREATE TABLE fiscalhost_db.sancion_autoliquidacion (
    sancion_id uuid NOT NULL,
    usuario_id uuid NOT NULL,
    obligacion_id uuid,
    tipo_sancion text NOT NULL,
    descripcion text NOT NULL,
    fecha_omision timestamp with time zone NOT NULL,
    monto_base_adeudado numeric NOT NULL,
    multa_base_calculada numeric NOT NULL,
    porcentaje_reduccion numeric NOT NULL,
    monto_reduccion numeric NOT NULL,
    multa_reducida numeric NOT NULL,
    intereses_acumulados numeric NOT NULL,
    monto_total_pagar numeric NOT NULL,
    detalle_calculo jsonb NOT NULL,
    estado text NOT NULL,
    CONSTRAINT "PK_sancion_autoliquidacion" PRIMARY KEY (sancion_id),
    CONSTRAINT "FK_sancion_autoliquidacion_obligacion_tributaria_obligacion_id" FOREIGN KEY (obligacion_id) REFERENCES fiscalhost_db.obligacion_tributaria (obligacion_id),
    CONSTRAINT "FK_sancion_autoliquidacion_usuario_usuario_id" FOREIGN KEY (usuario_id) REFERENCES fiscalhost_db.usuario (usuario_id) ON DELETE CASCADE
);

CREATE INDEX "IX_acceso_contador_anfitrion_id" ON fiscalhost_db.acceso_contador (anfitrion_id);

CREATE INDEX "IX_acceso_contador_contador_id" ON fiscalhost_db.acceso_contador (contador_id);

CREATE INDEX "IX_alerta_obligacion_id" ON fiscalhost_db.alerta (obligacion_id);

CREATE INDEX "IX_alerta_usuario_id" ON fiscalhost_db.alerta (usuario_id);

CREATE INDEX "IX_auditoria_configuracion_ConfiguracionTributariaId" ON fiscalhost_db.auditoria_configuracion ("ConfiguracionTributariaId");

CREATE INDEX "IX_auditoria_llave_LlaveCriptograficaLlaveId" ON fiscalhost_db.auditoria_llave ("LlaveCriptograficaLlaveId");

CREATE INDEX "IX_calculo_fiscal_periodo_id" ON fiscalhost_db.calculo_fiscal (periodo_id);

CREATE INDEX "IX_calculo_fiscal_usuario_id" ON fiscalhost_db.calculo_fiscal (usuario_id);

CREATE UNIQUE INDEX "IX_catalogo_actividad_economica_codigo" ON fiscalhost_db.catalogo_actividad_economica (codigo);

CREATE INDEX "IX_ConfiguracionTributaria_ActividadEconomicaCodigo" ON fiscalhost_db."ConfiguracionTributaria" ("ActividadEconomicaCodigo");

CREATE INDEX "IX_contenido_educativo_autor_id" ON fiscalhost_db.contenido_educativo (autor_id);

CREATE INDEX "IX_exportacion_CalculoFiscalCalculoId" ON fiscalhost_db.exportacion ("CalculoFiscalCalculoId");

CREATE INDEX "IX_exportacion_usuario_id" ON fiscalhost_db.exportacion (usuario_id);

CREATE INDEX "IX_gasto_propiedad_id" ON fiscalhost_db.gasto (propiedad_id);

CREATE INDEX "IX_gasto_usuario_id" ON fiscalhost_db.gasto (usuario_id);

CREATE INDEX "IX_importacion_masiva_usuario_id" ON fiscalhost_db.importacion_masiva (usuario_id);

CREATE INDEX "IX_llave_criptografica_usuario_id" ON fiscalhost_db.llave_criptografica (usuario_id);

CREATE INDEX "IX_obligacion_tributaria_CalculoFiscalCalculoId" ON fiscalhost_db.obligacion_tributaria ("CalculoFiscalCalculoId");

CREATE INDEX "IX_obligacion_tributaria_periodo_id" ON fiscalhost_db.obligacion_tributaria (periodo_id);

CREATE INDEX "IX_obligacion_tributaria_usuario_id" ON fiscalhost_db.obligacion_tributaria (usuario_id);

CREATE INDEX "IX_perfil_tributario_codigo_actividad_economica" ON fiscalhost_db.perfil_tributario (codigo_actividad_economica);

CREATE UNIQUE INDEX "IX_perfil_tributario_usuario_id" ON fiscalhost_db.perfil_tributario (usuario_id);

CREATE INDEX "IX_propiedad_usuario_id" ON fiscalhost_db.propiedad (usuario_id);

CREATE INDEX "IX_reserva_importacion_id" ON fiscalhost_db.reserva (importacion_id);

CREATE INDEX "IX_reserva_propiedad_id" ON fiscalhost_db.reserva (propiedad_id);

CREATE INDEX "IX_reserva_usuario_id" ON fiscalhost_db.reserva (usuario_id);

CREATE INDEX "IX_sancion_autoliquidacion_obligacion_id" ON fiscalhost_db.sancion_autoliquidacion (obligacion_id);

CREATE INDEX "IX_sancion_autoliquidacion_usuario_id" ON fiscalhost_db.sancion_autoliquidacion (usuario_id);

CREATE INDEX "IX_simulacion_fiscal_usuario_id" ON fiscalhost_db.simulacion_fiscal (usuario_id);

INSERT INTO fiscalhost_db."__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260622222412_UpdateAuditoriaOperacion', '8.0.0');

COMMIT;


-- Update Enum TipoFormulario
ALTER TYPE fiscalhost_db.tipo_formulario ADD VALUE IF NOT EXISTS 'D104';
ALTER TYPE fiscalhost_db.tipo_formulario ADD VALUE IF NOT EXISTS 'D125';

