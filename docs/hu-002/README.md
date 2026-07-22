# HU-002 — Configuración de Actividad Económica y Vinculación al TRIBU-CR

**Versión:** 1.0.0  
**Fecha:** 2026-06-07  
**Épica:** Identidad y Perfil Tributario  
**Prioridad:** 🔴 Alta  
**Estado:** ✅ Completado  

---

## Descripción

Como anfitrión, quiero configurar mi actividad económica y vincularme a TRIBU-CR, para mantener actualizada mi información tributaria ante la Dirección General de Tributación.

---

## Contenido

| Archivo | Descripción |
|---------|-------------|
| [endpoints.md](./endpoints.md) | Documentación de la API REST |
| [reglas-negocio.md](./reglas-negocio.md) | Reglas de validación y lógica |
| [catalogo-actividades.sql](./catalogo-actividades.sql) | Script de carga del catálogo DGT |
| [CHANGELOG.md](./CHANGELOG.md) | Historial de versiones |

---

## Estructura de archivos del código

```
FiscalHost.Api.CR/
├── Controllers/
│   └── ConfiguracionTributariaController.cs
├── Services/
│   └── ConfiguracionTributariaService.cs
├── Repositories/
│   ├── ActividadEconomicaRepository.cs
│   └── ConfiguracionTributariaRepository.cs
├── Models/
│   ├── Entities/
│   │   ├── ActividadEconomica.cs
│   │   ├── ConfiguracionTributaria.cs
│   │   └── AuditoriaConfiguracion.cs
│   ├── DTOs/
│   │   └── ConfiguracionTributariaDto.cs
│   └── Emums/
│       └── EstadoConfiguracion.cs
FiscalHost.Tests/
├── ConfiguracionTributariaServiceTests.cs
└── Integration/
    └── ConfiguracionTributariaIntegrationTests.cs
```

---

## Definition of Done

- [x] Desarrollo completado
- [x] Pruebas unitarias aprobadas (7/7)
- [x] Pruebas de integración aprobadas (5/5)
- [x] Auditoría registrada correctamente
- [x] Criterios de aceptación cumplidos
- [x] Documentación actualizada
- [ ] Catálogo de actividades cargado en BD
