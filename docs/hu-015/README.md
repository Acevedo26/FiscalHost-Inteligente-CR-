# HU-015 - Exportacion de datos compatibles con Hacienda

## Objetivo

Permitir que el anfitrion exporte informacion fiscal en formatos utiles para Hacienda o para su contador.

## Alcance inicial

- Exportacion XML para declaraciones.
- Exportacion CSV de ingresos y gastos.
- Validacion para impedir exportaciones vacias.
- Proteccion opcional del archivo mediante cifrado con contraseña.
- Registro de la exportacion en la tabla `exportacion`.

## Endpoint

- `POST /api/exportaciones-hacienda`

## Nota tecnica

La proteccion implementada cifra el contenido generado con AES antes de retornarlo en Base64. No se agrega almacenamiento fisico de archivos en esta version del sprint.
