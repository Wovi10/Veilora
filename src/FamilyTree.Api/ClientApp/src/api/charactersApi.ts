import { apiFetch } from './apiFetch';
import type { CharacterDto, CreateCharacterDto, UpdateCharacterDto } from '../types/character';

export async function getCharactersByWorld(worldId: string): Promise<CharacterDto[]> {
  const res = await apiFetch(`/api/characters?worldId=${worldId}`);
  return res.json();
}

export async function getCharacter(id: string): Promise<CharacterDto> {
  const res = await apiFetch(`/api/characters/${id}`);
  return res.json();
}

export async function createCharacter(dto: CreateCharacterDto): Promise<CharacterDto> {
  const res = await apiFetch('/api/characters', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  return res.json();
}

export async function updateCharacter(id: string, dto: UpdateCharacterDto): Promise<CharacterDto> {
  const res = await apiFetch(`/api/characters/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  return res.json();
}

export async function deleteCharacter(id: string): Promise<void> {
  await apiFetch(`/api/characters/${id}`, { method: 'DELETE' });
}
