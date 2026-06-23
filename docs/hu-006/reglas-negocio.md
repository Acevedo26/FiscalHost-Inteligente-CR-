# Reglas de negocio - HU-006

## Calculo de estancia

`DiasEstancia = FechaSalida.Date - FechaEntrada.Date`

La fecha de salida debe ser posterior a la fecha de entrada.

## Clasificacion de IVA

| Condicion | Clasificacion | Formula IVA |
| --- | --- | --- |
| `DiasEstancia < 30` | Gravado 13% IVA | `MontoBruto * 0.13` |
| `DiasEstancia >= 30` y huesped residente | Exento de IVA | `0` |

Si la estancia es de 30 dias o mas pero el huesped no es residente, se mantiene como gravado.

## Retencion extranjera

Si la fuente del ingreso es extranjera y no existe factura electronica nacional:

`MontoRetencion = MontoBruto * 0.15`

## Renta de capital inmobiliario

`BaseImponibleRenta = MontoBruto * 0.85`

`ImpuestoRenta = BaseImponibleRenta * 0.15`

## Neto para el anfitrion

`NetoAnfitrion = MontoBruto - MontoIva - ImpuestoRenta - MontoRetencion`

## Reclasificacion manual

La reclasificacion manual exige justificacion. Si la justificacion esta vacia, el sistema rechaza el cambio.

Cada reclasificacion registra:

- UsuarioId
- FechaEvento
- ValorAnterior
- ValorNuevo
- Justificacion
