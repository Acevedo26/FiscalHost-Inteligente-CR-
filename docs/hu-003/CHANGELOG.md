# CHANGELOG — HU-003

## [1.0.0] — 2026-06-07

### Added
- Entidades: `LlaveCriptografica`, `AuditoriaLlave`
- DTOs: `CargarLlaveRequest`, `ActualizarContrasenaRequest`, `LlaveCriptograficaResponse`
- Repositorio: `LlaveCriptograficaRepository`
- Servicio: `LlaveCriptograficaService` con validación X.509, cifrado AES-256 del archivo y HMAC-SHA256 de contraseña
- Servicio: `NotificacionService` (implementación con log; integración de email pendiente — DT-003-01)
- Controller: `LlaveCriptograficaController` con endpoints POST `/cargar`, PUT `/actualizar-contrasena`, GET `/{anfitrionId}`
- Auditoría automática para acciones `CARGA` y `ACTUALIZAR_CONTRASENA`
- DbSets `LlavesCriptograficas` y `AuditoriasLlave` en `AppDbContext`
- Configuración `Cifrado:Clave` en `appsettings.json`
- Pruebas unitarias: 6 casos en `LlaveCriptograficaServiceTests`
- Pruebas de integración: 3 casos en `LlaveCriptograficaIntegrationTests`
- Documentación: `endpoints.md`, `reglas-negocio.md`, `CHANGELOG.md`

### Fixed
- Comparación de contraseña reemplazada de AES (no determinístico) a HMAC-SHA256 determinístico
