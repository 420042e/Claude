# ApiTarjetas — Proyecto de referencia (Clase 02)

API REST de ejemplo (ASP.NET Core 8 + SQL Server) usada en el **Laboratorio 02**.
Este repo **no tiene `CLAUDE.md` a propósito**: lo escribes tú durante el laboratorio.

## Estructura

```
ApiTarjetas/
├── src/
│   ├── ApiTarjetas.Api/       Controladores, Program.cs, appsettings.json
│   ├── ApiTarjetas.Dominio/   Entidades, interfaces y servicios (sin dependencias)
│   └── ApiTarjetas.Datos/     EF Core (solo lectura) + llamadas a Stored Procedures (escritura)
├── tests/
│   └── ApiTarjetas.Tests/     Pruebas xUnit de la capa de dominio
└── database/
    ├── 01_Esquema/            Esquema [api]
    ├── 02_Tablas/             Una tabla por archivo (TARJETA.sql)
    ├── 03_StoredProcedures/   ABM: TARJETA_Add / _Update / _Delete
    └── 04_Permisos/           GRANT EXECUTE por SP (los aplica un humano)
```

## Comandos

```powershell
dotnet build
dotnet test
dotnet run --project src/ApiTarjetas.Api
```

Los scripts de `database/` se aplican en orden (01 → 02 → 03 → 04) con `sqlcmd` o SSMS.

## Probar la API

- **Swagger UI** (Development): https://localhost:49857/swagger
- **Postman**: importa la colección y el entorno de `postman/` (ver `postman/README.md`).
