export interface EventDto {
  id: string;
  name: string;
  worldId: string;
  description?: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateEventDto {
  name: string;
  worldId: string;
  description?: string;
}

export interface UpdateEventDto {
  name: string;
  description?: string;
}
