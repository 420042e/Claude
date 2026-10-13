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
/*************************************************************
*Descripcion        : Baja lógica de una tarjeta (ESTADO_BT = 0).
*                     Solo actúa sobre tarjetas activas y sin saldo;
*                     en otro caso no modifica nada.
*Fecha Crea         : 2026-09-22
*Fecha Mod          : 2026-10-13
*Parametros         : @TARJETA_ID_IN           -> Id de la tarjeta
*                     @USUARIO_MODIFICACION_VC -> Usuario que realiza la baja
*Autor              : Equipo Tarjetas
*Version            : 1.1
*Cambios Importantes: 1.1 (HU-TAR-07) Se agrega ESTADO_BT = 1 y SALDO_DC = 0
*                     al WHERE: no sobrescribe la auditoría de una baja
*                     previa ni da de baja tarjetas con saldo. Firma sin cambios.
**************************************************************/
BEGIN
    SET NOCOUNT ON;

    UPDATE [api].[TARJETA]
    SET ESTADO_BT               = 0,
        USUARIO_MODIFICACION_VC = @USUARIO_MODIFICACION_VC,
        FECHA_MODIFICACION_DT   = GETDATE()
    WHERE TARJETA_ID_IN = @TARJETA_ID_IN
      AND ESTADO_BT     = 1
      AND SALDO_DC      = 0;
END
GO
