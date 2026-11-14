# Instructions

- Following Playwright test failed.
- Explain why, be concise, respect Playwright best practices.
- Provide a snippet of code with the fix, if possible.

# Test info

- Name: bugs-validation.spec.ts >> BUG-007: Dashboard - Métricas no en cero >> Dashboard debe mostrar métricas calculadas (no ₡0)
- Location: tests\e2e\bugs-validation.spec.ts:529:3

# Error details

```
Error: expect(locator).toBeVisible() failed

Locator: locator('.dashboard-card:has-text("Ingresos Brutos")')
Expected: visible
Timeout: 15000ms
Error: element(s) not found

Call log:
  - Expect "toBeVisible" locator('.dashboard-card:has-text("Ingresos Brutos")') with timeout 15000ms
  - waiting for locator('.dashboard-card:has-text("Ingresos Brutos")')

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
  - text: Dashboard
  - img
  - textbox "Buscar..."
  - button "2":
    - img
    - text: "2"
  - button "ET"
- main:
  - heading "Hola, E2E Test User 👋" [level=1]
  - paragraph: Este es tu centro de tranquilidad y cumplimiento fiscal
  - button "Importar Datos"
  - 'heading "Riesgo Alto: Riesgo fiscal" [level=3]'
  - paragraph: Has superado el límite de exención anual. Debes preparar y presentar tu declaración tributaria.
  - text: 0%
  - button "Generar Borrador"
  - text: Ingresos Brutos ₡0 Acumulado 2026 IVA Retenido ₡0 Vence en 3 días Renta Estimada ₡0 Proyección anual Neto Disponible ₡0 Después de impuestos
  - heading "Ingresos Recientes" [level=3]
  - button "Ver todos"
  - text: "15/04/2026 Reserva Airbnb #2847 +₡45,000 13% 14/04/2026 Reserva Booking #B3921 +₡32,000 13% 12/04/2026 Limpieza Villa Sol -₡15,000 10/04/2026 Reserva Directa +₡28,000 Exento 08/04/2026 Mantenimiento AC -₡35,000"
  - heading "Alertas Activas" [level=3]
  - text: 2 D-104 vence pronto Genera tu borrador antes del 15 de mayo Clasificación pendiente 2 ingresos requieren revisión manual
  - heading "Evolución Mensual" [level=3]
  - combobox:
    - option "Último año" [selected]
    - option "Últimos 6 meses"
  - paragraph: No hay datos disponibles para este período.
  - paragraph: Importe o registre información para ver la evolución fiscal.
  - button "Importar datos"
  - button "Registrar manualmente"
- button "Abrir asistente FisCal":
  - img
  - text: ¿Necesitas ayuda?
- button "Abrir asistente FisCal":
  - img
```

# Test source

```ts
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
> 533 |     await expect(page.locator(SELECTORS.dashboard.cards.ingresosBrutos)).toBeVisible({ timeout: TIMEOUTS.medium });
      |                                                                          ^ Error: expect(locator).toBeVisible() failed
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
  587 | 
  588 |     // Verificar que NO muestra modal "Recurso No Encontrado"
  589 |     await expect(page.locator('.swal2-popup:has-text("Recurso No Encontrado")')).toBeHidden({ timeout: TIMEOUTS.medium });
  590 |     await expect(page.locator('.swal2-popup:has-text("No se encontró configuración")')).toBeHidden({ timeout: TIMEOUTS.medium });
  591 | 
  592 |     // Verificar que la tabla de auditoría carga (puede estar vacía pero sin error)
  593 |     await expect(page.locator('table, .auditoria-table, .data-table').first()).toBeVisible({ timeout: TIMEOUTS.medium });
  594 |   });
  595 | });
  596 | 
  597 | test.describe('BUG-017: Import CSV mixto - No rechazar archivo no vacío', () => {
  598 |   test.beforeEach(async ({ page }) => {
  599 |     await login(page);
  600 |     await navigateTo(page, '/ingresos');
  601 |   });
  602 | 
  603 |   test('CSV con 8 válidas + 2 inválidas debe importar 8 y reportar 2 errores', async ({ page }) => {
  604 |     // Ya cubierto en BUG-002 pero repetimos como test específico
  605 |     await page.click(SELECTORS.ingresos.buttons.importarBtn);
  606 |     await waitForModal(page, 'Importar Reservas CSV');
  607 | 
  608 |     await page.locator(SELECTORS.ingresos.importModal.platformButtons).filter({ hasText: 'Airbnb' }).click();
  609 | 
  610 |     const fileInput = page.locator(SELECTORS.ingresos.importModal.fileInput);
  611 |     await fileInput.setInputFiles({
  612 |       name: 'fechas_mixtas.csv',
  613 |       mimeType: 'text/csv',
  614 |       buffer: Buffer.from(TEST_CSV_MIXED),
  615 |     });
  616 | 
  617 |     await expect(page.locator(SELECTORS.ingresos.importModal.progressFill)).toHaveAttribute('style', /100%/, { timeout: TIMEOUTS.long });
  618 | 
  619 |     // Resultado COMPLETADO_PARCIAL
  620 |     await expect(page.locator(SELECTORS.ingresos.importModal.resultOk)).toContainText('8 reservas');
  621 |     await expect(page.locator(SELECTORS.ingresos.importModal.resultOmitidos)).toContainText('2 filas');
  622 |     await expect(page.locator(SELECTORS.ingresos.importModal.downloadErrorsBtn)).toBeVisible();
  623 | 
  624 |     // Verificar descarga reporte errores
  625 |     const downloadPromise = page.waitForEvent('download');
  626 |     await page.click(SELECTORS.ingresos.importModal.downloadErrorsBtn);
  627 |     const download = await downloadPromise;
  628 |     expect(download.suggestedFilename()).toMatch(/errores_importacion/);
  629 | 
  630 |     await page.click(SELECTORS.ingresos.importModal.cerrarBtn);
  631 |   });
  632 | });
  633 | 
```