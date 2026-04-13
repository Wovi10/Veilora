export interface WorldDto {
  id: string;
  name: string;
  author?: string;
  description?: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateWorldDto {
  name: string;
  author?: string;
  description?: string;
}

export interface UpdateWorldDto {
  name: string;
  author?: string;
  description?: string;
}
