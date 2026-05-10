# Instrucciones — Base Project API

Documentación de uso de todas las funcionalidades del proyecto base.

---

## Tabla de contenido

1. [DbExecutor — Acceso a base de datos](#1-dbexecutor--acceso-a-base-de-datos)
2. [HttpExecutor — Consumo de APIs externas](#2-httpexecutor--consumo-de-apis-externas)
3. [ApiResponse\<T\> — Respuesta estándar](#3-apiresponset--respuesta-estándar)
4. [Excepciones personalizadas](#4-excepciones-personalizadas)
5. [Paginación](#5-paginación)
6. [Versionado de API y Swagger](#6-versionado-de-api-y-swagger)
7. [Autenticación JWT](#7-autenticación-jwt)
8. [Validación con FluentValidation](#8-validación-con-fluentvalidation)
9. [Mapeo con AutoMapper](#9-mapeo-con-automapper)
10. [Logging con Serilog](#10-logging-con-serilog)
11. [Health Checks](#11-health-checks)
12. [Configuración (appsettings)](#12-configuración-appsettings)

---

## 1. DbExecutor — Acceso a base de datos

**Ubicación:** `Shared/Helpers/DbExecutor.cs`  
**Inyección:** directa en el Repository (sin interfaz)

```csharp
public class UserRepository(DbExecutor db) { }
```

Todos los métodos ejecutan **stored procedures** en PostgreSQL. Nunca consultas SQL en texto plano.

---

### ExecuteAsync

Ejecuta un SP sin retorno de datos. Retorna el número de filas afectadas.  
Usar para INSERT, UPDATE, DELETE.

```csharp
await _db.ExecuteAsync("sp_delete_user", new { p_id = id });
```

---

### QueryAsync\<T\>

Ejecuta un SP y retorna una colección de registros.

```csharp
var users = await _db.QueryAsync<UserResponse>("sp_get_users");

var filtered = await _db.QueryAsync<UserResponse>("sp_get_users_by_role", new { p_role = "admin" });
```

---

### QuerySingleAsync\<T\>

Ejecuta un SP y retorna un único registro. Retorna `null` si no existe.

```csharp
var user = await _db.QuerySingleAsync<UserResponse>("sp_get_user_by_id", new { p_id = id });

if (user is null)
    throw new NotFoundException("El usuario no fue encontrado.");
```

---

### ExecuteScalarAsync\<T\>

Ejecuta un SP y retorna un valor escalar (COUNT, SUM, ID generado, etc.).

```csharp
var total = await _db.ExecuteScalarAsync<int>("sp_count_active_users");

var newId = await _db.ExecuteScalarAsync<int>("sp_insert_user_return_id", new { p_name = "Juan" });
```

---

### QueryMultipleAsync\<T1, T2\>

Ejecuta un SP que hace dos SELECT y retorna ambos conjuntos de resultados.

```csharp
var (users, roles) = await _db.QueryMultipleAsync<UserResponse, RoleResponse>("sp_get_users_with_roles");
```

---

### QueryWithOutputAsync\<T\>

Ejecuta un SP que retorna un listado de registros **y** parámetros de salida al mismo tiempo.  
El caso principal es la paginación: el SP devuelve los registros y el total de registros en un solo llamado.

Los parámetros de salida se leen del mismo `DynamicParameters` después de la ejecución.

```csharp
var parameters = new DynamicParameters();
parameters.Add("p_page", request.Page);
parameters.Add("p_page_size", request.PageSize);
parameters.Add("p_name", request.Name);
parameters.Add("p_total_records", dbType: DbType.Int32, direction: ParameterDirection.Output);

var records = await _db.QueryWithOutputAsync<UserResponse>("sp_get_users_paged", parameters);
int total = parameters.Get<int>("p_total_records");

return PaginatedResponse<UserResponse>.Create(records, request.Page, request.PageSize, total);
```

---

### ExecuteWithOutputAsync

Ejecuta un SP con parámetros de salida (INOUT). No retorna datos, solo parámetros de salida.  
Usar cuando el SP genera un valor (ID, código) que hay que recuperar.

```csharp
var parameters = new DynamicParameters();
parameters.Add("p_name", request.Name);
parameters.Add("p_email", request.Email);
parameters.Add("p_id", dbType: DbType.Int32, direction: ParameterDirection.Output);

await _db.ExecuteWithOutputAsync("sp_create_user", parameters);

int generatedId = parameters.Get<int>("p_id");
```

---

### ExecuteInTransactionAsync

Ejecuta múltiples operaciones dentro de una transacción atómica. Si alguna falla, todas se revierten.

```csharp
await _db.ExecuteInTransactionAsync(async (conn, trans) =>
{
    await conn.ExecuteAsync("sp_insert_order", orderParams, trans,
        commandType: CommandType.StoredProcedure);

    await conn.ExecuteAsync("sp_insert_order_detail", detailParams, trans,
        commandType: CommandType.StoredProcedure);
});
```

---

## 2. HttpExecutor — Consumo de APIs externas

**Ubicación:** `Shared/Helpers/HttpExecutor.cs`  
**Inyección:** directa en el Service (sin interfaz)

```csharp
public class NotificationService(HttpExecutor http) { }
```

Cada método tiene dos variantes:
- `TResponse` — deserializa la respuesta JSON al tipo indicado
- `bool` — solo indica si la petición fue exitosa (2xx)

Todos los métodos aceptan opcionalmente:
- `queryParams` — `Dictionary<string, string>` que se construye y agrega a la URL
- `headers` — `Dictionary<string, string>` para cabeceras adicionales (`Authorization`, `x-api-key`, etc.)

---

### GetAsync\<TResponse\>

```csharp
var product = await _http.GetAsync<ProductResponse>(
    "https://api.example.com/products/1");

var products = await _http.GetAsync<List<ProductResponse>>(
    "https://api.example.com/products",
    queryParams: new Dictionary<string, string>
    {
        { "page", "1" },
        { "size", "10" }
    },
    headers: new Dictionary<string, string>
    {
        { "Authorization", "Bearer token123" }
    });
```

---

### PostAsync\<TResponse\> / PostAsync

```csharp
// Con respuesta deserializada
var order = await _http.PostAsync<OrderResponse>(
    "https://api.example.com/orders",
    body: new { ProductId = 1, Quantity = 3 });

// Solo verificar éxito
bool sent = await _http.PostAsync(
    "https://api.notifications.com/send",
    body: new { To = "user@example.com", Message = "Bienvenido" });
```

---

### PutAsync\<TResponse\> / PutAsync

```csharp
// Con respuesta deserializada
var updated = await _http.PutAsync<UserResponse>(
    "https://api.example.com/users/1",
    body: new { Name = "Juan Actualizado" });

// Solo verificar éxito
bool ok = await _http.PutAsync(
    "https://api.example.com/users/1/deactivate",
    body: new { });
```

---

### DeleteAsync\<TResponse\> / DeleteAsync

```csharp
// Con respuesta deserializada
var result = await _http.DeleteAsync<DeleteResponse>("https://api.example.com/users/1");

// Solo verificar éxito
bool deleted = await _http.DeleteAsync("https://api.example.com/users/1");
```

---

## 3. ApiResponse\<T\> — Respuesta estándar

**Ubicación:** `Shared/Commons/ApiResponse.cs`

Todo endpoint debe retornar `ApiResponse<T>`. Garantiza un contrato JSON consistente.

### Estructura

| Propiedad | Tipo | Descripción |
|---|---|---|
| `success` | `bool` | `true` si la petición fue exitosa |
| `message` | `string` | Mensaje visible al cliente |
| `data` | `T?` | Resultado de la operación |
| `errorDetail` | `string?` | Detalle interno del error (para logs/debug) |

### Uso

```csharp
// Éxito con datos
return ApiResponse<List<UserResponse>>.Ok(users);
return ApiResponse<List<UserResponse>>.Ok(users, "Usuarios obtenidos.");

// Éxito sin datos
return ApiResponse<object>.Ok("Usuario creado correctamente.");

// Error
return ApiResponse<object>.Fail("No se pudo procesar la solicitud.", ex.Message);
```

### Ejemplo JSON

```json
{
  "success": true,
  "message": "Solicitud procesada correctamente.",
  "data": [{ "id": 1, "name": "Juan" }],
  "errorDetail": null
}
```

---

## 4. Excepciones personalizadas

**Ubicación:** `Shared/Commons/Exceptions/`

Se lanzan desde cualquier capa (Service, Repository). El middleware las captura y retorna el HTTP status correcto automáticamente, sin código adicional en los Controllers.

| Excepción | HTTP | Cuándo usarla |
|---|---|---|
| `NotFoundException` | 404 | El recurso no existe |
| `BadRequestException` | 400 | Datos de entrada inválidos |
| `UnauthorizedException` | 401 | No autenticado o token inválido |
| `ForbiddenException` | 403 | Autenticado pero sin permisos |
| `ConflictException` | 409 | Recurso duplicado o conflicto de estado |

### Uso

```csharp
// Recurso no encontrado
var user = await _db.QuerySingleAsync<UserResponse>("sp_get_user", new { p_id = id });
if (user is null)
    throw new NotFoundException("El usuario no fue encontrado.", $"id={id} no existe en BD");

// Datos inválidos (sin FluentValidation)
if (string.IsNullOrEmpty(request.Email))
    throw new BadRequestException("El correo es requerido.");

// Conflicto de duplicado
if (await EmailExistsAsync(request.Email))
    throw new ConflictException("El correo ya está registrado.");

// Sin permisos
if (!user.IsAdmin)
    throw new ForbiddenException("No tiene permisos para esta operación.");
```

El constructor acepta dos parámetros:
1. `publicMessage` — se retorna al cliente
2. `internalDetail` (opcional) — se registra en los logs, no se expone al cliente

### Respuesta JSON resultante

```json
{
  "success": false,
  "message": "El usuario no fue encontrado.",
  "data": null,
  "errorDetail": "id=99 no existe en BD"
}
```

---

## 5. Paginación

**Ubicación:** `Shared/Commons/`

### PaginationRequest

Clase base para requests paginados. Se puede extender con filtros propios.

```csharp
// Uso directo
[HttpGet]
public async Task<ApiResponse<PaginatedResponse<UserResponse>>> GetAll(
    [FromQuery] PaginationRequest request) { }

// Extendido con filtros
public class GetUsersRequest : PaginationRequest
{
    public string? Name { get; set; }
    public bool? IsActive { get; set; }
}
```

| Propiedad | Default | Restricción |
|---|---|---|
| `Page` | 1 | Mínimo 1 |
| `PageSize` | 10 | Mínimo 1, máximo 100 |

### PaginatedResponse\<T\>

| Propiedad | Descripción |
|---|---|
| `Data` | Lista de registros de la página actual |
| `Page` | Página actual |
| `PageSize` | Registros por página |
| `TotalRecords` | Total de registros (viene del SP) |
| `TotalPages` | Calculado automáticamente |
| `HasPreviousPage` | `true` si existe página anterior |
| `HasNextPage` | `true` si existe página siguiente |

### Flujo completo

```csharp
// Repository
public async Task<PaginatedResponse<UserResponse>> GetPagedAsync(GetUsersRequest request)
{
    var parameters = new DynamicParameters();
    parameters.Add("p_page", request.Page);
    parameters.Add("p_page_size", request.PageSize);
    parameters.Add("p_name", request.Name);
    parameters.Add("p_total_records", dbType: DbType.Int32, direction: ParameterDirection.Output);

    var records = await _db.QueryWithOutputAsync<UserResponse>("sp_get_users_paged", parameters);
    var total = parameters.Get<int>("p_total_records");

    return PaginatedResponse<UserResponse>.Create(records, request.Page, request.PageSize, total);
}

// Controller
[HttpGet]
public async Task<ApiResponse<PaginatedResponse<UserResponse>>> GetAll([FromQuery] GetUsersRequest request)
{
    var result = await _repository.GetPagedAsync(request);
    return ApiResponse<PaginatedResponse<UserResponse>>.Ok(result);
}
```

### JSON de respuesta

```json
{
  "success": true,
  "message": "Solicitud procesada correctamente.",
  "data": {
    "page": 1,
    "pageSize": 10,
    "totalRecords": 45,
    "totalPages": 5,
    "hasPreviousPage": false,
    "hasNextPage": true,
    "data": [{ "id": 1, "name": "Juan" }]
  }
}
```

---

## 6. Versionado de API y Swagger

El versionado es automático. Swagger detecta las versiones sin tocar `Program.cs`.

### Declarar un Controller versionado

```csharp
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class UsersController : ControllerBase { }
```

URL resultante: `GET /api/v1/Users`

### Agregar una nueva versión

Solo se decora el controller con la nueva versión:

```csharp
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class UsersController : ControllerBase
{
    [HttpGet]
    [MapToApiVersion("1.0")]
    public ApiResponse<List<UserV1Response>> GetV1() { }

    [HttpGet]
    [MapToApiVersion("2.0")]
    public ApiResponse<List<UserV2Response>> GetV2() { }
}
```

Swagger registra automáticamente la nueva versión y genera su documentación separada.

### Acceso a Swagger

Solo disponible en entorno Development: `http://localhost:{puerto}/swagger`

El `ProjectName` que aparece como título en Swagger se configura en `appsettings.json`.

---

## 7. Autenticación JWT

**Ubicación:** `Shared/Extensions/JwtExtensions.cs`

El middleware está activo pero ningún endpoint lo requiere por defecto. Se aplica a nivel de controller o endpoint.

### Configuración en appsettings

```json
"Jwt": {
  "Key": "your-super-secret-key-minimum-32-chars!!",
  "Issuer": "base-project-api",
  "Audience": "base-project-api-clients",
  "ExpirationMinutes": 60
}
```

| Clave | Descripción |
|---|---|
| `Key` | Clave secreta de firma (mínimo 32 caracteres) |
| `Issuer` | Emisor del token |
| `Audience` | Audiencia válida del token |
| `ExpirationMinutes` | Tiempo de expiración |

### Proteger un controller completo

```csharp
[Authorize]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class UsersController : ControllerBase { }
```

### Proteger un endpoint específico

```csharp
[HttpDelete("{id}")]
[Authorize]
public async Task<ApiResponse<object>> Delete(int id) { }
```

### Permitir acceso anónimo dentro de un controller protegido

```csharp
[HttpPost("login")]
[AllowAnonymous]
public async Task<ApiResponse<TokenResponse>> Login(LoginRequest request) { }
```

---

## 8. Validación con FluentValidation

**Ubicación:** `Core/{Modulo}/Validators/`

Los validators se registran automáticamente al iniciar la aplicación. No hay que declararlos en `Program.cs`.

### Crear un validator

```csharp
public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress().WithMessage("El correo no tiene formato válido.");

        RuleFor(x => x.Age)
            .GreaterThan(0).WithMessage("La edad debe ser mayor a 0.");
    }
}
```

### Usar en Service

```csharp
public class UserService(IValidator<CreateUserRequest> validator, UserRepository repository)
{
    public async Task CreateAsync(CreateUserRequest request)
    {
        var result = await validator.ValidateAsync(request);
        if (!result.IsValid)
            throw new BadRequestException(result.Errors.First().ErrorMessage);

        await repository.CreateAsync(request);
    }
}
```

---

## 9. Mapeo con AutoMapper

**Ubicación:** `Core/{Modulo}/Mappers/`

Los perfiles de mapeo se registran automáticamente al iniciar la aplicación. No hay que declararlos en `Program.cs`.

### Crear un perfil

```csharp
public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<UserEntity, UserResponse>();
        CreateMap<CreateUserRequest, UserEntity>();
    }
}
```

### Usar en Service

```csharp
public class UserService(IMapper mapper, UserRepository repository)
{
    public async Task<UserResponse> GetAsync(int id)
    {
        var entity = await repository.GetByIdAsync(id);
        if (entity is null)
            throw new NotFoundException("El usuario no fue encontrado.");

        return mapper.Map<UserResponse>(entity);
    }

    public async Task<List<UserResponse>> GetAllAsync()
    {
        var entities = await repository.GetAllAsync();
        return mapper.Map<List<UserResponse>>(entities);
    }
}
```

---

## 10. Logging con Serilog

**Ubicación:** `Shared/Extensions/SerilogExtensions.cs`

Escribe logs en consola y en archivos rotativos diarios con retención de 3 días.

### Archivos generados

```
Resources/
└── Logs/
    ├── log-20260430.txt
    ├── log-20260429.txt
    └── log-20260428.txt
```

Los archivos anteriores a 3 días se eliminan automáticamente. La carpeta se crea sola al iniciar.

### Niveles configurados

| Origen | Nivel |
|---|---|
| Aplicación | `Information` |
| Microsoft / ASP.NET Core | `Warning` (reduce ruido del framework) |

### Uso en cualquier clase

```csharp
public class UserService(ILogger<UserService> logger)
{
    public async Task<UserResponse> GetAsync(int id)
    {
        logger.LogInformation("Buscando usuario con id={Id}", id);

        var user = await _repository.GetByIdAsync(id);
        if (user is null)
        {
            logger.LogWarning("Usuario no encontrado: id={Id}", id);
            throw new NotFoundException("El usuario no fue encontrado.");
        }

        return user;
    }
}
```

El middleware de excepciones ya registra automáticamente:
- `LogWarning` para `BaseApiException` (errores controlados)
- `LogError` para `Exception` (errores no controlados)

---

## 11. Health Checks

**Ubicación:** `Shared/Extensions/HealthCheckExtensions.cs`

| Endpoint | Descripción |
|---|---|
| `GET /health` | Liveness — estado de la aplicación sin checks de infraestructura |
| `GET /health/db` | Estado exclusivo de PostgreSQL |

### Respuesta JSON

```json
{
  "status": "Healthy",
  "duration": "12.5ms",
  "checks": [
    {
      "name": "postgresql",
      "status": "Healthy",
      "duration": "11.2ms",
      "description": null
    }
  ]
}
```

Los posibles valores de `status`: `Healthy`, `Degraded`, `Unhealthy`.

### Agregar un nuevo check

En `Shared/Extensions/HealthCheckExtensions.cs`:

```csharp
services.AddHealthChecks()
    .AddNpgSql(connectionString, name: "postgresql", tags: ["db"])
    .AddRedis(redisConnection, name: "redis", tags: ["cache"]);
```

---

## 12. Configuración (appsettings)

### appsettings.json — valores base

```json
{
  "ProjectName": "Base Project API",
  "SwaggerEndPoint": "",
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=mydb;Username=postgres;Password=yourpassword"
  },
  "Jwt": {
    "Key": "your-super-secret-key-minimum-32-chars!!",
    "Issuer": "base-project-api",
    "Audience": "base-project-api-clients",
    "ExpirationMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### appsettings.Development.json — sobrescrituras locales

```json
{
  "ProjectName": "Base Project API (Dev)",
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=mydb_dev;Username=postgres;Password=yourpassword"
  },
  "Jwt": {
    "ExpirationMinutes": 480
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  }
}
```

| Clave | Descripción |
|---|---|
| `ProjectName` | Título que aparece en Swagger |
| `SwaggerEndPoint` | Prefijo de ruta (vacío en local, útil detrás de un API Gateway) |
| `ConnectionStrings:DefaultConnection` | Cadena de conexión a PostgreSQL |
| `Jwt:Key` | Clave secreta para firmar tokens (mínimo 32 caracteres) |
| `Jwt:Issuer` | Emisor del token JWT |
| `Jwt:Audience` | Audiencia válida del token JWT |
| `Jwt:ExpirationMinutes` | Duración del token en minutos |
