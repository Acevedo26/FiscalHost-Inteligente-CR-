# HU-016 - Gestion segura de contadores

## Objetivo

Permitir que un anfitrion autorice a su contador con permisos controlados, expiracion temporal y revocacion segura.

## Alcance inicial

- Invitacion por correo con permisos de solo lectura.
- Registro de permisos en `acceso_contador`.
- Validacion de acceso autorizado por permiso.
- Revocacion con justificacion obligatoria.
- Procesamiento de expiraciones y notificacion previa.
- Auditoria de invitaciones, revocaciones y expiraciones.

## Endpoints

- `POST /api/accesos-contadores/invitar`
- `GET /api/accesos-contadores/anfitrion/{anfitrionId}`
- `GET /api/accesos-contadores/validar`
- `PUT /api/accesos-contadores/{accesoId}/revocar`
- `POST /api/accesos-contadores/procesar-expiraciones`

## Nota tecnica

El envio real de correo reutiliza `INotificacionService`, que por ahora registra mensajes en logs como mecanismo simulado.
