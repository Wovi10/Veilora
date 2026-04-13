import { apiFetch } from './apiFetch';
import type { EntityDto, CreateEntityDto, UpdateEntityDto } from '../types/entity';

export async function getEntities(): Promise<EntityDto[]> {
  const res = await apiFetch('/api/entities');
  return res.json();
}

export async function getEntity(id: string): Promise<EntityDto> {
  const res = await apiFetch(`/api/entities/${id}`);
  return res.json();
}

export async function createEntity(dto: CreateEntityDto): Promise<EntityDto> {
  const res = await apiFetch('/api/entities', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  return res.json();
}

export async function updateEntity(id: string, dto: UpdateEntityDto): Promise<EntityDto> {
  const res = await apiFetch(`/api/entities/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  return res.json();
}

export async function deleteEntity(id: string): Promise<void> {
  await apiFetch(`/api/entities/${id}`, { method: 'DELETE' });
}
