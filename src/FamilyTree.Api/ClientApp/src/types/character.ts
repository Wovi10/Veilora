import type { EntityRefDto } from './entityRef';
import type { LanguageDto } from './language';

export type Gender = 'Male' | 'Female' | 'Other' | 'Unknown';

export interface CharacterDto {
  id: string;
  name: string;
  worldId: string;
  description?: string;
  firstName?: string;
  lastName?: string;
  middleName?: string;
  maidenName?: string;
  species?: string;
  gender?: Gender;
  birthDate?: string;
  birthDateSuffix?: string;
  deathDate?: string;
  deathDateSuffix?: string;
  birthPlaceEntityId?: string;
  birthPlaceEntityName?: string;
  deathPlaceEntityId?: string;
  deathPlaceEntityName?: string;
  residence?: string;
  biography?: string;
  profilePhotoUrl?: string;
  otherNames?: string;
  position?: string;
  height?: string;
  hairColour?: string;
  parent1Id?: string;
  parent2Id?: string;
  locations: EntityRefDto[];
  affiliations: EntityRefDto[];
  languages: LanguageDto[];
  spouses: EntityRefDto[];
  children: EntityRefDto[];
  createdAt: string;
  updatedAt: string;
}

export interface CreateCharacterDto {
  name: string;
  worldId: string;
  description?: string;
  firstName?: string;
  lastName?: string;
  middleName?: string;
  maidenName?: string;
  species?: string;
  gender?: Gender;
  birthDate?: string;
  birthDateSuffix?: string;
  deathDate?: string;
  deathDateSuffix?: string;
  residence?: string;
  biography?: string;
  parent1Id?: string;
  parent2Id?: string;
}

export interface UpdateCharacterDto {
  name: string;
  description?: string;
  firstName?: string;
  lastName?: string;
  middleName?: string;
  maidenName?: string;
  species?: string;
  gender?: Gender;
  birthDate?: string;
  birthDateSuffix?: string;
  deathDate?: string;
  deathDateSuffix?: string;
  birthPlaceEntityId?: string | null;
  deathPlaceEntityId?: string | null;
  residence?: string;
  biography?: string;
  profilePhotoUrl?: string;
  otherNames?: string;
  position?: string;
  height?: string;
  hairColour?: string;
  parent1Id?: string | null;
  parent2Id?: string | null;
  locationIds: string[];
  affiliationIds: string[];
  languageIds: string[];
  spouseIds: string[];
  childIds: string[];
}
