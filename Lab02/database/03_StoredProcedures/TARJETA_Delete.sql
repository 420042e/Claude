/*
  Objeto      : [api].[TARJETA_Delete]
  Descripción : Baja lógica de una tarjeta (ESTADO_BT = 0).
  Autor       : Equipo Tarjetas
  Fecha       : 2026-09-22
*/
CREATE OR ALTER PROCEDURE [api].[TARJETA_Delete]
    @TARJETA_ID_IN            INT,
    @USUARIO_MODIFICACION_VC  VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [api].[TARJETA]
    SET ESTADO_BT               = 0,
        USUARIO_MODIFICACION_VC = @USUARIO_MODIFICACION_VC,
        FECHA_MODIFICACION_DT   = GETDATE()
    WHERE TARJETA_ID_IN = @TARJETA_ID_IN;
END
GO
