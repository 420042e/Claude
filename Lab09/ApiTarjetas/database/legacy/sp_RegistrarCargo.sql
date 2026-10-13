-- SP heredado (pre-estándar). NO usar como modelo: lo migra el subagente migrador-sp.
CREATE PROCEDURE sp_RegistrarCargo
    @Id int,
    @Monto decimal(18,2),
    @Usuario varchar(50)
AS
    declare @saldoActual decimal(18,2), @limite decimal(18,2)
    select @saldoActual = SALDO_DC, @limite = LIMITE_CREDITO_DC from TARJETA where TARJETA_ID_IN = @Id
    if (@saldoActual + @Monto > @limite)
        raiserror('Excede el limite', 16, 1)
    else
        update TARJETA set SALDO_DC = SALDO_DC + @Monto, USUARIO_MODIFICACION_VC = @Usuario, FECHA_MODIFICACION_DT = getdate() where TARJETA_ID_IN = @Id
GO

GRANT CONTROL ON SCHEMA::dbo TO [usuario_app]
GO
