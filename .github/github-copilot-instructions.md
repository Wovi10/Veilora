# GitHub Copilot Instructions for Family Tree Application

## Project Overview
This is a full-stack family tree visualization application that allows users to create, manage, and visualize multiple independent family trees with comprehensive relationship support.

## Technology Stack

### Backend
- **Framework**: ASP.NET Core 10.0 Web API
- **Language**: C# 13
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core
- **Architecture**: Clean Architecture (4 layers: API, Application, Domain, Infrastructure)
- **Validation**: FluentValidation
- **Mapping**: Manual mapping (no AutoMapper)
- **Testing**: xUnit

### Frontend
- **Framework**: React with TypeScript
- **Visualization**: React Flow (for interactive family tree graphs)
- **Styling**: Material-UI (MUI) - preferred component library
- **State Management**: TBD (Context API, Zustand, or React Query)

## Project Structure

```
FamilyTree/
├── src/
│   ├── FamilyTree.API/           # Controllers, Middleware, Program.cs
│   ├── FamilyTree.Application/   # Services, DTOs, Validators, Mapping
│   ├── FamilyTree.Domain/        # Entities, Enums (no external dependencies)
│   └── FamilyTree.Infrastructure/# DbContext, Repositories, Configurations
└── tests/
    ├── FamilyTree.UnitTests/
    └── FamilyTree.IntegrationTests/
```

## Coding Standards & Preferences

### General
- Use **async/await** for all I/O operations
- Always suffix async methods with `Async`
- Use **Guid** for all entity IDs
- Use **UTC timestamps** for all datetime fields
- Follow **SOLID principles**
- Program to **interfaces, not implementations**

### Naming Conventions
- **PascalCase** for classes, methods, properties, public fields
- **camelCase** for local variables, parameters, private fields
- **Interface names** start with `I` (e.g., `IPersonService`)
- **Async methods** end with `Async` (e.g., `GetPersonAsync`)
- **DTOs** end with `Dto` (e.g., `PersonDto`, `CreatePersonDto`)

### C# / .NET Preferences
- Use **.NET 10** and **C# 13** features
- Use **nullable reference types** (`string?` for nullable strings)
- Use **primary constructors** when appropriate (C# 12+)
- Use **record types** for DTOs
- Use **collection expressions** `[]` instead of `new List<>()`
- Prefer **LINQ** for collections over loops where appropriate
- Use **pattern matching** where it improves readability
- Keep controllers **thin** - delegate logic to services
- Use **dependency injection** for all services
- **Manual mapping**: Create explicit mapping methods (no AutoMapper)
  - Create static mapper classes or extension methods
  - Example: `PersonMapper.ToDto(person)` or `person.ToDto()`
  - Keeps mappings explicit and easier to debug

### Entity Framework Core
- Use **Fluent API** for entity configuration (not data annotations)
- Create separate configuration classes (e.g., `PersonConfiguration.cs`)
- Always use **parameterized queries** (EF Core does this by default)
- Use **Include()** for eager loading related entities
- Use **AsNoTracking()** for read-only queries
- Create **indexes** for frequently queried fields

### React / TypeScript Preferences
- Use **functional components** with hooks (no class components)
- Use **TypeScript** for type safety
- Use **React Flow** built-in components (MiniMap, Controls, Background)
- Keep components **small and focused** (single responsibility)
- Use **custom hooks** for reusable logic
- Use **Material-UI (MUI)** components and styling:
  - Prefer MUI components over custom HTML
  - Use `sx` prop for styling
  - Use MUI theme for consistent design
  - Leverage MUI's responsive utilities
- Use **proper semantic HTML**

## Domain Model

### Core Entities
1. **Person**: FirstName, LastName, BirthDate, DeathDate, BirthPlace, Residence, Gender, Biography
2. **Relationship**: Person1, Person2, RelationshipType, StartDate, EndDate, Notes
3. **Tree**: Name, Description, CreatedBy
4. **PersonTree**: Junction table (Person ↔ Tree many-to-many)

### Relationship Types (Enum)
- `ParentChildBiological`
- `ParentChildAdopted`
- `Spouse`
- `Partner`
- `StepParent`
- `StepChild`
- `Godparent`
- `Guardian`
- `CloseFriend`

### Key Design Decisions
- **Multiple trees**: A person can exist in multiple trees via PersonTree junction table
- **Relationships are global**: Not tree-specific
- **No photos in v1**: Deferred to Phase 2
- **Small trees**: Optimized for <50 people per tree
- **Single user**: Multi-user support deferred to Phase 3

## Feature Priorities

### Phase 1 (MVP) - Current Focus
- ✅ CRUD operations for persons, relationships, trees
- ✅ **Smooth navigation** (pan, zoom, mini-map, keyboard shortcuts) - **TOP PRIORITY**
- ✅ Interactive graph visualization with React Flow
- ✅ Multiple layout options (top-down, bottom-up, left-to-right)
- ✅ Search and filter
- ✅ Export to image/PDF
- ✅ Rich text biography support
- ✅ Location information (birthplace, residence)

### Phase 2 (Future)
- Photo uploads and storage
- Timeline view
- Advanced search
- Statistics and reports

### Phase 3 (Future)
- User authentication and authorization
- Multi-user support with permissions
- Sharing and collaboration
- Mobile-friendly responsive design
- GEDCOM import/export

## API Design Patterns

### REST Endpoints
```
GET    /api/persons
GET    /api/persons/{id}
POST   /api/persons
PUT    /api/persons/{id}
DELETE /api/persons/{id}

GET    /api/trees
GET    /api/trees/{treeId}
GET    /api/trees/{treeId}/persons
POST   /api/trees
PUT    /api/trees/{treeId}
DELETE /api/trees/{treeId}

GET    /api/relationships
POST   /api/relationships
PUT    /api/relationships/{id}
DELETE /api/relationships/{id}
```

### Response Format
- **Success**: Return DTO or collection of DTOs
- **Created**: Return 201 with Location header
- **Not Found**: Return 404 with error message
- **Validation Error**: Return 400 with validation details
- **Server Error**: Return 500 with generic error (don't expose internals)

## Navigation & UX Requirements

### Critical Navigation Features (v1)
- **Smooth panning**: Click and drag anywhere to move
- **Smooth zooming**: Mouse wheel, trackpad pinch, zoom buttons
- **Mini-map**: Overview in corner showing viewport position
- **Focus feature**: Click person → center and highlight with smooth animation
- **Fit view button**: Fit entire tree in viewport
- **Keyboard shortcuts**:
  - Arrow keys: Pan
  - `+` / `-`: Zoom
  - `0`: Reset to fit view
  - `F`: Focus on selected person
- **60 FPS performance target**

### React Flow Components to Use
- `ReactFlow` - Main component
- `MiniMap` - Mini-map for navigation
- `Controls` - Zoom buttons
- `Background` - Grid or dots for spatial reference
- `useReactFlow` - Hook for programmatic navigation

## Testing Guidelines

### Unit Tests
- Test business logic in service classes
- Test validators
- Mock repositories and external dependencies
- Use xUnit assertions

### Integration Tests
- Test API endpoints end-to-end
- Use in-memory database or test database
- Test full request/response cycle
- Verify database state after operations

## Code Examples & Templates

### Service Method Template
```csharp
public async Task<PersonDto> GetByIdAsync(Guid id)
{
    var person = await _repository.GetByIdAsync(id);
    
    if (person == null)
        throw new NotFoundException($"Person with ID {id} not found");
    
    return PersonMapper.ToDto(person);
}
```

### Manual Mapping Example
```csharp
public static class PersonMapper
{
    public static PersonDto ToDto(Person person)
    {
        return new PersonDto
        {
            Id = person.Id,
            FirstName = person.FirstName,
            MiddleName = person.MiddleName,
            LastName = person.LastName,
            BirthDate = person.BirthDate,
            DeathDate = person.DeathDate,
            BirthPlace = person.BirthPlace,
            Residence = person.Residence,
            Gender = person.Gender.ToString(),
            Biography = person.Biography,
            CreatedAt = person.CreatedAt,
            UpdatedAt = person.UpdatedAt
        };
    }

    public static Person ToEntity(CreatePersonDto dto)
    {
        return new Person
        {
            FirstName = dto.FirstName,
            MiddleName = dto.MiddleName,
            LastName = dto.LastName,
            BirthDate = dto.BirthDate,
            DeathDate = dto.DeathDate,
            BirthPlace = dto.BirthPlace,
            Residence = dto.Residence,
            Gender = Enum.Parse<Gender>(dto.Gender),
            Biography = dto.Biography
        };
    }
}
```

### Controller Action Template
```csharp
[HttpGet("{id}")]
public async Task<ActionResult<PersonDto>> GetById(Guid id)
{
    try
    {
        var person = await _personService.GetByIdAsync(id);
        return Ok(person);
    }
    catch (NotFoundException ex)
    {
        return NotFound(new { message = ex.Message });
    }
}
```

### React Component Template
```tsx
import { Box, Typography } from '@mui/material';

interface PersonNodeProps {
  data: {
    id: string;
    name: string;
    birthDate?: string;
    deathDate?: string;
  };
}

export const PersonNode: React.FC<PersonNodeProps> = ({ data }) => {
  return (
    <Box
      sx={{
        px: 2,
        py: 1.5,
        bgcolor: 'background.paper',
        border: 2,
        borderColor: 'divider',
        borderRadius: 1,
        boxShadow: 1,
      }}
    >
      <Typography variant="subtitle1" fontWeight="bold">
        {data.name}
      </Typography>
      {data.birthDate && (
        <Typography variant="body2" color="text.secondary">
          {data.birthDate}
        </Typography>
      )}
    </Box>
  );
};
```

## When Suggesting Code

### DO:
- Follow the clean architecture pattern
- Use dependency injection
- Create separate DTOs for create/update operations
- **Use manual mapping** with static mapper classes or extension methods
- Add XML documentation comments for public APIs
- Include error handling
- Use async/await consistently
- Add appropriate validation
- Follow the established naming conventions
- Consider performance (use AsNoTracking for reads)
- Add logging where appropriate
- **Use Material-UI components** for all UI elements
- Use MUI's `sx` prop for component styling

### DON'T:
- Mix concerns (keep controllers thin)
- Use raw SQL queries (use EF Core LINQ)
- Return entities directly from controllers (use DTOs)
- Ignore null safety
- Use magic strings (use constants or enums)
- **Use AutoMapper** (use manual mapping instead)
- **Use Tailwind CSS** (use Material-UI instead)
- Implement features beyond Phase 1 unless explicitly asked
- Add authentication/authorization logic (Phase 3)
- Add photo upload logic (Phase 2)

## Common Questions

**Q: Should I add authentication?**
A: No, authentication is Phase 3. For now, assume single-user local development.

**Q: Should I add photo upload functionality?**
A: No, photo storage is deferred to Phase 2. Just include the ProfilePhotoUrl field as a string.

**Q: Which state management should I use in React?**
A: TBD - start with React Context API for simplicity, can migrate to Zustand or React Query later if needed.

**Q: Should I create separate create/update DTOs?**
A: Yes, always create separate DTOs (CreatePersonDto, UpdatePersonDto) even if they're similar. It provides flexibility.

**Q: How should I handle soft deletes?**
A: Not implemented yet. Use hard deletes for now. Can add soft delete (IsDeleted flag) in Phase 2 if needed.

**Q: Should relationships be bidirectional?**
A: Relationships are stored once with Person1 and Person2. The application layer determines how to display them (e.g., parent-child can be queried both ways).

## Database Queries

### Example: Get all ancestors of a person
```sql
WITH RECURSIVE ancestors AS (
    -- Base case: direct parents
    SELECT p.*, 1 as generation
    FROM persons p
    JOIN relationships r ON p.id = r.person1_id
    WHERE r.person2_id = @personId 
      AND r.relationship_type LIKE 'ParentChild%'
    
    UNION ALL
    
    -- Recursive case: parents of parents
    SELECT p.*, a.generation + 1
    FROM persons p
    JOIN relationships r ON p.id = r.person1_id
    JOIN ancestors a ON r.person2_id = a.id
    WHERE r.relationship_type LIKE 'ParentChild%'
)
SELECT * FROM ancestors;
```

## Performance Considerations
- **Indexes**: Add indexes on LastName, FirstName, TreeId foreign keys
- **Pagination**: Implement pagination for person lists (future enhancement)
- **Lazy loading**: Not needed for v1 (trees are small <50 people)
- **Caching**: Not needed for v1
- **N+1 queries**: Use Include() to eager load related entities

## Security Considerations (Phase 3)
- Input validation (FluentValidation)
- SQL injection prevention (EF Core parameterized queries)
- XSS prevention (React escapes by default)
- CORS configuration (allow React dev server)
- Authentication/authorization (deferred to Phase 3)

## Documentation
- Add XML comments to public APIs
- Keep README.md updated with setup instructions
- Document any non-obvious design decisions
- Update this file when making architectural changes

## Additional Context
- **Developer**: Full-stack developer with .NET backend experience
- **Learning**: Open to learning new technologies (chose React Flow)
- **Philosophy**: Get v1 working quickly, but navigation must be excellent
- **Pain point**: Frustrated with poor navigation in existing family tree apps

---

**Last Updated**: February 26, 2026
**Version**: 1.0
**Status**: Planning Phase / Early Development
