# HU-002 — Configuración de Actividad Económica y Vinculación al TRIBU-CR

## Épica
Identidad y Perfil Tributario

## Descripción
Como anfitrión, quiero configurar mi actividad económica y vincularme a TRIBU-CR, para mantener actualizada mi información tributaria ante la Dirección General de Tributación.

---

## Endpoints

| Método | Ruta | Descripción |
|--------|------|-------------|
| `GET` | `/api/configuracion-tributaria/actividades` | Obtiene el catálogo de actividades económicas activas |
| `GET` | `/api/configuracion-tributaria/{anfitrionId}` | Obtiene la configuración tributaria de un anfitrión |
| `POST` | `/api/configuracion-tributaria` | Crea o actualiza la configuración tributaria |

### POST — Body de ejemplo
```json
{
  "anfitrionId": "anf-001",
  "codigoActividad": "551001",
  "direccionFiscal": "San José, Costa Rica",
  "nise": "1234567890"
}
```

### POST — Response exitoso
```json
{
  "id": 1,
  "anfitrionId": "anf-001",
  "codigoActividad": "551001",
  "descripcionActividad": "Hoteles y alojamiento turístico",
  "tribuCr": "TRIBU-ANF-001-551001",
  "direccionFiscal": "San José, Costa Rica",
  "nise": "1234567890",
  "estado": "Activa",
  "fechaActualizacion": "2026-06-07T00:00:00Z",
  "advertencia": null
}
```

### POST — Response con advertencia (cambio de actividad)
```json
{
  "advertencia": "⚠️ El cambio de actividad económica puede afectar sus obligaciones fiscales vigentes. Consulte con un contador autorizado."
}
```

### POST — Response error código inválido (`422`)
```json
{
  "mensaje": "El código de actividad económica '999999' no existe en el catálogo DGT."
}
```

### POST — Response error NISE inválido (`422`)
```json
{
  "mensaje": "El NISE debe contener exactamente 10 dígitos numéricos."
}
```

---

## Reglas de negocio

- El código de actividad económica debe existir en el catálogo DGT (`ActividadesEconomicas`).
- El NISE debe contener exactamente 10 dígitos numéricos.
- El TRIBU-CR se genera automáticamente con el formato `TRIBU-{anfitrionId[..6]}-{codigoActividad}`.
- Un anfitrión solo puede tener una configuración tributaria activa.
- Cualquier cambio de actividad económica genera un registro de auditoría y muestra una advertencia.

---

## Estructura de archivos

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
├── ConfiguracionTributariaServiceTests.cs   ← Pruebas unitarias
└── Integration/
    └── ConfiguracionTributariaIntegrationTests.cs  ← Pruebas de integración
```

---

## Pruebas

### Unitarias (`ConfiguracionTributariaServiceTests`)
| Prueba | Escenario |
|--------|-----------|
| `GuardarConfiguracion_CodigoInvalido_RetornaError` | Código de actividad inexistente |
| `GuardarConfiguracion_NiseInvalido_RetornaError` | NISE con formato incorrecto |
| `GuardarConfiguracion_NuevaConfiguracion_VinculaTribuCrYGuarda` | Vinculación exitosa nueva |
| `GuardarConfiguracion_CambioActividad_MuestraAdvertencia` | Cambio de actividad muestra advertencia |
| `GuardarConfiguracion_CambioActividad_RegistraAuditoria` | Cambio registra auditoría |
| `GuardarConfiguracion_SinCambioActividad_SinAdvertencia` | Sin cambio no hay advertencia |
| `GetActividades_RetornaListaActiva` | Catálogo de actividades |

### Integración (`ConfiguracionTributariaIntegrationTests`)
| Prueba | Escenario |
|--------|-----------|
| `GuardarYObtener_ConfiguracionNueva_PersisteTodosLosCampos` | Flujo completo contra BD en memoria |
| `CambioActividad_RegistraAuditoriaEnBd` | Auditoría persiste en BD |
| `CodigoInvalido_NoPersisteDatos` | Rollback implícito por validación |
| `NiseInvalido_NoPersisteDatos` | Rollback implícito por validación |
| `ActualizacionSinCambioActividad_NoGeneraAuditoria` | Sin cambio de actividad, sin auditoría |

---

## Definition of Done
- [x] Desarrollo completado
- [x] Pruebas unitarias aprobadas
- [x] Pruebas de integración aprobadas
- [x] Auditoría registrada correctamente
- [x] Criterios de aceptación cumplidos
- [x] Documentación actualizada
