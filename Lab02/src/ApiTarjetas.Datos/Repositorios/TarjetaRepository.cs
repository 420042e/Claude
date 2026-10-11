using System.Data;
using ApiTarjetas.Dominio.Entidades;
using ApiTarjetas.Dominio.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ApiTarjetas.Datos.Repositorios;

public class TarjetaRepository(TarjetasDbContext db) : ITarjetaRepository
{
    public async Task<IReadOnlyList<Tarjeta>> ListarAsync(CancellationToken ct = default)
        => await db.Tarjetas.AsNoTracking().Where(t => t.Estado).ToListAsync(ct);

    public Task<Tarjeta?> ObtenerPorIdAsync(int tarjetaId, CancellationToken ct = default)
        => db.Tarjetas.AsNoTracking().FirstOrDefaultAsync(t => t.TarjetaId == tarjetaId, ct);

    public async Task<int> AgregarAsync(Tarjeta tarjeta, CancellationToken ct = default)
    {
        var id = new SqlParameter("@TARJETA_ID_IN", SqlDbType.Int) { Direction = ParameterDirection.Output };

        await EjecutarSpAsync("api.TARJETA_Add", ct,
            new SqlParameter("@NUMERO_TARJETA_VC", tarjeta.NumeroTarjeta),
            new SqlParameter("@NOMBRE_TITULAR_VC", tarjeta.NombreTitular),
            new SqlParameter("@TIPO_TARJETA_VC", tarjeta.TipoTarjeta),
            new SqlParameter("@LIMITE_CREDITO_DC", tarjeta.LimiteCredito),
            new SqlParameter("@FECHA_VENCIMIENTO_DT", tarjeta.FechaVencimiento),
            new SqlParameter("@USUARIO_CREACION_VC", tarjeta.UsuarioCreacion),
            id);

        return (int)id.Value;
    }

    public Task ActualizarAsync(Tarjeta tarjeta, string usuario, CancellationToken ct = default)
        => EjecutarSpAsync("api.TARJETA_Update", ct,
            new SqlParameter("@TARJETA_ID_IN", tarjeta.TarjetaId),
            new SqlParameter("@NOMBRE_TITULAR_VC", tarjeta.NombreTitular),
            new SqlParameter("@LIMITE_CREDITO_DC", tarjeta.LimiteCredito),
            new SqlParameter("@USUARIO_MODIFICACION_VC", usuario));

    public Task EliminarAsync(int tarjetaId, string usuario, CancellationToken ct = default)
        => EjecutarSpAsync("api.TARJETA_Delete", ct,
            new SqlParameter("@TARJETA_ID_IN", tarjetaId),
            new SqlParameter("@USUARIO_MODIFICACION_VC", usuario));

    private async Task EjecutarSpAsync(string nombreSp, CancellationToken ct, params SqlParameter[] parametros)
    {
        var conexion = db.Database.GetDbConnection();
        await using var cmd = conexion.CreateCommand();
        cmd.CommandText = nombreSp;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddRange(parametros);

        if (conexion.State != ConnectionState.Open)
            await conexion.OpenAsync(ct);

        await cmd.ExecuteNonQueryAsync(ct);
    }
}
