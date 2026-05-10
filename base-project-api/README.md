# Base Project API

Plantilla base reutilizable para microservicios en **.NET 10**. Arquitectura modular orientada a dominios, lista para clonarse como punto de partida de cada microservicio.

---

## Stack

| | |
|---|---|
| Framework | ASP.NET Core .NET 10 |
| Base de datos | PostgreSQL + Dapper (stored procedures, sin EF Core) |
| Autenticación | JWT Bearer |
| Validación | FluentValidation (auto-discovery) |
| Mapeo | AutoMapper (auto-discovery) |
| Documentación | Swagger / OpenAPI (por versión, dinámico) |
| Logging | Serilog — consola + archivos rotativos 3 días |
| Health Checks | `/health` y `/health/db` |

---

## Inicio rápido

### Requisitos

- .NET 10 SDK
- PostgreSQL

### Configuración

1. Ajustar la cadena de conexión en `appsettings.Development.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=mydb_dev;Username=postgres;Password=yourpassword"
}
```

2. Ajustar las claves JWT si se van a usar endpoints protegidos:

```json
"Jwt": {
  "Key": "your-super-secret-key-minimum-32-chars!!"
}
```

### Ejecutar

```bash
dotnet run
```

La aplicación abre automáticamente Swagger en el navegador.

---

## Endpoints base

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/v1/WeatherForecast` | Ejemplo de endpoint versionado |
| GET | `/health` | Estado general de la aplicación |
| GET | `/health/db` | Estado de la conexión a PostgreSQL |

---

## Documentación

| Documento | Contenido |
|---|---|
| [Docs/arquitecture.md](Docs/arquitecture.md) | Estructura de carpetas, capas, reglas de dependencias, flujo de peticiones, escalabilidad a microservicios |
| [Docs/instrucciones.md](Docs/instrucciones.md) | Uso detallado de DbExecutor, HttpExecutor, ApiResponse, excepciones, paginación, JWT, FluentValidation, AutoMapper, Serilog, Health Checks y configuración |

---

## Estructura resumida

```
base-project-api/
├── Controllers/v1/
├── Core/{Modulo}/
│   ├── Dtos/Request/ & Response/
│   ├── Entities/
│   ├── Mappers/
│   ├── Validators/
│   ├── Repositories/
│   └── Services/
├── Shared/
│   ├── Commons/         — ApiResponse<T>, excepciones, paginación
│   ├── Extensions/      — JWT, Swagger, HealthChecks, Serilog, DI
│   ├── Helpers/         — DbExecutor, HttpExecutor
│   └── Middlewares/     — GlobalExceptionHandlingMiddleware
├── Docs/
└── Program.cs
```

**Regla clave:** El Controller delega a **Service** o **Repository**, nunca a ambos directamente.
