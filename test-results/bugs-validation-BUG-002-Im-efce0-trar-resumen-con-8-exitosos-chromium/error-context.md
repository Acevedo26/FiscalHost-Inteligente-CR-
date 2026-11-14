# Instructions

- Following Playwright test failed.
- Explain why, be concise, respect Playwright best practices.
- Provide a snippet of code with the fix, if possible.

# Test info

- Name: bugs-validation.spec.ts >> BUG-002: Importación CSV - HTTP 500 con CSV válido >> Debe importar CSV válido y mostrar resumen con 8 exitosos
- Location: tests\e2e\bugs-validation.spec.ts:251:3

# Error details

```
Error: expect(locator).toHaveAttribute(expected) failed

Locator: locator('.progress-fill')
Expected pattern: /100%/
Timeout: 5000ms
Error: element(s) not found

Call log:
  - Expect "toHaveAttribute" locator('.progress-fill') with timeout 5000ms
  - waiting for locator('.progress-fill')

```

```yaml
- dialog "Error al Importar CSV":
  - heading "Error al Importar CSV" [level=2]
  - text: Request failed with status code 415
  - button "Entendido"
```

# Test source

```ts
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
  199 |     await expect(page.locator(SELECTORS.global.sweetAlertTitle)).toContainText('Registro Creado');
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
> 268 |     await expect(page.locator(SELECTORS.ingresos.importModal.progressFill)).toHaveAttribute('style', /100%/);
      |                                                                             ^ Error: expect(locator).toHaveAttribute(expected) failed
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
  300 |     await expect(page.locator(SELECTORS.ingresos.importModal.downloadErrorsBtn)).toBeVisible();
  301 | 
  302 |     // Verificar que el botón de descarga funciona (click no navega, abre en nueva pestaña)
  303 |     const downloadPromise = page.waitForEvent('download');
  304 |     await page.click(SELECTORS.ingresos.importModal.downloadErrorsBtn);
  305 |     const download = await downloadPromise;
  306 |     expect(download.suggestedFilename()).toContain('errores_importacion');
  307 | 
  308 |     await page.click(SELECTORS.ingresos.importModal.cerrarBtn);
  309 |   });
  310 | 
  311 |   test('Debe rechazar archivo vacío con mensaje claro', async ({ page }) => {
  312 |     await page.click(SELECTORS.ingresos.buttons.importarBtn);
  313 |     await waitForModal(page, 'Importar Reservas CSV');
  314 | 
  315 |     await page.locator(SELECTORS.ingresos.importModal.platformButtons).filter({ hasText: 'Airbnb' }).click();
  316 | 
  317 |     const fileInput = page.locator(SELECTORS.ingresos.importModal.fileInput);
  318 |     await fileInput.setInputFiles({
  319 |       name: 'vacio.csv',
  320 |       mimeType: 'text/csv',
  321 |       buffer: Buffer.from(TEST_CSV_EMPTY),
  322 |     });
  323 | 
  324 |     // Debe mostrar error "Archivo Vacío"
  325 |     await expect(page.locator(SELECTORS.ingresos.importModal.errorEmpty)).toBeVisible({ timeout: TIMEOUTS.medium });
  326 |     await expect(page.locator(SELECTORS.ingresos.importModal.errorEmpty)).toContainText('vacío');
  327 | 
  328 |     await page.click(SELECTORS.ingresos.importModal.cancelarBtn);
  329 |   });
  330 | });
  331 | 
  332 | test.describe('BUG-003 y BUG-004: Estancia corta / 30 noches - Campo noches', () => {
  333 |   test.beforeEach(async ({ page }) => {
  334 |     await login(page);
  335 |     await navigateTo(page, '/ingresos');
  336 |   });
  337 | 
  338 |   test('Debe calcular días de estancia automáticamente desde Fecha Entrada y Fecha Salida', async ({ page }) => {
  339 |     // Este test valida que el frontend usa fecha_inicio y fecha_fin
  340 |     // y el backend calcula DiasEstancia = (FechaSalida - FechaEntrada).Days
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
```