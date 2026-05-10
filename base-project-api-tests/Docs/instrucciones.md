# Instrucciones — Base Project API Tests

Documentación de uso de las herramientas y patrones del proyecto de tests.

---

## Tabla de contenido

1. [GlobalUsings — Imports globales](#1-globalusings--imports-globales)
2. [xUnit — Atributos y estructura de tests](#2-xunit--atributos-y-estructura-de-tests)
3. [Moq — Mocking de dependencias](#3-moq--mocking-de-dependencias)
4. [FluentAssertions — Assertions expresivas](#4-fluentassertions--assertions-expresivas)
5. [Patrón: Controller Tests](#5-patrón-controller-tests)
6. [Patrón: Service Tests](#6-patrón-service-tests)
7. [Patrón: Repository Tests](#7-patrón-repository-tests)
8. [Cobertura de código](#8-cobertura-de-código)

---

## 1. GlobalUsings — Imports globales

**Ubicación:** `GlobalUsings.cs`

```csharp
global using Xunit;
global using Moq;
global using FluentAssertions;
global using Microsoft.Extensions.Logging;
```

Estos usings aplican a todos los archivos del proyecto. No hay que repetirlos en cada test.

---

## 2. xUnit — Atributos y estructura de tests

### [Fact] — Test sin parámetros

```csharp
[Fact]
public void Get_ShouldReturnSuccess()
{
    var response = _sut.Get();

    response.Success.Should().BeTrue();
}
```

### [Theory] + [InlineData] — Test parametrizado

```csharp
[Theory]
[InlineData(0)]
[InlineData(-1)]
[InlineData(int.MinValue)]
public async Task GetById_WhenIdIsInvalid_ShouldThrowBadRequestException(int id)
{
    var act = async () => await _sut.GetByIdAsync(id);

    await act.Should().ThrowAsync<BadRequestException>();
}
```

### [Fact(Skip = "...")] — Test de ejemplo desactivado

```csharp
[Fact(Skip = "Example — implement UserService first")]
public async Task GetAllAsync_ShouldReturnAllUsers()
{
    // ...
}
```

Los tests con `Skip` compilan y aparecen en el runner como `Omitidos`. No bloquean el build ni el pipeline.

### Estructura Arrange / Act / Assert

```csharp
[Fact]
public async Task GetAllAsync_WhenRepositoryHasData_ShouldReturnAllUsers()
{
    // Arrange
    var expected = new List<UserResponse> { new() { Id = 1, Name = "Juan" } };
    _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(expected);

    // Act
    var result = await _sut.GetAllAsync();

    // Assert
    result.Should().BeEquivalentTo(expected);
}
```

---

## 3. Moq — Mocking de dependencias

### Crear un mock

```csharp
var repositoryMock = new Mock<IUsersRepository>();
```

### Setup — definir retorno

```csharp
// Retorno de colección
repositoryMock.Setup(r => r.GetAllAsync())
    .ReturnsAsync(new List<UserResponse> { ... });

// Retorno de un elemento
repositoryMock.Setup(r => r.GetByIdAsync(1))
    .ReturnsAsync(new UserResponse { Id = 1 });

// Retorno null (simulando no encontrado)
repositoryMock.Setup(r => r.GetByIdAsync(99))
    .ReturnsAsync((UserResponse?)null);

// Lanzar excepción
repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
    .ThrowsAsync(new NotFoundException("No encontrado."));
```

### It.IsAny\<T\> — cualquier valor del tipo

```csharp
repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
    .ReturnsAsync(new UserResponse());
```

### Verify — verificar que se llamó el método

```csharp
// Verificar que se llamó exactamente una vez
repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);

// Verificar que se llamó con un argumento específico
repositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);

// Verificar que nunca se llamó
repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
```

### Obtener la instancia para inyectar

```csharp
var sut = new UsersService(repositoryMock.Object);
```

---

## 4. FluentAssertions — Assertions expresivas

### Valores simples

```csharp
result.Should().Be(5);
result.Should().BeTrue();
result.Should().BeNull();
result.Should().NotBeNull();
result.Should().BeGreaterThan(0);
result.Should().BeInRange(-20, 55);
```

### Strings

```csharp
result.Should().Be("Healthy");
result.Should().StartWith("Error");
result.Should().Contain("usuario");
result.Should().BeNullOrEmpty();
```

### Colecciones

```csharp
result.Should().NotBeEmpty();
result.Should().BeEmpty();
result.Should().HaveCount(5);
result.Should().Contain(x => x.Id == 1);
result.Should().BeEquivalentTo(expected);
```

### Objetos complejos

```csharp
result.Should().BeEquivalentTo(expected);

result.Should().BeEquivalentTo(expected, options =>
    options.Excluding(x => x.CreatedAt));
```

### Excepciones

```csharp
// Síncrono
var act = () => _sut.Delete(-1);
act.Should().Throw<BadRequestException>()
    .WithMessage("El id no es válido.");

// Asíncrono
var act = async () => await _sut.GetByIdAsync(99);
await act.Should().ThrowAsync<NotFoundException>();
```

### Fechas

```csharp
result.Date.Should().BeAfter(DateOnly.FromDateTime(DateTime.Now));
result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
```

---

## 5. Patrón: Controller Tests

Los tests de Controller validan que el controller delega correctamente al Service y retorna el `ApiResponse<T>` esperado.

**Dependencias mockeadas:** `Mock<I{Modulo}Service>`

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

    [Fact]
    public async Task GetAll_ShouldReturnSuccess()
    {
        // Arrange
        var data = new List<UserResponse> { new() { Id = 1, Name = "Juan" } };
        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(data);

        // Act
        var response = await _sut.GetAll();

        // Assert
        response.Success.Should().BeTrue();
        response.Data.Should().BeEquivalentTo(data);
        _serviceMock.Verify(s => s.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetById_WhenNotFound_ShouldReturnNotFoundResponse()
    {
        // Arrange
        _serviceMock.Setup(s => s.GetByIdAsync(99))
            .ThrowsAsync(new NotFoundException("El usuario no fue encontrado."));

        // Act
        var act = async () => await _sut.GetById(99);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
```

---

## 6. Patrón: Service Tests

Los tests de Service validan la lógica de negocio aislando el acceso a datos.

**Dependencias mockeadas:** `Mock<I{Modulo}Repository>`, `Mock<ILogger<T>>` (cuando aplique)

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

    [Fact]
    public async Task GetAllAsync_WhenRepositoryHasData_ShouldReturnAllUsers()
    {
        // Arrange
        var expected = new List<UserResponse>
        {
            new() { Id = 1, Name = "Juan" },
            new() { Id = 2, Name = "María" }
        };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(expected);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().BeEquivalentTo(expected);
        _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((UserResponse?)null);

        // Act
        var act = async () => await _sut.GetByIdAsync(99);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("El usuario no fue encontrado.");
    }

    [Fact]
    public async Task CreateAsync_WhenEmailAlreadyExists_ShouldThrowConflictException()
    {
        // Arrange
        var request = new CreateUserRequest { Email = "juan@email.com" };
        _repositoryMock.Setup(r => r.ExistsByEmailAsync("juan@email.com"))
            .ReturnsAsync(true);

        // Act
        var act = async () => await _sut.CreateAsync(request);

        // Assert
        await act.Should().ThrowAsync<ConflictException>();
    }
}
```

---

## 7. Patrón: Repository Tests

Los tests de Repository validan el acceso a datos contra una base de datos en memoria. **No se usa `Mock<IRepository>`** — se testea la implementación real.

Requiere agregar el paquete `Microsoft.EntityFrameworkCore.InMemory` al proyecto de tests cuando se utilice EF Core.

```bash
dotnet add package Microsoft.EntityFrameworkCore.InMemory
```

```csharp
public class UsersRepositoryTests
{
    private AppDbContext CreateInMemoryContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task GetAllAsync_WhenDatabaseHasRecords_ShouldReturnAll()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        await context.Users.AddRangeAsync(
            new UserEntity { Name = "Juan", Email = "juan@email.com" },
            new UserEntity { Name = "María", Email = "maria@email.com" }
        );
        await context.SaveChangesAsync();
        var sut = new UsersRepository(context);

        // Act
        var result = await sut.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllAsync_WhenDatabaseIsEmpty_ShouldReturnEmptyList()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var sut = new UsersRepository(context);

        // Act
        var result = await sut.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_WhenRecordExists_ShouldReturnRecord()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var entity = new UserEntity { Name = "Juan", Email = "juan@email.com" };
        context.Users.Add(entity);
        await context.SaveChangesAsync();
        var sut = new UsersRepository(context);

        // Act
        var result = await sut.GetByIdAsync(entity.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Juan");
    }
}
```

> **Nota:** Si el proyecto usa Dapper con stored procedures (sin EF Core), los Repository Tests son tests de integración y requieren una base de datos PostgreSQL real. En ese caso se recomienda un contenedor Docker dedicado para tests.

---

## 8. Cobertura de código

### Generar reporte de cobertura

```bash
dotnet test --collect:"XPlat Code Coverage"
```

El archivo `coverage.cobertura.xml` se genera en `TestResults/`.

### Visualizar en Visual Studio / Rider

Ambos IDEs leen el archivo de cobertura automáticamente. En VS usar **Test → Analyze Code Coverage**.

### Generar reporte HTML con ReportGenerator

```bash
dotnet tool install -g dotnet-reportgenerator-globaltool

reportgenerator \
  -reports:"TestResults/**/coverage.cobertura.xml" \
  -targetdir:"coverage" \
  -reporttypes:Html
```

El reporte HTML queda en `coverage/index.html`. Esta carpeta está ignorada por `.gitignore`.
