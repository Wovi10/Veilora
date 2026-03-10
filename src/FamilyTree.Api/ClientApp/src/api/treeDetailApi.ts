import type { TreeWithPersonsDto } from '../types/tree';
import type { RelationshipDto } from '../types/relationship';

export async function getTreeDetails(id: string): Promise<TreeWithPersonsDto> {
  const res = await fetch(`/api/trees/${id}/details`);
  if (!res.ok) throw new Error('Failed to fetch tree details');
  return res.json() as Promise<TreeWithPersonsDto>;
}

export async function getTreeRelationships(treeId: string): Promise<RelationshipDto[]> {
  const res = await fetch(`/api/trees/${treeId}/relationships`);
  if (!res.ok) throw new Error('Failed to fetch tree relationships');
  return res.json() as Promise<RelationshipDto[]>;
}
