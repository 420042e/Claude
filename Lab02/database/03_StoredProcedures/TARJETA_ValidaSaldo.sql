/*
  Objeto      : [api].[TARJETA_ValidaSaldo]
  Descripción : Valida si una tarjeta activa y vigente tiene saldo disponible para un monto.
  Autor       : Equipo Tarjetas
  Fecha       : 2026-10-11
*/
CREATE OR ALTER PROCEDURE [api].[TARJETA_ValidaSaldo]
    @TARJETA_ID_IN          INT,
    @MONTO_DC               DECIMAL(18,2),
    @SALDO_DISPONIBLE_DC    DECIMAL(18,2) OUTPUT,
    @SALDO_SUFICIENTE_BT    BIT           OUTPUT,
    @MENSAJE_VC             VARCHAR(100)  OUTPUT
AS
/*************************************************************
*Descripcion        : Calcula el saldo disponible de una tarjeta
*                     (LIMITE_CREDITO_DC - SALDO_DC) e indica si
*                     alcanza para cubrir @MONTO_DC. Solo lectura.
*Fecha Crea         : 2026-10-11
*Fecha Mod          :
*Parametros         : @TARJETA_ID_IN       -> Id de la tarjeta
*                     @MONTO_DC            -> Monto a validar (> 0)
*                     @SALDO_DISPONIBLE_DC -> (OUTPUT) Saldo disponible; NULL si la tarjeta no existe
*                     @SALDO_SUFICIENTE_BT -> (OUTPUT) 1 = alcanza, 0 = no alcanza o no es válida
*                     @MENSAJE_VC          -> (OUTPUT) Motivo del resultado
*Autor              : Equipo Tarjetas
*Version            : 1.0
*Cambios Importantes:
**************************************************************/
BEGIN
    SET NOCOUNT ON;

    DECLARE @LIMITE_CREDITO_DC     DECIMAL(18,2),
            @SALDO_DC              DECIMAL(18,2),
            @ESTADO_BT             BIT,
            @FECHA_VENCIMIENTO_DT  DATETIME;

    SET @SALDO_DISPONIBLE_DC = NULL;
    SET @SALDO_SUFICIENTE_BT = 0;

    IF @MONTO_DC IS NULL OR @MONTO_DC <= 0
    BEGIN
        SET @MENSAJE_VC = 'El monto debe ser mayor a cero.';
        RETURN;
    END

    SELECT @LIMITE_CREDITO_DC    = LIMITE_CREDITO_DC,
           @SALDO_DC             = SALDO_DC,
           @ESTADO_BT            = ESTADO_BT,
           @FECHA_VENCIMIENTO_DT = FECHA_VENCIMIENTO_DT
    FROM [api].[TARJETA]
    WHERE TARJETA_ID_IN = @TARJETA_ID_IN;

    IF @ESTADO_BT IS NULL
    BEGIN
        SET @MENSAJE_VC = 'La tarjeta no existe.';
        RETURN;
    END

    SET @SALDO_DISPONIBLE_DC = @LIMITE_CREDITO_DC - @SALDO_DC;

    IF @ESTADO_BT = 0
    BEGIN
        SET @MENSAJE_VC = 'La tarjeta está inactiva.';
        RETURN;
    END

    IF @FECHA_VENCIMIENTO_DT < GETDATE()
    BEGIN
        SET @MENSAJE_VC = 'La tarjeta está vencida.';
        RETURN;
    END

    IF @SALDO_DISPONIBLE_DC >= @MONTO_DC
    BEGIN
        SET @SALDO_SUFICIENTE_BT = 1;
        SET @MENSAJE_VC = 'Saldo suficiente.';
    END
    ELSE
        SET @MENSAJE_VC = 'Saldo insuficiente.';
END
GO
