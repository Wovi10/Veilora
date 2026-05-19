export type PhotoTargetType = 'Character' | 'Location' | 'Entity';

export interface PhotoDto {
  id: string;
  entityId: string;
  entityType: PhotoTargetType;
  url: string;
  displayOrder: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreatePhotoDto {
  url: string;
}

export interface UpdatePhotoDto {
  url: string;
}

export interface ReorderPhotosDto {
  orderedIds: string[];
}
