import { apiFetch } from './apiFetch';
import type { DateSuffixDto, CreateDateSuffixDto, UpdateDateSuffixDto } from '../types/dateSuffix';

export async function getDateSuffixesByWorld(worldId: string): Promise<DateSuffixDto[]> {
  const res = await apiFetch(`/api/date-suffixes?worldId=${worldId}`);
  return res.json();
}

export async function createDateSuffix(dto: CreateDateSuffixDto): Promise<DateSuffixDto> {
  const res = await apiFetch('/api/date-suffixes', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  return res.json();
}

export async function updateDateSuffix(id: string, dto: UpdateDateSuffixDto): Promise<DateSuffixDto> {
  const res = await apiFetch(`/api/date-suffixes/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  return res.json();
}

export async function deleteDateSuffix(id: string): Promise<void> {
  await apiFetch(`/api/date-suffixes/${id}`, { method: 'DELETE' });
}
