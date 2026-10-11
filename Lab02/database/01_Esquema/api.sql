/*
  Objeto      : Esquema [api]
  Descripción : Esquema del aplicativo ApiTarjetas.
  Autor       : Equipo Tarjetas
  Fecha       : 2026-09-22
*/
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'api')
    EXEC ('CREATE SCHEMA [api]');
GO
