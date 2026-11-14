# Instructions

- Following Playwright test failed.
- Explain why, be concise, respect Playwright best practices.
- Provide a snippet of code with the fix, if possible.

# Test info

- Name: bugs-validation.spec.ts >> BUG-023: Cifrado en reposo - Comprobantes >> Subida de comprobante debe completarse (cifrado transparente en backend)
- Location: tests\e2e\bugs-validation.spec.ts:755:3

# Error details

```
Error: expect(locator).toContainText(expected) failed

Locator: locator('.swal2-title')
Expected substring: "Guardado"
Received string:    "Solicitud Inválida"
Timeout: 5000ms

Call log:
  - Expect "toContainText" locator('.swal2-title') with timeout 5000ms
  - waiting for locator('.swal2-title')
    14 × locator resolved to <h2 id="swal2-title" class="swal2-title serene-swal-title">Solicitud Inválida</h2>
       - unexpected value "Solicitud Inválida"

```

```yaml
- heading "Solicitud Inválida" [level=2]
```

# Test source

```ts
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
> 773 |     await expect(page.locator(SELECTORS.global.sweetAlertTitle)).toContainText('Guardado');
      |                                                                  ^ Error: expect(locator).toContainText(expected) failed
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
  799 |       test.skip('Ruta /reconstruccion-historica no encontrada - verificar integración en otra página');
  800 |     }
  801 |   });
  802 | });
  803 | 
  804 | // ============================================================================
  805 | // GLOBAL SETUP / TEARDOWN
  806 | // ============================================================================
  807 | 
  808 | test.beforeAll(async () => {
  809 |   console.log('🚀 Starting E2E Bug Validation Suite');
  810 |   console.log('📋 Bugs covered: 001-009, 017-023');
  811 |   console.log('🌐 Frontend:', process.env.FRONTEND_URL || 'http://localhost:5173');
  812 |   console.log('🔧 Backend:', process.env.API_URL || 'http://localhost:5062');
  813 | });
  814 | 
  815 | test.afterAll(async () => {
  816 |   console.log('✅ E2E Bug Validation Suite completed');
  817 | });
```