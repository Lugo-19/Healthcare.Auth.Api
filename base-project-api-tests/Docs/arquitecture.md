# Arquitectura — Base Project API Tests

## Descripción

Proyecto de tests unitarios que sigue la misma estructura modular que `base-project-api`. Cada módulo de `Core/` tiene su carpeta espejo con tests para las tres capas: Controller, Service y Repository.

---

## Estructura de carpetas

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

Cuando se agrega un nuevo módulo en `base-project-api/Core/{Modulo}/`, se crea la carpeta `Core/{Modulo}/` en este proyecto con los tres archivos de tests correspondientes.

---

## Capas testeadas

| Capa | Archivo | Dependencias mockeadas |
|---|---|---|
| Controller | `{Modulo}ControllerTests.cs` | `Mock<I{Modulo}Service>` |
| Service | `{Modulo}ServiceTests.cs` | `Mock<I{Modulo}Repository>`, `Mock<ILogger<T>>` |
| Repository | `{Modulo}RepositoryTests.cs` | `AppDbContext` con InMemory database |

---

## Regla de dependencias en tests

```
ControllerTests   → Mock<IService>
ServiceTests      → Mock<IRepository>
RepositoryTests   → InMemoryDatabase (sin mocks de DbContext)
```

**Los RepositoryTests nunca usan `Mock<IRepository>`** — se testea la implementación real contra una base de datos en memoria o integración. Mockear el repositorio en su propio test no tiene valor.

---

## Convenciones de nombres

### Archivos

```
{Modulo}ControllerTests.cs
{Modulo}ServiceTests.cs
{Modulo}RepositoryTests.cs
```

### Métodos de test

```
{Metodo}_{Condicion}_{ResultadoEsperado}
```

Ejemplos:

```
GetAllAsync_WhenRepositoryHasData_ShouldReturnAllForecasts
GetAllAsync_WhenRepositoryIsEmpty_ShouldReturnEmptyList
GetById_WhenIdDoesNotExist_ShouldThrowNotFoundException
Create_WhenEmailAlreadyExists_ShouldThrowConflictException
```

### Variables internas

| Variable | Descripción |
|---|---|
| `_sut` | System Under Test — la clase siendo testeada |
| `_*Mock` | Instancia de `Mock<T>` (ej. `_repositoryMock`) |

---

## Anatomía de un módulo de tests

```
Core/Users/
├── UsersControllerTests.cs   ← testea UsersController con Mock<IUsersService>
├── UsersServiceTests.cs      ← testea UsersService con Mock<IUsersRepository>
└── UsersRepositoryTests.cs   ← testea UsersRepository con InMemory DbContext
```

### Patrón de constructor (Service)

```csharp
public class UsersServiceTests
{
    private readonly Mock<IUsersRepository> _repositoryMock;
    private readonly UsersService _sut;

    public UsersServiceTests()
    {
        _repositoryMock = new Mock<IUsersRepository>();
        _sut = new UsersService(_repositoryMock.Object);
    }
}
```

### Patrón de constructor (Controller)

```csharp
public class UsersControllerTests
{
    private readonly Mock<IUsersService> _serviceMock;
    private readonly UsersController _sut;

    public UsersControllerTests()
    {
        _serviceMock = new Mock<IUsersService>();
        _sut = new UsersController(_serviceMock.Object);
    }
}
```

### Patrón de constructor (Repository)

```csharp
public class UsersRepositoryTests
{
    private AppDbContext CreateInMemoryContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
}
```

Se usa `Guid.NewGuid()` como nombre de la base de datos para garantizar aislamiento entre tests.

---

## Tests de ejemplo (Skip)

Los tests marcados con `[Fact(Skip = "...")]` son plantillas. Para activarlos:

1. Implementar la clase bajo prueba (`Service` o `Repository`)
2. Descomentar la instanciación del `_sut` en el constructor
3. Descomentar el `// Act` y el `// Assert`
4. Eliminar el atributo `Skip`

---

## Stack tecnológico

| Componente | Librería | Versión |
|---|---|---|
| Framework de tests | xUnit | 2.9.3 |
| Runner Visual Studio | xunit.runner.visualstudio | 3.1.4 |
| Mocking | Moq | 4.20.72 |
| Assertions | FluentAssertions | 8.9.0 |
| Cobertura | coverlet.collector | 6.0.4 |
| SDK de tests | Microsoft.NET.Test.Sdk | 17.14.1 |
