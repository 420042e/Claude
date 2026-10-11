using ApiTarjetas.Datos;
using ApiTarjetas.Datos.Repositorios;
using ApiTarjetas.Dominio.Interfaces;
using ApiTarjetas.Dominio.Servicios;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TarjetasDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Tarjetas"))
       .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

builder.Services.AddScoped<ITarjetaRepository, TarjetaRepository>();
builder.Services.AddScoped<TarjetaService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new() { Title = "ApiTarjetas", Version = "v1" }));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
