import { createContext, useContext, useState, useEffect, type ReactNode } from 'react';
import type { ReadingSessionDto, ReadingNoteDto } from '../types/readingSession';
import {
  getCurrentSession, startSession, pauseSession, resumeSession, clearSession,
  addNote, getNotes, deleteNote,
} from '../api/readingSessionsApi';

interface ReadingSessionContextValue {
  session: ReadingSessionDto | null;
  notes: ReadingNoteDto[];
  loading: boolean;
  start: (worldId: string) => Promise<void>;
  pause: () => Promise<void>;
  resume: () => Promise<void>;
  clear: () => Promise<string>;
  captureNote: (text: string) => Promise<ReadingNoteDto | null>;
  removeNote: (noteId: string) => Promise<void>;
}

const ReadingSessionContext = createContext<ReadingSessionContextValue | null>(null);

export function ReadingSessionProvider({ children }: { children: ReactNode }) {
  const [session, setSession] = useState<ReadingSessionDto | null>(null);
  const [notes, setNotes] = useState<ReadingNoteDto[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getCurrentSession()
      .then(async s => {
        setSession(s);
        if (s) setNotes(await getNotes(s.id));
      })
      .catch(() => {})
      .finally(() => setLoading(false));
  }, []);

  async function start(worldId: string) {
    const s = await startSession({ worldId });
    setSession(s);
    setNotes([]);
  }

  async function pause() {
    if (!session) return;
    await pauseSession(session.id);
    setSession(prev => prev ? { ...prev, isActive: false, endedAt: new Date().toISOString() } : prev);
  }

  async function resume() {
    if (!session) return;
    await resumeSession(session.id);
    setSession(prev => prev ? { ...prev, isActive: true, endedAt: undefined } : prev);
  }

  async function clear(): Promise<string> {
    if (!session) return '';
    const { worldId } = await clearSession(session.id);
    setSession(null);
    setNotes([]);
    return worldId;
  }

  async function captureNote(text: string): Promise<ReadingNoteDto | null> {
    if (!session) return null;
    const note = await addNote(session.id, text);
    setNotes(prev => [...prev, note]);
    setSession(prev => prev ? { ...prev, noteCount: prev.noteCount + 1 } : prev);
    return note;
  }

  async function removeNote(noteId: string) {
    await deleteNote(noteId);
    setNotes(prev => prev.filter(n => n.id !== noteId));
    setSession(prev => prev ? { ...prev, noteCount: prev.noteCount - 1 } : prev);
  }

  return (
    <ReadingSessionContext.Provider value={{ session, notes, loading, start, pause, resume, clear, captureNote, removeNote }}>
      {children}
    </ReadingSessionContext.Provider>
  );
}

export function useReadingSession(): ReadingSessionContextValue {
  const ctx = useContext(ReadingSessionContext);
  if (!ctx) throw new Error('useReadingSession must be used within ReadingSessionProvider');
  return ctx;
}
