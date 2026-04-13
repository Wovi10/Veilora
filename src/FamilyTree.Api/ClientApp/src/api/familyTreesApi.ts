import { apiFetch } from './apiFetch';
import type { FamilyTreeDto, FamilyTreeWithEntitiesDto, CreateFamilyTreeDto, UpdateEntityPositionDto } from '../types/familyTree';
import type { RelationshipDto } from '../types/relationship';

export async function getFamilyTrees(): Promise<FamilyTreeDto[]> {
  const res = await apiFetch('/api/family-trees');
  return res.json();
}

export async function getFamilyTree(id: string): Promise<FamilyTreeDto> {
  const res = await apiFetch(`/api/family-trees/${id}`);
  return res.json();
}

export async function getFamilyTreeWithEntities(id: string): Promise<FamilyTreeWithEntitiesDto> {
  const res = await apiFetch(`/api/family-trees/${id}/details`);
  return res.json();
}

export async function getFamilyTreeRelationships(familyTreeId: string): Promise<RelationshipDto[]> {
  const res = await apiFetch(`/api/family-trees/${familyTreeId}/relationships`);
  return res.json();
}

export async function createFamilyTree(dto: CreateFamilyTreeDto): Promise<FamilyTreeDto> {
  const res = await apiFetch('/api/family-trees', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  return res.json();
}

export async function updateEntityPosition(
  familyTreeId: string,
  entityId: string,
  dto: UpdateEntityPositionDto,
): Promise<void> {
  await apiFetch(`/api/family-trees/${familyTreeId}/entities/${entityId}/position`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
}

export async function addEntityToFamilyTree(familyTreeId: string, entityId: string): Promise<void> {
  await apiFetch(`/api/family-trees/${familyTreeId}/entities/${entityId}`, { method: 'POST' });
}

export async function removeEntityFromFamilyTree(familyTreeId: string, entityId: string): Promise<void> {
  await apiFetch(`/api/family-trees/${familyTreeId}/entities/${entityId}`, { method: 'DELETE' });
}
