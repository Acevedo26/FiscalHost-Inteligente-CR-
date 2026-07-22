# Endpoints - HU-010

## Reconstruir bases

`POST /api/reconstrucciones-bases`

```json
{
  "usuarioId": "00000000-0000-0000-0000-000000000000",
  "anioFiscal": 2025,
  "continuarConDatosIncompletos": false
}
```

## Validar archivo historico

`POST /api/reconstrucciones-bases/validar-historico`

Recibe un archivo CSV por `multipart/form-data` en el campo `archivo`.

## Descargar plantilla

`GET /api/reconstrucciones-bases/plantilla`
