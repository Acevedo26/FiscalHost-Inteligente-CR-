# Endpoints — HU-003

**Base URL:** `/api/llaves-criptograficas`

---

## POST `/cargar`

Carga y almacena una llave criptográfica `.p12` de forma cifrada.

**Content-Type:** `multipart/form-data`

| Campo | Tipo | Requerido | Descripción |
|-------|------|-----------|-------------|
| `anfitrionId` | string | ✅ | Identificador del anfitrión |
| `archivo` | file (.p12) | ✅ | Archivo de certificado digital |
| `contrasena` | string | ✅ | Contraseña asociada al certificado |

**Response 200 — Almacenamiento exitoso**
```json
{
  "id": 1,
  "anfitrionId": "anf-001",
  "nombreArchivo": "llave.p12",
  "fechaActualizacion": "2026-06-07T00:00:00Z",
  "activa": true
}
```

**Response 422 — Formato de archivo incorrecto**
```json
{
  "mensaje": "Solo se permiten archivos con extensión .p12."
}
```

**Response 422 — Archivo .p12 inválido**
```json
{
  "mensaje": "El archivo no es un certificado .p12 válido."
}
```

**Response 422 — Contraseña incorrecta**
```json
{
  "mensaje": "La contraseña proporcionada es incorrecta."
}
```

**Response 400 — Campos requeridos faltantes**
```json
{
  "errors": {
    "AnfitrionId": ["The AnfitrionId field is required."]
  }
}
```

---

## PUT `/actualizar-contrasena`

Actualiza la contraseña asociada a la llave activa del anfitrión. Registra auditoría y envía notificación.

**Content-Type:** `application/json`

```json
{
  "anfitrionId": "anf-001",
  "contrasenaActual": "<contrasena-actual>",
  "contrasenaNueva": "<contrasena-nueva>"
}
```

| Campo | Tipo | Requerido |
|-------|------|-----------|
| `anfitrionId` | string | ✅ |
| `contrasenaActual` | string | ✅ |
| `contrasenaNueva` | string | ✅ |

**Response 200**
```json
{
  "mensaje": "Contraseña actualizada correctamente."
}
```

**Response 422 — Anfitrión sin llave activa**
```json
{
  "mensaje": "No se encontró una llave activa para el anfitrión."
}
```

**Response 422 — Contraseña actual incorrecta**
```json
{
  "mensaje": "La contraseña actual es incorrecta."
}
```

---

## GET `/{anfitrionId}`

Retorna el estado de la llave criptográfica activa de un anfitrión.

| Parámetro | Tipo | Requerido |
|-----------|------|-----------|
| `anfitrionId` | string (path) | ✅ |

**Response 200**
```json
{
  "id": 1,
  "anfitrionId": "anf-001",
  "nombreArchivo": "llave.p12",
  "fechaActualizacion": "2026-06-07T00:00:00Z",
  "activa": true
}
```

**Response 404**
```json
{
  "mensaje": "No se encontró llave activa para 'anf-001'."
}
```
