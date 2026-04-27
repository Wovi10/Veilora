import { apiFetch } from './apiFetch';

export interface UserInfoDto {
  id: string;
  email: string;
  displayName: string | null;
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
