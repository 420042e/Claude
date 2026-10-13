namespace ApiTarjetas.Dominio.Entidades;

/// <summary>
/// Tarjeta de un cliente. Mapea a la tabla [api].[TARJETA].
/// </summary>
public class Tarjeta
{
    public int TarjetaId { get; set; }
    public string NumeroTarjeta { get; set; } = string.Empty;
    public string NombreTitular { get; set; } = string.Empty;
    public string TipoTarjeta { get; set; } = string.Empty;
    public decimal LimiteCredito { get; set; }
    public decimal Saldo { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public bool Estado { get; set; }
    public string UsuarioCreacion { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
}
