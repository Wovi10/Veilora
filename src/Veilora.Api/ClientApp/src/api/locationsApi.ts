import { apiFetch } from './apiFetch';
import type { LocationDto, CreateLocationDto, UpdateLocationDto } from '../types/location';
import type { PagedResult } from '../types/paged';

export async function getLocationsByWorld(worldId: string): Promise<LocationDto[]> {
  const res = await apiFetch(`/api/locations/world/${worldId}`);
  return res.json();
}

export async function getLocationsByWorldPaged(worldId: string, page: number, pageSize: number, name?: string): Promise<PagedResult<LocationDto>> {
  const url = `/api/locations/world/${worldId}?page=${page}&pageSize=${pageSize}${name ? `&name=${encodeURIComponent(name)}` : ''}`;
  const res = await apiFetch(url);
  return res.json();
}

export async function getLocation(id: string): Promise<LocationDto> {
  const res = await apiFetch(`/api/locations/${id}`);
  return res.json();
}

export async function createLocation(dto: CreateLocationDto): Promise<LocationDto> {
  const res = await apiFetch('/api/locations', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  return res.json();
}

export async function updateLocation(id: string, dto: UpdateLocationDto): Promise<LocationDto> {
  const res = await apiFetch(`/api/locations/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  return res.json();
}

export async function deleteLocation(id: string): Promise<void> {
  await apiFetch(`/api/locations/${id}`, { method: 'DELETE' });
}
