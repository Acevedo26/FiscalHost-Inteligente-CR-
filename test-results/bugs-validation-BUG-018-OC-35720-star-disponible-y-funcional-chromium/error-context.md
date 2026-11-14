# Instructions

- Following Playwright test failed.
- Explain why, be concise, respect Playwright best practices.
- Provide a snippet of code with the fix, if possible.

# Test info

- Name: bugs-validation.spec.ts >> BUG-018: OCR ilegible - Permite ingreso manual / guardar sin OCR >> Checkbox "Guardar sin extracción automática" debe estar disponible y funcional
- Location: tests\e2e\bugs-validation.spec.ts:640:3

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
  - text: "Archivo seleccionado: factura_ilegible.pdf. Haz clic en Guardar para procesar con OCR en el servidor."
  - button "Cancelar"
  - button "Confirmar y Guardar"
```

# Test source

```ts
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
  634 | test.describe('BUG-018: OCR ilegible - Permite ingreso manual / guardar sin OCR', () => {
  635 |   test.beforeEach(async ({ page }) => {
  636 |     await login(page);
  637 |     await navigateTo(page, '/comprobantes');
  638 |   });
  639 | 
  640 |   test('Checkbox "Guardar sin extracción automática" debe estar disponible y funcional', async ({ page }) => {
  641 |     await page.click(SELECTORS.comprobantes.buttons.subirBtn);
  642 |     await waitForModal(page, 'Subir Comprobante');
  643 | 
  644 |     // Subir archivo
  645 |     const fileInput = page.locator(SELECTORS.comprobantes.uploadModal.fileInput);
  646 |     await fileInput.setInputFiles({
  647 |       name: 'factura_ilegible.pdf',
  648 |       mimeType: 'application/pdf',
  649 |       buffer: Buffer.from('%PDF-1.4 contenido ilegible'),
  650 |     });
  651 | 
  652 |     await expect(page.locator(SELECTORS.comprobantes.uploadModal.bannerSuccess)).toBeVisible({ timeout: TIMEOUTS.medium });
  653 | 
  654 |     // Verificar checkbox sin OCR
> 655 |     await expect(page.locator(SELECTORS.comprobantes.uploadModal.sinOcrCheckbox)).toBeVisible();
      |                                                                                   ^ Error: expect(locator).toBeVisible() failed
  656 | 
  657 |     // Marcar y verificar banner
  658 |     await page.check(SELECTORS.comprobantes.uploadModal.sinOcrCheckbox);
  659 |     await expect(page.locator(SELECTORS.comprobantes.uploadModal.bannerWarning)).toBeVisible();
  660 |     await expect(page.locator(SELECTORS.comprobantes.uploadModal.bannerWarning)).toContainText('crédito fiscal');
  661 | 
  662 |     // Botón guardar debe estar habilitado
  663 |     await expect(page.locator(SELECTORS.comprobantes.uploadModal.guardarBtn)).toBeEnabled();
  664 | 
  665 |     await closeModal(page);
  666 |   });
  667 | });
  668 | 
  669 | test.describe('BUG-020: Acceso Contador - Avisos 7 y 1 día', () => {
  670 |   test.beforeEach(async ({ page }) => {
  671 |     await login(page);
  672 |     await navigateTo(page, '/reportes');
  673 |   });
  674 | 
  675 |   test('Sección Autorizar Contador debe mostrar accesos con estado EXPIRADO cuando corresponda', async ({ page }) => {
  676 |     await expect(page.locator(SELECTORS.reportes.autorizarContadorCard)).toBeVisible({ timeout: TIMEOUTS.medium });
  677 | 
  678 |     // Verificar que la card muestra contadores (mock data incluye uno EXPIRADO)
  679 |     await expect(page.locator(SELECTORS.reportes.autorizarContadorCard)).toContainText('EXPIRADO');
  680 |     await expect(page.locator(SELECTORS.reportes.autorizarContadorCard)).toContainText('ACTIVO');
  681 |   });
  682 | });
  683 | 
  684 | test.describe('BUG-021: Simulador - Validación límite superior (10M)', () => {
  685 |   test.beforeEach(async ({ page }) => {
  686 |     await login(page);
  687 |     await navigateTo(page, '/simulador');
  688 |   });
  689 | 
  690 |   test('Debe rechazar escenarios con ingresos > 10M', async ({ page }) => {
  691 |     // La página Simulador tiene formulario para agregar escenarios
  692 |     // Buscar botón "Agregar Escenario" o similar
  693 |     const agregarBtn = page.locator('button:has-text("Agregar"), button:has-text("Nuevo Escenario")').first();
  694 |     if (await agregarBtn.isVisible({ timeout: 5000 })) {
  695 |       await agregarBtn.click();
  696 | 
  697 |       // Llenar con monto > 10M
  698 |       await page.fill('input[name="nombre"], input[placeholder*="nombre" i]', 'Test Límite');
  699 |       await page.fill('input[name="ingresos"], input[placeholder*="ingreso" i]', '15000000');
  700 |       await page.fill('input[name="gastos"], input[placeholder*="gasto" i]', '100000');
  701 | 
  702 |       await page.click('button:has-text("Guardar"), button:has-text("Crear")');
  703 | 
  704 |       // Debe mostrar error de límite
  705 |       await expect(page.locator(SELECTORS.global.sweetAlert)).toBeVisible({ timeout: TIMEOUTS.medium });
  706 |       await expect(page.locator(SELECTORS.global.sweetAlertTitle)).toContainText(/Límite|Advertencia|Máximo/);
  707 |       await handleSweetAlert(page);
  708 |     }
  709 |   });
  710 | 
  711 |   test('Debe aceptar escenarios dentro del límite (≤ 10M)', async ({ page }) => {
  712 |     const agregarBtn = page.locator('button:has-text("Agregar"), button:has-text("Nuevo Escenario")').first();
  713 |     if (await agregarBtn.isVisible({ timeout: 5000 })) {
  714 |       await agregarBtn.click();
  715 | 
  716 |       await page.fill('input[name="nombre"], input[placeholder*="nombre" i]', 'Test Válido');
  717 |       await page.fill('input[name="ingresos"], input[placeholder*="ingreso" i]', '5000000');
  718 |       await page.fill('input[name="gastos"], input[placeholder*="gasto" i]', '1000000');
  719 | 
  720 |       await page.click('button:has-text("Guardar"), button:has-text("Crear")');
  721 | 
  722 |       // Debe aceptar (o mostrar éxito)
  723 |       await expect(page.locator(SELECTORS.global.sweetAlert)).toBeVisible({ timeout: TIMEOUTS.medium });
  724 |       await handleSweetAlert(page);
  725 |     }
  726 |   });
  727 | });
  728 | 
  729 | test.describe('BUG-022: HTTPS Redirect + HSTS (Local)', () => {
  730 |   test('Backend deshabilita redirección HTTP->HTTPS en desarrollo (no HSTS)', async ({ page }) => {
  731 |     // En desarrollo, HTTPS redirection está deshabilitado (Program.cs)
  732 |     const httpResponse = await page.request.get('http://localhost:5062/api/health', { timeout: 10000 }).catch(() => null);
  733 | 
  734 |     if (httpResponse) {
  735 |       // En desarrollo NO debe redirigir, debe responder 200 OK
  736 |       expect(httpResponse.status()).toBe(200);
  737 |     }
  738 | 
  739 |     // Verificar que HSTS NO se envía en desarrollo
  740 |     const httpsResponse = await page.request.get('https://localhost:7079/api/health', { timeout: 10000 }).catch(() => null);
  741 |     if (httpsResponse) {
  742 |       const hsts = httpsResponse.headers()['strict-transport-security'];
  743 |       // HSTS no debería estar en desarrollo
  744 |       // (si el puerto HTTPS está disponible, verificar que no hay HSTS)
  745 |     }
  746 |   });
  747 | });
  748 | 
  749 | test.describe('BUG-023: Cifrado en reposo - Comprobantes', () => {
  750 |   test.beforeEach(async ({ page }) => {
  751 |     await login(page);
  752 |     await navigateTo(page, '/comprobantes');
  753 |   });
  754 | 
  755 |   test('Subida de comprobante debe completarse (cifrado transparente en backend)', async ({ page }) => {
```