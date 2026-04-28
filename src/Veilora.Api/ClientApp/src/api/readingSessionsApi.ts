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

export async function getCurrentSession(): Promise<ReadingSessionDto | null> {
  try {
    const res = await apiFetch('/api/reading-sessions/current');
    if (res.status === 204) return null;
    return res.json();
  } catch {
    return null;
  }
}

export async function pauseSession(sessionId: string): Promise<void> {
  await apiFetch(`/api/reading-sessions/${sessionId}/pause`, { method: 'POST' });
}

export async function resumeSession(sessionId: string): Promise<void> {
  await apiFetch(`/api/reading-sessions/${sessionId}/resume`, { method: 'POST' });
}

export async function clearSession(sessionId: string): Promise<{ worldId: string }> {
  const res = await apiFetch(`/api/reading-sessions/${sessionId}`, { method: 'DELETE' });
  return res.json();
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
