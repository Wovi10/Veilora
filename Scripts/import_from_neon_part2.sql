-- ============================================================
-- Neon → Local import script — Part 2
-- Run AFTER import_from_neon.sql (Users, Worlds, Locations already imported).
-- Fill in the INSERT sections as you receive data per table.
-- ============================================================

BEGIN;

-- ------------------------------------------------------------
-- 4. Languages
-- ------------------------------------------------------------
-- INSERT INTO "Languages" ("Id", "Name", "WorldId", "CreatedAt", "UpdatedAt")
-- VALUES (...);

-- ------------------------------------------------------------
-- 5. Entities
-- ------------------------------------------------------------
-- INSERT INTO "Entities" ("Id", "Name", "Type", "Description", "WorldId", "CreatedAt", "UpdatedAt")
-- VALUES (...);

-- ------------------------------------------------------------
-- 6. Characters (insert in topological order: parents before children)
-- Col order: Id,Name,WorldId,Description,FirstName,LastName,MiddleName,MaidenName,OtherNames,
--            Species,Gender,BirthDate,BirthDateSuffix,DeathDate,DeathDateSuffix,
--            BirthPlaceLocationId,DeathPlaceLocationId,Residence,Biography,ProfilePhotoUrl,
--            Position,Height,HairColour,Parent1Id,Parent2Id,CreatedAt,UpdatedAt
-- ------------------------------------------------------------
-- INSERT INTO "Characters" ("Id","Name","WorldId","Description","FirstName","LastName","MiddleName","MaidenName","OtherNames","Species","Gender","BirthDate","BirthDateSuffix","DeathDate","DeathDateSuffix","BirthPlaceLocationId","DeathPlaceLocationId","Residence","Biography","ProfilePhotoUrl","Position","Height","HairColour","Parent1Id","Parent2Id","CreatedAt","UpdatedAt")
-- VALUES (...);

-- ------------------------------------------------------------
-- 7. FamilyTrees
-- ------------------------------------------------------------
-- INSERT INTO "FamilyTrees" ("Id", "Name", "Description", "WorldId", "CreatedBy", "CreatedAt", "UpdatedAt")
-- VALUES (...);

-- ------------------------------------------------------------
-- 8. Notes
-- ------------------------------------------------------------
-- INSERT INTO "Notes" ("Id", "Content", "WorldId", "EntityId", "CreatedAt", "UpdatedAt")
-- VALUES (...);

-- ------------------------------------------------------------
-- 9. Relationships
-- ------------------------------------------------------------
-- INSERT INTO "Relationships" ("Id", "Character1Id", "Character2Id", "RelationshipType", "StartDate", "EndDate", "Notes", "CreatedAt", "UpdatedAt")
-- VALUES (...);

COMMIT;
