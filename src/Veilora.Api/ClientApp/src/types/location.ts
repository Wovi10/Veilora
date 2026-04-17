export interface LocationDto {
  id: string;
  name: string;
  worldId: string;
  description?: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateLocationDto {
  name: string;
  worldId: string;
  description?: string;
}

export interface UpdateLocationDto {
  name: string;
  description?: string;
}
