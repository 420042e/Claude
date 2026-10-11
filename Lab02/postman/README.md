# ApiTarjetas — Consumo con Postman y Swagger

## Swagger (Development)

Con la API corriendo desde Visual Studio (perfil `ApiTarjetas.Api`):

| | URL |
|---|---|
| Swagger UI | https://localhost:49857/swagger |
| Especificación OpenAPI | https://localhost:49857/swagger/v1/swagger.json |

Swagger solo se habilita con `ASPNETCORE_ENVIRONMENT=Development`.
La especificación también se puede importar en Postman: **Import → Link** → pegar la URL de `swagger.json`.

## Postman

### 1. Importar
En Postman: **Import** → arrastra los dos archivos de esta carpeta:
- `ApiTarjetas.postman_collection.json` (colección)
- `ApiTarjetas-Local.postman_environment.json` (entorno con `baseUrl`)

Selecciona el entorno **ApiTarjetas - Local (Visual Studio)** en la esquina superior derecha.

### 2. Certificado HTTPS de desarrollo
La API usa el certificado de desarrollo de .NET. Si Postman muestra *SSL Error: self signed certificate*:
**Settings → General → SSL certificate verification → OFF**.
Alternativa: cambia `baseUrl` a `http://localhost:49858`.

### 3. Ejecutar
Ejecuta las peticiones en orden (o **Run collection**). El POST guarda el id creado en la variable
`tarjetaId`, que usan las siguientes peticiones.

## Endpoints

Base: `{{baseUrl}}/api/tarjetas`

| # | Método | Ruta | SP / acceso | Respuesta |
|---|---|---|---|---|
| 1 | POST | `/api/tarjetas` | `api.TARJETA_Add` | `201` + `{ "id": n }` |
| 2 | GET | `/api/tarjetas/{id}` | EF Core (lectura) | `200` tarjeta · `404` si no existe |
| 3 | GET | `/api/tarjetas` | EF Core (lectura) | `200` lista de tarjetas activas |
| 4 | PUT | `/api/tarjetas/{id}` | `api.TARJETA_Update` | `204` |
| 5 | DELETE | `/api/tarjetas/{id}` | `api.TARJETA_Delete` (baja lógica) | `204` |

### POST — Registrar tarjeta
```json
{
  "numeroTarjeta": "4111111111111111",
  "nombreTitular": "JUAN PEREZ",
  "tipoTarjeta": "CREDITO",
  "limiteCredito": 5000,
  "fechaVencimiento": "2029-12-31",
  "usuarioCreacion": "lab"
}
```
- `numeroTarjeta`: exactamente 16 dígitos y único (`UQ_TARJETA_NUMERO_TARJETA_VC`). La colección genera uno nuevo en cada ejecución.
- `limiteCredito`: no negativo.

### GET — Respuesta de ejemplo
```json
{
  "tarjetaId": 1,
  "numeroTarjeta": "4111111111111111",
  "nombreTitular": "JUAN PEREZ",
  "tipoTarjeta": "CREDITO",
  "limiteCredito": 5000.00,
  "saldo": 0.00,
  "fechaVencimiento": "2029-12-31T00:00:00",
  "estado": true,
  "usuarioCreacion": "lab",
  "fechaCreacion": "2026-09-22T15:48:33.99"
}
```

### PUT — Actualizar tarjeta
Solo se toman `nombreTitular` y `limiteCredito`; el id va en la URL.
```json
{ "nombreTitular": "JUAN PEREZ LOPEZ", "limiteCredito": 7500 }
```

### DELETE — Baja lógica
Pone `ESTADO_BT = 0`. La tarjeta desaparece del listado, pero `GET /api/tarjetas/{id}` la sigue devolviendo con `"estado": false`.

## Errores conocidos

| Caso | Respuesta actual |
|---|---|
| Número de tarjeta inválido o límite negativo | `500` (la `ArgumentException` no se traduce a `400`) |
| Número de tarjeta duplicado | `500` (violación de `UQ_TARJETA_NUMERO_TARJETA_VC`) |
| SQL Server no accesible | `500` con `SqlException` |
