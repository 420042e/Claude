using ApiTarjetas.Dominio.Entidades;
using ApiTarjetas.Dominio.Interfaces;
using ApiTarjetas.Dominio.Servicios;

namespace ApiTarjetas.Tests;

public class TarjetaServiceTests
{
    private sealed class RepositorioFalso : ITarjetaRepository
    {
        public List<Tarjeta> Agregadas { get; } = [];
        public List<(int Id, string Usuario)> Eliminadas { get; } = [];

        public Task<IReadOnlyList<Tarjeta>> ListarAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<Tarjeta>>(Agregadas);

        // Igual que TarjetaRepository: solo devuelve tarjetas activas.
        public Task<Tarjeta?> ObtenerPorIdAsync(int tarjetaId, CancellationToken ct = default)
            => Task.FromResult(Agregadas.FirstOrDefault(t => t.TarjetaId == tarjetaId && t.Estado));

        public Task<int> AgregarAsync(Tarjeta tarjeta, CancellationToken ct = default)
        {
            tarjeta.TarjetaId = Agregadas.Count + 1;
            Agregadas.Add(tarjeta);
            return Task.FromResult(tarjeta.TarjetaId);
        }

        public Task ActualizarAsync(Tarjeta tarjeta, string usuario, CancellationToken ct = default) => Task.CompletedTask;
        public Task EliminarAsync(int tarjetaId, string usuario, CancellationToken ct = default)
        {
            Eliminadas.Add((tarjetaId, usuario));
            return Task.CompletedTask;
        }
    }

    private static RepositorioFalso RepositorioCon(bool estado, decimal saldo)
    {
        var repositorio = new RepositorioFalso();
        repositorio.Agregadas.Add(new Tarjeta { TarjetaId = 1, NumeroTarjeta = "4111111111111111", Estado = estado, Saldo = saldo });
        return repositorio;
    }

    [Fact]
    public async Task Registrar_TarjetaValida_DevuelveId()
    {
        var service = new TarjetaService(new RepositorioFalso());

        var id = await service.RegistrarAsync(new Tarjeta { NumeroTarjeta = "4111111111111111", LimiteCredito = 5000 });

        Assert.Equal(1, id);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("41111111111111AB")]
    public async Task Registrar_NumeroInvalido_LanzaExcepcion(string numero)
    {
        var service = new TarjetaService(new RepositorioFalso());

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.RegistrarAsync(new Tarjeta { NumeroTarjeta = numero }));
    }

    [Fact]
    public async Task Registrar_LimiteNegativo_LanzaExcepcion()
    {
        var service = new TarjetaService(new RepositorioFalso());

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.RegistrarAsync(new Tarjeta { NumeroTarjeta = "4111111111111111", LimiteCredito = -1 }));
    }
}
