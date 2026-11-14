# Instructions

- Following Playwright test failed.
- Explain why, be concise, respect Playwright best practices.
- Provide a snippet of code with the fix, if possible.

# Test info

- Name: bugs-validation.spec.ts >> BUG-006: Reportes - Botón Invitar Contador visible >> Debe mostrar tarjeta Autorizar Contador con botón Invitar Contador
- Location: tests\e2e\bugs-validation.spec.ts:482:3

# Error details

```
Error: expect(locator).toBeVisible() failed

Locator: locator('.autorizar-contador-card')
Expected: visible
Timeout: 15000ms
Error: element(s) not found

Call log:
  - Expect "toBeVisible" locator('.autorizar-contador-card') with timeout 15000ms
  - waiting for locator('.autorizar-contador-card')

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
  - text: Reportes Fiscales
  - img
  - textbox "Buscar..."
  - button "2":
    - img
    - text: "2"
  - button "ET"
- main:
  - heading "Reportes" [level=1]
  - paragraph: Genera y exporta tus reportes fiscales
  - heading "Generar Reportes" [level=3]
  - text: Resumen Mensual Resumen de ingresos y gastos del mes Declaración IVA Formulario de declaración de IVA Declaración Renta Declaración anual de renta Detalle de Gastos Listado detallado de gastos Exportación ATV Exportación en formato ATV
  - heading "Reportes Recientes" [level=3]
  - text: Resumen Abril 2026 18/04/2026 LISTO
  - button "Descargar"
  - text: Declaración IVA - Marzo 2026 15/04/2026 ENVIADO
  - button "Descargar"
  - text: Declaración Renta 2025 01/03/2026 ENVIADO
  - button "Descargar"
  - text: Detalle Gastos 2026 20/02/2026 LISTO
  - button "Descargar"
  - heading "Autorizar Contador" [level=3]
  - button "Invitar Contador"
  - text: "MV Maria Vargas maria@contadorescr.com ACTIVO Permisos: Solo Lectura Expira: 14/6/2026"
  - button "Revocar"
  - text: "CL Carlos Lopez carlos@contabilidad.com EXPIRADO Permisos: Exportación Expira: 30/4/2026"
  - button "Revocar"
- button "Abrir asistente FisCal":
  - img
  - text: ¿Necesitas ayuda?
- button "Abrir asistente FisCal":
  - img
```

# Test source

```ts
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
  441 |     await expect(page.locator(SELECTORS.comprobantes.uploadModal.sinOcrCheckbox)).toBeVisible();
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
> 486 |     await expect(page.locator(SELECTORS.reportes.autorizarContadorCard)).toBeVisible({ timeout: TIMEOUTS.medium });
      |                                                                          ^ Error: expect(locator).toBeVisible() failed
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
  542 |     expect(ingresosText).not.toMatch(/₡\s*0\s*$/); // No debe terminar en ₡0
  543 |   });
  544 | });
  545 | 
  546 | test.describe('BUG-008: Gasto - Concepto se refleja en listado (XSS safe)', () => {
  547 |   test.beforeEach(async ({ page }) => {
  548 |     await login(page);
  549 |     await navigateTo(page, '/ingresos');
  550 |     await page.click(SELECTORS.ingresos.tabs.gastosTab);
  551 |   });
  552 | 
  553 |   test('Debe mostrar concepto ingresado en listado de gastos (no ejecutar HTML/JS)', async ({ page }) => {
  554 |     const xssConcepto = '<script>alert(1)</script>';
  555 | 
  556 |     await page.click(SELECTORS.ingresos.buttons.agregarBtn);
  557 |     await waitForModal(page, 'Agregar Gasto');
  558 | 
  559 |     await page.fill(SELECTORS.ingresos.addModal.fechaInput, VALID_GASTO.fecha);
  560 |     await page.fill(SELECTORS.ingresos.addModal.conceptoInput, xssConcepto);
  561 |     await page.fill(SELECTORS.ingresos.addModal.montoInput, VALID_GASTO.monto);
  562 |     await page.fill(SELECTORS.ingresos.addModal.proveedorInput, VALID_GASTO.proveedor);
  563 | 
  564 |     await page.click(SELECTORS.ingresos.addModal.guardarBtn);
  565 |     await handleSweetAlert(page);
  566 | 
  567 |     // Verificar en tabla que el concepto se muestra como texto plano (no ejecutado)
  568 |     const row = page.locator(SELECTORS.ingresos.table.row).first();
  569 |     await expect(row).toContainText(xssConcepto, { timeout: TIMEOUTS.medium });
  570 | 
  571 |     // Verificar que NO hay script ejecutado (no hay alert)
  572 |     // Playwright habría fallado si se ejecutó alert()
  573 |   });
  574 | });
  575 | 
  576 | test.describe('BUG-009: Configuración - Log de Auditoría accesible', () => {
  577 |   test.beforeEach(async ({ page }) => {
  578 |     await login(page);
  579 |     await navigateTo(page, '/configuracion');
  580 |   });
  581 | 
  582 |   test('Debe mostrar pestaña Auditoria y cargar bitácora sin error 404', async ({ page }) => {
  583 |     // La página Configuracion tiene tabs: Perfil, Notificaciones, Actividad, Auditoria, Seguridad
  584 |     const auditoriaTab = page.locator('button:has-text("Auditoría"), button:has-text("Auditoria"), a[href*="auditoria"]').first();
  585 |     await expect(auditoriaTab).toBeVisible({ timeout: TIMEOUTS.medium });
  586 |     await auditoriaTab.click();
```