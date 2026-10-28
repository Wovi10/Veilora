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
- **Reading Notes** — Capture notes while reading via a persistent FAB. A dedicated page shows all notes alongside a live entity workspace: search, edit, or create Characters, Locations, and Entities (Group, Event, Concept) without leaving the page
- **Languages** — Track languages spoken in a world
- **Custom Calendars** — Define custom date systems with `DateSuffix` (anchor years, scale, reversal)
- **Permissions** — Share worlds with other users (read or edit access)
- **World Search** — Name-based search bar on the world dashboard, filters all sections in place
- **JWT Auth** — Secure registration and login

---

## Progress

### Done

- [x] Clean Architecture project setup (.NET 10)
- [x] PostgreSQL + EF Core with Fluent API configuration (13 migrations)
- [x] Domain models: World, Character, FamilyTree, Entity, Event, Location, Note, Language, DateSuffix, WorldPermission, ReadingSession, ReadingNote, User
- [x] Full CRUD API: 15 controllers, 60+ endpoints
- [x] JWT authentication (register + login)
- [x] World-level permission system (read/edit, transfer ownership)
- [x] React + TypeScript + Vite frontend
- [x] Login / register page
- [x] Home page with world list
- [x] World dashboard with tabbed navigation
- [x] World settings page (permissions management)
- [x] User settings page
- [x] Family tree canvas (React Flow, drag-and-drop, relationship edges)
- [x] Character detail page (genealogy, relationships)
- [x] Character list page with search
- [x] Location list and detail pages
- [x] Entity list page and Events page
- [x] Reading notes — persistent FAB for quick capture, dedicated page with note list and live entity workspace (search, edit, create)
- [x] World search — name-based search across all entity types, filters sections in place
- [x] FluentValidation integration and centralized exception handling middleware
- [x] Rich text editor for character biographies (Tiptap)
- [x] Mobile-responsive design — all pages usable at 360 px minimum; fullScreen dialogs on xs, BottomSheet on family tree and reading pages, touch pan/pinch-zoom on canvas

### Planned

- [ ] Photo uploads for characters
- [ ] Export family tree as image / PDF
- [ ] GEDCOM import/export
- [ ] User notification on photo upload failure

---

## Getting Started

> See [ONBOARDING.md](ONBOARDING.md) for a full step-by-step setup guide, including the local SMTP setup for testing the forgot-password flow without a real email provider.