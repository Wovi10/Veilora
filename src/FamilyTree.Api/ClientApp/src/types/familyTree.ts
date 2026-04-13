import type { EntityDto } from './entity';

export interface FamilyTreeDto {
  id: string;
  name: string;
  description?: string;
  worldId: string;
  createdAt: string;
  updatedAt: string;
}

export interface EntityInFamilyTreeDto {
  entity: EntityDto;
  positionX: number | null;
  positionY: number | null;
}

export interface FamilyTreeWithEntitiesDto {
  id: string;
  name: string;
  description?: string;
  worldId: string;
  entities: EntityInFamilyTreeDto[];
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
