using ApiTarjetas.Dominio.Entidades;
using ApiTarjetas.Dominio.Interfaces;

namespace ApiTarjetas.Dominio.Servicios;

public class TarjetaService(ITarjetaRepository repository)
{
    public Task<IReadOnlyList<Tarjeta>> ListarAsync(CancellationToken ct = default)
        => repository.ListarAsync(ct);

    public Task<Tarjeta?> ObtenerAsync(int tarjetaId, CancellationToken ct = default)
        => repository.ObtenerPorIdAsync(tarjetaId, ct);

    public Task<int> RegistrarAsync(Tarjeta tarjeta, CancellationToken ct = default)
    {
        if (tarjeta.NumeroTarjeta.Length != 16 || !tarjeta.NumeroTarjeta.All(char.IsDigit))
            throw new ArgumentException("El número de tarjeta debe tener 16 dígitos.");

        if (tarjeta.LimiteCredito < 0)
            throw new ArgumentException("El límite de crédito no puede ser negativo.");

        return repository.AgregarAsync(tarjeta, ct);
    }

    public Task ActualizarAsync(Tarjeta tarjeta, string usuario, CancellationToken ct = default)
        => repository.ActualizarAsync(tarjeta, usuario, ct);

    public Task EliminarAsync(int tarjetaId, string usuario, CancellationToken ct = default)
        => repository.EliminarAsync(tarjetaId, usuario, ct);
}
