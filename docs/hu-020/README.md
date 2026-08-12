# HU-020 - Registro de auditoria inalterable

## Objetivo

Garantizar que los cambios relevantes del sistema queden registrados en un log consultable, exportable e inalterable.

## Alcance inicial

- Registro manual/controlado de eventos de auditoria.
- Captura de usuario, operacion, tabla, registro, valores anteriores/nuevos y justificacion.
- Validacion de justificacion obligatoria para campos sensibles.
- Consulta cronologica del historial.
- Exportacion del historial a CSV.
- Bloqueo de modificaciones y eliminaciones sobre `audit_log` desde `AppDbContext`.

## Endpoints

- `POST /api/auditoria/registrar`
- `GET /api/auditoria/historial`
- `GET /api/auditoria/exportar`

## Nota tecnica

La inmutabilidad se implementa bloqueando entidades `AuditoriaOperacion` en estado `Modified` o `Deleted` durante `SaveChanges` y `SaveChangesAsync`.
