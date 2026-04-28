import { apiFetch } from './apiFetch';

export interface UserInfoDto {
  id: string;
  email: string;
  displayName: string | null;
}

export interface UserMeDto {
  id: string;
  email: string;
  displayName: string | null;
  backupUserEmail: string | null;
  backupUserDisplayName: string | null;
}

export async function getMe(): Promise<UserMeDto> {
  const res = await apiFetch('/api/users/me');
  return res.json();
}

export async function updateDisplayName(displayName: string | null): Promise<UserInfoDto> {
  const res = await apiFetch('/api/users/me/display-name', {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ displayName }),
  });
  return res.json();
}

export async function changePassword(currentPassword: string, newPassword: string): Promise<void> {
  await apiFetch('/api/users/me/password', {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ currentPassword, newPassword }),
  });
}

export async function setBackupUser(email: string): Promise<UserMeDto> {
  const res = await apiFetch('/api/users/me/backup', {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email }),
  });
  return res.json();
}

export async function removeBackupUser(): Promise<UserMeDto> {
  const res = await apiFetch('/api/users/me/backup', { method: 'DELETE' });
  return res.json();
}

export async function deleteAccount(): Promise<void> {
  await apiFetch('/api/users/me', { method: 'DELETE' });
}
