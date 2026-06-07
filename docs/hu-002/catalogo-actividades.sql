-- =============================================================
-- Catálogo Actividades Económicas DGT — Costa Rica
-- HU-002 | Versión 1.0.0 | 2026-06-07
-- Fuente: Dirección General de Tributación (DGT)
-- Ejecutar después de: dotnet ef database update
-- =============================================================

SET IDENTITY_INSERT ActividadesEconomicas ON;

INSERT INTO ActividadesEconomicas (Id, Codigo, Descripcion, Activa)
VALUES
  -- Alojamiento
  (1,  '551001', 'Hoteles y alojamiento turístico',                    1),
  (2,  '551002', 'Alquiler de habitaciones en casas de familia',       1),
  (3,  '552001', 'Actividades de campamentos y albergues',             1),
  (4,  '559001', 'Otros tipos de alojamiento temporal',                1),

  -- Inmuebles
  (5,  '682001', 'Alquiler de bienes inmuebles propios o arrendados',  1),
  (6,  '682002', 'Alquiler de propiedades residenciales',              1),

  -- Turismo y cultura
  (7,  '791001', 'Agencias de viajes y operadores turísticos',         1),
  (8,  '900001', 'Actividades artísticas y culturales',                1),

  -- Restaurantes y bebidas
  (9,  '561001', 'Restaurantes y servicios de comida',                 1),
  (10, '561002', 'Sodas y cafeterías',                                 1),
  (11, '563001', 'Bares y cantinas',                                   1),

  -- Transporte
  (12, '492001', 'Transporte de pasajeros por carretera',              1),
  (13, '492002', 'Servicios de taxi',                                  1),

  -- Servicios profesionales
  (14, '691001', 'Actividades jurídicas',                              1),
  (15, '692001', 'Actividades de contabilidad y auditoría',            1),
  (16, '702001', 'Actividades de consultoría de gestión empresarial',  1),

  -- Comercio
  (17, '471001', 'Venta al por menor en establecimientos no especializados', 1),
  (18, '471002', 'Supermercados y minisupers',                         1),

  -- Salud
  (19, '861001', 'Actividades de hospitales',                          1),
  (20, '862001', 'Actividades de médicos y odontólogos',               1);

SET IDENTITY_INSERT ActividadesEconomicas OFF;
