# HU-010 - Reconstruccion de bases imponibles retroactivas

## Objetivo

Permitir que un anfitrion reconstruya bases imponibles mensuales de un año fiscal anterior para apoyar procesos de regularizacion voluntaria.

## Alcance inicial

- Reconstruccion mensual a partir de reservas historicas ya registradas.
- Aplicacion de tarifas historicas desde `PeriodoFiscal`.
- Deteccion de meses sin datos y meses sin normativa.
- Generacion de resumen consolidado anual.
- Validacion basica de archivo historico CSV y plantilla de referencia.

## Endpoints

- `POST /api/reconstrucciones-bases`
- `POST /api/reconstrucciones-bases/validar-historico`
- `GET /api/reconstrucciones-bases/plantilla`

## Nota tecnica

Esta version usa tablas existentes (`reserva`, `periodo_fiscal`, `calculo_fiscal`) para evitar agregar esquema nuevo a Neon durante el sprint.
