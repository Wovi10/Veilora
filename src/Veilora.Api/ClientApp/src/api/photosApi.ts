import { apiFetch } from './apiFetch';
import type { PhotoDto, CreatePhotoDto, UpdatePhotoDto, ReorderPhotosDto, PhotoTargetType } from '../types/photo';

export async function getPhotos(entityType: PhotoTargetType, entityId: string): Promise<PhotoDto[]> {
  const res = await apiFetch(`/api/photos/${entityType}/${entityId}`);
  return res.json();
}

export async function addPhoto(entityType: PhotoTargetType, entityId: string, dto: CreatePhotoDto): Promise<PhotoDto> {
  const res = await apiFetch(`/api/photos/${entityType}/${entityId}`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  return res.json();
}

export async function updatePhoto(entityType: PhotoTargetType, entityId: string, photoId: string, dto: UpdatePhotoDto): Promise<PhotoDto> {
  const res = await apiFetch(`/api/photos/${entityType}/${entityId}/${photoId}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  return res.json();
}

export async function deletePhoto(entityType: PhotoTargetType, entityId: string, photoId: string): Promise<void> {
  await apiFetch(`/api/photos/${entityType}/${entityId}/${photoId}`, { method: 'DELETE' });
}

export async function reorderPhotos(entityType: PhotoTargetType, entityId: string, dto: ReorderPhotosDto): Promise<PhotoDto[]> {
  const res = await apiFetch(`/api/photos/${entityType}/${entityId}/reorder`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  return res.json();
}
