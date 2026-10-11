using ApiTarjetas.Dominio.Entidades;

namespace ApiTarjetas.Dominio.Interfaces;

public interface ITarjetaRepository
{
    // Lectura (EF Core, sin tracking)
    Task<IReadOnlyList<Tarjeta>> ListarAsync(CancellationToken ct = default);
    Task<Tarjeta?> ObtenerPorIdAsync(int tarjetaId, CancellationToken ct = default);

    // Escritura (Stored Procedures TARJETA_Add / _Update / _Delete)
    Task<int> AgregarAsync(Tarjeta tarjeta, CancellationToken ct = default);
    Task ActualizarAsync(Tarjeta tarjeta, string usuario, CancellationToken ct = default);
    Task EliminarAsync(int tarjetaId, string usuario, CancellationToken ct = default);
}
