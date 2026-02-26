# Family Tree Application - Project Specification

**Last Updated:** February 26, 2026  
**Project Status:** Planning Phase

---

## Project Overview

A web-based family tree application that allows users to create, visualize, and manage family relationships through an interactive interface.

### Key Decisions Summary
- **User Base:** Single-user application (personal use)
- **Multiple Trees:** Support for creating multiple independent family trees
- **Tree Size:** Optimized for small trees (under 50 people)
- **TOP PRIORITY:** Smooth, effortless navigation (pan, zoom, focus) - must feel like Google Maps
- **Relationship Types:** Comprehensive support including biological, adopted, step-relationships, and extended family (godparents, guardians, close friends)
- **Data Storage:** Names, dates, locations (birthplace/residence), and rich text biographies
- **Visualization:** Interactive graph only with React Flow
- **Layouts:** Top-down, bottom-up, and left-to-right horizontal
- **Phase 1 Priority:** Simple and working quickly (but navigation must be excellent)
- **Core Features v1:** CRUD operations, search/filter, export to image/PDF, mini-map, keyboard shortcuts
- **Deferred to Phase 2+:** Photo storage, mobile interface, GEDCOM import

---

## Technology Stack

### Backend
- **Framework:** ASP.NET Core (Web API)
- **Database:** PostgreSQL
- **ORM:** Entity Framework Core
- **Language:** C#

### Frontend
- **Framework:** React
- **Language:** TypeScript (recommended) or JavaScript
- **Visualization Library:** React Flow
- **Styling:** TBD (Tailwind CSS, Material-UI, or custom)

### Infrastructure (Future Consideration)
- Hosting: TBD (Azure, AWS, or other)
- File Storage: TBD (for photos/documents)
- Authentication: TBD (Identity Server, Auth0, or ASP.NET Core Identity)

---

## Database Schema (Draft)

### Core Tables

#### Person
```sql
- id (UUID/GUID, Primary Key)
- first_name (VARCHAR)
- middle_name (VARCHAR, nullable)
- last_name (VARCHAR)
- maiden_name (VARCHAR, nullable)
- birth_date (DATE, nullable)
- death_date (DATE, nullable)
- birth_place (VARCHAR, nullable) - City, Country format
- residence (VARCHAR, nullable) - Current/last known residence
- gender (VARCHAR or ENUM)
- biography (TEXT, nullable) - Rich text HTML or Markdown
- profile_photo_url (VARCHAR, nullable) - For Phase 2
- created_at (TIMESTAMP)
- updated_at (TIMESTAMP)
```

#### Relationship
```sql
- id (UUID/GUID, Primary Key)
- person1_id (UUID, Foreign Key -> Person)
- person2_id (UUID, Foreign Key -> Person)
- relationship_type (ENUM: parent_child_biological, parent_child_adopted, 
                           spouse, partner, step_parent, step_child,
                           godparent, guardian, close_friend)
- is_biological (BOOLEAN, default true for parent_child)
- start_date (DATE, nullable) - for marriages/partnerships
- end_date (DATE, nullable) - for divorces/separations
- notes (TEXT, nullable)
- created_at (TIMESTAMP)
- updated_at (TIMESTAMP)
```

#### TreePermission (For future sharing - Phase 3)
```sql
- id (UUID/GUID, Primary Key)
- tree_id (UUID, Foreign Key -> Tree)
- user_id (UUID, Foreign Key -> User)
- permission_level (ENUM: owner, editor, viewer)
- granted_at (TIMESTAMP)
```

**Note:** This allows multiple users to access the same tree with different permission levels.

#### User (For future multi-user support - Phase 3)
```sql
- id (UUID/GUID, Primary Key)
- email (VARCHAR, UNIQUE, NOT NULL)
- password_hash (VARCHAR, NOT NULL)
- display_name (VARCHAR)
- created_at (TIMESTAMP)
- updated_at (TIMESTAMP)
```

#### Tree
```sql
- id (UUID/GUID, Primary Key)
- name (VARCHAR, NOT NULL) - e.g., "Smith Family", "Maternal Line"
- description (TEXT, nullable)
- created_by (UUID, Foreign Key -> User) - For future multi-user support
- created_at (TIMESTAMP)
- updated_at (TIMESTAMP)
```

#### PersonTree (Junction table - assigns people to trees)
```sql
- person_id (UUID, Foreign Key -> Person)
- tree_id (UUID, Foreign Key -> Tree)
- PRIMARY KEY (person_id, tree_id)
```

**Note:** A person can exist in multiple trees. For example, if you have a "Smith Family" tree and a "Jones Family" tree, and someone married between the families, they could appear in both trees.

### PostgreSQL Specific Features to Leverage
- **Recursive CTEs** for ancestor/descendant queries
- **ltree extension** for hierarchical tree paths (optional optimization)
- **JSONB columns** for flexible metadata storage

### Data Model Design Notes

**Person Sharing Across Trees:**
- A single Person record can belong to multiple Trees via the PersonTree junction table
- This is useful when someone appears in multiple family lines (e.g., through marriage)
- Relationships (parent-child, spouse, etc.) are global and not tree-specific
- When viewing a specific tree, you filter persons and relationships to only show those in that tree

**Example Scenario:**
- John Smith appears in both "Smith Family Tree" and "Johnson Family Tree" (married a Johnson)
- John exists once in the Person table
- Two entries in PersonTree: (John, SmithTree) and (John, JohnsonTree)
- His relationships (spouse, children) are visible in both trees if the related persons are also in those trees

**Alternative Approach (if you want isolation):**
- Keep trees completely separate (no person sharing)
- Duplicate Person records across trees
- Simpler queries but more data duplication
- Consider this if you never want overlap between trees

---

## Key Design Decisions to Make

### 1. Data Modeling
- [x] **Relationship types to support:**
  - Parent-child (biological)
  - Spouse/Partner
  - Adoptions
  - Step-relationships
  - Extended relationships (godparents, guardians, close friends)
- [x] Support multiple marriages/partners per person
- [x] Store location information (birthplace, residence)
- [x] Rich text biography/stories for each person
- [ ] Timeline view of life events - Future phase

### 2. User Management
- [x] **Single-user application** (for now)
- [x] **Multiple separate family trees** support required
- [ ] Privacy levels for different family members? - Not needed for v1
- [ ] Collaboration features (multiple editors)? - Future consideration
- [x] Private trees only (no public sharing in v1)

### 3. Features (Priority)
#### Phase 1 (MVP) - Priority: Simple and Working Quickly
**CRITICAL: Smooth navigation is top priority - must feel effortless**

- [x] Create/Read/Update/Delete persons
- [x] Store basic info: name, birth/death dates, gender
- [x] Store location information (birthplace, residence)
- [x] Rich text biography field
- [x] Define parent-child relationships (including biological/adopted)
- [x] Define spouse/partner relationships
- [x] Define step-relationships
- [x] Define extended relationships (godparent, guardian, close friend)
- [x] Interactive tree visualization with React Flow
- [x] **PRIORITY: Excellent navigation controls:**
  - Smooth pan/drag (click and drag to move around)
  - Smooth zoom (mouse wheel, pinch on trackpad)
  - Zoom controls (buttons for zoom in/out/fit view)
  - Mini-map for orientation on larger trees
  - "Focus on person" - click person to center and highlight them
  - Keyboard shortcuts (arrow keys to pan, +/- to zoom)
  - Reset view button (fit entire tree in view)
  - Smooth animations when moving between views
- [x] Search and filter people
- [x] Export tree as image or PDF
- [x] Multiple tree layout options:
  - Top-down (ancestors above)
  - Bottom-up (ancestors below) 
  - Left-to-right horizontal
- [x] Multiple separate family trees (not connected)
- [x] Expected tree size: Small (under 50 people)

#### Phase 2 - Comprehensive Features
- [ ] Photo uploads and gallery (deferred from v1)
- [ ] Document attachments
- [ ] Timeline view of life events
- [ ] Advanced search with filters
- [ ] Undo/redo functionality
- [ ] Tree comparison view
- [ ] Statistics and reports (e.g., longest lifespan, family size)

#### Phase 3 - Advanced & Mobile
- [ ] Mobile-friendly responsive design (3rd priority)
- [ ] GEDCOM import/export (4th priority - genealogy standard format)
- [ ] User authentication (if opening to family members)
- [ ] Sharing and collaboration
- [ ] Cloud storage for photos (Azure Blob or AWS S3)
- [ ] Advanced querying and reports
- [ ] Historical records integration

### 4. Navigation & User Experience (TOP PRIORITY)
**Problem:** Most family tree apps have poor navigation - they're clunky, slow, and frustrating to use.

**Solution:** Make navigation feel like using Google Maps - smooth, intuitive, responsive.

#### Must-Have Navigation Features (v1):
- [x] **Smooth panning:** Click and drag anywhere to move around
- [x] **Smooth zooming:** Mouse wheel, trackpad pinch, zoom buttons
- [x] **Mini-map:** Small overview in corner showing current viewport position
- [x] **Focus feature:** Click person → tree centers and highlights them with smooth animation
- [x] **Fit view button:** Instantly fit entire tree in viewport
- [x] **Keyboard shortcuts:**
  - Arrow keys: Pan in direction
  - `+` / `-`: Zoom in/out
  - `0`: Reset to fit view
  - `F`: Focus on selected person
- [x] **Visual feedback:**
  - Smooth animations (not instant jumps)
  - Show viewport boundaries clearly
  - Highlight hovered nodes
  - Clear indication of selected person
- [x] **Performance:** 60 FPS even with 50 people on screen

#### React Flow Built-in Features to Leverage:
- `MiniMap` component
- `Controls` component (zoom buttons)
- `Background` component (grid/dots for spatial reference)
- `useReactFlow` hook for programmatic navigation
- Smooth zoom/pan animations built-in

#### Nice-to-Have Enhancements (Phase 2):
- [ ] Breadcrumb trail showing navigation path
- [ ] "Back" button to return to previous view
- [ ] Save favorite views/bookmarks
- [ ] Touch gestures for mobile (pinch, swipe)
- [ ] Animated "fly to" transitions between persons

### 5. Visualization Strategy
- [x] **Client-side layout calculation** (React Flow handles this)
- [x] **Tree layouts to support:**
  - Top-down (ancestors above)
  - Bottom-up (ancestors below)
  - Left-to-right horizontal
- [x] **Expected tree size:** Small (under 50 people) - no lazy loading needed for v1
- [x] **View modes:** Full tree with ability to focus on specific person
- [ ] Collapsible branches - Future enhancement

### 6. File Storage
- [x] **Not storing photos/documents in v1** - Deferred to Phase 2
- [ ] Future consideration: Cloud storage (Azure Blob or AWS S3) for scalability

---

## API Endpoints (Draft)

### Person Management
```
GET    /api/persons                      - List all persons (across all trees)
GET    /api/trees/{treeId}/persons       - List persons in specific tree
GET    /api/persons/{id}                 - Get person details
POST   /api/persons                      - Create new person
PUT    /api/persons/{id}                 - Update person
DELETE /api/persons/{id}                 - Delete person
POST   /api/trees/{treeId}/persons/{id}  - Add existing person to tree
DELETE /api/trees/{treeId}/persons/{id}  - Remove person from tree (doesn't delete person)
GET    /api/persons/{id}/ancestors       - Get all ancestors
GET    /api/persons/{id}/descendants     - Get all descendants
```

### Relationship Management
```
GET    /api/relationships                    - List all relationships
GET    /api/trees/{treeId}/relationships     - List relationships in specific tree
POST   /api/relationships                    - Create relationship
PUT    /api/relationships/{id}               - Update relationship
DELETE /api/relationships/{id}               - Delete relationship
GET    /api/persons/{id}/relationships       - Get all relationships for a person
```

### Tree Visualization
```
GET    /api/trees                        - List all family trees
GET    /api/trees/{treeId}               - Get specific family tree data
POST   /api/trees                        - Create new family tree
PUT    /api/trees/{treeId}               - Update tree metadata
DELETE /api/trees/{treeId}               - Delete family tree
GET    /api/trees/{treeId}/persons       - Get all persons in a tree
GET    /api/trees/{treeId}/export/image  - Export tree as image
GET    /api/trees/{treeId}/export/pdf    - Export tree as PDF
```

---

## Frontend Architecture

### Component Structure (Draft)
```
src/
├── components/
│   ├── Tree/
│   │   ├── FamilyTreeViewer.tsx      - Main React Flow component
│   │   ├── PersonNode.tsx            - Custom node for React Flow
│   │   ├── RelationshipEdge.tsx      - Custom edge styling
│   │   ├── TreeControls.tsx          - Zoom, layout options, fit view
│   │   ├── TreeMiniMap.tsx           - Mini-map for navigation
│   │   ├── TreeToolbar.tsx           - Top toolbar with actions
│   │   └── NavigationPanel.tsx       - Keyboard shortcuts help overlay
│   ├── Person/
│   │   ├── PersonCard.tsx            - Person details display
│   │   ├── PersonForm.tsx            - Create/edit form
│   │   └── PersonList.tsx            - List view for search results
│   └── Layout/
│       ├── Header.tsx
│       ├── Sidebar.tsx
│       └── Layout.tsx
├── services/
│   ├── api.ts                        - Axios/fetch wrapper
│   ├── personService.ts
│   └── relationshipService.ts
├── hooks/
│   ├── useFamilyTree.ts
│   ├── usePersons.ts
│   ├── useTreeNavigation.ts          - Custom hook for navigation controls
│   └── useKeyboardShortcuts.ts       - Keyboard navigation
├── types/
│   ├── Person.ts
│   └── Relationship.ts
└── utils/
    ├── treeLayout.ts                 - Layout algorithm helpers
    └── navigationHelpers.ts          - Focus, center, zoom utilities
```

### State Management
- [ ] Decision needed: Context API, Redux, Zustand, or React Query?
- [ ] Consider React Query for server state management

---

## Development Phases

### Phase 1: Foundation (MVP)
1. Set up .NET Core API project
2. Configure PostgreSQL database and EF Core
3. Create Person and Relationship models
4. Implement basic CRUD API endpoints
5. Set up React project with TypeScript
6. Integrate React Flow
7. Create basic person nodes and relationship edges
8. Implement create/edit person form
9. Display simple family tree

**Deliverable:** Working application where you can add people, define relationships, and see a basic tree visualization

### Phase 2: Enhanced Features
1. Photo upload functionality
2. Rich text biography editor
3. Search and filtering
4. Improved tree layouts
5. Mobile responsive design

### Phase 3: Advanced Features
1. User authentication
2. Multiple family trees
3. GEDCOM import/export
4. Sharing capabilities
5. Advanced querying and reports

---

## Technical Considerations

### Performance
- Index foreign keys and frequently queried fields
- Implement pagination for large person lists
- Consider caching strategies for tree queries
- Lazy load tree branches for very large families

### Security
- Input validation on all forms
- Parameterized queries (EF Core handles this)
- CORS configuration
- Rate limiting on API endpoints
- File upload validation (size, type)

### Testing Strategy
- Unit tests for business logic
- Integration tests for API endpoints
- E2E tests for critical user flows
- Consider: xUnit for backend, Jest/React Testing Library for frontend

---

## Next Steps

1. [ ] Finalize data model decisions (relationship types, multiple marriages, etc.)
2. [ ] Set up development environment
3. [ ] Create GitHub repository
4. [ ] Initialize .NET Core API project
5. [ ] Initialize React project
6. [ ] Design database schema in detail
7. [ ] Create EF Core migrations
8. [ ] Build first API endpoint
9. [ ] Build first React component
10. [ ] Integrate React Flow with sample data

---

## Questions to Answer

1. ✅ Will this be a personal project or shared with others? **Personal - single user with multiple trees**
2. ✅ How many people do you expect in a typical tree? **Small trees (under 50 people)**
3. ❌ Do you need mobile app versions eventually? **Phase 3 - lower priority**
4. ✅ Privacy requirements - who can see what? **Private, single-user only for v1**
5. ❌ Do you want to integrate with existing genealogy services? **GEDCOM import deferred to Phase 3**
6. ❌ Budget for hosting/cloud services? **Local development for v1, cloud consideration for Phase 2+**

### Additional Decisions Made:
- ✅ **Relationship types:** All types including biological, adopted, step, spouse, partner, godparent, guardian, close friend
- ✅ **Person information:** Name, dates, gender, birthplace, residence, rich text biography
- ✅ **View mode:** Interactive graph visualization only (no list view in v1)
- ✅ **Tree layouts:** Top-down, bottom-up, and left-to-right horizontal
- ✅ **File storage:** Deferred to Phase 2
- ✅ **Core v1 features:** CRUD operations, search/filter, export to image/PDF

---

## Resources

### Documentation
- [React Flow Documentation](https://reactflow.dev/)
- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)

### GEDCOM Standard
- [GEDCOM Specification](https://www.gedcom.org/) - Standard for genealogy data exchange

### Inspiration
- Ancestry.com
- MyHeritage
- FamilySearch.org

---

## Notes

- Keep the initial scope small - focus on core functionality first
- Consider starting with a single-user application before adding auth
- React Flow is very flexible - can be customized extensively
- PostgreSQL's recursive queries are perfect for family tree traversal
- Consider using UUIDs for IDs to avoid conflicts if merging trees later

