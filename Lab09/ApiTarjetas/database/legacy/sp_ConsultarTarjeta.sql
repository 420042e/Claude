-- SP heredado (pre-estándar). NO usar como modelo: lo migra el subagente migrador-sp.
create procedure dbo.sp_ConsultarTarjeta
    @IdTarjeta int
as
select *
from dbo.TARJETA
where TARJETA_ID_IN = @IdTarjeta
go
