# Reglas de negocio - HU-016

## Invitacion

El anfitrion indica el correo del contador y permisos de consulta. El correo debe tener formato valido.

Permisos soportados:

- ingresos
- gastos
- reportes

## Acceso temporal

Si existe fecha de expiracion, debe ser futura. Un acceso expirado se considera no autorizado.

## Revocacion

La revocacion requiere justificacion. El sistema cambia el estado a `REVOCADO` y registra auditoria.

## Auditoria

Se registra la accion sobre `acceso_contador` en `audit_log` usando `AuditoriaOperacion`.
