# Endpoints - HU-006

## Clasificar ingreso

`POST /api/clasificacion-ingresos/clasificar`

```json
{
  "anfitrionId": "anf-001",
  "fechaEntrada": "2026-05-01T00:00:00Z",
  "fechaSalida": "2026-05-10T00:00:00Z",
  "montoBruto": 1000,
  "fuenteIngreso": 1,
  "tieneFacturaElectronicaNacional": false,
  "huespedResidente": true
}
```

`fuenteIngreso`: `1 = Nacional`, `2 = Extranjera`.

## Reclasificar ingreso

`PUT /api/clasificacion-ingresos/{id}/reclasificar`

```json
{
  "usuarioId": "usr-001",
  "clasificacionIva": 2,
  "justificacion": "Contrato de estancia prolongada validado."
}
```

`clasificacionIva`: `1 = Gravado13`, `2 = Exento`.

## Consultar clasificacion

`GET /api/clasificacion-ingresos/{id}`
