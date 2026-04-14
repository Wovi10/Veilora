import type { CharacterDto } from './character';

export interface FamilyTreeDto {
  id: string;
  name: string;
  description?: string;
  worldId: string;
  createdAt: string;
  updatedAt: string;
}

export interface CharacterInFamilyTreeDto {
  character: CharacterDto;
  positionX: number | null;
  positionY: number | null;
}

export interface FamilyTreeWithEntitiesDto {
  id: string;
  name: string;
  description?: string;
  worldId: string;
  characters: CharacterInFamilyTreeDto[];
  createdAt: string;
  updatedAt: string;
}

export interface CreateFamilyTreeDto {
  name: string;
  description?: string;
  worldId: string;
}

export interface UpdateEntityPositionDto {
  x: number;
  y: number;
}
