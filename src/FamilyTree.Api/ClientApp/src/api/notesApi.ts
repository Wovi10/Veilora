import { apiFetch } from './apiFetch';
import type { NoteDto, CreateNoteDto } from '../types/note';

export async function getNotes(): Promise<NoteDto[]> {
  const res = await apiFetch('/api/notes');
  return res.json();
}

export async function createNote(dto: CreateNoteDto): Promise<NoteDto> {
  const res = await apiFetch('/api/notes', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  return res.json();
}

export async function deleteNote(id: string): Promise<void> {
  await apiFetch(`/api/notes/${id}`, { method: 'DELETE' });
}
