# Instructions

- Following Playwright test failed.
- Explain why, be concise, respect Playwright best practices.
- Provide a snippet of code with the fix, if possible.

# Test info

- Name: bugs-validation.spec.ts >> BUG-001: Reserva Directa - HTTP 500 al guardar >> Debe guardar reserva directa válida sin error 500
- Location: tests\e2e\bugs-validation.spec.ts:186:3

# Error details

```
Error: expect(locator).toContainText(expected) failed

Locator: locator('.swal2-title')
Expected substring: "Registro Creado"
Received string:    "Error al Registrar"
Timeout: 5000ms

Call log:
  - Expect "toContainText" locator('.swal2-title') with timeout 5000ms
  - waiting for locator('.swal2-title')
    14 × locator resolved to <h2 id="swal2-title" class="swal2-title serene-swal-title">Error al Registrar</h2>
       - unexpected value "Error al Registrar"

```

```yaml
- heading "Error al Registrar" [level=2]
```

# Test source

```ts
  99  |   await page.waitForLoadState('networkidle');
  100 |   
  101 |   // Check current URL
  102 |   const currentUrl = page.url();
  103 |   console.log('Current URL after login:', currentUrl);
  104 |   
  105 |   // Check if there's a SweetAlert error
  106 |   const sweetAlert = page.locator(SELECTORS.global.sweetAlert);
  107 |   if (await sweetAlert.isVisible({ timeout: 5000 })) {
  108 |     const title = await sweetAlert.locator(SELECTORS.global.sweetAlertTitle).innerText();
  109 |     const text = await sweetAlert.locator(SELECTORS.global.sweetAlertText).innerText();
  110 |     console.log('SweetAlert error:', title, text);
  111 |     await page.click(SELECTORS.global.sweetAlertConfirm);
  112 |   }
  113 |   
  114 |   // Wait for dashboard to be visible (more reliable than URL)
  115 |   await expect(page.locator(SELECTORS.dashboard.pageTitle)).toBeVisible({ timeout: TIMEOUTS.medium });
  116 | }
  117 | 
  118 | async function navigateTo(page: Page, path: string) {
  119 |   await page.goto(path);
  120 |   await page.waitForLoadState('networkidle');
  121 | }
  122 | 
  123 | async function waitForModal(page: Page, titleText: string) {
  124 |   await expect(page.locator(SELECTORS.global.modal).filter({ hasText: titleText })).toBeVisible({ timeout: TIMEOUTS.medium });
  125 | }
  126 | 
  127 | // Helper to interact with custom Select component (button-based dropdown)
  128 | async function selectCustomOption(page: Page, label: string, value: string) {
  129 |   // Click the select trigger button (by label text)
  130 |   const trigger = page.locator(`.select-container:has(.select-label:has-text("${label}")) .select-trigger`).first();
  131 |   await trigger.click();
  132 |   // Wait for dropdown to open and click the option
  133 |   const option = page.locator(`.select-dropdown .select-option:has-text("${value}")`).first();
  134 |   await option.click();
  135 | }
  136 | 
  137 | async function closeModal(page: Page) {
  138 |   const closeBtn = page.locator(SELECTORS.global.modalClose).first();
  139 |   if (await closeBtn.isVisible({ timeout: 2000 })) {
  140 |     await closeBtn.click();
  141 |   } else {
  142 |     await page.keyboard.press('Escape');
  143 |   }
  144 |   await expect(page.locator(SELECTORS.global.modal)).toBeHidden({ timeout: TIMEOUTS.short });
  145 | }
  146 | 
  147 | async function handleSweetAlert(page: Page, action: 'confirm' | 'cancel' = 'confirm') {
  148 |   await expect(page.locator(SELECTORS.global.sweetAlert)).toBeVisible({ timeout: TIMEOUTS.medium });
  149 |   if (action === 'confirm') {
  150 |     await page.click(SELECTORS.global.sweetAlertConfirm);
  151 |   } else {
  152 |     await page.click(SELECTORS.global.sweetAlertCancel);
  153 |   }
  154 |   await expect(page.locator(SELECTORS.global.sweetAlert)).toBeHidden({ timeout: TIMEOUTS.short });
  155 | }
  156 | 
  157 | async function fillAddTransactionForm(page: Page, data: typeof VALID_RESERVA | typeof VALID_GASTO, isGasto = false) {
  158 |   await page.fill(SELECTORS.ingresos.addModal.fechaInput, data.fecha);
  159 |   await page.fill(SELECTORS.ingresos.addModal.conceptoInput, data.concepto);
  160 |   await page.fill(SELECTORS.ingresos.addModal.montoInput, data.monto);
  161 | 
  162 |   if (!isGasto) {
  163 |     await selectCustomOption(page, 'Fuente', data.fuente as string);
  164 |     await selectCustomOption(page, 'Clasificación', data.clasificacion as string);
  165 |   } else {
  166 |     await page.fill(SELECTORS.ingresos.addModal.proveedorInput, data.proveedor as string);
  167 |   }
  168 | }
  169 | 
  170 | async function createTestFile(content: string, filename: string): Promise<File> {
  171 |   return new File([content], filename, { type: 'text/csv' });
  172 | }
  173 | 
  174 | // ============================================================================
  175 | // TEST SUITES
  176 | // ============================================================================
  177 | 
  178 | test.describe.configure({ retries: 0 });
  179 | 
  180 | test.describe('BUG-001: Reserva Directa - HTTP 500 al guardar', () => {
  181 |   test.beforeEach(async ({ page }) => {
  182 |     await login(page);
  183 |     await navigateTo(page, '/ingresos');
  184 |   });
  185 | 
  186 |   test('Debe guardar reserva directa válida sin error 500', async ({ page }) => {
  187 |     // Abrir modal Agregar
  188 |     await page.click(SELECTORS.ingresos.buttons.agregarBtn);
  189 |     await waitForModal(page, 'Agregar Ingreso');
  190 | 
  191 |     // Llenar formulario válido
  192 |     await fillAddTransactionForm(page, VALID_RESERVA);
  193 | 
  194 |     // Guardar
  195 |     await page.click(SELECTORS.ingresos.addModal.guardarBtn);
  196 | 
  197 |     // Verificar SweetAlert de éxito
  198 |     await expect(page.locator(SELECTORS.global.sweetAlert)).toBeVisible({ timeout: TIMEOUTS.medium });
> 199 |     await expect(page.locator(SELECTORS.global.sweetAlertTitle)).toContainText('Registro Creado');
      |                                                                  ^ Error: expect(locator).toContainText(expected) failed
  200 |     await expect(page.locator(SELECTORS.global.sweetAlertText)).toContainText('guardado exitosamente');
  201 |     await handleSweetAlert(page);
  202 | 
  203 |     // Verificar que el modal se cerró
  204 |     await expect(page.locator(SELECTORS.ingresos.addModal.modal)).toBeHidden({ timeout: TIMEOUTS.short });
  205 | 
  206 |     // Verificar que aparece en la tabla
  207 |     await expect(page.locator(SELECTORS.ingresos.table.row).first()).toContainText(VALID_RESERVA.concepto);
  208 |   });
  209 | 
  210 |   test('Debe validar monto > 0 y mostrar error amigable', async ({ page }) => {
  211 |     await page.click(SELECTORS.ingresos.buttons.agregarBtn);
  212 |     await waitForModal(page, 'Agregar Ingreso');
  213 | 
  214 |     await page.fill(SELECTORS.ingresos.addModal.fechaInput, VALID_RESERVA.fecha);
  215 |     await page.fill(SELECTORS.ingresos.addModal.conceptoInput, VALID_RESERVA.concepto);
  216 |     await page.fill(SELECTORS.ingresos.addModal.montoInput, '0');
  217 | 
  218 |     await page.click(SELECTORS.ingresos.addModal.guardarBtn);
  219 | 
  220 |     await expect(page.locator(SELECTORS.global.sweetAlert)).toBeVisible({ timeout: TIMEOUTS.medium });
  221 |     await expect(page.locator(SELECTORS.global.sweetAlertTitle)).toContainText('Monto Inválido');
  222 |     await handleSweetAlert(page);
  223 |   });
  224 | 
  225 |   test('Debe validar fecha no futura y mostrar error amigable', async ({ page }) => {
  226 |     await page.click(SELECTORS.ingresos.buttons.agregarBtn);
  227 |     await waitForModal(page, 'Agregar Ingreso');
  228 | 
  229 |     const futureDate = new Date();
  230 |     futureDate.setDate(futureDate.getDate() + 30);
  231 |     const futureStr = futureDate.toISOString().split('T')[0];
  232 | 
  233 |     await page.fill(SELECTORS.ingresos.addModal.fechaInput, futureStr);
  234 |     await page.fill(SELECTORS.ingresos.addModal.conceptoInput, VALID_RESERVA.concepto);
  235 |     await page.fill(SELECTORS.ingresos.addModal.montoInput, VALID_RESERVA.monto);
  236 | 
  237 |     await page.click(SELECTORS.ingresos.addModal.guardarBtn);
  238 | 
  239 |     await expect(page.locator(SELECTORS.global.sweetAlert)).toBeVisible({ timeout: TIMEOUTS.medium });
  240 |     await expect(page.locator(SELECTORS.global.sweetAlertTitle)).toContainText('Fecha Inválida');
  241 |     await handleSweetAlert(page);
  242 |   });
  243 | });
  244 | 
  245 | test.describe('BUG-002: Importación CSV - HTTP 500 con CSV válido', () => {
  246 |   test.beforeEach(async ({ page }) => {
  247 |     await login(page);
  248 |     await navigateTo(page, '/ingresos');
  249 |   });
  250 | 
  251 |   test('Debe importar CSV válido y mostrar resumen con 8 exitosos', async ({ page }) => {
  252 |     await page.click(SELECTORS.ingresos.buttons.importarBtn);
  253 |     await waitForModal(page, 'Importar Reservas CSV');
  254 | 
  255 |     // Paso 1: Seleccionar plataforma
  256 |     await page.locator(SELECTORS.ingresos.importModal.platformButtons).filter({ hasText: 'Airbnb' }).click();
  257 | 
  258 |     // Paso 2: Subir archivo
  259 |     const fileInput = page.locator(SELECTORS.ingresos.importModal.fileInput);
  260 |     await fileInput.setInputFiles({
  261 |       name: 'reservas_validas.csv',
  262 |       mimeType: 'text/csv',
  263 |       buffer: Buffer.from(TEST_CSV_VALID),
  264 |     });
  265 | 
  266 |     // Esperar progreso
  267 |     await expect(page.locator(SELECTORS.ingresos.importModal.progressFill)).toBeVisible({ timeout: TIMEOUTS.medium });
  268 |     await expect(page.locator(SELECTORS.ingresos.importModal.progressFill)).toHaveAttribute('style', /100%/);
  269 | 
  270 |     // Verificar resultado: 8 exitosos, 0 omitidos
  271 |     await expect(page.locator(SELECTORS.ingresos.importModal.resultOk)).toBeVisible({ timeout: TIMEOUTS.medium });
  272 |     await expect(page.locator(SELECTORS.ingresos.importModal.resultOk)).toContainText('8 reservas');
  273 | 
  274 |     // Cerrar modal
  275 |     await page.click(SELECTORS.ingresos.importModal.cerrarBtn);
  276 |   });
  277 | 
  278 |   test('Debe importar CSV mixto (8 válidas + 2 inválidas) y ofrecer descarga de reporte de errores', async ({ page }) => {
  279 |     await page.click(SELECTORS.ingresos.buttons.importarBtn);
  280 |     await waitForModal(page, 'Importar Reservas CSV');
  281 | 
  282 |     await page.locator(SELECTORS.ingresos.importModal.platformButtons).filter({ hasText: 'Airbnb' }).click();
  283 | 
  284 |     const fileInput = page.locator(SELECTORS.ingresos.importModal.fileInput);
  285 |     await fileInput.setInputFiles({
  286 |       name: 'fechas_mixtas.csv',
  287 |       mimeType: 'text/csv',
  288 |       buffer: Buffer.from(TEST_CSV_MIXED),
  289 |     });
  290 | 
  291 |     // Esperar progreso
  292 |     await expect(page.locator(SELECTORS.ingresos.importModal.progressFill)).toHaveAttribute('style', /100%/, { timeout: TIMEOUTS.long });
  293 | 
  294 |     // Verificar resultado parcial: 8 exitosos, 2 omitidos
  295 |     await expect(page.locator(SELECTORS.ingresos.importModal.resultOk)).toBeVisible({ timeout: TIMEOUTS.medium });
  296 |     await expect(page.locator(SELECTORS.ingresos.importModal.resultOk)).toContainText('8 reservas');
  297 | 
  298 |     await expect(page.locator(SELECTORS.ingresos.importModal.resultOmitidos)).toBeVisible();
  299 |     await expect(page.locator(SELECTORS.ingresos.importModal.resultOmitidos)).toContainText('2 filas');
```