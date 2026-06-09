# HU-003 — Gestión y Custodia de Llaves Criptográficas

**Versión:** 1.0.0  
**Fecha:** 2026-06-07  
**Épica:** Identidad y Perfil Tributario  
**Prioridad:** 🔴 Alta  
**Estado:** ✅ Completado  

---

## Descripción

Como anfitrión, quiero gestionar y custodiar mis llaves criptográficas (.p12), para firmar comprobantes electrónicos de forma segura.

---

## Contenido

| Archivo | Descripción |
|---------|-------------|
| [endpoints.md](./endpoints.md) | Documentación de la API REST |
| [reglas-negocio.md](./reglas-negocio.md) | Reglas de validación y lógica |
| [CHANGELOG.md](./CHANGELOG.md) | Historial de versiones |

---

## Estructura de archivos del código

```
FiscalHost.Api.CR/
├── Controllers/
│   └── LlaveCriptograficaController.cs
├── Services/
│   ├── LlaveCriptograficaService.cs
│   └── NotificacionService.cs
├── Repositories/
│   └── LlaveCriptograficaRepository.cs
├── Models/
│   ├── Entities/
│   │   ├── LlaveCriptografica.cs
│   │   └── AuditoriaLlave.cs
│   └── DTOs/
│       └── LlaveCriptograficaDto.cs
FiscalHost.Tests/
├── Unit/
│   └── LlaveCriptograficaServiceTests.cs
└── Integration/
    └── LlaveCriptograficaIntegrationTests.cs
```

---

## Deuda Técnica

| ID | Descripción | Etiqueta |
|----|-------------|----------|
| DT-003-01 | `NotificacionService` solo loguea. Integrar con AWS SES o SendGrid para envío real de email. | `feature` |

---

## Definition of Done

- [x] Desarrollo completado
- [x] Almacenamiento seguro con AES-256 validado
- [x] Contraseña verificada con HMAC-SHA256 determinístico
- [x] Pruebas unitarias aprobadas (6/6)
- [x] Pruebas de integración aprobadas (3/3)
- [x] Auditoría implementada
- [x] Criterios de aceptación cumplidos
- [x] Documentación actualizada
- [ ] Notificación de email real implementada (DT-003-01)
