# Base Project

Plantilla base reutilizable para microservicios en **.NET 10**. Incluye la API y su proyecto de tests, listos para clonarse como punto de partida de cada nuevo servicio.

---

## Proyectos

| Proyecto | Descripción |
|---|---|
| [`base-project-api`](base-project-api/README.md) | API REST con versionado, JWT, Swagger, Serilog, Health Checks y arquitectura modular por dominios |
| [`base-project-api-tests`](base-project-api-tests/README.md) | Tests unitarios con xUnit, Moq y FluentAssertions. Cubre Controllers, Services y Repositories |

---

## Stack general

| | |
|---|---|
| Framework | ASP.NET Core .NET 10 |
| Base de datos | PostgreSQL + Dapper (stored procedures) |
| Autenticación | JWT Bearer |
| Validación | FluentValidation |
| Mapeo | AutoMapper |
| Documentación | Swagger / OpenAPI |
| Logging | Serilog |
| Tests | xUnit + Moq + FluentAssertions |

---

## Inicio rápido

### Requisitos

- .NET 10 SDK
- PostgreSQL (opcional — la API levanta sin BD; solo `/health/db` reportará `Unhealthy`)

### 1. Configurar la conexión

Editar `base-project-api/appsettings.Development.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=mydb_dev;Username=postgres;Password=yourpassword"
}
```

### 2. Ejecutar la API

```bash
cd base-project-api
dotnet run
```

Swagger disponible en `http://localhost:{puerto}/swagger` (solo en Development).

### 3. Ejecutar los tests

```bash
cd base-project-api-tests
dotnet test
```

---

## Estructura del repositorio

```
base-project/
├── base-project-api/
│   ├── Controllers/v1/
│   ├── Core/{Modulo}/
│   ├── Shared/
│   ├── Docs/
│   └── Program.cs
├── base-project-api-tests/
│   ├── Core/{Modulo}/
│   └── Docs/
├── Base-Project.slnx
└── .gitignore
```

---

## Documentación

| Documento | Contenido |
|---|---|
| [base-project-api/README.md](base-project-api/README.md) | Stack, inicio rápido y endpoints de la API |
| [base-project-api/Docs/arquitecture.md](base-project-api/Docs/arquitecture.md) | Estructura, capas, flujo de peticiones y escalabilidad a microservicios |
| [base-project-api/Docs/instrucciones.md](base-project-api/Docs/instrucciones.md) | DbExecutor, HttpExecutor, ApiResponse, excepciones, paginación, JWT, FluentValidation, AutoMapper, Serilog, Health Checks |
| [base-project-api-tests/README.md](base-project-api-tests/README.md) | Stack de tests, comandos y estado de los tests de ejemplo |
| [base-project-api-tests/Docs/arquitecture.md](base-project-api-tests/Docs/arquitecture.md) | Estructura, capas testeadas, convenciones de nombres |
| [base-project-api-tests/Docs/instrucciones.md](base-project-api-tests/Docs/instrucciones.md) | xUnit, Moq, FluentAssertions y patrones por capa |
