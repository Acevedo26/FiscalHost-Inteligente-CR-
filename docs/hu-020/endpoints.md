# Endpoints - HU-020

## Registrar auditoria

`POST /api/auditoria/registrar`

```json
{
  "usuarioId": "00000000-0000-0000-0000-000000000000",
  "correoUsuario": "usuario@demo.com",
  "operacion": 1,
  "tablaAfectada": "usuario",
  "registroId": "00000000-0000-0000-0000-000000000000",
  "oldValues": "{\"correo\":\"old@demo.com\"}",
  "newValues": "{\"correo\":\"new@demo.com\"}",
  "camposModificados": ["correo"],
  "esCampoSensible": true,
  "justificacion": "Cambio solicitado por el anfitrion."
}
```

## Consultar historial

`GET /api/auditoria/historial?usuarioId={id}&tablaAfectada=usuario&registroId={id}`

## Exportar historial

`GET /api/auditoria/exportar?usuarioId={id}&tablaAfectada=usuario`
