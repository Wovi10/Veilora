import { apiFetch } from './apiFetch';
import type { WorldDto, CreateWorldDto, UpdateWorldDto } from '../types/world';

export async function getWorlds(): Promise<WorldDto[]> {
  const res = await apiFetch('/api/worlds');
  return res.json();
}

export async function getWorld(id: string): Promise<WorldDto> {
  const res = await apiFetch(`/api/worlds/${id}`);
  return res.json();
}

export async function createWorld(dto: CreateWorldDto): Promise<WorldDto> {
  const res = await apiFetch('/api/worlds', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  return res.json();
}

export async function updateWorld(id: string, dto: UpdateWorldDto): Promise<WorldDto> {
  const res = await apiFetch(`/api/worlds/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  return res.json();
}

export async function deleteWorld(id: string): Promise<void> {
  await apiFetch(`/api/worlds/${id}`, { method: 'DELETE' });
}

export async function transferOwnership(id: string, email: string): Promise<boolean> {
  const res = await apiFetch(`/api/worlds/${id}/owner`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email }),
  });
  if (res.status === 404) return false;
  if (!res.ok) throw new Error('Failed to transfer ownership.');
  return true;
}
