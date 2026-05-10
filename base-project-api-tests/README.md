# Base Project API — Tests

Proyecto de pruebas unitarias para **Base Project API**. Cubre las tres capas de la arquitectura: Controllers, Services y Repositories.

---

## Stack

| | |
|---|---|
| Framework de tests | xUnit |
| Mocking | Moq |
| Assertions | FluentAssertions |
| Cobertura | coverlet.collector |
| Target | .NET 10 |

---

## Inicio rápido

### Requisitos

- .NET 10 SDK
- Proyecto `base-project-api` compilable (referenciado directamente)

### Ejecutar todos los tests

```bash
dotnet test
```

### Ejecutar con reporte de cobertura

```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Ejecutar solo una categoría

```bash
dotnet test --filter "FullyQualifiedName~ControllerTests"
dotnet test --filter "FullyQualifiedName~ServiceTests"
dotnet test --filter "FullyQualifiedName~RepositoryTests"
```

---

## Estructura

```
base-project-api-tests/
├── Core/
│   └── {Modulo}/
│       ├── {Modulo}ControllerTests.cs
│       ├── {Modulo}ServiceTests.cs
│       └── {Modulo}RepositoryTests.cs
├── Docs/
├── GlobalUsings.cs
└── Base.Project.Api.Tests.csproj
```

Cada módulo del proyecto API tiene su carpeta espejo dentro de `Core/`.

---

## Tests de ejemplo

Los tests de ejemplo están marcados con `[Fact(Skip = "...")]` para que no fallen hasta que se implementen las clases reales.

| Archivo | Estado |
|---|---|
| `Core/WeatherForecast/WeatherForecastControllerTests.cs` | Activos — 4 tests pasan |
| `Core/WeatherForecast/WeatherForecastServiceTests.cs` | Ejemplo — requiere implementar `WeatherForecastService` |
| `Core/WeatherForecast/WeatherForecastRepositoryTests.cs` | Ejemplo — requiere implementar `WeatherForecastRepository` con DbContext |

---

## Documentación

| Documento | Contenido |
|---|---|
| [Docs/arquitecture.md](Docs/arquitecture.md) | Estructura de carpetas, capas testeadas, convenciones de nombres, qué se mockea en cada capa |
| [Docs/instrucciones.md](Docs/instrucciones.md) | Uso de xUnit, Moq, FluentAssertions, patrones por capa, cobertura |
