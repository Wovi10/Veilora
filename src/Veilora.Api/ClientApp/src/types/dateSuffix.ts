export interface DateSuffixDto {
  id: string;
  name: string;
  abbreviation: string;
  anchorYear: number;
  scale: number;
  isReversed: boolean;
  isDefault: boolean;
  worldId: string;
}

export interface CreateDateSuffixDto {
  name: string;
  abbreviation: string;
  anchorYear: number;
  scale: number;
  isReversed: boolean;
  isDefault: boolean;
  worldId: string;
}

export interface UpdateDateSuffixDto {
  name: string;
  abbreviation: string;
  anchorYear: number;
  scale: number;
  isReversed: boolean;
  isDefault: boolean;
}
