import { apiFetch } from './apiFetch';
import type { WorldPermissionDto, UpsertWorldPermissionDto } from '../types/worldPermission';

export async function getPermissions(worldId: string): Promise<WorldPermissionDto[]> {
  const res = await apiFetch(`/api/worlds/${worldId}/permissions`);
  if (!res.ok) throw new Error('Failed to fetch permissions.');
  return res.json();
}

export async function addPermissionByEmail(worldId: string, email: string, canEdit: boolean): Promise<WorldPermissionDto> {
  const res = await apiFetch(`/api/worlds/${worldId}/permissions`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, canEdit }),
  });
  if (res.status === 404) throw new NotFoundError();
  if (!res.ok) throw new Error('Failed to add permission.');
  return res.json();
}

export class NotFoundError extends Error {}

export async function upsertPermission(worldId: string, userId: string, dto: UpsertWorldPermissionDto): Promise<WorldPermissionDto> {
  const res = await apiFetch(`/api/worlds/${worldId}/permissions/${userId}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  if (!res.ok) throw new Error('Failed to save permission.');
  return res.json();
}

export async function deletePermission(worldId: string, userId: string): Promise<void> {
  const res = await apiFetch(`/api/worlds/${worldId}/permissions/${userId}`, { method: 'DELETE' });
  if (!res.ok) throw new Error('Failed to delete permission.');
}
