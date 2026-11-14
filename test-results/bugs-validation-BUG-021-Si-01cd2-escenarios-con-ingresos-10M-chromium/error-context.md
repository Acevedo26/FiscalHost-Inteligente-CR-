# Instructions

- Following Playwright test failed.
- Explain why, be concise, respect Playwright best practices.
- Provide a snippet of code with the fix, if possible.

# Test info

- Name: bugs-validation.spec.ts >> BUG-021: Simulador - Validación límite superior (10M) >> Debe rechazar escenarios con ingresos > 10M
- Location: tests\e2e\bugs-validation.spec.ts:690:3

# Error details

```
TimeoutError: page.fill: Timeout 15000ms exceeded.
Call log:
  - waiting for locator('input[name="nombre"], input[placeholder*="nombre" i]')

```

# Test source

```ts
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
  655 |     await expect(page.locator(SELECTORS.comprobantes.uploadModal.sinOcrCheckbox)).toBeVisible();
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
> 698 |       await page.fill('input[name="nombre"], input[placeholder*="nombre" i]', 'Test Límite');
      |                  ^ TimeoutError: page.fill: Timeout 15000ms exceeded.
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
  756 |     // Este test valida que el flujo funciona; el cifrado AES-256-CBC
  757 |     // ocurre en LocalStorageService.UploadAsync (backend)
  758 |     await page.click(SELECTORS.comprobantes.buttons.subirBtn);
  759 |     await waitForModal(page, 'Subir Comprobante');
  760 | 
  761 |     const fileInput = page.locator(SELECTORS.comprobantes.uploadModal.fileInput);
  762 |     await fileInput.setInputFiles({
  763 |       name: 'factura_cifrada.pdf',
  764 |       mimeType: 'application/pdf',
  765 |       buffer: Buffer.from('%PDF-1.4 test content for encryption'),
  766 |     });
  767 | 
  768 |     await expect(page.locator(SELECTORS.comprobantes.uploadModal.bannerSuccess)).toBeVisible({ timeout: TIMEOUTS.medium });
  769 | 
  770 |     await page.click(SELECTORS.comprobantes.uploadModal.guardarBtn);
  771 | 
  772 |     await expect(page.locator(SELECTORS.global.sweetAlert)).toBeVisible({ timeout: TIMEOUTS.medium });
  773 |     await expect(page.locator(SELECTORS.global.sweetAlertTitle)).toContainText('Guardado');
  774 |     await handleSweetAlert(page);
  775 | 
  776 |     // Verificar en tabla
  777 |     await expect(page.locator(SELECTORS.comprobantes.table.row).first()).toBeVisible({ timeout: TIMEOUTS.medium });
  778 |   });
  779 | });
  780 | 
  781 | test.describe('BUG-019: HU-010 Reconstrucción Histórica - UI', () => {
  782 |   test.beforeEach(async ({ page }) => {
  783 |     await login(page);
  784 |   });
  785 | 
  786 |   test('Debe existir página/ruta para reconstrucción histórica', async ({ page }) => {
  787 |     // Verificar si hay link en menú o navegación directa
  788 |     await navigateTo(page, '/reconstruccion-historica'); // o la ruta real
  789 | 
  790 |     // Si la página existe, verificar elementos clave
  791 |     const pageExists = await page.locator('h1:has-text("Reconstrucción"), h1:has-text("Histórica")').isVisible({ timeout: 5000 }).catch(() => false);
  792 | 
  793 |     if (pageExists) {
  794 |       await expect(page.locator('input[type="file"][accept=".csv"]')).toBeVisible();
  795 |       await expect(page.locator('button:has-text("Reconstruir"), button:has-text("Procesar")')).toBeVisible();
  796 |       await expect(page.locator('button:has-text("Detectar Períodos")')).toBeVisible();
  797 |     } else {
  798 |       // La funcionalidad puede estar integrada en otra página (ej. Impuestos)
```