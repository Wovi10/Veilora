import { apiFetch } from './apiFetch';
import type { LanguageDto } from '../types/language';

export async function getLanguagesByWorld(worldId: string): Promise<LanguageDto[]> {
  const res = await apiFetch(`/api/languages?worldId=${worldId}`);
  return res.json();
}

export async function getOrCreateLanguage(name: string, worldId: string): Promise<LanguageDto> {
  const res = await apiFetch('/api/languages', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ name, worldId }),
  });
  return res.json();
}
