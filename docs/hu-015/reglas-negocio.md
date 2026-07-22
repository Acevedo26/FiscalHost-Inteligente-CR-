# Reglas de negocio - HU-015

## Formatos permitidos

- `XML`
- `CSV`

## Tipos de contenido permitidos

- `DECLARACION`
- `MOVIMIENTOS`

## Validaciones

El sistema rechaza la exportacion si:

- el usuario no es valido
- el año fiscal no es valido
- el mes esta fuera del rango 1-12
- el formato no es XML o CSV
- no existen ingresos, gastos ni calculo fiscal para exportar
- se solicita proteccion sin contraseña

## Proteccion

Si el usuario solicita contraseña, el contenido se cifra con AES y se entrega como `application/octet-stream`.
