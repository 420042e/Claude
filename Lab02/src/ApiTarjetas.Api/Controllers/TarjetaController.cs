using ApiTarjetas.Dominio.Entidades;
using ApiTarjetas.Dominio.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace ApiTarjetas.Api.Controllers;

[ApiController]
[Route("api/tarjetas")]
public class TarjetaController(TarjetaService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken ct)
        => Ok(await service.ListarAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obtener(int id, CancellationToken ct)
    {
        var tarjeta = await service.ObtenerAsync(id, ct);
        return tarjeta is null ? NotFound() : Ok(tarjeta);
    }

    [HttpPost]
    public async Task<IActionResult> Registrar(Tarjeta tarjeta, CancellationToken ct)
    {
        var id = await service.RegistrarAsync(tarjeta, ct);
        return CreatedAtAction(nameof(Obtener), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, Tarjeta tarjeta, CancellationToken ct)
    {
        tarjeta.TarjetaId = id;
        await service.ActualizarAsync(tarjeta, User.Identity?.Name ?? "sistema", ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken ct)
    {
        await service.EliminarAsync(id, User.Identity?.Name ?? "sistema", ct);
        return NoContent();
    }
}
