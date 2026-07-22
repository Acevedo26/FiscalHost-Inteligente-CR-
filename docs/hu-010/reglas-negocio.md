# Reglas de negocio - HU-010

## Reconstruccion mensual

El sistema agrupa reservas por `PeriodoFiscalAnio` y `PeriodoFiscalMes`.

Para cada mes calcula:

- ingresos brutos
- ingresos gravados
- ingresos exentos
- debito fiscal
- retenciones acreditadas
- base imponible de renta
- impuesto de renta
- total a pagar

## Normativa historica

Las tarifas se toman de `PeriodoFiscal`:

- `TarifaIva`
- `TarifaRentaCapital`
- `DeduccionPlanaCapital`

Si no existe periodo fiscal para un mes con datos, el mes se reporta como sin normativa historica.

## Datos incompletos

Si faltan meses con datos o normativa, el sistema informa la situacion. Solo guarda calculos si `ContinuarConDatosIncompletos` es verdadero.

## Archivo historico

Columnas requeridas:

- `FechaInicio`
- `FechaFin`
- `MontoBruto`
- `MontoGravado`
- `MontoExento`
- `RetencionExtranjera`
