# CHANGELOG — HU-002

## [1.0.0] — 2026-06-07

### Added
- Entidades: `ActividadEconomica`, `ConfiguracionTributaria`, `AuditoriaConfiguracion`
- DTO: `ConfiguracionTributariaRequest`, `ConfiguracionTributariaResponse`, `ActividadEconomicaResponse`
- Enum: `EstadoConfiguracion` (Activa, Inactiva, PendienteValidacion)
- Repositorios: `ActividadEconomicaRepository`, `ConfiguracionTributariaRepository`
- Servicio: `ConfiguracionTributariaService` con validación de código DGT, NISE y generación de TRIBU-CR
- Controller: `ConfiguracionTributariaController` con endpoints GET y POST
- Migración inicial para SQL Server: `InitialCreate_SqlServer`
- Script de carga del catálogo DGT: `catalogo-actividades.sql`
- Documentación de endpoints, reglas de negocio y estructura

### Changed
- Migrado de SQLite a SQL Server
- Eliminado seed de `OnModelCreating`, reemplazado por script SQL independiente
