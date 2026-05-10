# Arquitectura — Base Project API

## Descripción

Plantilla base reutilizable para microservicios en .NET 10. Cada microservicio nuevo se crea clonando este repositorio y añadiendo módulos dentro de `Core/`.

---

## Estructura de carpetas

```
base-project-api/
├── Controllers/
│   └── v1/
│       └── WeatherForecastController.cs
├── Core/
│   └── {Modulo}/
│       ├── Dtos/
│       │   ├── Request/
│       │   └── Response/
│       ├── Entities/
│       ├── Interfaces/
│       ├── Mappers/
│       ├── Validators/
│       ├── Repositories/
│       └── Services/
├── Docs/
├── Properties/
├── Resources/
│   └── Logs/
├── Shared/
│   ├── Commons/
│   │   └── Exceptions/
│   ├── Constants/
│   ├── Extensions/
│   ├── Helpers/
│   └── Middlewares/
├── Program.cs
├── appsettings.json
└── appsettings.Development.json
```

---

## Capas y responsabilidades

| Capa | Carpeta | Responsabilidad |
|---|---|---|
| Controller | `Controllers/v{n}/` | Recibe el request HTTP, delega a una sola capa, retorna `ApiResponse<T>` |
| Service | `Core/{Modulo}/Services/` | Lógica de negocio, llamadas a APIs externas via `HttpExecutor` |
| Repository | `Core/{Modulo}/Repositories/` | Acceso a base de datos via `DbExecutor` |
| Shared | `Shared/` | Infraestructura transversal: helpers, middleware, extensiones |

---

## Regla de dependencias

```
Controller → Service   (lógica de negocio)
           → Repository (solo acceso a datos)

Service ↔ Repository   (pueden llamarse mutuamente)
```

**Un Controller nunca inyecta tanto Service como Repository al mismo tiempo.** Si un caso de uso necesita los dos, la orquestación va en el Service.

---

## Anatomía de un módulo

Cada módulo dentro de `Core/` es autónomo. Al extraerlo como microservicio solo se lleva su carpeta.

```
Core/Users/
├── Dtos/
│   ├── Request/
│   │   └── CreateUserRequest.cs
│   └── Response/
│       └── UserResponse.cs
├── Entities/
│   └── UserEntity.cs
├── Interfaces/          ← solo si hay múltiples implementaciones o tests de aislamiento
│   └── IUserRepository.cs
├── Mappers/
│   └── UserMapper.cs    ← hereda de AutoMapper.Profile
├── Validators/
│   └── CreateUserRequestValidator.cs   ← hereda de AbstractValidator<T>
├── Repositories/
│   └── UserRepository.cs   ← inyecta DbExecutor
└── Services/
    └── UserService.cs      ← inyecta HttpExecutor o Repository
```

---

## Flujo de una petición

```
HTTP Request
    │
    ▼
Controller (valida versión, autorización)
    │
    ▼
Service / Repository
    │
    ├── Service → HttpExecutor → API externa
    └── Repository → DbExecutor → PostgreSQL (stored procedures)
    │
    ▼
ApiResponse<T>
    │
    ▼
HTTP Response
```

### Flujo de error

```
Service / Repository lanza excepción personalizada (NotFoundException, etc.)
    │
    ▼
GlobalExceptionHandlingMiddleware
    │
    ├── BaseApiException → HTTP status del campo StatusCode + ApiResponse.Fail(publicMessage)
    └── Exception        → HTTP 500 + ApiResponse.Fail("Error interno")
    │
    ▼
HTTP Response con ApiResponse<object> { success: false }
```

---

## Escalabilidad hacia microservicios

Cada módulo de `Core/` puede convertirse en un microservicio independiente sin reestructuración:

```
Core/Users   → user-api
Core/Auth    → auth-api
Core/Orders  → orders-api
```

---

## Stack tecnológico

| Componente | Librería | Versión |
|---|---|---|
| Framework | ASP.NET Core | .NET 10 |
| ORM | Dapper + Npgsql | 2.1.72 / 10.0.2 |
| Versionado de API | Asp.Versioning | 10.0.0 |
| Documentación | Swashbuckle (Swagger) | 10.1.7 |
| Validación | FluentValidation | 12.1.1 |
| Mapeo | AutoMapper | 16.1.1 |
| Autenticación | JWT Bearer | 10.0.7 |
| Health Checks | AspNetCore.HealthChecks.NpgSql | 9.0.0 |
| Logging | Serilog | 10.0.0 |

---

## Convenciones

- Los Controllers no contienen lógica de negocio.
- Las Interfaces van en `Core/{Modulo}/Interfaces/` solo cuando aportan valor real (testing o múltiples implementaciones). No se crean interfaces por convención.
- `Shared/` no contiene lógica de dominio.
- Todos los endpoints retornan `ApiResponse<T>`.
- Todo acceso a base de datos se hace exclusivamente via stored procedures a través de `DbExecutor`.
