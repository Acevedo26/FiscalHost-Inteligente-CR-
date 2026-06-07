# Endpoints — HU-002

**Base URL:** `/api/configuracion-tributaria`

---

## GET `/actividades`

Retorna el catálogo de actividades económicas activas según la DGT.

**Response 200**
```json
[
  {
    "id": 1,
    "codigo": "551001",
    "descripcion": "Hoteles y alojamiento turístico"
  }
]
```

---

## GET `/{anfitrionId}`

Retorna la configuración tributaria de un anfitrión.

| Parámetro | Tipo | Requerido |
|-----------|------|-----------|
| `anfitrionId` | string (path) | ✅ |

**Response 200**
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

**Response 404**
```json
"No se encontró configuración para el anfitrión 'anf-001'."
```

---

## POST `/`

Crea o actualiza la configuración tributaria de un anfitrión.

**Body**
```json
{
  "anfitrionId": "anf-001",
  "codigoActividad": "551001",
  "direccionFiscal": "San José, Costa Rica",
  "nise": "1234567890"
}
```

| Campo | Tipo | Requerido | Validación |
|-------|------|-----------|------------|
| `anfitrionId` | string | ✅ | No vacío |
| `codigoActividad` | string | ✅ | Debe existir en catálogo DGT |
| `direccionFiscal` | string | ✅ | No vacío |
| `nise` | string | ✅ | Exactamente 10 dígitos numéricos |

**Response 200 — Creación exitosa**
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

**Response 200 — Cambio de actividad económica**
```json
{
  "advertencia": "⚠️ El cambio de actividad económica puede afectar sus obligaciones fiscales vigentes. Consulte con un contador autorizado."
}
```

**Response 422 — Código de actividad inválido**
```json
{
  "mensaje": "El código de actividad económica '999999' no existe en el catálogo DGT."
}
```

**Response 422 — NISE inválido**
```json
{
  "mensaje": "El NISE debe contener exactamente 10 dígitos numéricos."
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
