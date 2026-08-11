# Endpoints - HU-016

## Invitar contador

`POST /api/accesos-contadores/invitar`

```json
{
  "anfitrionId": "00000000-0000-0000-0000-000000000000",
  "correoContador": "contador@demo.com",
  "puedeVerIngresos": true,
  "puedeVerGastos": true,
  "puedeGenerarReportes": true,
  "fechaExpiracion": "2026-12-31T23:59:59Z"
}
```

## Validar permiso

`GET /api/accesos-contadores/validar?anfitrionId={id}&correoContador={correo}&permiso=INGRESOS`

## Revocar acceso

`PUT /api/accesos-contadores/{accesoId}/revocar`

```json
{
  "anfitrionId": "00000000-0000-0000-0000-000000000000",
  "justificacion": "Cambio de contador autorizado por el anfitrion."
}
```
