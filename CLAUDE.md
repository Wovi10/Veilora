# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Status

Early development — Phase 1 (MVP). The backend skeleton is scaffolded (domain entities, DTOs, validators, mappers, repositories, EF Core migrations). **No API controllers exist yet.** There is no frontend yet.

## Commands

```bash
# Build
dotnet build

# Run API (Swagger at https://localhost:7xxx/swagger)
dotnet run --project src/FamilyTree.Api

# Run all tests
dotnet test

# Run a single test project
dotnet test src/tests/FamilyTree.UnitTests
dotnet test src/tests/FamilyTree.IntegrationTests

# EF Core migrations
dotnet ef migrations add <Name> --project src/FamilyTree.Infrastructure --startup-project src/FamilyTree.Api
dotnet ef database update --project src/FamilyTree.Infrastructure --startup-project src/FamilyTree.Api
```

## Architecture

Clean Architecture with 4 layers:

- **`FamilyTree.Domain`** — Entities, enums, `BaseEntity`. No external dependencies.
- **`FamilyTree.Application`** — Services, DTOs, validators (FluentValidation), mappers. Depends on Domain.
- **`FamilyTree.Infrastructure`** — EF Core `ApplicationDbContext`, repository implementations, entity configurations (Fluent API), migrations. Depends on Domain + Application.
- **`FamilyTree.Api`** — Controllers, `Program.cs`, middleware. Depends on Application + Infrastructure.

Repository interfaces live in **both** `FamilyTree.Application/Repositories/Interfaces/` (consumed by services) and `FamilyTree.Infrastructure/Repositories/Interfaces/` (implementations registered there). The Application layer owns the contracts.

`SaveChangesAsync` in `ApplicationDbContext` auto-sets `CreatedAt`/`UpdatedAt` on all `BaseEntity` subclasses.

## Key Conventions

**Mapping**: Manual only — use static mapper classes (`PersonMapper.ToDto()`, `PersonMapper.ToEntity()`, `PersonMapper.UpdateEntity()`). **No AutoMapper.**

**DTOs**: Use `record` types. Always create separate `CreateXxxDto` and `UpdateXxxDto` even if similar.

**EF Core**: Fluent API only (no data annotations). Use `AsNoTracking()` for reads. Use `Include()` for eager loading. All entity configs are in `Infrastructure/Data/Configurations/`.

**C# style**: .NET 10 / C# 13. Primary constructors, collection expressions `[]`, nullable reference types, pattern matching.

**Controllers**: Thin — delegate all logic to services. Catch `NotFoundException` → 404, `ValidationException` → 400.

**Frontend** (not yet started): React + TypeScript + React Flow + Material-UI. Use MUI `sx` prop for styling. **No Tailwind CSS.**

## Domain Model

- **Person** ↔ **Tree** via `PersonTree` junction (many-to-many). A person can belong to multiple trees.
- **Relationship** is global (not tree-scoped) with `Person1Id`, `Person2Id`, `RelationshipType`, optional `StartDate`/`EndDate`.
- `RelationshipType` enum: `ParentChildBiological`, `ParentChildAdopted`, `Spouse`, `Partner`, `StepParent`, `StepChild`, `Godparent`, `Guardian`, `CloseFriend`.
- `User` and `TreePermission` entities exist in the domain but are **Phase 3** — do not wire them up yet.
- No soft deletes — use hard deletes for now.
- No photo storage in v1 — `ProfilePhotoUrl` is just a nullable string field.

## What NOT to Implement Yet

- Authentication/authorization → Phase 3
- Photo uploads → Phase 2
- `User`/`TreePermission` service wiring → Phase 3
- GEDCOM import/export → Phase 3
- Mobile/responsive design → Phase 3
