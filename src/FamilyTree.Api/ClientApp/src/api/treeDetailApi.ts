import type { TreeWithPersonsDto } from '../types/tree';
import type { RelationshipDto } from '../types/relationship';
import { apiFetch } from './apiFetch';

export async function getTreeDetails(id: string): Promise<TreeWithPersonsDto> {
  const res = await apiFetch(`/api/trees/${id}/details`);
  if (!res.ok) throw new Error('Failed to fetch tree details');
  return res.json() as Promise<TreeWithPersonsDto>;
}

export async function getTreeRelationships(treeId: string): Promise<RelationshipDto[]> {
  const res = await apiFetch(`/api/trees/${treeId}/relationships`);
  if (!res.ok) throw new Error('Failed to fetch tree relationships');
  return res.json() as Promise<RelationshipDto[]>;
}

export async function updatePersonPosition(treeId: string, personId: string, x: number, y: number): Promise<void> {
  const res = await apiFetch(`/api/trees/${treeId}/persons/${personId}/position`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ x, y }),
  });
  if (!res.ok) throw new Error('Failed to update person position');
}
