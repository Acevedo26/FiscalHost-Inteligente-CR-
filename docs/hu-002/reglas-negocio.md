# Reglas de Negocio — HU-002

---

## Actividad Económica

- El código debe existir en la tabla `ActividadesEconomicas` con `Activa = true`.
- Si el código no existe, se retorna `422` y no se persiste ningún dato.
- Un anfitrión solo puede tener una configuración tributaria activa a la vez.

---

## TRIBU-CR

- Se genera automáticamente, no lo ingresa el usuario.
- Formato: `TRIBU-{anfitrionId[primeros 6 chars en mayúscula]}-{codigoActividad}`
- Ejemplo: anfitrionId=`anf-001`, código=`551001` → `TRIBU-ANF-001-551001`

---

## NISE

- Debe contener exactamente **10 dígitos numéricos**.
- Patrón: `^\d{10}$`
- Si no cumple el patrón, se retorna `422` y no se persiste ningún dato.

---

## Cambio de Actividad Económica

- Si el anfitrión ya tiene configuración y cambia el código de actividad:
  1. Se actualiza `ActividadEconomicaId` y se recalcula `TribuCr`.
  2. Se registra auditoría con campo `CAMBIO_ACTIVIDAD`.
  3. Se retorna una advertencia en el response.
- Si no hay cambio de actividad, no se genera auditoría.

---

## Auditoría

| Campo `Campo` | Cuándo se registra |
|---------------|-------------------|
| `CREACION` | Primera vez que se guarda la configuración |
| `CAMBIO_ACTIVIDAD` | Cuando se cambia el código de actividad económica |

Campos registrados: `ValorAnterior`, `ValorNuevo`, `Descripcion`, `FechaEvento`.
