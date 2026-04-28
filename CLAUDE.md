# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What Veilora Is

A worldbuilding companion app for fantasy writers. Users create **Worlds** and populate them with Characters, Family Trees, Entities (factions/events/concepts), Locations, Notes, Languages, and custom calendar systems. The family tree builder (React Flow canvas) is one feature within a broader world management system.

## Project Status

Core features are fully implemented — 11 backend controllers, 11 services, 10+ frontend pages, JWT auth, world permissions, world-scoped name search. Active development continues on UI polish and new features.

## Commands

```bash
# Build
dotnet build

# Run API (Swagger at https://localhost:7xxx/swagger)
dotnet run --project src/Veilora.Api

# Run all tests
dotnet test

# Run a single test project
dotnet test src/tests/Veilora.UnitTests
dotnet test src/tests/Veilora.IntegrationTests

# EF Core migrations
dotnet ef migrations add <Name> --project src/Veilora.Infrastructure --startup-project src/Veilora.Api
dotnet ef database update --project src/Veilora.Infrastructure --startup-project src/Veilora.Api
```

## Architecture

Clean Architecture with 4 layers:

- **`Veilora.Domain`** — Entities, enums, `BaseEntity`. No external dependencies.
- **`Veilora.Application`** — Services, DTOs, validators (FluentValidation), mappers. Depends on Domain.
- **`Veilora.Infrastructure`** — EF Core `ApplicationDbContext`, repository implementations, entity configurations (Fluent API), migrations. Depends on Domain + Application.
- **`Veilora.Api`** — Controllers, `Program.cs`, middleware. Depends on Application + Infrastructure.

Repository interfaces live in `Veilora.Application/Repositories/Interfaces/` (consumed by services). Implementations are in `Veilora.Infrastructure/Repositories/`. The Application layer owns the contracts.

`SaveChangesAsync` in `ApplicationDbContext` auto-sets `CreatedAt`/`UpdatedAt` on all `BaseEntity` subclasses.

## Key Conventions

**Mapping**: Manual only — use static mapper classes (`CharacterMapper.ToDto()`, etc.). **No AutoMapper.**

**DTOs**: Use `record` types. Always create separate `CreateXxxDto` and `UpdateXxxDto` even if similar.

**EF Core**: Fluent API only (no data annotations). Use `AsNoTracking()` for reads. Use `Include()` for eager loading. All entity configs are in `Infrastructure/Data/Configurations/`.

**C# style**: .NET 10 / C# 13. Primary constructors, collection expressions `[]`, nullable reference types, pattern matching.

**Controllers**: Thin — delegate all logic to services. `ExceptionHandlingMiddleware` maps `NotFoundException` → 404, `ValidationException` → 400, `BusinessException` → 422.

**Frontend**: React + TypeScript + Material-UI. Use MUI `sx` prop for styling. **No Tailwind CSS.**

## Domain Model

### Core entities

- **World** — top-level container. Has `CreatedById` (owner), name, description. Owner can transfer ownership.
- **Character** — belongs to a World. Rich biographical fields (FirstName, LastName, Gender, BirthDate, DeathDate, Biography, Species, etc.). Self-referencing `Parent1Id`/`Parent2Id`. Can belong to multiple FamilyTrees via `CharacterFamilyTree` junction (which also stores PositionX/Y for the canvas).
- **FamilyTree** — belongs to a World. Characters are added via `CharacterFamilyTree`. Relationships between characters are visualized as edges.
- **Relationship** — global (not tree-scoped). `Character1Id`, `Character2Id`, `RelationshipType`, optional `StartDate`/`EndDate`, Notes.
- **Entity** — generic lore object in a World. `Type` enum: `Group`, `Event`, `Concept`. Has affiliations (characters), languages, locations.
- **Location** — place within a World. Linked to characters and entities via join tables.
- **Note** — free-form text attached to a World or Entity.
- **Language** — named language in a World. Auto-created on first use (`GetOrCreate`).
- **DateSuffix** — custom calendar entry: AnchorYear, Scale, IsReversed, IsDefault per World.
- **WorldPermission** — `WorldId`, `UserId`, `CanEdit` boolean. Fully implemented and wired.
- **TreePermission** — exists in domain but not yet wired to a service.
- **User** — Email, PasswordHash (BCrypt), DisplayName.

### Enums

- `RelationshipType`: `ParentChildBiological`, `ParentChildAdopted`, `Spouse`, `Partner`, `StepParent`, `StepChild`, `Godparent`, `Guardian`, `CloseFriend`, `Sibling`
- `EntityType`: `Group`, `Event`, `Concept`

### Rules

- No soft deletes — hard deletes only.
- No file storage — `ProfilePhotoUrl` on Character is a nullable string URL.
- All entities inherit `BaseEntity` (Guid Id, CreatedAt, UpdatedAt).

## Authentication

JWT Bearer auth. `POST /api/auth/register` and `POST /api/auth/login` return a token. All other endpoints require `Authorization: Bearer {token}`.

Token claims: `sub` (userId), `email`, `jti`, `displayName`. 30-day expiry by default.

Set the JWT secret via user secrets (never commit it):
```bash
dotnet user-secrets set "Jwt:Key" "your-secret-key-at-least-32-chars" --project src/Veilora.Api
```

## Frontend Structure

Pages: Login, Home (world list), World (tabbed dashboard), WorldSettings (permissions), FamilyTree (React Flow canvas), Character detail, Characters list, Location detail, Locations list, Entity list.

Contexts: `AuthContext` (token/userId in localStorage), `EditModeContext` (UI edit toggle), `ThemeModeContext` (light/dark).

API calls go through a shared `apiFetch` helper that injects the Bearer token.

## What NOT to Implement Yet

- Photo/file uploads (Phase 2)
- `TreePermission` service wiring — entity exists but service is not wired (Phase 3)
- GEDCOM import/export (Phase 3)
- Mobile-responsive design (Phase 3)
