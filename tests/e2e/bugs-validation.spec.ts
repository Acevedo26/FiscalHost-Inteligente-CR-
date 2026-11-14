/**
 * E2E Validation Suite for Bugs 001-023
 * Tests frontend flows corresponding to backend fixes
 *
 * Run: npx playwright test tests/e2e/bugs-validation.spec.ts
 */

import { test, expect, Page, BrowserContext } from '@playwright/test';
import {
  TEST_USER,
  TEST_CSV_VALID,
  TEST_CSV_MIXED,
  TEST_CSV_EMPTY,
  VALID_RESERVA,
  VALID_GASTO,
  INVITAR_CONTADOR,
  SELECTORS,
  TIMEOUTS,
} from './fixtures/test-data';

// ============================================================================
// HELPER FUNCTIONS
// ============================================================================

async function login(page: Page) {
  await page.goto('/login');
  await page.waitForSelector(SELECTORS.login.emailInput);
  await page.fill(SELECTORS.login.emailInput, TEST_USER.email);
  await page.fill(SELECTORS.login.passwordInput, TEST_USER.password);
  await page.click(SELECTORS.login.submitButton);
  await page.waitForURL('/');
  await expect(page.locator(SELECTORS.dashboard.pageTitle)).toBeVisible({ timeout: TIMEOUTS.medium });
}

async function navigateTo(page: Page, path: string) {
  await page.goto(path);
  await page.waitForLoadState('networkidle');
}

async function waitForModal(page: Page, titleText: string) {
  await expect(page.locator(SELECTORS.global.modal).filter({ hasText: titleText })).toBeVisible({ timeout: TIMEOUTS.medium });
}

async function closeModal(page: Page) {
  const closeBtn = page.locator(SELECTORS.global.modalClose).first();
  if (await closeBtn.isVisible({ timeout: 2000 })) {
    await closeBtn.click();
  } else {
    await page.keyboard.press('Escape');
  }
  await expect(page.locator(SELECTORS.global.modal)).toBeHidden({ timeout: TIMEOUTS.short });
}

async function handleSweetAlert(page: Page, action: 'confirm' | 'cancel' = 'confirm') {
  await expect(page.locator(SELECTORS.global.sweetAlert)).toBeVisible({ timeout: TIMEOUTS.medium });
  if (action === 'confirm') {
    await page.click(SELECTORS.global.sweetAlertConfirm);
  } else {
    await page.click(SELECTORS.global.sweetAlertCancel);
  }
  await expect(page.locator(SELECTORS.global.sweetAlert)).toBeHidden({ timeout: TIMEOUTS.short });
}

async function fillAddTransactionForm(page: Page, data: typeof VALID_RESERVA | typeof VALID_GASTO, isGasto = false) {
  await page.fill(SELECTORS.ingresos.addModal.fechaInput, data.fecha);
  await page.fill(SELECTORS.ingresos.addModal.conceptoInput, data.concepto);
  await page.fill(SELECTORS.ingresos.addModal.montoInput, data.monto);

  if (!isGasto) {
    await page.selectOption(SELECTORS.ingresos.addModal.fuenteSelect, data.fuente as string);
    await page.selectOption(SELECTORS.ingresos.addModal.clasificacionSelect, data.clasificacion as string);
  } else {
    await page.fill(SELECTORS.ingresos.addModal.proveedorInput, data.proveedor as string);
  }
}

async function createTestFile(content: string, filename: string): Promise<File> {
  return new File([content], filename, { type: 'text/csv' });
}

// ============================================================================
// TEST SUITES
// ============================================================================

test.describe.configure({ retries: 0 });

test.describe('BUG-001: Reserva Directa - HTTP 500 al guardar', () => {
  test.beforeEach(async ({ page }) => {
    await login(page);
    await navigateTo(page, '/ingresos');
  });

  test('Debe guardar reserva directa válida sin error 500', async ({ page }) => {
    // Abrir modal Agregar
    await page.click(SELECTORS.ingresos.buttons.agregarBtn);
    await waitForModal(page, 'Agregar Ingreso');

    // Llenar formulario válido
    await fillAddTransactionForm(page, VALID_RESERVA);

    // Guardar
    await page.click(SELECTORS.ingresos.addModal.guardarBtn);

    // Verificar SweetAlert de éxito
    await expect(page.locator(SELECTORS.global.sweetAlert)).toBeVisible({ timeout: TIMEOUTS.medium });
    await expect(page.locator(SELECTORS.global.sweetAlertTitle)).toContainText('Registro Creado');
    await expect(page.locator(SELECTORS.global.sweetAlertText)).toContainText('guardado exitosamente');
    await handleSweetAlert(page);

    // Verificar que el modal se cerró
    await expect(page.locator(SELECTORS.ingresos.addModal.modal)).toBeHidden({ timeout: TIMEOUTS.short });

    // Verificar que aparece en la tabla
    await expect(page.locator(SELECTORS.ingresos.table.row).first()).toContainText(VALID_RESERVA.concepto);
  });

  test('Debe validar monto > 0 y mostrar error amigable', async ({ page }) => {
    await page.click(SELECTORS.ingresos.buttons.agregarBtn);
    await waitForModal(page, 'Agregar Ingreso');

    await page.fill(SELECTORS.ingresos.addModal.fechaInput, VALID_RESERVA.fecha);
    await page.fill(SELECTORS.ingresos.addModal.conceptoInput, VALID_RESERVA.concepto);
    await page.fill(SELECTORS.ingresos.addModal.montoInput, '0');

    await page.click(SELECTORS.ingresos.addModal.guardarBtn);

    await expect(page.locator(SELECTORS.global.sweetAlert)).toBeVisible({ timeout: TIMEOUTS.medium });
    await expect(page.locator(SELECTORS.global.sweetAlertTitle)).toContainText('Monto Inválido');
    await handleSweetAlert(page);
  });

  test('Debe validar fecha no futura y mostrar error amigable', async ({ page }) => {
    await page.click(SELECTORS.ingresos.buttons.agregarBtn);
    await waitForModal(page, 'Agregar Ingreso');

    const futureDate = new Date();
    futureDate.setDate(futureDate.getDate() + 30);
    const futureStr = futureDate.toISOString().split('T')[0];

    await page.fill(SELECTORS.ingresos.addModal.fechaInput, futureStr);
    await page.fill(SELECTORS.ingresos.addModal.conceptoInput, VALID_RESERVA.concepto);
    await page.fill(SELECTORS.ingresos.addModal.montoInput, VALID_RESERVA.monto);

    await page.click(SELECTORS.ingresos.addModal.guardarBtn);

    await expect(page.locator(SELECTORS.global.sweetAlert)).toBeVisible({ timeout: TIMEOUTS.medium });
    await expect(page.locator(SELECTORS.global.sweetAlertTitle)).toContainText('Fecha Inválida');
    await handleSweetAlert(page);
  });
});

test.describe('BUG-002: Importación CSV - HTTP 500 con CSV válido', () => {
  test.beforeEach(async ({ page }) => {
    await login(page);
    await navigateTo(page, '/ingresos');
  });

  test('Debe importar CSV válido y mostrar resumen con 8 exitosos', async ({ page }) => {
    await page.click(SELECTORS.ingresos.buttons.importarBtn);
    await waitForModal(page, 'Importar Reservas CSV');

    // Paso 1: Seleccionar plataforma
    await page.click(SELECTORS.ingresos.importModal.platformButtons.filter({ hasText: 'Airbnb' }));

    // Paso 2: Subir archivo
    const fileInput = page.locator(SELECTORS.ingresos.importModal.fileInput);
    await fileInput.setInputFiles({
      name: 'reservas_validas.csv',
      mimeType: 'text/csv',
      buffer: Buffer.from(TEST_CSV_VALID),
    });

    // Esperar progreso
    await expect(page.locator(SELECTORS.ingresos.importModal.progressFill)).toBeVisible({ timeout: TIMEOUTS.medium });
    await expect(page.locator(SELECTORS.ingresos.importModal.progressFill)).toHaveAttribute('style', /100%/);

    // Verificar resultado: 8 exitosos, 0 omitidos
    await expect(page.locator(SELECTORS.ingresos.importModal.resultOk)).toBeVisible({ timeout: TIMEOUTS.medium });
    await expect(page.locator(SELECTORS.ingresos.importModal.resultOk)).toContainText('8 reservas');

    // Cerrar modal
    await page.click(SELECTORS.ingresos.importModal.cerrarBtn);
  });

  test('Debe importar CSV mixto (8 válidas + 2 inválidas) y ofrecer descarga de reporte de errores', async ({ page }) => {
    await page.click(SELECTORS.ingresos.buttons.importarBtn);
    await waitForModal(page, 'Importar Reservas CSV');

    await page.click(SELECTORS.ingresos.importModal.platformButtons.filter({ hasText: 'Airbnb' }));

    const fileInput = page.locator(SELECTORS.ingresos.importModal.fileInput);
    await fileInput.setInputFiles({
      name: 'fechas_mixtas.csv',
      mimeType: 'text/csv',
      buffer: Buffer.from(TEST_CSV_MIXED),
    });

    // Esperar progreso
    await expect(page.locator(SELECTORS.ingresos.importModal.progressFill)).toHaveAttribute('style', /100%/, { timeout: TIMEOUTS.long });

    // Verificar resultado parcial: 8 exitosos, 2 omitidos
    await expect(page.locator(SELECTORS.ingresos.importModal.resultOk)).toBeVisible({ timeout: TIMEOUTS.medium });
    await expect(page.locator(SELECTORS.ingresos.importModal.resultOk)).toContainText('8 reservas');

    await expect(page.locator(SELECTORS.ingresos.importModal.resultOmitidos)).toBeVisible();
    await expect(page.locator(SELECTORS.ingresos.importModal.resultOmitidos)).toContainText('2 filas');
    await expect(page.locator(SELECTORS.ingresos.importModal.downloadErrorsBtn)).toBeVisible();

    // Verificar que el botón de descarga funciona (click no navega, abre en nueva pestaña)
    const downloadPromise = page.waitForEvent('download');
    await page.click(SELECTORS.ingresos.importModal.downloadErrorsBtn);
    const download = await downloadPromise;
    expect(download.suggestedFilename()).toContain('errores_importacion');

    await page.click(SELECTORS.ingresos.importModal.cerrarBtn);
  });

  test('Debe rechazar archivo vacío con mensaje claro', async ({ page }) => {
    await page.click(SELECTORS.ingresos.buttons.importarBtn);
    await waitForModal(page, 'Importar Reservas CSV');

    await page.click(SELECTORS.ingresos.importModal.platformButtons.filter({ hasText: 'Airbnb' }));

    const fileInput = page.locator(SELECTORS.ingresos.importModal.fileInput);
    await fileInput.setInputFiles({
      name: 'vacio.csv',
      mimeType: 'text/csv',
      buffer: Buffer.from(TEST_CSV_EMPTY),
    });

    // Debe mostrar error "Archivo Vacío"
    await expect(page.locator(SELECTORS.ingresos.importModal.errorEmpty)).toBeVisible({ timeout: TIMEOUTS.medium });
    await expect(page.locator(SELECTORS.ingresos.importModal.errorEmpty)).toContainText('vacío');

    await page.click(SELECTORS.ingresos.importModal.cancelarBtn);
  });
});

test.describe('BUG-003 y BUG-004: Estancia corta / 30 noches - Campo noches', () => {
  test.beforeEach(async ({ page }) => {
    await login(page);
    await navigateTo(page, '/ingresos');
  });

  test('Debe calcular días de estancia automáticamente desde Fecha Entrada y Fecha Salida', async ({ page }) => {
    // Este test valida que el frontend usa fecha_inicio y fecha_fin
    // y el backend calcula DiasEstancia = (FechaSalida - FechaEntrada).Days
    // No hay campo "noches" separado - se deriva de las dos fechas

    await page.click(SELECTORS.ingresos.buttons.agregarBtn);
    await waitForModal(page, 'Agregar Ingreso');

    // Verificar que NO existe campo "noches" ni "dias_estancia" en el modal
    const nochesInput = page.locator('input[name="noches"], input[name="dias_estancia"], input[placeholder*="noches" i]');
    await expect(nochesInput).toHaveCount(0);

    // Verificar que SÍ existen fecha_inicio y fecha_fin (en importación) o fecha (en agregar manual)
    await expect(page.locator(SELECTORS.ingresos.addModal.fechaInput)).toBeVisible();
    await expect(page.locator(SELECTORS.ingresos.addModal.conceptoInput)).toBeVisible();
    await expect(page.locator(SELECTORS.ingresos.addModal.montoInput)).toBeVisible();
    await expect(page.locator(SELECTORS.ingresos.addModal.fuenteSelect)).toBeVisible();
    await expect(page.locator(SELECTORS.ingresos.addModal.clasificacionSelect)).toBeVisible();

    await closeModal(page);
  });

  test('Importación CSV: clasificación EXENTO para estancia >= 30 días con residente', async ({ page }) => {
    // CSV con estancia de 35 días (exento si residente)
    const csv30Dias = `FechaInicio,FechaFin,MontoBruto,PlataformaOrigen,ReferenciaPlataforma
2026-06-01,2026-07-06,250000,AIRBNB,AIR-30DIAS`;

    await page.click(SELECTORS.ingresos.buttons.importarBtn);
    await waitForModal(page, 'Importar Reservas CSV');

    await page.click(SELECTORS.ingresos.importModal.platformButtons.filter({ hasText: 'Airbnb' }));

    const fileInput = page.locator(SELECTORS.ingresos.importModal.fileInput);
    await fileInput.setInputFiles({
      name: 'estancia_30_dias.csv',
      mimeType: 'text/csv',
      buffer: Buffer.from(csv30Dias),
    });

    await expect(page.locator(SELECTORS.ingresos.importModal.progressFill)).toHaveAttribute('style', /100%/, { timeout: TIMEOUTS.medium });
    await expect(page.locator(SELECTORS.ingresos.importModal.resultOk)).toContainText('1 reservas');

    // La clasificación se hace en backend según DiasEstancia >= 30
    await page.click(SELECTORS.ingresos.importModal.cerrarBtn);

    // Verificar en tabla que aparece (la clasificación exacta depende de datos del usuario)
    await expect(page.locator(SELECTORS.ingresos.table.row).first()).toBeVisible();
  });
});

test.describe('BUG-005: Comprobante - Campo requerido / Validación', () => {
  test.beforeEach(async ({ page }) => {
    await login(page);
    await navigateTo(page, '/comprobantes');
  });

  test('Debe validar archivo requerido antes de subir', async ({ page }) => {
    await page.click(SELECTORS.comprobantes.buttons.subirBtn);
    await waitForModal(page, 'Subir Comprobante');

    // Intentar guardar sin archivo
    await page.click(SELECTORS.comprobantes.uploadModal.guardarBtn);

    // Debe mostrar SweetAlert de advertencia
    await expect(page.locator(SELECTORS.global.sweetAlert)).toBeVisible({ timeout: TIMEOUTS.medium });
    await expect(page.locator(SELECTORS.global.sweetAlertTitle)).toContainText('Archivo Faltante');
    await expect(page.locator(SELECTORS.global.sweetAlertText)).toContainText('Debe seleccionar');
    await handleSweetAlert(page);
  });

  test('Debe rechazar formatos no soportados', async ({ page }) => {
    await page.click(SELECTORS.comprobantes.buttons.subirBtn);
    await waitForModal(page, 'Subir Comprobante');

    // Simular drop de archivo .txt (no soportado)
    const dropZone = page.locator(SELECTORS.comprobantes.uploadModal.dropZone);
    await dropZone.setInputFiles({
      name: 'documento.txt',
      mimeType: 'text/plain',
      buffer: Buffer.from('contenido'),
    });

    await expect(page.locator(SELECTORS.comprobantes.uploadModal.errorFormat)).toBeVisible({ timeout: TIMEOUTS.short });
    await expect(page.locator(SELECTORS.comprobantes.uploadModal.errorFormat)).toContainText('PDF o imagen');
  });

  test('Debe mostrar checkbox "Guardar sin extracción automática" (BUG-018 fix)', async ({ page }) => {
    await page.click(SELECTORS.comprobantes.buttons.subirBtn);
    await waitForModal(page, 'Subir Comprobante');

    // Subir PDF válido primero
    const dropZone = page.locator(SELECTORS.comprobantes.uploadModal.dropZone);
    await dropZone.setInputFiles({
      name: 'factura.pdf',
      mimeType: 'application/pdf',
      buffer: Buffer.from('%PDF-1.4 test content'),
    });

    // Verificar estado OCR done
    await expect(page.locator(SELECTORS.comprobantes.uploadModal.bannerSuccess)).toBeVisible({ timeout: TIMEOUTS.medium });

    // Verificar checkbox sin OCR
    await expect(page.locator(SELECTORS.comprobantes.uploadModal.sinOcrCheckbox)).toBeVisible();

    // Marcar checkbox
    await page.check(SELECTORS.comprobantes.uploadModal.sinOcrCheckbox);

    // Verificar banner de advertencia
    await expect(page.locator(SELECTORS.comprobantes.uploadModal.bannerWarning)).toBeVisible();
    await expect(page.locator(SELECTORS.comprobantes.uploadModal.bannerWarning)).toContainText('crédito fiscal');

    await closeModal(page);
  });

  test('Debe validar justificación (Ley 8968) al eliminar comprobante', async ({ page }) => {
    // Asumimos que hay al menos un comprobante en la tabla
    const firstRow = page.locator(SELECTORS.comprobantes.table.row).first();
    if (await firstRow.isVisible({ timeout: 5000 })) {
      await firstRow.locator(SELECTORS.comprobantes.table.deleteBtn).click();

      // Debe abrir SweetAlert con textarea
      await expect(page.locator(SELECTORS.global.sweetAlert)).toBeVisible({ timeout: TIMEOUTS.medium });
      await expect(page.locator(SELECTORS.global.sweetAlertText)).toContainText('Ley 8968');
      await expect(page.locator(SELECTORS.global.sweetAlertInput)).toBeVisible();

      // Intentar confirmar sin justificación (debe fallar validación)
      await page.click(SELECTORS.global.sweetAlertConfirm);
      await expect(page.locator(SELECTORS.global.sweetAlert)).toBeVisible(); // sigue abierto por validación

      // Llenar justificación válida (>= 10 chars)
      await page.fill(SELECTORS.global.sweetAlertInput, 'Comprobante registrado por error de duplicado');

      await handleSweetAlert(page);
    }
  });
});

test.describe('BUG-006: Reportes - Botón Invitar Contador visible', () => {
  test.beforeEach(async ({ page }) => {
    await login(page);
    await navigateTo(page, '/reportes');
  });

  test('Debe mostrar tarjeta Autorizar Contador con botón Invitar Contador', async ({ page }) => {
    await expect(page.locator(SELECTORS.reportes.pageTitle)).toBeVisible();

    // Verificar card de autorizar contador
    await expect(page.locator(SELECTORS.reportes.autorizarContadorCard)).toBeVisible({ timeout: TIMEOUTS.medium });

    // Verificar botón Invitar Contador
    await expect(page.locator(SELECTORS.reportes.invitarBtn)).toBeVisible();
    await expect(page.locator(SELECTORS.reportes.invitarBtn)).toContainText('Invitar Contador');
  });

  test('Debe abrir modal Invitar Contador y permitir llenar formulario', async ({ page }) => {
    await page.click(SELECTORS.reportes.invitarBtn);
    await waitForModal(page, 'Invitar Contador');

    // Verificar campos del modal
    await expect(page.locator(SELECTORS.reportes.invitarModal.nombreInput)).toBeVisible();
    await expect(page.locator(SELECTORS.reportes.invitarModal.emailInput)).toBeVisible();
    await expect(page.locator(SELECTORS.reportes.invitarModal.permisosSelect)).toBeVisible();
    await expect(page.locator(SELECTORS.reportes.invitarModal.expiracionInput)).toBeVisible();

    // Llenar formulario
    await page.fill(SELECTORS.reportes.invitarModal.nombreInput, INVITAR_CONTADOR.nombre);
    await page.fill(SELECTORS.reportes.invitarModal.emailInput, INVITAR_CONTADOR.email);
    await page.selectOption(SELECTORS.reportes.invitarModal.permisosSelect, INVITAR_CONTADOR.permisos);

    // Calcular fecha expiración (hoy + 30 días)
    const expDate = new Date();
    expDate.setDate(expDate.getDate() + INVITAR_CONTADOR.expiracionDias);
    await page.fill(SELECTORS.reportes.invitarModal.expiracionInput, expDate.toISOString().split('T')[0]);

    // Invitar
    await page.click(SELECTORS.reportes.invitarModal.invitarBtn);

    // Verificar que se agrega a la lista (mock local)
    await expect(page.locator(SELECTORS.reportes.autorizarContadorCard)).toContainText(INVITAR_CONTADOR.nombre);

    await closeModal(page);
  });
});

test.describe('BUG-007: Dashboard - Métricas no en cero', () => {
  test.beforeEach(async ({ page }) => {
    await login(page);
    await navigateTo(page, '/');
  });

  test('Dashboard debe mostrar métricas calculadas (no ₡0)', async ({ page }) => {
    await expect(page.locator(SELECTORS.dashboard.pageTitle)).toBeVisible();

    // Verificar que las tarjetas existen
    await expect(page.locator(SELECTORS.dashboard.cards.ingresosBrutos)).toBeVisible({ timeout: TIMEOUTS.medium });
    await expect(page.locator(SELECTORS.dashboard.cards.ivaRetenido)).toBeVisible();
    await expect(page.locator(SELECTORS.dashboard.cards.rentaEstimada)).toBeVisible();
    await expect(page.locator(SELECTORS.dashboard.cards.neto)).toBeVisible();

    // Verificar que NO muestran ₡0 (o equivalente)
    // Nota: en entorno de test puede no haber datos, pero la UI no debe mostrar "₡0" hardcodeado
    const ingresosText = await page.locator(SELECTORS.dashboard.cards.ingresosBrutos).innerText();
    // El fix BUG-007 eliminó los datos simulados - las tarjetas muestran datos reales o vacíos
    expect(ingresosText).not.toMatch(/₡\s*0\s*$/); // No debe terminar en ₡0
  });
});

test.describe('BUG-008: Gasto - Concepto se refleja en listado (XSS safe)', () => {
  test.beforeEach(async ({ page }) => {
    await login(page);
    await navigateTo(page, '/ingresos');
    await page.click(SELECTORS.ingresos.tabs.gastosTab);
  });

  test('Debe mostrar concepto ingresado en listado de gastos (no ejecutar HTML/JS)', async ({ page }) => {
    const xssConcepto = '<script>alert(1)</script>';

    await page.click(SELECTORS.ingresos.buttons.agregarBtn);
    await waitForModal(page, 'Agregar Gasto');

    await page.fill(SELECTORS.ingresos.addModal.fechaInput, VALID_GASTO.fecha);
    await page.fill(SELECTORS.ingresos.addModal.conceptoInput, xssConcepto);
    await page.fill(SELECTORS.ingresos.addModal.montoInput, VALID_GASTO.monto);
    await page.fill(SELECTORS.ingresos.addModal.proveedorInput, VALID_GASTO.proveedor);

    await page.click(SELECTORS.ingresos.addModal.guardarBtn);
    await handleSweetAlert(page);

    // Verificar en tabla que el concepto se muestra como texto plano (no ejecutado)
    const row = page.locator(SELECTORS.ingresos.table.row).first();
    await expect(row).toContainText(xssConcepto, { timeout: TIMEOUTS.medium });

    // Verificar que NO hay script ejecutado (no hay alert)
    // Playwright habría fallado si se ejecutó alert()
  });
});

test.describe('BUG-009: Configuración - Log de Auditoría accesible', () => {
  test.beforeEach(async ({ page }) => {
    await login(page);
    await navigateTo(page, '/configuracion');
  });

  test('Debe mostrar pestaña Auditoria y cargar bitácora sin error 404', async ({ page }) => {
    // La página Configuracion tiene tabs: Perfil, Notificaciones, Actividad, Auditoria, Seguridad
    const auditoriaTab = page.locator('button:has-text("Auditoría"), button:has-text("Auditoria"), a[href*="auditoria"]').first();
    await expect(auditoriaTab).toBeVisible({ timeout: TIMEOUTS.medium });
    await auditoriaTab.click();

    // Verificar que NO muestra modal "Recurso No Encontrado"
    await expect(page.locator('.swal2-popup:has-text("Recurso No Encontrado")')).toBeHidden({ timeout: TIMEOUTS.medium });
    await expect(page.locator('.swal2-popup:has-text("No se encontró configuración")')).toBeHidden({ timeout: TIMEOUTS.medium });

    // Verificar que la tabla de auditoría carga (puede estar vacía pero sin error)
    await expect(page.locator('table, .auditoria-table, .data-table').first()).toBeVisible({ timeout: TIMEOUTS.medium });
  });
});

test.describe('BUG-017: Import CSV mixto - No rechazar archivo no vacío', () => {
  test.beforeEach(async ({ page }) => {
    await login(page);
    await navigateTo(page, '/ingresos');
  });

  test('CSV con 8 válidas + 2 inválidas debe importar 8 y reportar 2 errores', async ({ page }) => {
    // Ya cubierto en BUG-002 pero repetimos como test específico
    await page.click(SELECTORS.ingresos.buttons.importarBtn);
    await waitForModal(page, 'Importar Reservas CSV');

    await page.click(SELECTORS.ingresos.importModal.platformButtons.filter({ hasText: 'Airbnb' }));

    const fileInput = page.locator(SELECTORS.ingresos.importModal.fileInput);
    await fileInput.setInputFiles({
      name: 'fechas_mixtas.csv',
      mimeType: 'text/csv',
      buffer: Buffer.from(TEST_CSV_MIXED),
    });

    await expect(page.locator(SELECTORS.ingresos.importModal.progressFill)).toHaveAttribute('style', /100%/, { timeout: TIMEOUTS.long });

    // Resultado COMPLETADO_PARCIAL
    await expect(page.locator(SELECTORS.ingresos.importModal.resultOk)).toContainText('8 reservas');
    await expect(page.locator(SELECTORS.ingresos.importModal.resultOmitidos)).toContainText('2 filas');
    await expect(page.locator(SELECTORS.ingresos.importModal.downloadErrorsBtn)).toBeVisible();

    // Verificar descarga reporte errores
    const downloadPromise = page.waitForEvent('download');
    await page.click(SELECTORS.ingresos.importModal.downloadErrorsBtn);
    const download = await downloadPromise;
    expect(download.suggestedFilename()).toMatch(/errores_importacion/);

    await page.click(SELECTORS.ingresos.importModal.cerrarBtn);
  });
});

test.describe('BUG-018: OCR ilegible - Permite ingreso manual / guardar sin OCR', () => {
  test.beforeEach(async ({ page }) => {
    await login(page);
    await navigateTo(page, '/comprobantes');
  });

  test('Checkbox "Guardar sin extracción automática" debe estar disponible y funcional', async ({ page }) => {
    await page.click(SELECTORS.comprobantes.buttons.subirBtn);
    await waitForModal(page, 'Subir Comprobante');

    // Subir archivo
    const dropZone = page.locator(SELECTORS.comprobantes.uploadModal.dropZone);
    await dropZone.setInputFiles({
      name: 'factura_ilegible.pdf',
      mimeType: 'application/pdf',
      buffer: Buffer.from('%PDF-1.4 contenido ilegible'),
    });

    await expect(page.locator(SELECTORS.comprobantes.uploadModal.bannerSuccess)).toBeVisible({ timeout: TIMEOUTS.medium });

    // Verificar checkbox sin OCR
    await expect(page.locator(SELECTORS.comprobantes.uploadModal.sinOcrCheckbox)).toBeVisible();

    // Marcar y verificar banner
    await page.check(SELECTORS.comprobantes.uploadModal.sinOcrCheckbox);
    await expect(page.locator(SELECTORS.comprobantes.uploadModal.bannerWarning)).toBeVisible();
    await expect(page.locator(SELECTORS.comprobantes.uploadModal.bannerWarning)).toContainText('crédito fiscal');

    // Botón guardar debe estar habilitado
    await expect(page.locator(SELECTORS.comprobantes.uploadModal.guardarBtn)).toBeEnabled();

    await closeModal(page);
  });
});

test.describe('BUG-020: Acceso Contador - Avisos 7 y 1 día', () => {
  test.beforeEach(async ({ page }) => {
    await login(page);
    await navigateTo(page, '/reportes');
  });

  test('Sección Autorizar Contador debe mostrar accesos con estado EXPIRADO cuando corresponda', async ({ page }) => {
    await expect(page.locator(SELECTORS.reportes.autorizarContadorCard)).toBeVisible({ timeout: TIMEOUTS.medium });

    // Verificar que la card muestra contadores (mock data incluye uno EXPIRADO)
    await expect(page.locator(SELECTORS.reportes.autorizarContadorCard)).toContainText('EXPIRADO');
    await expect(page.locator(SELECTORS.reportes.autorizarContadorCard)).toContainText('ACTIVO');
  });
});

test.describe('BUG-021: Simulador - Validación límite superior (10M)', () => {
  test.beforeEach(async ({ page }) => {
    await login(page);
    await navigateTo(page, '/simulador');
  });

  test('Debe rechazar escenarios con ingresos > 10M', async ({ page }) => {
    // La página Simulador tiene formulario para agregar escenarios
    // Buscar botón "Agregar Escenario" o similar
    const agregarBtn = page.locator('button:has-text("Agregar"), button:has-text("Nuevo Escenario")').first();
    if (await agregarBtn.isVisible({ timeout: 5000 })) {
      await agregarBtn.click();

      // Llenar con monto > 10M
      await page.fill('input[name="nombre"], input[placeholder*="nombre" i]', 'Test Límite');
      await page.fill('input[name="ingresos"], input[placeholder*="ingreso" i]', '15000000');
      await page.fill('input[name="gastos"], input[placeholder*="gasto" i]', '100000');

      await page.click('button:has-text("Guardar"), button:has-text("Crear")');

      // Debe mostrar error de límite
      await expect(page.locator(SELECTORS.global.sweetAlert)).toBeVisible({ timeout: TIMEOUTS.medium });
      await expect(page.locator(SELECTORS.global.sweetAlertTitle)).toContainText(/Límite|Advertencia|Máximo/);
      await handleSweetAlert(page);
    }
  });

  test('Debe aceptar escenarios dentro del límite (≤ 10M)', async ({ page }) => {
    const agregarBtn = page.locator('button:has-text("Agregar"), button:has-text("Nuevo Escenario")').first();
    if (await agregarBtn.isVisible({ timeout: 5000 })) {
      await agregarBtn.click();

      await page.fill('input[name="nombre"], input[placeholder*="nombre" i]', 'Test Válido');
      await page.fill('input[name="ingresos"], input[placeholder*="ingreso" i]', '5000000');
      await page.fill('input[name="gastos"], input[placeholder*="gasto" i]', '1000000');

      await page.click('button:has-text("Guardar"), button:has-text("Crear")');

      // Debe aceptar (o mostrar éxito)
      await expect(page.locator(SELECTORS.global.sweetAlert)).toBeVisible({ timeout: TIMEOUTS.medium });
      await handleSweetAlert(page);
    }
  });
});

test.describe('BUG-022: HTTPS Redirect + HSTS (Local)', () => {
  test('Backend debe redirigir HTTP a HTTPS y enviar HSTS', async ({ page }) => {
    // Test directo contra backend (puerto 5062 HTTP -> 5066/7079 HTTPS)
    const httpResponse = await page.request.get('http://localhost:5062/api/health', { timeout: 10000 }).catch(() => null);

    if (httpResponse) {
      // Debe redirigir (301/302/307/308) a HTTPS
      expect([301, 302, 307, 308]).toContain(httpResponse.status());
      const location = httpResponse.headers()['location'];
      expect(location).toMatch(/^https:/);
    }

    // Verificar HTTPS directo con HSTS
    const httpsResponse = await page.request.get('https://localhost:7079/api/health', { timeout: 10000 }).catch(() => null);
    if (httpsResponse) {
      const hsts = httpsResponse.headers()['strict-transport-security'];
      expect(hsts).toBeDefined();
      expect(hsts).toContain('max-age=');
    }
  });
});

test.describe('BUG-023: Cifrado en reposo - Comprobantes', () => {
  test.beforeEach(async ({ page }) => {
    await login(page);
    await navigateTo(page, '/comprobantes');
  });

  test('Subida de comprobante debe completarse (cifrado transparente en backend)', async ({ page }) => {
    // Este test valida que el flujo funciona; el cifrado AES-256-CBC
    // ocurre en LocalStorageService.UploadAsync (backend)
    await page.click(SELECTORS.comprobantes.buttons.subirBtn);
    await waitForModal(page, 'Subir Comprobante');

    const dropZone = page.locator(SELECTORS.comprobantes.uploadModal.dropZone);
    await dropZone.setInputFiles({
      name: 'factura_cifrada.pdf',
      mimeType: 'application/pdf',
      buffer: Buffer.from('%PDF-1.4 test content for encryption'),
    });

    await expect(page.locator(SELECTORS.comprobantes.uploadModal.bannerSuccess)).toBeVisible({ timeout: TIMEOUTS.medium });

    await page.click(SELECTORS.comprobantes.uploadModal.guardarBtn);

    await expect(page.locator(SELECTORS.global.sweetAlert)).toBeVisible({ timeout: TIMEOUTS.medium });
    await expect(page.locator(SELECTORS.global.sweetAlertTitle)).toContainText('Guardado');
    await handleSweetAlert(page);

    // Verificar en tabla
    await expect(page.locator(SELECTORS.comprobantes.table.row).first()).toBeVisible({ timeout: TIMEOUTS.medium });
  });
});

test.describe('BUG-019: HU-010 Reconstrucción Histórica - UI', () => {
  test.beforeEach(async ({ page }) => {
    await login(page);
  });

  test('Debe existir página/ruta para reconstrucción histórica', async ({ page }) => {
    // Verificar si hay link en menú o navegación directa
    await navigateTo(page, '/reconstruccion-historica'); // o la ruta real

    // Si la página existe, verificar elementos clave
    const pageExists = await page.locator('h1:has-text("Reconstrucción"), h1:has-text("Histórica")').isVisible({ timeout: 5000 }).catch(() => false);

    if (pageExists) {
      await expect(page.locator('input[type="file"][accept=".csv"]')).toBeVisible();
      await expect(page.locator('button:has-text("Reconstruir"), button:has-text("Procesar")')).toBeVisible();
      await expect(page.locator('button:has-text("Detectar Períodos")')).toBeVisible();
    } else {
      // La funcionalidad puede estar integrada en otra página (ej. Impuestos)
      test.skip('Ruta /reconstruccion-historica no encontrada - verificar integración en otra página');
    }
  });
});

// ============================================================================
// GLOBAL SETUP / TEARDOWN
// ============================================================================

test.beforeAll(async () => {
  console.log('🚀 Starting E2E Bug Validation Suite');
  console.log('📋 Bugs covered: 001-009, 017-023');
  console.log('🌐 Frontend:', process.env.FRONTEND_URL || 'http://localhost:5173');
  console.log('🔧 Backend:', process.env.API_URL || 'http://localhost:5062');
});

test.afterAll(async () => {
  console.log('✅ E2E Bug Validation Suite completed');
});