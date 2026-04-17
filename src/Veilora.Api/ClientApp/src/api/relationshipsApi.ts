import { apiFetch } from './apiFetch';
import type { CreateRelationshipDto, RelationshipDto } from '../types/relationship';

export async function getRelationships(): Promise<RelationshipDto[]> {
  const res = await apiFetch('/api/relationships');
  return res.json();
}

export async function createRelationship(dto: CreateRelationshipDto): Promise<RelationshipDto> {
  const res = await apiFetch('/api/relationships', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  if (!res.ok) throw new Error('Failed to create relationship');
  return res.json() as Promise<RelationshipDto>;
}

export async function deleteRelationship(id: string): Promise<void> {
  await apiFetch(`/api/relationships/${id}`, { method: 'DELETE' });
}
