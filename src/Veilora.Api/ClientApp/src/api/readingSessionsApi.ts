import { apiFetch } from './apiFetch';
import type { ReadingSessionDto, ReadingNoteDto, CreateReadingSessionDto } from '../types/readingSession';

export async function startSession(dto: CreateReadingSessionDto): Promise<ReadingSessionDto> {
  const res = await apiFetch('/api/reading-sessions', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  return res.json();
}

export async function getActiveSession(): Promise<ReadingSessionDto | null> {
  try {
    const res = await apiFetch('/api/reading-sessions/active');
    if (res.status === 204) return null;
    return res.json();
  } catch {
    return null;
  }
}

export async function getAllSessions(): Promise<ReadingSessionDto[]> {
  const res = await apiFetch('/api/reading-sessions');
  return res.json();
}

export async function endSession(sessionId: string): Promise<void> {
  await apiFetch(`/api/reading-sessions/${sessionId}/end`, { method: 'POST' });
}

export async function getNotes(sessionId: string): Promise<ReadingNoteDto[]> {
  const res = await apiFetch(`/api/reading-sessions/${sessionId}/notes`);
  return res.json();
}

export async function addNote(sessionId: string, text: string): Promise<ReadingNoteDto> {
  const res = await apiFetch(`/api/reading-sessions/${sessionId}/notes`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ text }),
  });
  return res.json();
}

export async function deleteNote(noteId: string): Promise<void> {
  await apiFetch(`/api/reading-sessions/notes/${noteId}`, { method: 'DELETE' });
}
