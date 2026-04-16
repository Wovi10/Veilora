-- ============================================================
-- Neon → Local import script — Part 1
-- Run this on your LOCAL database.
-- Tables are cleared and re-inserted in FK-safe order.
-- Fill in the INSERT sections as you receive data per table.
-- ============================================================

BEGIN;

-- ============================================================
-- STEP 1: Clear all tables (reverse FK order)
-- ============================================================

DELETE FROM "TreePermissions";
DELETE FROM "WorldPermissions";
DELETE FROM "EntityLanguages";
DELETE FROM "EntityAffiliations";
DELETE FROM "CharacterLocations";
DELETE FROM "CharacterFamilyTrees";
DELETE FROM "Relationships";
DELETE FROM "Notes";
DELETE FROM "FamilyTrees";
DELETE FROM "Characters";
DELETE FROM "Entities";
DELETE FROM "Languages";
DELETE FROM "Locations";
DELETE FROM "Worlds";
DELETE FROM "Users";

-- ============================================================
-- STEP 2: Insert data (FK-safe order)
-- ============================================================

-- ------------------------------------------------------------
-- 1. Users
-- ------------------------------------------------------------
-- INSERT INTO "Users" ("Id", "Email", "PasswordHash", "DisplayName", "CreatedAt", "UpdatedAt")
-- VALUES (...);

-- ------------------------------------------------------------
-- 2. Worlds
-- ------------------------------------------------------------
-- INSERT INTO "Worlds" ("Id", "Name", "Author", "Description", "CreatedById", "CreatedAt", "UpdatedAt")
-- VALUES (...);

-- ------------------------------------------------------------
-- 3. Locations
-- ------------------------------------------------------------
-- INSERT INTO "Locations" ("Id", "Name", "WorldId", "Description", "CreatedAt", "UpdatedAt")
-- VALUES (...);

COMMIT;
