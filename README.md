# Family Tree

A personal web-based family tree application for creating and visualizing family relationships.

**Stack:** .NET 10 / C# 13 · PostgreSQL · EF Core · React 18 · TypeScript · React Flow · Material UI

---

## Progress

### Phase 1 — MVP

- [x] Clean Architecture project setup (.NET 10)
- [x] PostgreSQL + EF Core with Fluent API configuration
- [x] Domain models: Person, Tree, Relationship, User
- [x] CRUD API: PersonsController, TreesController, RelationshipsController
- [x] JWT authentication (register + login)
- [x] React + TypeScript + Vite frontend
- [x] React Flow canvas (TreePage)
- [x] Home page with tree list
- [x] Create/edit person form (add + edit dialogs)
- [x] Display full family tree with relationships and edges
- [x] Drag-and-drop node repositioning with persistence
- [x] Canvas relationship creation
- [x] H-bridge parent edges with relationship type icons

### Phase 2 — Enhanced Features

- [ ] Photo uploads
- [ ] Rich text biography editor
- [ ] Search and filter people
- [ ] Export tree as image / PDF
- [ ] Multiple tree layout options (top-down, bottom-up, horizontal)

### Phase 3 — Advanced

- [ ] GEDCOM import/export
- [ ] Tree sharing and collaboration (TreePermission)
- [ ] Mobile-responsive design
- [ ] Cloud photo storage

---

## Getting Started

```bash
# Run the API (Swagger at https://localhost:7xxx/swagger)
dotnet run --project src/Veilora.Api

# Run the frontend dev server
cd src/Veilora.Api/ClientApp
npm run dev
```

Set required secrets before running:

```bash
dotnet user-secrets set "Jwt:Key" "your-secret-key-at-least-32-chars" --project src/Veilora.Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=...;Database=...;Username=...;Password=..." --project src/Veilora.Api
```

## Running Tests

```bash
dotnet test
```
