# CLAUDE.md — ApiTarjetas

API REST de tarjetas (BCP) sobre SQL Server. Todo objeto de base de datos que crees o modifiques
**debe** cumplir el *Estándar de Programación SQL Server del BCP* (`Estandar - Programacion SQL Server.pdf`,
v1.0) aunque el usuario no lo pida explícitamente.

## Stack

- .NET 8 (`net8.0`, C# 12, `Nullable` e `ImplicitUsings` activos — ver `Directory.Build.props`)
- ASP.NET Core Web API + Swashbuckle (Swagger solo en Development)
- EF Core 8 SqlServer — **solo lectura** (`NoTracking`)
- SQL Server — **escritura solo vía Stored Procedures** del esquema `api`
- xUnit para pruebas de la capa de dominio

## Estructura de capas

```
src/ApiTarjetas.Api/      Controladores, Program.cs (DI), appsettings*.json
src/ApiTarjetas.Dominio/  Entidades, interfaces de repositorio, servicios. SIN dependencias externas.
src/ApiTarjetas.Datos/    TarjetasDbContext (lectura EF) + repositorios que llaman SPs (escritura)
tests/ApiTarjetas.Tests/  xUnit del Dominio con repositorios falsos (no tocan BD)
database/01_Esquema/      Esquemas
database/02_Tablas/       Una tabla por archivo: <TABLA>.sql
database/03_StoredProcedures/  Un SP por archivo: <TABLA>_<Accion>.sql
database/04_Permisos/     GRANT EXECUTE por SP: <TABLA>_<Accion>_Grant.sql (gate 4: los aplica un humano)
```

Dependencias permitidas: `Api → Dominio, Datos`; `Datos → Dominio`; `Dominio → nada`.
Las validaciones de negocio van en `Dominio/Servicios`; los repositorios solo mapean parámetros.

## Comandos

```powershell
dotnet build
dotnet test
dotnet run --project src/ApiTarjetas.Api     # https://localhost:49857/swagger
```

**Aplicar SQL** (lo hace un humano, ver *Límites*): ejecutar los scripts en orden de carpeta
`01_Esquema → 02_Tablas → 03_StoredProcedures → 04_Permisos`, con SSMS o:

```powershell
sqlcmd -S <servidor> -d <BASE> -E -i database\02_Tablas\TARJETA.sql
```

Los scripts deben ser **idempotentes**: `IF NOT EXISTS` / `IF OBJECT_ID(...) IS NULL` para esquemas y
tablas, `CREATE OR ALTER PROCEDURE` para SPs, y terminar con `GO`.

## Estándar SQL Server BCP (obligatorio)

### 1. Nomenclatura: MAYÚSCULAS, singular, separadas por `_`
- Tablas en **MAYÚSCULAS y singular**: `CUENTA`, `CUENTA_DETALLE` (nunca `Cuentas`, `CuentaDetalle`).
- Base de datos en MAYÚSCULAS (`BD_TARJETAS`); esquema en **minúsculas** (`api`).
- Sin espacios ni puntos en identificadores. Tablas temporales: prefijo `TEMP_` (`api.TEMP_CUENTA`).
- Siempre calificar con esquema: `[api].[CUENTA]`.

### 2. Columnas, parámetros y variables con sufijo de tipo
`NOMBRE_SUFIJO` en mayúsculas. Los parámetros de SP se llaman **igual que la columna** que afectan
(`@SALDO_DC` → `SALDO_DC`).

| Tipo | Sufijo | Tipo | Sufijo | Tipo | Sufijo |
|---|---|---|---|---|---|
| bigint | `BI` | datetime | `DT` | nvarchar | `NV` |
| int | `IN` | smalldatetime | `SD` | ntext | `NT` |
| smallint | `SI` | date | `DA` | binary | `BY` |
| tinyint | `TI` | time | `TM` | varbinary | `VY` |
| bit | `BT` | char | `CH` | image | `IM` |
| decimal | `DC` | varchar | `VC` | timestamp | `TS` |
| numeric | `NU` | text | `TX` | uniqueidentifier | `UN` |
| money | `MO` | nchar | `NC` | xml | `XM` |
| float / real | `FL` / `RE` | | | | |

Ejemplos: `NUMERO_VC`, `SALDO_DC`, `FECHA_CREACION_DT`, `CUENTA_ID_IN`, `ESTADO_BT`.

Toda tabla incluye las columnas de auditoría usadas en el repo:
`ESTADO_BT` (default 1), `USUARIO_CREACION_VC`, `FECHA_CREACION_DT` (default `GETDATE()`),
`USUARIO_MODIFICACION_VC NULL`, `FECHA_MODIFICACION_DT NULL`.

Constraints e índices:
- `PK_TABLA`, `FK_TABLAORIGEN_TABLAREFERENCIADA`, `UQ_TABLA_COLUMNA`, `DF_TABLA_COLUMNA`, `CK_TABLA_NOMBRE`
- Índices `I_AK_…` (unique), `I_CL_…` (clustered), `I_NCL_…` (nonclustered)
- Triggers `TR_TABLA_Insert|Update|Delete`; vistas `VW_VERBO_INFINITIVO` (p. ej. `VW_CONSULTAR_PRODUCTO`)

### 3. ABM solo por Stored Procedures
- Nombre `TABLA_Accion`: `_Add`, `_Update`, `_Delete` (baja **lógica**: `ESTADO_BT = 0`),
  `_GetByID`, `_ListByXxx`. **Nunca** prefijo `sp` / `sp_`.
- El código C# **nunca** hace `INSERT/UPDATE/DELETE` por EF (`SaveChanges`) ni SQL embebido: llama al SP
  desde el repositorio (ver `TarjetaRepository.EjecutarSpAsync`).
- Cada SP: `SET NOCOUNT ON;`, palabras reservadas y funciones en MAYÚSCULAS, una cláusula por línea,
  identidad con `SCOPE_IDENTITY()` en parámetro `OUTPUT` (no `@@IDENTITY`).
- Cabecera obligatoria:

```sql
/*************************************************************
*Descripcion        : <funcionalidad>
*Fecha Crea         : AAAA-MM-DD
*Fecha Mod          :
*Parametros         : @PARAM_XX -> descripción
*Autor              : <autor>
*Version            : 1.0
*Cambios Importantes:
**************************************************************/
```

- Permisos granulares: `GRANT EXECUTE ON [api].[TABLA_Accion] TO [UsrBD_<APLICACION>]`. Usuarios de BD
  con nombre `UsrBD_<APLICACION>`.

### 4. Cadena de conexión con `Application Name`
- `Application Name=<PREFIJO>_<Aplicativo>` donde el prefijo es:
  `WEBS_` (web / web services — **este repo: `WEBS_ApiTarjetas`**), `WINS_` (servicio Windows), `EJEC_` (ejecutable).
- Preferir `Integrated Security=True`. Si es usuario SQL, usar `UsrBD_<APLICACION>`, nunca `sa`.
- No escribir credenciales en archivos versionados. `appsettings.Development.json` está en `.gitignore`:
  no leas, copies ni muestres su contenido.

### 5. Prohibido
- **Cursores** (usar operaciones de conjunto).
- **Linked Servers**.
- **Scripts anidados / consultas entre bases** (`OtraBase.dbo.Tabla`).
- Dar `AUTHORIZATION`, `OWNER` o `CONTROL` sobre esquemas.
- SPs con prefijo `sp`.

## Límites (gates) — requieren aprobación humana explícita

Claude **NUNCA** ejecuta nada contra una base de datos (ni `sqlcmd`, ni `Invoke-Sqlcmd`, ni
`dotnet ef database update`, ni la API contra una BD real). Escribe los scripts; un humano los revisa y aplica.

**Detente y pide aprobación antes de:**
1. Escribir o modificar DDL destructivo o que altere datos existentes: `DROP`, `TRUNCATE`,
   `ALTER TABLE` que elimine/renombre columnas o cambie tipos/nulabilidad.
2. Cualquier DML de datos (`INSERT/UPDATE/DELETE`) fuera de un SP, o sin `WHERE`.
3. Cambiar la **firma** (parámetros, tipos, resultado) de un SP existente — rompe a sus consumidores.
4. `GRANT`/`REVOKE`, creación de usuarios, logins, roles o esquemas.
5. Cambiar cadenas de conexión o cualquier `appsettings*.json`.
6. Agregar migraciones EF Core o hacer que EF escriba en la BD.
7. Agregar o actualizar paquetes NuGet.
8. Cualquier excepción al estándar BCP (debe justificarse y aprobarse).

**Permitido sin pedir permiso:**
- Crear scripts nuevos e idempotentes en `database/` (tablas nuevas, SPs nuevos) siguiendo el estándar.
- Editar código C# en `src/` y `tests/`, agregar pruebas.
- Ejecutar `dotnet build` y `dotnet test`.

Al terminar una tarea de BD, lista los scripts creados/modificados y el orden en que el humano debe aplicarlos.
