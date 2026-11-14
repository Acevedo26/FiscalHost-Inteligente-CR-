# Playwright E2E Tests - FiscalHost Bug Validation

Suite de pruebas End-to-End para validar la corrección de bugs 001-023 mediante flujos reales de usuario en el frontend.

## 📋 Bugs Cubiertos

| Bug | Descripción | Test File | Estado Backend |
|-----|-------------|-----------|----------------|
| **BUG-001** | Reserva directa HTTP 500 | `bugs-validation.spec.ts` | ✅ Resuelto |
| **BUG-002** | Import CSV HTTP 500 | `bugs-validation.spec.ts` | ✅ Resuelto |
| **BUG-003** | Estancia corta sin campo noches | `bugs-validation.spec.ts` | ✅ Implementado |
| **BUG-004** | 30 noches sin campo duración | `bugs-validation.spec.ts` | ✅ Implementado |
| **BUG-005** | Comprobante "field required" | `bugs-validation.spec.ts` | ✅ Resuelto |
| **BUG-006** | Invitar contador ausente | `bugs-validation.spec.ts` | ⚠️ No reproducido |
| **BUG-007** | Dashboard métricas en cero | `bugs-validation.spec.ts` | ✅ Resuelto |
| **BUG-008** | Concepto gasto no reflejado (XSS) | `bugs-validation.spec.ts` | ✅ Resuelto |
| **BUG-009** | Log auditoría 404 | `bugs-validation.spec.ts` | ✅ Resuelto |
| **BUG-017** | CSV mixto rechazado como vacío | `bugs-validation.spec.ts` | ✅ Resuelto |
| **BUG-018** | OCR ilegible sin alternativa manual | `bugs-validation.spec.ts` | ✅ Resuelto |
| **BUG-019** | HU-010 Reconstrucción histórica | `bugs-validation.spec.ts` | ✅ Implementado |
| **BUG-020** | Avisos 7d/1d expiración | `bugs-validation.spec.ts` | ✅ Resuelto |
| **BUG-021** | Simulador sin límite superior | `bugs-validation.spec.ts` | ✅ Resuelto |
| **BUG-022** | HTTPS redirect + HSTS | `bugs-validation.spec.ts` | ✅ Resuelto |
| **BUG-023** | Cifrado en reposo | `bugs-validation.spec.ts` | ✅ Resuelto |

---

## 🚀 Instalación y Ejecución

### Prerrequisitos

1. **Node.js 20+** instalado
2. **Frontend corriendo** en `http://localhost:5173` (`npm run dev` en `Frontend-Fiscalhost-Inteligente-CR`)
3. **Backend corriendo** en `http://localhost:5062` / `https://localhost:7079` (`dotnet run` en `FiscalHost.Api.CR`)
4. **Base de datos** accesible (Neon/local) con usuario de prueba `anfitrion.qa1@test.com` / `Test123!`

### Instalar dependencias

```bash
cd FiscalHost-Inteligente-CR-clean
npm install
npx playwright install
# Opcional: dependencias del sistema (solo Linux)
npx playwright install-deps
```

### Ejecutar tests

```bash
# Todos los tests (headless)
npm test

# Con UI visible (headed)
npm run test:headed

# Modo debug (pausa en cada paso)
npm run test:debug

# UI interactiva de Playwright
npm run test:ui

# Ver reporte HTML después de ejecutar
npm run test:report
```

### Variables de entorno opcionales

```bash
# URLs personalizadas
FRONTEND_URL=http://localhost:5173
API_URL=http://localhost:5062

# Ejemplo:
FRONTEND_URL=http://localhost:5173 API_URL=http://localhost:5062 npm test
```

---

## 📁 Estructura

```
tests/e2e/
├── bugs-validation.spec.ts    # Tests principales (16 suites)
├── fixtures/
│   └── test-data.ts           # Datos de prueba, selectores, helpers
├── README.md                  # Este archivo
└── playwright.config.ts       # Configuración Playwright (en raíz)
```

---

## 🎯 Selectores y Mantenimiento

Los selectores están centralizados en `fixtures/test-data.ts` bajo `SELECTORS`. Si el frontend cambia:

1. Actualiza el selector en `test-data.ts`
2. Todos los tests usan la referencia centralizada
3. No necesitas tocar los tests individuales

### Patrones de selectores usados

- **Por texto visible**: `button:has-text("Agregar")` - robusto a cambios de clase/CSS
- **Por atributo semántico**: `input[type="email"]`, `input[placeholder*="correo" i]`
- **Por role/aria**: `button[aria-label*="Editar"]`
- **Evitar**: selectores por clase CSS (`btn-primary`) o IDs generados

---

## 🔧 Troubleshooting

### Frontend no levanta
```bash
cd ../Frontend-Fiscalhost-Inteligente-CR
npm install
npm run dev
# Verificar en http://localhost:5173
```

### Backend no responde
```bash
cd FiscalHost-Inteligente-CR-clean
dotnet run --project FiscalHost.Api.CR/FiscalHost.Api.CR.csproj
# Verificar en http://localhost:5062/swagger
```

### Usuario de prueba no existe
Ejecutar seed de base de datos o registrar usuario `anfitrion.qa1@test.com` / `Test123!` manualmente.

### Tests fallan por timeouts
- Aumentar `TIMEOUTS` en `test-data.ts`
- Verificar que backend no está bloqueado en migraciones
- Revisar logs de backend para errores 500

### SweetAlert2 no detectado
Los tests esperan `.swal2-popup`. Si el frontend usa otra librería de modales, actualizar `SELECTORS.global.sweetAlert` en `test-data.ts`.

---

## 📊 Reportes

Después de ejecutar:
- **HTML**: `playwright-report/index.html` (abre con `npm run test:report`)
- **JSON**: `test-results/e2e-results.json`
- **Screenshots/Videos**: `test-results/` (solo en fallos)

---

## 🔄 CI/CD

Para GitHub Actions / Azure Pipelines:

```yaml
- name: Run E2E Tests
  run: |
    cd FiscalHost-Inteligente-CR-clean
    npm ci
    npx playwright install --with-deps
    # Levantar servicios (docker-compose, etc.)
    FRONTEND_URL=http://localhost:5173 API_URL=http://localhost:5062 npm test
  env:
    FRONTEND_URL: http://localhost:5173
    API_URL: http://localhost:5062
```

---

## 📝 Notas de Implementación

### Credenciales de prueba
```typescript
const TEST_USER = {
  email: 'anfitrion.qa1@test.com',
  password: 'Test123!',
};
```

### Datos CSV de prueba
- `TEST_CSV_VALID`: 8 filas válidas
- `TEST_CSV_MIXED`: 8 válidas + 2 fechas inválidas (31/02, 2026-13-01)
- `TEST_CSV_EMPTY`: Solo headers

### Selectores clave por página

| Página | Selectores principales |
|--------|------------------------|
| Login | `input[type="email"]`, `input[type="password"]`, `button[type="submit"]` |
| Ingresos | `.page-actions button:has-text("Agregar")`, `.page-actions button:has-text("Importar CSV")` |
| Modal Agregar | `input[type="date"]`, `input[placeholder*="Reserva"]`, `input[type="number"]`, `select[name="fuente"]`, `select[name="clasificacion"]` |
| Modal Importar | `.platform-btn`, `.drop-zone input[type="file"]`, `.progress-fill`, `.result-success`, `.banner-warning-import` |
| Comprobantes | `button:has-text("Subir Comprobante")`, `.drop-zone-comp`, `input[type="checkbox"]` (sin OCR) |
| Reportes | `.autorizar-contador-card`, `button:has-text("Invitar Contador")` |
| Dashboard | `.dashboard-card:has-text("Ingresos Brutos")`, etc. |

---

## 🐛 Reportar Issues

Si un test falla incorrectamente (falso positivo):
1. Verificar que el backend/frontend estén en la versión correcta (commit `12edf21` o posterior)
2. Revisar selectores en `test-data.ts`
3. Ejecutar en modo headed: `npm run test:headed`
4. Crear issue con: bug ID, pasos, selector fallido, screenshot

---

**Última actualización**: 14/11/2026 - Commit `12edf21` - Informe consolidado bugs 001-023