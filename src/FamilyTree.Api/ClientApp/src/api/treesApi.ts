import type { TreeDto } from '../types/tree';
import { apiFetch } from './apiFetch';

export async function getTrees(): Promise<TreeDto[]> {
  const res = await apiFetch('/api/trees');
  if (!res.ok) throw new Error('Failed to fetch trees');
  return res.json() as Promise<TreeDto[]>;
}

export async function createTree(name: string, description?: string): Promise<TreeDto> {
  const res = await apiFetch('/api/trees', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ name, description: description || undefined }),
  });
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || 'Failed to create tree');
  }
  return res.json() as Promise<TreeDto>;
}
