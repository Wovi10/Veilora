export type EntityType = 'Character' | 'Place' | 'Faction' | 'Event' | 'Concept';
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
  deathDate?: string;
  birthPlace?: string;
  residence?: string;
  gender?: Gender;
  biography?: string;
  profilePhotoUrl?: string;
  parent1Id?: string;
  parent2Id?: string;
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
  deathDate?: string;
  birthPlace?: string;
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
  deathDate?: string;
  birthPlace?: string;
  residence?: string;
  gender?: Gender;
  biography?: string;
  parent1Id?: string | null;
  parent2Id?: string | null;
}
