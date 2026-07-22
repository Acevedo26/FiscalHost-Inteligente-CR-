# Endpoints - HU-015

## Exportar datos

`POST /api/exportaciones-hacienda`

```json
{
  "usuarioId": "00000000-0000-0000-0000-000000000000",
  "calculoId": null,
  "anioFiscal": 2026,
  "mes": 1,
  "formato": "XML",
  "tipoContenido": "DECLARACION",
  "protegerConContrasena": true,
  "contrasena": "ClaveTemporal123"
}
```

La respuesta incluye metadatos del archivo y `contenidoBase64`.
