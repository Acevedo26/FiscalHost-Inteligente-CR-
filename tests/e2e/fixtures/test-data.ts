/**
 * Test data and utilities for E2E bug validation
 * Covers bugs: 001-006, 017-023
 */

export const TEST_USER = {
  email: 'anfitrion.qa1@test.com',
  password: 'Test123!',
};

export const TEST_CSV_VALID = `FechaInicio,FechaFin,MontoBruto,PlataformaOrigen,ReferenciaPlataforma
2026-06-01,2026-06-05,250000,AIRBNB,AIR-001
2026-06-10,2026-06-15,300000,AIRBNB,AIR-002
2026-07-01,2026-07-10,180000,BOOKING,BKG-001
2026-08-01,2026-08-07,220000,VRBO,VRB-001
2026-09-01,2026-09-03,150000,DIRECTA,DIR-001
2026-10-01,2026-10-05,280000,AIRBNB,AIR-003
2026-11-01,2026-11-10,350000,BOOKING,BKG-002
2026-12-01,2026-12-05,190000,VRBO,VRB-002`;

export const TEST_CSV_MIXED = `FechaInicio,FechaFin,MontoBruto,PlataformaOrigen,ReferenciaPlataforma
2026-06-01,2026-06-05,250000,AIRBNB,AIR-101
2026-06-10,2026-06-15,300000,AIRBNB,AIR-102
2026-07-01,2026-07-10,180000,BOOKING,BKG-101
2026-08-01,2026-08-07,220000,VRBO,VRB-101
2026-09-01,2026-09-03,150000,DIRECTA,DIR-101
2026-10-01,2026-10-05,280000,AIRBNB,AIR-103
2026-11-01,2026-11-10,350000,BOOKING,BKG-102
2026-12-01,2026-12-05,190000,VRBO,VRB-103
31/02/2026,2026-06-15,250000,AIRBNB,AIR-104
2026-13-01,2026-06-20,250000,AIRBNB,AIR-105`;

export const TEST_CSV_EMPTY = `FechaInicio,FechaFin,MontoBruto,PlataformaOrigen,ReferenciaPlataforma`;

export const TEST_CSV_INVALID_COLUMNS = `WrongColumn,AnotherColumn
value1,value2`;

export const VALID_RESERVA = {
  fecha: '2026-06-15',
  concepto: 'Reserva Directa QA Test',
  monto: '150000',
  fuente: 'AIRBNB',
  clasificacion: 'GRAVADO_13',
};

export const VALID_GASTO = {
  fecha: '2026-06-15',
  concepto: 'Mantenimiento Aire Acondicionado',
  monto: '75000',
  proveedor: 'Servicios Técnicos CR',
};

export const VALID_COMPROBANTE_PDF = 'test-comprobante.pdf';

export const INVITAR_CONTADOR = {
  nombre: 'Contador QA Test',
  email: 'contador.qa@test.com',
  permisos: 'Solo Lectura',
  expiracionDias: 30,
};

export const SELECTORS = {
  // Login
  login: {
    emailInput: 'input[type="email"]',
    passwordInput: 'input[type="password"]',
    submitButton: 'button[type="submit"]',
    googleButton: 'button:has-text("Continuar con Google")',
    errorAlert: '.login-alert',
  },

  // Layout / Navigation
  nav: {
    sidebar: '.sidebar',
    ingresosLink: 'a[href="/ingresos"], button:has-text("Mis Ingresos")',
    comprobantesLink: 'a[href="/comprobantes"], button:has-text("Comprobantes")',
    reportesLink: 'a[href="/reportes"], button:has-text("Reportes")',
    dashboardLink: 'a[href="/"], button:has-text("Dashboard")',
    simuladorLink: 'a[href="/simulador"], button:has-text("Simulador")',
    impuestosLink: 'a[href="/impuestos"], button:has-text("Impuestos")',
    configuracionLink: 'a[href="/configuracion"], button:has-text("Configuración")',
  },

  // Ingresos Page
  ingresos: {
    pageTitle: 'h1:has-text("Mis Ingresos")',
    tabs: {
      ingresosTab: 'button.tab:has-text("Ingresos")',
      gastosTab: 'button.tab:has-text("Gastos")',
    },
    buttons: {
      agregarBtn: 'button:has-text("Agregar")',
      importarBtn: 'button:has-text("Importar CSV")',
    },
    table: {
      row: '.ingresos-table tbody tr',
      cell: '.ingresos-table td',
      editBtn: 'button[aria-label*="Editar"]',
      deleteBtn: 'button[aria-label*="Eliminar"]',
      reclassifyBtn: 'button[aria-label*="Reclasificar"]',
    },
    addModal: {
      modal: '.modal:has-text("Agregar Ingreso")',
      fechaInput: 'input[type="date"]',
      conceptoInput: 'input[placeholder*="Reserva"]',
      montoInput: 'input[type="number"]',
      fuenteSelect: 'select[name="fuente"]',
      clasificacionSelect: 'select[name="clasificacion"]',
      proveedorInput: 'input[placeholder*="proveedor"]',
      guardarBtn: 'button:has-text("Guardar")',
      cancelarBtn: 'button:has-text("Cancelar")',
      errorFecha: '.input-error:has-text("Fecha")',
      errorConcepto: '.input-error:has-text("Concepto")',
      errorMonto: '.input-error:has-text("Monto")',
    },
    importModal: {
      modal: '.modal:has-text("Importar Reservas CSV")',
      platformButtons: '.platform-btn',
      dropZone: '.drop-zone',
      fileInput: 'input[type="file"][accept=".csv,text/csv"]',
      progressBar: '.progress-bar',
      progressFill: '.progress-fill',
      resultOk: '.import-result .result-success',
      resultOmitidos: '.banner-warning-import',
      downloadErrorsBtn: 'button:has-text("Descargar reporte de errores")',
      downloadTemplateBtn: 'button:has-text("Descargar plantilla oficial CSV")',
      errorColumns: '.import-error-modal',
      errorEmpty: '.import-error-modal:has-text("vacío")',
      importarBtn: 'button:has-text("Importar")',
      cerrarBtn: 'button:has-text("Cerrar")',
      cancelarBtn: 'button:has-text("Cancelar")',
    },
    editModal: {
      modal: '.modal:has-text("Editar")',
      montoInput: 'input[type="number"]',
      razonInput: 'textarea',
      guardarBtn: 'button:has-text("Guardar")',
      errorRazon: '.input-error:has-text("Justificación")',
    },
    reclassifyModal: {
      modal: '.modal:has-text("Reclasificar")',
      clasificacionSelect: 'select[name="clasificacion"]',
      justificacionInput: 'textarea',
      guardarBtn: 'button:has-text("Guardar")',
    },
  },

  // Comprobantes Page
  comprobantes: {
    pageTitle: 'h1:has-text("Comprobantes de Gastos")',
    buttons: {
      subirBtn: 'button:has-text("Subir Comprobante")',
    },
    summary: {
      validados: '.resumen-value:has-text("Comprobantes validados") + .resumen-value',
      creditoFiscal: '.resumen-value.resumen-success',
      sinOcr: '.resumen-value.resumen-warning',
    },
    table: {
      row: '.comprobantes-table tbody tr',
      deleteBtn: 'button[aria-label*="Eliminar"]',
    },
    uploadModal: {
      modal: '.modal:has-text("Subir Comprobante")',
      dropZone: '.drop-zone-comp',
      fileInput: 'input[type="file"][accept=".pdf,image/*"]',
      sinOcrCheckbox: 'input[type="checkbox"]',
      bannerWarning: '.banner-warning',
      bannerSuccess: '.banner-success',
      cancelarBtn: 'button:has-text("Cancelar")',
      guardarBtn: 'button:has-text("Confirmar y Guardar"), button:has-text("Subiendo...")',
      errorFormat: '.inline-error:has-text("Formato no soportado")',
      errorDuplicate: '.inline-error:has-text("Ya existe")',
    },
    deleteConfirm: {
      textarea: 'textarea[aria-label*="Justificación"]',
      confirmBtn: 'button:has-text("Confirmar Eliminación")',
      cancelBtn: 'button:has-text("Cancelar")',
    },
  },

  // Reportes Page
  reportes: {
    pageTitle: 'h1:has-text("Reportes")',
    autorizarContadorCard: '.autorizar-contador-card',
    invitarBtn: 'button:has-text("Invitar Contador")',
    invitarModal: {
      modal: '.modal:has-text("Invitar Contador")',
      nombreInput: 'input[placeholder*="nombre"]',
      emailInput: 'input[type="email"]',
      permisosSelect: 'select[name="permisos"]',
      expiracionInput: 'input[type="date"]',
      invitarBtn: 'button:has-text("Invitar")',
      cancelarBtn: 'button:has-text("Cancelar")',
    },
    revocarModal: {
      modal: '.modal:has-text("Revocar")',
      confirmarBtn: 'button:has-text("Confirmar")',
    },
  },

  // Dashboard
  dashboard: {
    pageTitle: 'h1:has-text("Dashboard")',
    cards: {
      ingresosBrutos: '.dashboard-card:has-text("Ingresos Brutos")',
      ivaRetenido: '.dashboard-card:has-text("IVA Retenido")',
      rentaEstimada: '.dashboard-card:has-text("Renta Estimada")',
      neto: '.dashboard-card:has-text("Neto")',
    },
  },

  // Global / Common
  global: {
    modal: '.modal',
    modalTitle: '.modal-title',
    modalClose: '.modal-close, button[aria-label="Cerrar"]',
    sweetAlert: '.swal2-popup',
    sweetAlertTitle: '.swal2-title',
    sweetAlertText: '.swal2-html-container',
    sweetAlertConfirm: '.swal2-confirm',
    sweetAlertCancel: '.swal2-cancel',
    sweetAlertInput: '.swal2-input, .swal2-textarea',
    toast: '.toast, .notification',
    loading: '.loading, .spinner',
  },
};

export const TIMEOUTS = {
  short: 5000,
  medium: 15000,
  long: 30000,
  veryLong: 60000,
};

export function generateTestCSV(rows: Array<{
  fechaInicio: string;
  fechaFin: string;
  montoBruto: string;
  plataformaOrigen: string;
  referenciaPlataforma: string;
}>): string {
  const header = 'FechaInicio,FechaFin,MontoBruto,PlataformaOrigen,ReferenciaPlataforma\n';
  const data = rows.map(r => `${r.fechaInicio},${r.fechaFin},${r.montoBruto},${r.plataformaOrigen},${r.referenciaPlataforma}`).join('\n');
  return header + data;
}

export function createTestFile(content: string, filename: string, mimeType: string): File {
  return new File([content], filename, { type: mimeType });
}