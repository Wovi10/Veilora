export type Gender = 'Male' | 'Female' | 'Other' | 'PreferNotToSay';

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
  createdAt: string;
  updatedAt: string;
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
