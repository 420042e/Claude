---
name: convenciones-sql-server-bcp
description: Estándar de Programación SQL Server del BCP (nomenclatura y prohibiciones T-SQL). Úsala SIEMPRE que vayas a crear, modificar, revisar o nombrar objetos de base de datos en SQL Server / T-SQL — tablas, columnas, stored procedures (SP de alta/baja/modificación/consulta, ABM/CRUD), índices, constraints (PK, FK, UQ, DF, CK), triggers, vistas, esquemas, variables y parámetros, usuarios de BD, permisos GRANT o cadenas de conexión — o scripts .sql (CREATE TABLE, CREATE PROCEDURE, ALTER, CREATE INDEX), aunque el usuario no mencione el estándar ni el BCP. Ej. "crea la tabla CUENTA_DETALLE con su SP de alta". No aplica a otros motores (Oracle, PostgreSQL, MySQL) ni a SQL embebido sin objetos de BD.
---

# Convenciones SQL Server BCP

Fuente: *Estándar de Programación SQL Server*, Arquitectura y Estándares TI – BCP, v1.0 (04/07/2019).
Cada regla tiene un ID (`R-xx`) para citarla al entregar código. Las reglas marcadas
**[PDF]** vienen literalmente del estándar; las marcadas **[práctica]** son interpretaciones
o buenas prácticas que lo complementan sin contradecirlo.

## Cómo usar esta skill

1. Antes de escribir T-SQL, identifica qué objetos vas a crear (tabla, SP, índice, …).
2. Aplica las reglas de las secciones A–F correspondientes.
3. Al entregar, pasa el **checklist de la sección G** y añade una nota corta con los IDs de
   las reglas cumplidas (y cualquier excepción justificada).

---

## A. Nomenclatura general

- **R-01 [PDF]** Nombres cortos y descriptivos (`CLIENTE`). Nombres de objetos únicos: no crear
  una tabla y una vista/rol con el mismo nombre.
- **R-02 [PDF]** Sin espacios ni puntos (`.`) dentro de un identificador.
- **R-03 [PDF]** Palabras separadas por underscore `_` (`HISTORIAL_SESSION`).
- **R-04 [PDF]** Base de datos en **MAYÚSCULAS** (`API_BCP`); archivos `API_BCP_data` (MDF) y
  `API_BCP_log` (LDF).
- **R-05 [PDF]** Cada aplicación tiene **su esquema, en minúsculas** (`api`). **[práctica]**
  Calificar siempre los objetos con el esquema: `[api].[CUENTA]`, nunca `dbo` implícito.

## B. Tablas y columnas

- **R-06 [PDF]** Tabla: descriptiva, **singular** y **MAYÚSCULAS** → `CUENTA`, `CUENTA_DETALLE`
  (✗ `Cuentas`, ✗ `CuentaDetalle`, ✗ `TBL_cuenta`).
- **R-07 [PDF]** Tablas temporales (de trabajo) con prefijo `TEMP_` → `[api].[TEMP_CUENTA]`.
- **R-08 [PDF]** Columna: MAYÚSCULAS, descriptiva y **con sufijo del tipo de dato**:
  `NOMBRE_XX` → `MONEDA_VC`, `SALDO_DC`, `CUENTA_ID_IN`.

  | Tipo | XX | Tipo | XX | Tipo | XX |
  |---|---|---|---|---|---|
  | bigint | `BI` | datetime | `DT` | nvarchar | `NV` |
  | int | `IN` | smalldatetime | `SD` | ntext | `NT` |
  | smallint | `SI` | date | `DA` | binary | `BY` |
  | tinyint | `TI` | time | `TM` | varbinary | `VY` |
  | bit | `BT` | char | `CH` | image | `IM` |
  | decimal | `DC` | varchar | `VC` | timestamp | `TS` |
  | numeric | `NU` | text | `TX` | uniqueidentifier | `UN` |
  | money | `MO` | nchar | `NC` | xml | `XM` |
  | float | `FL` | real | `RE` | | |

- **R-09 [PDF]** Columnas de auditoría (según la tabla ejemplo del estándar):
  `USUARIO_CREACION_VC`, `FECHA_CREACION_DT`, `USUARIO_MODIFICACION_VC`,
  `FECHA_MODIFICACION_DT`, `ESTADO_BT` (NOT NULL).
- **R-10 [PDF]** Tablas históricas de aplicativos transaccionales: `NOMBRE_TABLA_AAAAMM`
  (hasta 6 meses) → `TBL_LOG_TRANSACCION_201905`.

## C. Constraints, índices, triggers y vistas

- **R-11 [PDF]** Toda constraint **con nombre explícito** (**[práctica]**: nunca dejar que SQL
  Server genere `PK__CUENTA__3214EC07…`):

  | Tipo | Formato | Ejemplo |
  |---|---|---|
  | Primary Key | `PK_TABLA` | `PK_CLIENTE` |
  | Foreign Key | `FK_TABLAORIGEN_TABLAREFERENCIADA` | `FK_FACTURA_CLIENTE` |
  | Unique | `UQ_TABLA_NOMBREUNIQUE` | `UQ_CLIENTE_CODIGO_CLIENTE` |
  | Default | `DF_TABLA_COLUMNA` | `DF_CLIENTE_FECHA` |
  | Check | `CK_TABLA_NOMBRECHECK` | `CK_CLIENTE_CODIGO_CLIENTE` |

- **R-12 [PDF]** Índices `I_<TIPO>_<NOMBRE>` con TIPO = `AK` (unique), `CL` (clustered),
  `NCL` (nonclustered) → `I_AK_CNTA_NUMERO_VC`, `I_CL_CCUSTODIA`, `I_NCL_CODDEPOSITANTE`.
- **R-13 [PDF]** Triggers `TR_TABLA_Accion` → `TR_CUENTA_Insert`, `TR_CUENTA_Update`,
  `TR_CUENTA_Delete`.
- **R-14 [PDF]** Vistas `VW_<VERBO_INFINITIVO>_…` en MAYÚSCULAS → `VW_CONSULTAR_PRODUCTO`.
  Su estructura (cabecera, formato) es igual a la de un SP.

## D. Stored Procedures

- **R-15 [PDF]** El ABM (alta/baja/modificación) de tablas se hace **solo vía Stored Procedures**.
- **R-16 [PDF]** Nombre `TABLA_Accion` (tabla en MAYÚSCULAS, acción en PascalCase):

  | Acción | Nombre |
  |---|---|
  | Insert / alta | `TABLA_Add` (también se acepta `TABLA_Insert`) |
  | Update | `TABLA_Update` |
  | Delete (física o cambio de estado) | `TABLA_Delete` |
  | Un registro por PK/AK | `TABLA_GetByID` |
  | Varios registros | `TABLA_ListByXxx` / `TABLA_ListXxx` |
  | Proceso complejo | `TABLA_VerboXxx` (ej. `EMPLEADO_ProcesaSueldo`) |

- **R-17 [PDF]** **PROHIBIDO** el prefijo `sp` / `sp_` (SQL Server lo busca primero en `master`).
- **R-18 [PDF]** Variables y parámetros `@NOMBRE_XX` con el sufijo de tipo de R-08. Si el
  parámetro afecta una columna, se llama **igual que la columna** (`@SALDO_DC` → `SALDO_DC`).
  Si no corresponde a ninguna columna, puede omitir el sufijo (`@FLAG_BT` / `@FLAG`).
- **R-19 [PDF]** `SET NOCOUNT ON;` al inicio del cuerpo.
- **R-20 [PDF]** Estructura: firma (nombre + parámetros) → `AS` → **cabecera de comentarios** →
  `BEGIN` → declaración de variables locales → sentencias SQL → `END`. Cabecera obligatoria:

  ```sql
  /*************************************************************
  *Descripcion        : <funcionalidad del SP>
  *Fecha Crea         : AAAA-MM-DD
  *Fecha Mod          :
  *Parametros         :
  *    @PARAM_XX -> <descripción>
  *Autor              : <autor>
  *Version            : 1.0
  *Cambios Importantes:
  **************************************************************/
  ```

- **R-21 [PDF]** Palabras reservadas y funciones del sistema en **MAYÚSCULAS** (`SELECT`,
  `INSERT INTO`, `GETDATE()`); columnas y variables en MAYÚSCULAS; **cada cláusula en una línea
  nueva** e indentada (formato tipo poorsql.com).
- **R-22 [práctica]** Recuperar la identidad con `SCOPE_IDENTITY()` (en parámetro `OUTPUT`),
  no con `@@IDENTITY` (que puede devolver el ID insertado por un trigger).

## E. Seguridad, usuarios y conexión

- **R-23 [PDF]** Permisos granulares por objeto: `GRANT EXECUTE ON [api].[TABLA_Accion] TO [UsrBD_APP]`.
- **R-24 [PDF]** Usuarios de BD `UsrBD_NOMBRE_APLICACION` (`UsrBD_APIBCP`).
- **R-25 [PDF]** Cadena de conexión con `Application Name=<PREFIJO>_<Aplicativo>`:
  `WEBS_` (web/web services), `WINS_` (servicio Windows), `EJEC_` (ejecutables).
  Preferir `Integrated Security=True`.

## F. Prohibiciones (sección 18 del estándar)

- **R-26 [PDF]** ✗ **Cursores** (`DECLARE … CURSOR`, `FETCH NEXT`) → usar operaciones de conjunto.
- **R-27 [PDF]** ✗ **Linked Servers** (`OPENQUERY`, `SERVIDOR.BD.esquema.tabla`, `sp_addlinkedserver`).
- **R-28 [PDF]** ✗ **Scripts anidados / consultas cruzadas a otra base** del servidor
  (`OTRA_BD.dbo.TABLA`).
- **R-29 [PDF]** ✗ Otorgar `AUTHORIZATION`, `OWNER` o `CONTROL` sobre esquemas.
- (R-17) ✗ SP con prefijo `sp`.

---

## G. Checklist de entrega (verificable)

Antes de entregar T-SQL, comprueba cada punto y reporta los IDs cumplidos:

- [ ] Tablas en MAYÚSCULAS, singular, `_`, con esquema (R-05, R-06)
- [ ] Toda columna/variable/parámetro termina en un sufijo válido de la tabla R-08 y coincide con su tipo (R-08, R-18)
- [ ] Columnas de auditoría presentes (R-09)
- [ ] PK/FK/UQ/DF/CK con nombre explícito y formato correcto (R-11)
- [ ] Índices `I_AK_` / `I_CL_` / `I_NCL_` (R-12)
- [ ] SP `TABLA_Accion`, sin `sp`, con cabecera, `SET NOCOUNT ON`, `BEGIN…END` (R-16, R-17, R-19, R-20)
- [ ] Palabras reservadas en MAYÚSCULAS, una cláusula por línea (R-21)
- [ ] `SCOPE_IDENTITY()` en vez de `@@IDENTITY` (R-22)
- [ ] `GRANT EXECUTE` granular a `UsrBD_…` (R-23, R-24)
- [ ] Ningún cursor, linked server, consulta a otra BD ni `AUTHORIZATION/OWNER/CONTROL` (R-26–R-29)

Búsqueda rápida de violaciones en un archivo `.sql` (debe devolver vacío):

```bash
grep -niE "CURSOR|FETCH NEXT|OPENQUERY|sp_addlinkedserver|@@IDENTITY|PROCEDURE +\[?[a-z_]*\]?\.?\[?sp|AUTHORIZATION|ALTER AUTHORIZATION|GRANT +CONTROL" archivo.sql
```

---

## H. Ejemplo de referencia

```sql
CREATE TABLE [api].[CLIENTE] (
    [CLIENTE_ID_IN]           INT IDENTITY(1,1) NOT NULL,
    [CODIGO_CLIENTE_VC]       VARCHAR(15)       NOT NULL,
    [NOMBRE_VC]               VARCHAR(100)      NOT NULL,
    [FECHA_NACIMIENTO_DA]     DATE              NULL,
    [USUARIO_CREACION_VC]     VARCHAR(20)       NOT NULL,
    [FECHA_CREACION_DT]       DATETIME          NOT NULL CONSTRAINT [DF_CLIENTE_FECHA_CREACION_DT] DEFAULT (GETDATE()),
    [USUARIO_MODIFICACION_VC] VARCHAR(20)       NULL,
    [FECHA_MODIFICACION_DT]   DATETIME          NULL,
    [ESTADO_BT]               BIT               NOT NULL CONSTRAINT [DF_CLIENTE_ESTADO_BT] DEFAULT (1),
    CONSTRAINT [PK_CLIENTE] PRIMARY KEY CLUSTERED ([CLIENTE_ID_IN]),
    CONSTRAINT [UQ_CLIENTE_CODIGO_CLIENTE] UNIQUE ([CODIGO_CLIENTE_VC])
);
GO

CREATE NONCLUSTERED INDEX [I_NCL_CLIENTE_NOMBRE_VC] ON [api].[CLIENTE] ([NOMBRE_VC]);
GO

CREATE PROCEDURE [api].[CLIENTE_Add]
     @CODIGO_CLIENTE_VC   VARCHAR(15)
    ,@NOMBRE_VC           VARCHAR(100)
    ,@FECHA_NACIMIENTO_DA DATE
    ,@USUARIO_CREACION_VC VARCHAR(20)
    ,@CLIENTE_ID_IN       INT OUTPUT
AS
/*************************************************************
*Descripcion        : Registra un nuevo cliente
*Fecha Crea         : 2026-01-15
*Fecha Mod          :
*Parametros         :
*    @CODIGO_CLIENTE_VC   -> Código único del cliente
*    @NOMBRE_VC           -> Nombre del cliente
*    @FECHA_NACIMIENTO_DA -> Fecha de nacimiento
*    @USUARIO_CREACION_VC -> Usuario que registra
*    @CLIENTE_ID_IN       -> (OUTPUT) ID generado
*Autor              : <autor>
*Version            : 1.0
*Cambios Importantes:
**************************************************************/
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [api].[CLIENTE] (
         CODIGO_CLIENTE_VC
        ,NOMBRE_VC
        ,FECHA_NACIMIENTO_DA
        ,USUARIO_CREACION_VC
    )
    VALUES (
         @CODIGO_CLIENTE_VC
        ,@NOMBRE_VC
        ,@FECHA_NACIMIENTO_DA
        ,@USUARIO_CREACION_VC
    );

    SET @CLIENTE_ID_IN = SCOPE_IDENTITY();
END
GO

GRANT EXECUTE ON [api].[CLIENTE_Add] TO [UsrBD_APIBCP];
GO
```

Errores típicos a corregir: `CREATE TABLE Clientes` (plural/minúsculas), `Nombre VARCHAR(100)`
(sin sufijo), `PRIMARY KEY` sin nombre, `CREATE PROC sp_InsertCliente`, `SELECT @@IDENTITY`,
uso de `CURSOR` para recorrer filas.
