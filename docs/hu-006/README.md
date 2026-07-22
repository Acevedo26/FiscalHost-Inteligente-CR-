# HU-006 - Clasificacion automatica de ingresos

## Objetivo

Clasificar automaticamente los ingresos de reservas como gravados o exentos para apoyar el calculo de obligaciones tributarias del anfitrion.

## Alcance inicial

- Clasificacion automatica segun dias de estancia.
- Calculo de IVA, base imponible de renta, impuesto de renta, retencion extranjera y neto para el anfitrion.
- Reclasificacion manual con justificacion obligatoria.
- Registro de auditoria para cambios manuales.

## Reglas implementadas

- Estancias menores a 30 dias: `Gravado 13% IVA`.
- Estancias iguales o superiores a 30 dias con huesped residente: `Exento de IVA`.
- Ingresos de fuente extranjera sin factura electronica nacional: retencion del 15%.
- Base imponible de renta: `MontoBruto * 0.85`.
- Impuesto de renta: `BaseImponibleRenta * 0.15`.

## Endpoints

- `POST /api/clasificacion-ingresos/clasificar`
- `PUT /api/clasificacion-ingresos/{id}/reclasificar`
- `GET /api/clasificacion-ingresos/{id}`

## Nota tecnica

Esta version no incluye migracion de base de datos porque el proyecto esta en transicion entre SQL Server LocalDB y Neon PostgreSQL. Las entidades y el `DbContext` ya estan preparados para persistencia, y el flujo se valida con pruebas usando base InMemory.
