import type { PersonDto } from './person';

export interface TreeDto {
  id: string;
  name: string;
  description?: string;
  createdAt: string;
  updatedAt: string;
}

export interface PersonInTreeDto {
  person: PersonDto;
  positionX: number | null;
  positionY: number | null;
}

export interface TreeWithPersonsDto {
  id: string;
  name: string;
  description?: string;
  persons: PersonInTreeDto[];
  createdAt: string;
  updatedAt: string;
}
