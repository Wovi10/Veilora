import type { EntityRefDto } from './entityRef';
import type { LanguageDto } from './language';

export type EntityType = 'Character' | 'Place' | 'Group' | 'Event' | 'Concept';
export type Gender = 'Male' | 'Female' | 'Other' | 'Unknown';

export interface EntityDto {
  id: string;
  name: string;
  type: EntityType;
  worldId: string;
  description?: string;
  firstName?: string;
  lastName?: string;
  middleName?: string;
  maidenName?: string;
  species?: string;
  birthDate?: string;
  birthDateSuffix?: string;
  deathDate?: string;
  deathDateSuffix?: string;
  birthPlaceEntityId?: string;
  birthPlaceEntityName?: string;
  deathPlaceEntityId?: string;
  deathPlaceEntityName?: string;
  residence?: string;
  gender?: Gender;
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

export interface CreateEntityDto {
  name: string;
  type: EntityType;
  worldId: string;
  description?: string;
  firstName?: string;
  lastName?: string;
  middleName?: string;
  maidenName?: string;
  species?: string;
  birthDate?: string;
  birthDateSuffix?: string;
  deathDate?: string;
  deathDateSuffix?: string;
  residence?: string;
  gender?: Gender;
  biography?: string;
  parent1Id?: string;
  parent2Id?: string;
}

export interface UpdateEntityDto {
  name: string;
  type: EntityType;
  description?: string;
  firstName?: string;
  lastName?: string;
  middleName?: string;
  maidenName?: string;
  species?: string;
  birthDate?: string;
  birthDateSuffix?: string;
  deathDate?: string;
  deathDateSuffix?: string;
  birthPlaceEntityId?: string | null;
  deathPlaceEntityId?: string | null;
  residence?: string;
  gender?: Gender;
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
