import type { CreatePersonDto, PersonDto } from '../types/person';
import { apiFetch } from './apiFetch';

export async function createPerson(dto: CreatePersonDto): Promise<PersonDto> {
  const res = await apiFetch('/api/persons', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || 'Failed to create person');
  }
  return res.json() as Promise<PersonDto>;
}

export async function addPersonToTree(treeId: string, personId: string): Promise<void> {
  const res = await apiFetch(`/api/trees/${treeId}/persons/${personId}`, {
    method: 'POST',
  });
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || 'Failed to add person to tree');
  }
}
