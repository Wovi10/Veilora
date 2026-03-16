export type Gender = 'Male' | 'Female' | 'Other' | 'Unknown';

export interface PersonDto {
  id: string;
  firstName: string;
  middleName?: string;
  lastName: string;
  maidenName?: string;
  birthDate?: string;
  deathDate?: string;
  birthPlace?: string;
  residence?: string;
  gender: Gender;
  biography?: string;
  profilePhotoUrl?: string;
  parent1Id?: string;
  parent2Id?: string;
  createdAt: string;
  updatedAt: string;
}

export interface UpdatePersonDto {
  firstName: string;
  middleName?: string;
  lastName: string;
  maidenName?: string;
  birthDate?: string;
  deathDate?: string;
  birthPlace?: string;
  residence?: string;
  gender: Gender;
  biography?: string;
  parent1Id?: string | null;
  parent2Id?: string | null;
}

export interface CreatePersonDto {
  firstName: string;
  middleName?: string;
  lastName: string;
  maidenName?: string;
  birthDate?: string;
  deathDate?: string;
  birthPlace?: string;
  residence?: string;
  gender: Gender;
  biography?: string;
}
