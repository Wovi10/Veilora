export type EntityType = 'Group' | 'Event' | 'Concept';

export interface EntityDto {
  id: string;
  name: string;
  type: EntityType;
  worldId: string;
  description?: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateEntityDto {
  name: string;
  type: EntityType;
  worldId: string;
  description?: string;
}

export interface UpdateEntityDto {
  name: string;
  type: EntityType;
  description?: string;
}
