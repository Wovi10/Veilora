import type { TreeDto } from '../types/tree';

export async function getTrees(): Promise<TreeDto[]> {
  const res = await fetch('/api/trees');
  if (!res.ok) throw new Error('Failed to fetch trees');
  return res.json() as Promise<TreeDto[]>;
}
