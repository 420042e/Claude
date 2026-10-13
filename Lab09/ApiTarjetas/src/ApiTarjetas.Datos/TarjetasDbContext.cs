using ApiTarjetas.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace ApiTarjetas.Datos;

/// <summary>
/// Contexto de EF Core usado SOLO para lectura. La escritura se hace vía Stored Procedures.
/// </summary>
public class TarjetasDbContext(DbContextOptions<TarjetasDbContext> options) : DbContext(options)
{
    public DbSet<Tarjeta> Tarjetas => Set<Tarjeta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tarjeta>(e =>
        {
            e.ToTable("TARJETA", "api");
            e.HasKey(x => x.TarjetaId);
            e.Property(x => x.TarjetaId).HasColumnName("TARJETA_ID_IN");
            e.Property(x => x.NumeroTarjeta).HasColumnName("NUMERO_TARJETA_VC");
            e.Property(x => x.NombreTitular).HasColumnName("NOMBRE_TITULAR_VC");
            e.Property(x => x.TipoTarjeta).HasColumnName("TIPO_TARJETA_VC");
            e.Property(x => x.LimiteCredito).HasColumnName("LIMITE_CREDITO_DC").HasPrecision(18, 2);
            e.Property(x => x.Saldo).HasColumnName("SALDO_DC").HasPrecision(18, 2);
            e.Property(x => x.FechaVencimiento).HasColumnName("FECHA_VENCIMIENTO_DT");
            e.Property(x => x.Estado).HasColumnName("ESTADO_BT");
            e.Property(x => x.UsuarioCreacion).HasColumnName("USUARIO_CREACION_VC");
            e.Property(x => x.FechaCreacion).HasColumnName("FECHA_CREACION_DT");
        });
    }
}
