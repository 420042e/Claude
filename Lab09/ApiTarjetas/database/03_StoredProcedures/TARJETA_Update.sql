/*
  Objeto      : [api].[TARJETA_Update]
  Descripción : Modificación de titular y límite de una tarjeta.
  Autor       : Equipo Tarjetas
  Fecha       : 2026-09-22
*/
CREATE OR ALTER PROCEDURE [api].[TARJETA_Update]
    @TARJETA_ID_IN            INT,
    @NOMBRE_TITULAR_VC        VARCHAR(100),
    @LIMITE_CREDITO_DC        DECIMAL(18,2),
    @USUARIO_MODIFICACION_VC  VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [api].[TARJETA]
    SET NOMBRE_TITULAR_VC       = @NOMBRE_TITULAR_VC,
        LIMITE_CREDITO_DC       = @LIMITE_CREDITO_DC,
        USUARIO_MODIFICACION_VC = @USUARIO_MODIFICACION_VC,
        FECHA_MODIFICACION_DT   = GETDATE()
    WHERE TARJETA_ID_IN = @TARJETA_ID_IN;
END
GO
