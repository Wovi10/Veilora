import { apiFetch } from './apiFetch';
import type { EntityType } from '../types/entity';

export interface EntitySearchItem {
  id: string;
  name: string;
  type: EntityType;
}

export interface NamedItem {
  id: string;
  name: string;
}

export interface WorldSearchResult {
  entities: EntitySearchItem[];
  characters: NamedItem[];
  locations: NamedItem[];
  events: NamedItem[];
}

export async function searchWorld(worldId: string, q: string): Promise<WorldSearchResult> {
  const res = await apiFetch(`/api/search?worldId=${worldId}&q=${encodeURIComponent(q)}`);
  return res.json();
}
