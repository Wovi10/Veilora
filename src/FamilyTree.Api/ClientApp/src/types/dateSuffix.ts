export interface DateSuffixDto {
  id: string;
  name: string;
  abbreviation: string;
  order: number;
  isDefault: boolean;
  worldId: string;
}

export interface CreateDateSuffixDto {
  name: string;
  abbreviation: string;
  order: number;
  isDefault: boolean;
  worldId: string;
}

export interface UpdateDateSuffixDto {
  name: string;
  abbreviation: string;
  order: number;
  isDefault: boolean;
}
