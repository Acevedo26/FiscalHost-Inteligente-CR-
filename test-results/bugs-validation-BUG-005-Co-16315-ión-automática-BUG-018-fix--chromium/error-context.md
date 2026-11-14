# Instructions

- Following Playwright test failed.
- Explain why, be concise, respect Playwright best practices.
- Provide a snippet of code with the fix, if possible.

# Test info

- Name: bugs-validation.spec.ts >> BUG-005: Comprobante - Campo requerido / Validación >> Debe mostrar checkbox "Guardar sin extracción automática" (BUG-018 fix)
- Location: tests\e2e\bugs-validation.spec.ts:425:3

# Error details

```
Error: expect(locator).toBeVisible() failed

Locator: locator('input[type="checkbox"]')
Expected: visible
Timeout: 5000ms
Error: element(s) not found

Call log:
  - Expect "toBeVisible" locator('input[type="checkbox"]') with timeout 5000ms
  - waiting for locator('input[type="checkbox"]')

```

```yaml
- complementary:
  - img
  - text: FiscalHost
  - button "Colapsar barra lateral"
  - navigation:
    - link "Dashboard":
      - /url: /
    - link "Mis Ingresos":
      - /url: /ingresos
    - link "Centro de Cálculo":
      - /url: /impuestos
    - link "Alertas 2":
      - /url: /alertas
    - link "Reportes":
      - /url: /reportes
    - link "Simulador":
      - /url: /simulador
    - link "Comprobantes":
      - /url: /comprobantes
    - link "Autoliquidación":
      - /url: /autoliquidacion
    - link "Mora e Intereses":
      - /url: /mora
    - link "Guía Educativa":
      - /url: /help
    - link "Configuración":
      - /url: /configuracion
  - text: ET E2E Test User e2e.test@test.com
- banner:
  - text: Comprobantes de Gastos
  - img
  - textbox "Buscar..."
  - button "2":
    - img
    - text: "2"
  - button "ET"
- main:
  - heading "Comprobantes de Gastos" [level=1]
  - paragraph: Gestiona tus facturas y crédito fiscal
  - button "Subir Comprobante"
  - text: Comprobantes validados 0 Crédito fiscal disponible ₡0 Sin OCR (pendientes) 0
  - table:
    - rowgroup:
      - row "N° Factura Proveedor Fecha Monto Estado Crédito Fiscal Acciones":
        - columnheader "N° Factura"
        - columnheader "Proveedor"
        - columnheader "Fecha"
        - columnheader "Monto"
        - columnheader "Estado"
        - columnheader "Crédito Fiscal"
        - columnheader "Acciones"
    - rowgroup:
      - row "No hay comprobantes.":
        - cell "No hay comprobantes."
- button "Abrir asistente FisCal":
  - img
  - text: ¿Necesitas ayuda?
- button "Abrir asistente FisCal":
  - img
- dialog:
  - heading "Subir Comprobante" [level=3]
  - button "Cerrar":
    - img
  - text: "Archivo seleccionado: factura.pdf. Haz clic en Guardar para procesar con OCR en el servidor."
  - button "Cancelar"
  - button "Confirmar y Guardar"
```

# Test source

```ts
  341 |     // No hay campo "noches" separado - se deriva de las dos fechas
  342 | 
  343 |     await page.click(SELECTORS.ingresos.buttons.agregarBtn);
  344 |     await waitForModal(page, 'Agregar Ingreso');
  345 | 
  346 |     // Verificar que NO existe campo "noches" ni "dias_estancia" en el modal
  347 |     const nochesInput = page.locator('input[name="noches"], input[name="dias_estancia"], input[placeholder*="noches" i]');
  348 |     await expect(nochesInput).toHaveCount(0);
  349 | 
  350 |     // Verificar que SÍ existen fecha_inicio y fecha_fin (en importación) o fecha (en agregar manual)
  351 |     await expect(page.locator(SELECTORS.ingresos.addModal.fechaInput)).toBeVisible();
  352 |     await expect(page.locator(SELECTORS.ingresos.addModal.conceptoInput)).toBeVisible();
  353 |     await expect(page.locator(SELECTORS.ingresos.addModal.montoInput)).toBeVisible();
  354 |     // Custom Select components (check trigger buttons)
  355 |     await expect(page.locator(SELECTORS.ingresos.addModal.fuenteSelect)).toBeVisible();
  356 |     await expect(page.locator(SELECTORS.ingresos.addModal.clasificacionSelect)).toBeVisible();
  357 | 
  358 |     await closeModal(page);
  359 |   });
  360 | 
  361 |   test('Importación CSV: clasificación EXENTO para estancia >= 30 días con residente', async ({ page }) => {
  362 |     // CSV con estancia de 35 días (exento si residente)
  363 |     const csv30Dias = `FechaInicio,FechaFin,MontoBruto,PlataformaOrigen,ReferenciaPlataforma
  364 | 2026-06-01,2026-07-06,250000,AIRBNB,AIR-30DIAS`;
  365 | 
  366 |     await page.click(SELECTORS.ingresos.buttons.importarBtn);
  367 |     await waitForModal(page, 'Importar Reservas CSV');
  368 | 
  369 |     await page.locator(SELECTORS.ingresos.importModal.platformButtons).filter({ hasText: 'Airbnb' }).click();
  370 | 
  371 |     const fileInput = page.locator(SELECTORS.ingresos.importModal.fileInput);
  372 |     await fileInput.setInputFiles({
  373 |       name: 'estancia_30_dias.csv',
  374 |       mimeType: 'text/csv',
  375 |       buffer: Buffer.from(csv30Dias),
  376 |     });
  377 | 
  378 |     await expect(page.locator(SELECTORS.ingresos.importModal.progressFill)).toHaveAttribute('style', /100%/, { timeout: TIMEOUTS.medium });
  379 |     await expect(page.locator(SELECTORS.ingresos.importModal.resultOk)).toContainText('1 reservas');
  380 | 
  381 |     // La clasificación se hace en backend según DiasEstancia >= 30
  382 |     await page.click(SELECTORS.ingresos.importModal.cerrarBtn);
  383 | 
  384 |     // Verificar en tabla que aparece (la clasificación exacta depende de datos del usuario)
  385 |     await expect(page.locator(SELECTORS.ingresos.table.row).first()).toBeVisible();
  386 |   });
  387 | });
  388 | 
  389 | test.describe('BUG-005: Comprobante - Campo requerido / Validación', () => {
  390 |   test.beforeEach(async ({ page }) => {
  391 |     await login(page);
  392 |     await navigateTo(page, '/comprobantes');
  393 |   });
  394 | 
  395 |   test('Debe validar archivo requerido antes de subir', async ({ page }) => {
  396 |     await page.click(SELECTORS.comprobantes.buttons.subirBtn);
  397 |     await waitForModal(page, 'Subir Comprobante');
  398 | 
  399 |     // Intentar guardar sin archivo
  400 |     await page.click(SELECTORS.comprobantes.uploadModal.guardarBtn);
  401 | 
  402 |     // Debe mostrar SweetAlert de advertencia
  403 |     await expect(page.locator(SELECTORS.global.sweetAlert)).toBeVisible({ timeout: TIMEOUTS.medium });
  404 |     await expect(page.locator(SELECTORS.global.sweetAlertTitle)).toContainText('Archivo Faltante');
  405 |     await expect(page.locator(SELECTORS.global.sweetAlertText)).toContainText('Debe seleccionar');
  406 |     await handleSweetAlert(page);
  407 |   });
  408 | 
  409 |   test('Debe rechazar formatos no soportados', async ({ page }) => {
  410 |     await page.click(SELECTORS.comprobantes.buttons.subirBtn);
  411 |     await waitForModal(page, 'Subir Comprobante');
  412 | 
  413 |     // Simular drop de archivo .txt (no soportado)
  414 |     const fileInput = page.locator(SELECTORS.comprobantes.uploadModal.fileInput);
  415 |     await fileInput.setInputFiles({
  416 |       name: 'documento.txt',
  417 |       mimeType: 'text/plain',
  418 |       buffer: Buffer.from('contenido'),
  419 |     });
  420 | 
  421 |     await expect(page.locator(SELECTORS.comprobantes.uploadModal.errorFormat)).toBeVisible({ timeout: TIMEOUTS.short });
  422 |     await expect(page.locator(SELECTORS.comprobantes.uploadModal.errorFormat)).toContainText('PDF o imagen');
  423 |   });
  424 | 
  425 |   test('Debe mostrar checkbox "Guardar sin extracción automática" (BUG-018 fix)', async ({ page }) => {
  426 |     await page.click(SELECTORS.comprobantes.buttons.subirBtn);
  427 |     await waitForModal(page, 'Subir Comprobante');
  428 | 
  429 |     // Subir PDF válido primero
  430 |     const fileInput = page.locator(SELECTORS.comprobantes.uploadModal.fileInput);
  431 |     await fileInput.setInputFiles({
  432 |       name: 'factura.pdf',
  433 |       mimeType: 'application/pdf',
  434 |       buffer: Buffer.from('%PDF-1.4 test content'),
  435 |     });
  436 | 
  437 |     // Verificar estado OCR done
  438 |     await expect(page.locator(SELECTORS.comprobantes.uploadModal.bannerSuccess)).toBeVisible({ timeout: TIMEOUTS.medium });
  439 | 
  440 |     // Verificar checkbox sin OCR
> 441 |     await expect(page.locator(SELECTORS.comprobantes.uploadModal.sinOcrCheckbox)).toBeVisible();
      |                                                                                   ^ Error: expect(locator).toBeVisible() failed
  442 | 
  443 |     // Marcar checkbox
  444 |     await page.check(SELECTORS.comprobantes.uploadModal.sinOcrCheckbox);
  445 | 
  446 |     // Verificar banner de advertencia
  447 |     await expect(page.locator(SELECTORS.comprobantes.uploadModal.bannerWarning)).toBeVisible();
  448 |     await expect(page.locator(SELECTORS.comprobantes.uploadModal.bannerWarning)).toContainText('crédito fiscal');
  449 | 
  450 |     await closeModal(page);
  451 |   });
  452 | 
  453 |   test('Debe validar justificación (Ley 8968) al eliminar comprobante', async ({ page }) => {
  454 |     // Asumimos que hay al menos un comprobante en la tabla
  455 |     const firstRow = page.locator(SELECTORS.comprobantes.table.row).first();
  456 |     if (await firstRow.isVisible({ timeout: 5000 })) {
  457 |       await firstRow.locator(SELECTORS.comprobantes.table.deleteBtn).click();
  458 | 
  459 |       // Debe abrir SweetAlert con textarea
  460 |       await expect(page.locator(SELECTORS.global.sweetAlert)).toBeVisible({ timeout: TIMEOUTS.medium });
  461 |       await expect(page.locator(SELECTORS.global.sweetAlertText)).toContainText('Ley 8968');
  462 |       await expect(page.locator(SELECTORS.global.sweetAlertInput)).toBeVisible();
  463 | 
  464 |       // Intentar confirmar sin justificación (debe fallar validación)
  465 |       await page.click(SELECTORS.global.sweetAlertConfirm);
  466 |       await expect(page.locator(SELECTORS.global.sweetAlert)).toBeVisible(); // sigue abierto por validación
  467 | 
  468 |       // Llenar justificación válida (>= 10 chars)
  469 |       await page.fill(SELECTORS.global.sweetAlertInput, 'Comprobante registrado por error de duplicado');
  470 | 
  471 |       await handleSweetAlert(page);
  472 |     }
  473 |   });
  474 | });
  475 | 
  476 | test.describe('BUG-006: Reportes - Botón Invitar Contador visible', () => {
  477 |   test.beforeEach(async ({ page }) => {
  478 |     await login(page);
  479 |     await navigateTo(page, '/reportes');
  480 |   });
  481 | 
  482 |   test('Debe mostrar tarjeta Autorizar Contador con botón Invitar Contador', async ({ page }) => {
  483 |     await expect(page.locator(SELECTORS.reportes.pageTitle)).toBeVisible();
  484 | 
  485 |     // Verificar card de autorizar contador
  486 |     await expect(page.locator(SELECTORS.reportes.autorizarContadorCard)).toBeVisible({ timeout: TIMEOUTS.medium });
  487 | 
  488 |     // Verificar botón Invitar Contador
  489 |     await expect(page.locator(SELECTORS.reportes.invitarBtn)).toBeVisible();
  490 |     await expect(page.locator(SELECTORS.reportes.invitarBtn)).toContainText('Invitar Contador');
  491 |   });
  492 | 
  493 |   test('Debe abrir modal Invitar Contador y permitir llenar formulario', async ({ page }) => {
  494 |     await page.click(SELECTORS.reportes.invitarBtn);
  495 |     await waitForModal(page, 'Invitar Contador');
  496 | 
  497 |     // Verificar campos del modal
  498 |     await expect(page.locator(SELECTORS.reportes.invitarModal.nombreInput)).toBeVisible();
  499 |     await expect(page.locator(SELECTORS.reportes.invitarModal.emailInput)).toBeVisible();
  500 |     await expect(page.locator(SELECTORS.reportes.invitarModal.permisosSelect)).toBeVisible();
  501 |     await expect(page.locator(SELECTORS.reportes.invitarModal.expiracionInput)).toBeVisible();
  502 | 
  503 |     // Llenar formulario
  504 |     await page.fill(SELECTORS.reportes.invitarModal.nombreInput, INVITAR_CONTADOR.nombre);
  505 |     await page.fill(SELECTORS.reportes.invitarModal.emailInput, INVITAR_CONTADOR.email);
  506 |     await page.selectOption(SELECTORS.reportes.invitarModal.permisosSelect, INVITAR_CONTADOR.permisos);
  507 | 
  508 |     // Calcular fecha expiración (hoy + 30 días)
  509 |     const expDate = new Date();
  510 |     expDate.setDate(expDate.getDate() + INVITAR_CONTADOR.expiracionDias);
  511 |     await page.fill(SELECTORS.reportes.invitarModal.expiracionInput, expDate.toISOString().split('T')[0]);
  512 | 
  513 |     // Invitar
  514 |     await page.click(SELECTORS.reportes.invitarModal.invitarBtn);
  515 | 
  516 |     // Verificar que se agrega a la lista (mock local)
  517 |     await expect(page.locator(SELECTORS.reportes.autorizarContadorCard)).toContainText(INVITAR_CONTADOR.nombre);
  518 | 
  519 |     await closeModal(page);
  520 |   });
  521 | });
  522 | 
  523 | test.describe('BUG-007: Dashboard - Métricas no en cero', () => {
  524 |   test.beforeEach(async ({ page }) => {
  525 |     await login(page);
  526 |     await navigateTo(page, '/');
  527 |   });
  528 | 
  529 |   test('Dashboard debe mostrar métricas calculadas (no ₡0)', async ({ page }) => {
  530 |     await expect(page.locator(SELECTORS.dashboard.pageTitle)).toBeVisible();
  531 | 
  532 |     // Verificar que las tarjetas existen
  533 |     await expect(page.locator(SELECTORS.dashboard.cards.ingresosBrutos)).toBeVisible({ timeout: TIMEOUTS.medium });
  534 |     await expect(page.locator(SELECTORS.dashboard.cards.ivaRetenido)).toBeVisible();
  535 |     await expect(page.locator(SELECTORS.dashboard.cards.rentaEstimada)).toBeVisible();
  536 |     await expect(page.locator(SELECTORS.dashboard.cards.neto)).toBeVisible();
  537 | 
  538 |     // Verificar que NO muestran ₡0 (o equivalente)
  539 |     // Nota: en entorno de test puede no haber datos, pero la UI no debe mostrar "₡0" hardcodeado
  540 |     const ingresosText = await page.locator(SELECTORS.dashboard.cards.ingresosBrutos).innerText();
  541 |     // El fix BUG-007 eliminó los datos simulados - las tarjetas muestran datos reales o vacíos
```