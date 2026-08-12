# Reglas de negocio - HU-020

## Registro

Cada evento de auditoria puede almacenar:

- usuario
- fecha y hora
- operacion
- tabla afectada
- registro afectado
- valores anteriores
- valores nuevos
- campos modificados
- justificacion

## Campo sensible

Si `EsCampoSensible` es verdadero, la justificacion es obligatoria. El sistema impide registrar el cambio si no existe justificacion.

## Consulta

El historial se retorna en orden cronologico ascendente y puede filtrarse por:

- usuario
- tabla afectada
- registro afectado

## Inmutabilidad

Los registros de auditoria no pueden modificarse ni eliminarse una vez guardados.
