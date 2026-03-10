import type { PersonDto } from './person';

export interface TreeDto {
  id: string;
  name: string;
  description?: string;
  createdAt: string;
  updatedAt: string;
}

export interface TreeWithPersonsDto {
  id: string;
  name: string;
  description?: string;
  persons: PersonDto[];
  createdAt: string;
  updatedAt: string;
}
