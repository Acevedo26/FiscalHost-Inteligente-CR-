# Instructions

- Following Playwright test failed.
- Explain why, be concise, respect Playwright best practices.
- Provide a snippet of code with the fix, if possible.

# Test info

- Name: bugs-validation.spec.ts >> BUG-002: Importación CSV - HTTP 500 con CSV válido >> Debe rechazar archivo vacío con mensaje claro
- Location: tests\e2e\bugs-validation.spec.ts:311:3

# Error details

```
Error: expect(locator).toBeVisible() failed

Locator: locator('.import-error-modal:has-text("vacío")')
Expected: visible
Timeout: 15000ms
Error: element(s) not found

Call log:
  - Expect "toBeVisible" locator('.import-error-modal:has-text("vacío")') with timeout 15000ms
  - waiting for locator('.import-error-modal:has-text("vacío")')

```

```yaml
- dialog "Error al Importar CSV":
  - heading "Error al Importar CSV" [level=2]
  - text: Request failed with status code 415
  - button "Entendido"
```

# Test source

```ts
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
> 325 |     await expect(page.locator(SELECTORS.ingresos.importModal.errorEmpty)).toBeVisible({ timeout: TIMEOUTS.medium });
      |                                                                           ^ Error: expect(locator).toBeVisible() failed
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
```