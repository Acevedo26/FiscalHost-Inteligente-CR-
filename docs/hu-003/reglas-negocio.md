# Reglas de Negocio — HU-003

---

## Validación del archivo

- Solo se aceptan archivos con extensión `.p12` (insensible a mayúsculas).
- El contenido se valida abriendo el certificado con `X509Certificate2`. Si falla, se retorna `422`.
- Si la contraseña no corresponde al certificado, se retorna `422` con mensaje descriptivo.
- Si el archivo no es un `.p12` válido (bytes corruptos o formato incorrecto), se retorna `422`.

---

## Almacenamiento cifrado

- El contenido del archivo `.p12` se cifra con **AES-256** antes de persistir en base de datos.
  - Se genera un IV aleatorio por cada operación de cifrado.
  - El IV se almacena antepuesto al contenido cifrado (`IV[16 bytes] + contenido cifrado`).
  - La clave AES se deriva aplicando SHA-256 al valor de configuración `Cifrado:Clave`.
- La contraseña del certificado **no se almacena en texto plano**. Se guarda su hash **HMAC-SHA256** usando la misma clave de configuración.
  - HMAC-SHA256 es determinístico: permite comparar contraseñas sin descifrar.

---

## Actualización de contraseña

- Se verifica que el anfitrión tenga una llave activa. Si no existe, se retorna `422`.
- Se compara el HMAC-SHA256 de `contrasenaActual` contra el hash almacenado. Si no coincide, se retorna `422`.
- Se valida que `contrasenaNueva` sea correcta para el certificado almacenado (descifrado temporal).
- Si todo es válido:
  1. Se actualiza `ContrasenaHash` con el HMAC de la nueva contraseña.
  2. Se registra auditoría con acción `ACTUALIZAR_CONTRASENA`.
  3. Se envía notificación al anfitrión.

---

## Auditoría

| Acción | Cuándo se registra |
|--------|--------------------|
| `CARGA` | Al almacenar o reemplazar la llave `.p12` |
| `ACTUALIZAR_CONTRASENA` | Al cambiar exitosamente la contraseña |

Campos registrados: `Accion`, `Descripcion`, `FechaEvento`.

---

## Notificaciones

- Se notifica al anfitrión tras una actualización de contraseña exitosa.
- La implementación actual registra la notificación en el log del sistema.
- **Deuda técnica DT-003-01:** integrar con proveedor de email transaccional (AWS SES / SendGrid).

---

## Configuración requerida

| Clave | Descripción |
|-------|-------------|
| `Cifrado:Clave` | Secreto base para derivar la clave AES-256 y el HMAC-SHA256. Debe tener al menos 32 caracteres. Nunca incluir en control de versiones; usar variables de entorno o AWS Secrets Manager. |
