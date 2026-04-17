# Family Tree Application - Backend Project Structure

**Framework:** ASP.NET Core Web API  
**Database:** PostgreSQL with Entity Framework Core  
**Architecture:** Clean Architecture / Layered Architecture

---

## Recommended Project Structure

```
FamilyTree/
├── Veilora.sln                          # Solution file
│
├── src/
│   ├── Veilora.API/                     # Web API Layer
│   │   ├── Controllers/
│   │   │   ├── PersonsController.cs
│   │   │   ├── RelationshipsController.cs
│   │   │   ├── TreesController.cs
│   │   │   └── AuthController.cs           # register + login
│   │   ├── Middleware/
│   │   │   ├── ExceptionHandlingMiddleware.cs
│   │   │   └── RequestLoggingMiddleware.cs
│   │   ├── Filters/
│   │   │   └── ValidationFilter.cs
│   │   ├── Extensions/
│   │   │   └── ServiceCollectionExtensions.cs
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   ├── Program.cs
│   │   └── Veilora.API.csproj
│   │
│   ├── Veilora.Application/             # Business Logic Layer
│   │   ├── DTOs/                           # Data Transfer Objects
│   │   │   ├── Person/
│   │   │   │   ├── PersonDto.cs
│   │   │   │   ├── CreatePersonDto.cs
│   │   │   │   └── UpdatePersonDto.cs
│   │   │   ├── Relationship/
│   │   │   │   ├── RelationshipDto.cs
│   │   │   │   ├── CreateRelationshipDto.cs
│   │   │   │   └── UpdateRelationshipDto.cs
│   │   │   └── Tree/
│   │   │       ├── TreeDto.cs
│   │   │       ├── CreateTreeDto.cs
│   │   │       └── TreeWithPersonsDto.cs
│   │   ├── Services/
│   │   │   ├── Interfaces/
│   │   │   │   ├── IPersonService.cs
│   │   │   │   ├── IRelationshipService.cs
│   │   │   │   ├── ITreeService.cs
│   │   │   │   └── IExportService.cs
│   │   │   ├── PersonService.cs
│   │   │   ├── RelationshipService.cs
│   │   │   ├── TreeService.cs
│   │   │   └── ExportService.cs            # PDF/Image export
│   │   ├── Mapping/
│   │   │   └── MappingProfile.cs           # AutoMapper profiles
│   │   ├── Validators/
│   │   │   ├── PersonValidator.cs          # FluentValidation
│   │   │   ├── RelationshipValidator.cs
│   │   │   └── TreeValidator.cs
│   │   ├── Exceptions/
│   │   │   ├── NotFoundException.cs
│   │   │   ├── ValidationException.cs
│   │   │   └── BusinessException.cs
│   │   └── Veilora.Application.csproj
│   │
│   ├── Veilora.Domain/                  # Domain Layer (Entities)
│   │   ├── Entities/
│   │   │   ├── Person.cs
│   │   │   ├── Relationship.cs
│   │   │   ├── Tree.cs
│   │   │   ├── PersonTree.cs
│   │   │   ├── User.cs                     # For Phase 3
│   │   │   └── TreePermission.cs           # For Phase 3
│   │   ├── Enums/
│   │   │   ├── RelationshipType.cs
│   │   │   ├── Gender.cs
│   │   │   └── PermissionLevel.cs          # For Phase 3
│   │   ├── Common/
│   │   │   └── BaseEntity.cs               # Base class with Id, CreatedAt, UpdatedAt
│   │   └── Veilora.Domain.csproj
│   │
│   └── Veilora.Infrastructure/          # Data Access Layer
│       ├── Data/
│       │   ├── ApplicationDbContext.cs
│       │   ├── Configurations/             # EF Core entity configurations
│       │   │   ├── PersonConfiguration.cs
│       │   │   ├── RelationshipConfiguration.cs
│       │   │   ├── TreeConfiguration.cs
│       │   │   └── PersonTreeConfiguration.cs
│       │   └── Migrations/                 # EF Core migrations
│       ├── Repositories/
│       │   ├── Interfaces/
│       │   │   ├── IRepository.cs          # Generic repository interface
│       │   │   ├── IPersonRepository.cs
│       │   │   ├── IRelationshipRepository.cs
│       │   │   └── ITreeRepository.cs
│       │   ├── Repository.cs               # Generic repository implementation
│       │   ├── PersonRepository.cs
│       │   ├── RelationshipRepository.cs
│       │   └── TreeRepository.cs
│       ├── Extensions/
│       │   └── QueryExtensions.cs          # Extension methods for queries
│       └── Veilora.Infrastructure.csproj
│
├── tests/
│   ├── Veilora.UnitTests/
│   │   ├── Services/
│   │   │   ├── PersonServiceTests.cs
│   │   │   ├── RelationshipServiceTests.cs
│   │   │   └── TreeServiceTests.cs
│   │   ├── Validators/
│   │   │   └── PersonValidatorTests.cs
│   │   └── Veilora.UnitTests.csproj
│   │
│   └── Veilora.IntegrationTests/
│       ├── Controllers/
│       │   ├── PersonsControllerTests.cs
│       │   └── TreesControllerTests.cs
│       ├── TestFixtures/
│       │   └── WebApplicationFactory.cs
│       └── Veilora.IntegrationTests.csproj
│
├── .gitignore
└── README.md
```

---

## Layer Responsibilities

### 1. Veilora.API (Presentation Layer)
**Purpose:** HTTP endpoints, request/response handling, routing

**Responsibilities:**
- Define API endpoints (controllers)
- Handle HTTP requests/responses
- Input validation (model binding)
- Authentication/Authorization (Phase 3)
- Middleware (error handling, logging, CORS)
- Dependency injection configuration

**Dependencies:**
- References: `Veilora.Application`, `Veilora.Infrastructure`
- NuGet Packages:
  - `Microsoft.AspNetCore.OpenApi`
  - `Swashbuckle.AspNetCore` (Swagger/OpenAPI)
  - `Serilog.AspNetCore` (Logging)

### 2. Veilora.Application (Business Logic Layer)
**Purpose:** Business logic, use cases, orchestration

**Responsibilities:**
- Business rules and logic
- Application services (coordinate between API and Domain)
- DTOs (Data Transfer Objects)
- Validation logic (FluentValidation)
- Mapping between entities and DTOs (AutoMapper)
- Exception handling

**Dependencies:**
- References: `Veilora.Domain`
- NuGet Packages:
  - `AutoMapper`
  - `FluentValidation`
  - `MediatR` (optional - for CQRS pattern)

### 3. Veilora.Domain (Core Layer)
**Purpose:** Business entities, domain logic, core models

**Responsibilities:**
- Entity definitions (Person, Relationship, Tree)
- Enums and value objects
- Domain interfaces
- Business validation rules (on entities)
- **No dependencies on other layers**

**Dependencies:**
- None (pure domain logic, no external dependencies)

### 4. Veilora.Infrastructure (Data Access Layer)
**Purpose:** Database access, external services, persistence

**Responsibilities:**
- Entity Framework Core DbContext
- Repository implementations
- Database migrations
- Data seeding
- Entity configurations (Fluent API)
- External service integrations (future: file storage, email)

**Dependencies:**
- References: `Veilora.Domain`, `Veilora.Application` (for interfaces)
- NuGet Packages:
  - `Npgsql.EntityFrameworkCore.PostgreSQL`
  - `Microsoft.EntityFrameworkCore.Tools`
  - `Microsoft.EntityFrameworkCore.Design`

---

## Key Files Explained

### Program.cs (Veilora.API)
```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddScoped<ITreeRepository, TreeRepository>();

// Services
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<ITreeService, TreeService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<PersonValidator>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=familytree_db;Username=your_username;Password=your_password"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### ApplicationDbContext.cs (Veilora.Infrastructure)
```csharp
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Person> Persons { get; set; }
    public DbSet<Relationship> Relationships { get; set; }
    public DbSet<Tree> Trees { get; set; }
    public DbSet<PersonTree> PersonTrees { get; set; }
    // Phase 3: public DbSet<User> Users { get; set; }
    // Phase 3: public DbSet<TreePermission> TreePermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from the assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
```

### Person.cs (Veilora.Domain)
```csharp
public class Person : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string? MaidenName { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
    public string? BirthPlace { get; set; }
    public string? Residence { get; set; }
    public Gender Gender { get; set; }
    public string? Biography { get; set; }
    public string? ProfilePhotoUrl { get; set; }

    // Navigation properties
    public ICollection<PersonTree> PersonTrees { get; set; } = new List<PersonTree>();
    public ICollection<Relationship> RelationshipsAsPerson1 { get; set; } = new List<Relationship>();
    public ICollection<Relationship> RelationshipsAsPerson2 { get; set; } = new List<Relationship>();
}
```

### BaseEntity.cs (Veilora.Domain)
```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### PersonConfiguration.cs (Veilora.Infrastructure)
```csharp
public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.MiddleName)
            .HasMaxLength(100);

        builder.Property(p => p.MaidenName)
            .HasMaxLength(100);

        builder.Property(p => p.BirthPlace)
            .HasMaxLength(200);

        builder.Property(p => p.Residence)
            .HasMaxLength(200);

        builder.Property(p => p.Gender)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(p => p.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Indexes for better query performance
        builder.HasIndex(p => p.LastName);
        builder.HasIndex(p => new { p.FirstName, p.LastName });
    }
}
```

### IPersonService.cs (Veilora.Application)
```csharp
public interface IPersonService
{
    Task<IEnumerable<PersonDto>> GetAllAsync();
    Task<IEnumerable<PersonDto>> GetPersonsByTreeIdAsync(Guid treeId);
    Task<PersonDto?> GetByIdAsync(Guid id);
    Task<PersonDto> CreateAsync(CreatePersonDto dto);
    Task<PersonDto> UpdateAsync(Guid id, UpdatePersonDto dto);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<PersonDto>> SearchAsync(string searchTerm);
    Task<IEnumerable<PersonDto>> GetAncestorsAsync(Guid personId);
    Task<IEnumerable<PersonDto>> GetDescendantsAsync(Guid personId);
}
```

### PersonsController.cs (Veilora.API)
```csharp
[ApiController]
[Route("api/[controller]")]
public class PersonsController : ControllerBase
{
    private readonly IPersonService _personService;
    private readonly ILogger<PersonsController> _logger;

    public PersonsController(IPersonService personService, ILogger<PersonsController> logger)
    {
        _personService = personService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PersonDto>>> GetAll()
    {
        var persons = await _personService.GetAllAsync();
        return Ok(persons);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PersonDto>> GetById(Guid id)
    {
        var person = await _personService.GetByIdAsync(id);
        if (person == null)
            return NotFound();

        return Ok(person);
    }

    [HttpPost]
    public async Task<ActionResult<PersonDto>> Create([FromBody] CreatePersonDto dto)
    {
        var person = await _personService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = person.Id }, person);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PersonDto>> Update(Guid id, [FromBody] UpdatePersonDto dto)
    {
        var person = await _personService.UpdateAsync(id, dto);
        return Ok(person);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _personService.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<PersonDto>>> Search([FromQuery] string q)
    {
        var persons = await _personService.SearchAsync(q);
        return Ok(persons);
    }

    [HttpGet("{id}/ancestors")]
    public async Task<ActionResult<IEnumerable<PersonDto>>> GetAncestors(Guid id)
    {
        var ancestors = await _personService.GetAncestorsAsync(id);
        return Ok(ancestors);
    }

    [HttpGet("{id}/descendants")]
    public async Task<ActionResult<IEnumerable<PersonDto>>> GetDescendants(Guid id)
    {
        var descendants = await _personService.GetDescendantsAsync(id);
        return Ok(descendants);
    }
}
```

---

## NuGet Packages by Project

### Veilora.API
```xml
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.*" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.*" />
<PackageReference Include="Serilog.AspNetCore" Version="8.0.*" />
```

### Veilora.Application
```xml
<PackageReference Include="AutoMapper" Version="12.0.*" />
<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.*" />
<PackageReference Include="FluentValidation" Version="11.9.*" />
<PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.9.*" />
```

### Veilora.Domain
```xml
<!-- No external packages - pure domain logic -->
```

### Veilora.Infrastructure
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.*" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.*" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.*" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.*" />
```

---

## Database Migration Commands

### Create Initial Migration
```bash
# From solution root
dotnet ef migrations add InitialCreate --project src/Veilora.Infrastructure --startup-project src/Veilora.API

# Apply migration to database
dotnet ef database update --project src/Veilora.Infrastructure --startup-project src/Veilora.API
```

### Add New Migration (after model changes)
```bash
dotnet ef migrations add AddBiographyField --project src/Veilora.Infrastructure --startup-project src/Veilora.API
dotnet ef database update --project src/Veilora.Infrastructure --startup-project src/Veilora.API
```

### Remove Last Migration (if not applied to database)
```bash
dotnet ef migrations remove --project src/Veilora.Infrastructure --startup-project src/Veilora.API
```

---

## Project Creation Commands

```bash
# Create solution
dotnet new sln -n FamilyTree

# Create projects
dotnet new webapi -n Veilora.API -o src/Veilora.API
dotnet new classlib -n Veilora.Application -o src/Veilora.Application
dotnet new classlib -n Veilora.Domain -o src/Veilora.Domain
dotnet new classlib -n Veilora.Infrastructure -o src/Veilora.Infrastructure

# Create test projects
dotnet new xunit -n Veilora.UnitTests -o tests/Veilora.UnitTests
dotnet new xunit -n Veilora.IntegrationTests -o tests/Veilora.IntegrationTests

# Add projects to solution
dotnet sln add src/Veilora.API/Veilora.API.csproj
dotnet sln add src/Veilora.Application/Veilora.Application.csproj
dotnet sln add src/Veilora.Domain/Veilora.Domain.csproj
dotnet sln add src/Veilora.Infrastructure/Veilora.Infrastructure.csproj
dotnet sln add tests/Veilora.UnitTests/Veilora.UnitTests.csproj
dotnet sln add tests/Veilora.IntegrationTests/Veilora.IntegrationTests.csproj

# Add project references
dotnet add src/Veilora.API reference src/Veilora.Application
dotnet add src/Veilora.API reference src/Veilora.Infrastructure
dotnet add src/Veilora.Application reference src/Veilora.Domain
dotnet add src/Veilora.Infrastructure reference src/Veilora.Domain
dotnet add src/Veilora.Infrastructure reference src/Veilora.Application

# Add test references
dotnet add tests/Veilora.UnitTests reference src/Veilora.Application
dotnet add tests/Veilora.UnitTests reference src/Veilora.Domain
dotnet add tests/Veilora.IntegrationTests reference src/Veilora.API
```

---

## Development Workflow

### 1. Starting Development
```bash
# Restore packages
dotnet restore

# Build solution
dotnet build

# Run API (starts on https://localhost:7000 by default)
dotnet run --project src/Veilora.API

# Access Swagger UI
# Open browser: https://localhost:7000/swagger
```

### 2. Database Setup
```bash
# Make sure PostgreSQL is running
# Create database (if not exists)
# Update connection string in appsettings.json

# Run migrations
dotnet ef database update --project src/Veilora.Infrastructure --startup-project src/Veilora.API
```

### 3. Testing
```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/Veilora.UnitTests

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

---

## Best Practices

### 1. Dependency Injection
- Always program to interfaces, not implementations
- Register services in `Program.cs`
- Use appropriate lifetimes:
  - `AddScoped` for repositories and services (per request)
  - `AddSingleton` for stateless utilities
  - `AddTransient` for lightweight, stateless services

### 2. Error Handling
- Use custom exceptions (`NotFoundException`, `ValidationException`)
- Implement global exception handling middleware
- Return appropriate HTTP status codes
- Log errors with structured logging (Serilog)

### 3. Validation
- Use FluentValidation for complex validation rules
- Validate DTOs in the Application layer
- Use Data Annotations for simple validations
- Return validation errors in a consistent format

### 4. Async/Await
- Always use async methods for I/O operations
- Suffix async methods with `Async`
- Use `ConfigureAwait(false)` in library code (not needed in ASP.NET Core)

### 5. Repository Pattern
- Generic repository for basic CRUD
- Specific repositories for complex queries
- Keep repositories focused on data access only

### 6. Security
- JWT Bearer authentication (implemented in Phase 1)
- All endpoints except `/api/auth/*` require `Authorization: Bearer {token}`
- Set JWT secret via user secrets: `dotnet user-secrets set "Jwt:Key" "..."`
- Validate all inputs
- Use parameterized queries (EF Core does this automatically)
- Enable CORS only for trusted origins

---

## Alternative Patterns (Optional)

### CQRS with MediatR
For more complex applications, consider using CQRS:
- Commands: Mutate state (Create, Update, Delete)
- Queries: Read data
- Use MediatR to handle command/query dispatch
- Better separation of read/write concerns

### Vertical Slice Architecture
Instead of layers, organize by features:
```
Features/
├── Persons/
│   ├── Create/
│   ├── Update/
│   ├── Delete/
│   └── GetById/
```

---

## Notes

- This structure follows **Clean Architecture** principles
- **Domain** layer has no dependencies (pure business logic)
- **Infrastructure** depends on Domain (implements interfaces)
- **Application** depends on Domain (orchestrates business logic)
- **API** depends on Application and Infrastructure (entry point)
- Easy to test - mock interfaces in unit tests
- Scalable - can easily add new features without affecting existing code
- For a simple MVP, you could start with fewer layers, but this structure will serve you well as the project grows

