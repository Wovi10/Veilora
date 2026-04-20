# Veilora

A personal web-based worldbuilding companion for fantasy writers and creators. Organize the people, factions, locations, events, concepts, and languages of your fictional worlds — all in one place. Includes a family tree builder as one of its core features.

**Stack:** .NET 10 / C# 13 · PostgreSQL (Neon) · EF Core · React 18 · TypeScript · React Flow · Material UI

---

## Features

- **Worlds** — Create and manage multiple fictional worlds, each with its own cast and lore
- **Characters** — Track characters with biographical details, parentage, family trees, and inter-character relationships
- **Family Trees** — Visualize character relationships as an interactive node graph (React Flow), with drag-and-drop positioning
- **Entities** — Organize groups, events, and concepts (factions, battles, religions, etc.)
- **Locations** — Define and manage places within a world
- **Notes** — Attach free-form notes to worlds and entities
- **Languages** — Track languages spoken in a world
- **Custom Calendars** — Define custom date systems with `DateSuffix` (anchor years, scale, reversal)
- **Permissions** — Share worlds with other users (read or edit access)
- **JWT Auth** — Secure registration and login

---

## Progress

### Done

- [x] Clean Architecture project setup (.NET 10)
- [x] PostgreSQL + EF Core with Fluent API configuration (10 migrations)
- [x] Domain models: World, Character, FamilyTree, Entity, Location, Note, Language, DateSuffix, WorldPermission, User
- [x] Full CRUD API: 11 controllers, 60+ endpoints
- [x] JWT authentication (register + login)
- [x] World-level permission system (read/edit, transfer ownership)
- [x] React + TypeScript + Vite frontend
- [x] Login / register page
- [x] Home page with world list
- [x] World dashboard with tabbed navigation
- [x] World settings page (permissions management)
- [x] Family tree canvas (React Flow, drag-and-drop, relationship edges)
- [x] Character detail page (genealogy, relationships)
- [x] Character list page with search
- [x] Location list and detail pages
- [x] Entity list page
- [x] FluentValidation integration and centralized exception handling middleware

### Planned

- [ ] Rich text biography / notes editor
- [ ] Photo uploads for characters
- [ ] Export family tree as image / PDF
- [ ] Advanced search and filtering across a world
- [ ] Mobile-responsive design
- [ ] GEDCOM import/export

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

# Single project
dotnet test src/tests/Veilora.UnitTests
dotnet test src/tests/Veilora.IntegrationTests
```
