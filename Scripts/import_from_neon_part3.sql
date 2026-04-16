-- ============================================================
-- Neon → Local import script — Part 3
-- Run AFTER import_from_neon_part2.sql.
-- Fill in the INSERT sections as you receive data per table.
-- ============================================================

BEGIN;

-- ------------------------------------------------------------
-- 10. CharacterFamilyTrees
-- ------------------------------------------------------------
-- INSERT INTO "CharacterFamilyTrees" ("CharacterId", "FamilyTreeId", "PositionX", "PositionY")
-- VALUES (...);

-- ------------------------------------------------------------
-- 11. CharacterLocations
-- ------------------------------------------------------------
-- INSERT INTO "CharacterLocations" ("CharacterId", "LocationId")
-- VALUES (...);

-- ------------------------------------------------------------
-- 12. EntityAffiliations
-- ------------------------------------------------------------
-- INSERT INTO "EntityAffiliations" ("CharacterId", "GroupId")
-- VALUES (...);

-- ------------------------------------------------------------
-- 13. EntityLanguages
-- ------------------------------------------------------------
-- INSERT INTO "EntityLanguages" ("CharacterId", "LanguageId")
-- VALUES (...);

-- ------------------------------------------------------------
-- 14. WorldPermissions
-- ------------------------------------------------------------
-- INSERT INTO "WorldPermissions" ("Id", "WorldId", "UserId", "CanEdit", "CreatedAt", "UpdatedAt")
-- VALUES (...);

-- ------------------------------------------------------------
-- 15. TreePermissions
-- ------------------------------------------------------------
-- INSERT INTO "TreePermissions" ("Id", "TreeId", "UserId", "PermissionLevel", "GrantedAt", "CreatedAt", "UpdatedAt")
-- VALUES (...);

COMMIT;
