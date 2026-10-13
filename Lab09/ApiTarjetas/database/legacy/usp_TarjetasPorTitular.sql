-- SP heredado (pre-estándar). NO usar como modelo: lo migra el subagente migrador-sp.
CREATE PROCEDURE usp_TarjetasPorTitular @Titular varchar(100)
AS
BEGIN
    create table #Resultado (Id int, Numero varchar(16), Saldo decimal(18,2))
    declare @id int, @num varchar(16), @saldo decimal(18,2)

    declare c cursor for select TARJETA_ID_IN, NUMERO_TARJETA_VC, SALDO_DC from TARJETA where NOMBRE_TITULAR_VC like '%' + @Titular + '%' and ESTADO_BT = 1
    open c
    fetch next from c into @id, @num, @saldo
    while @@fetch_status = 0
    begin
        insert into #Resultado values (@id, @num, @saldo)
        fetch next from c into @id, @num, @saldo
    end
    close c
    deallocate c

    select * from #Resultado order by Id
END
GO
