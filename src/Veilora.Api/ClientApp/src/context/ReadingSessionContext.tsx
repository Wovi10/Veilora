import { createContext, useContext, useState, useEffect, type ReactNode } from 'react';
import type { ReadingSessionDto, ReadingNoteDto } from '../types/readingSession';
import {
  getActiveSession, startSession, endSession, addNote, getNotes, deleteNote,
} from '../api/readingSessionsApi';

const SESSION_ID_KEY = 'reading_session_id';

interface ReadingSessionContextValue {
  activeSession: ReadingSessionDto | null;
  notes: ReadingNoteDto[];
  loading: boolean;
  start: (worldId: string) => Promise<void>;
  end: () => Promise<void>;
  captureNote: (text: string) => Promise<ReadingNoteDto | null>;
  removeNote: (noteId: string) => Promise<void>;
}

const ReadingSessionContext = createContext<ReadingSessionContextValue | null>(null);

export function ReadingSessionProvider({ children }: { children: ReactNode }) {
  const [activeSession, setActiveSession] = useState<ReadingSessionDto | null>(null);
  const [notes, setNotes] = useState<ReadingNoteDto[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const savedId = localStorage.getItem(SESSION_ID_KEY);
    if (!savedId) { setLoading(false); return; }
    getActiveSession()
      .then(async session => {
        setActiveSession(session);
        if (session) {
          const fetched = await getNotes(session.id);
          setNotes(fetched);
        }
      })
      .catch(() => setActiveSession(null))
      .finally(() => setLoading(false));
  }, []);

  async function start(worldId: string) {
    const session = await startSession({ worldId });
    localStorage.setItem(SESSION_ID_KEY, session.id);
    setActiveSession(session);
    setNotes([]);
  }

  async function end() {
    if (!activeSession) return;
    await endSession(activeSession.id);
    localStorage.removeItem(SESSION_ID_KEY);
    setActiveSession(null);
    setNotes([]);
  }

  async function captureNote(text: string): Promise<ReadingNoteDto | null> {
    if (!activeSession) return null;
    const note = await addNote(activeSession.id, text);
    setNotes(prev => [...prev, note]);
    setActiveSession(prev => prev ? { ...prev, noteCount: prev.noteCount + 1 } : prev);
    return note;
  }

  async function removeNote(noteId: string) {
    await deleteNote(noteId);
    setNotes(prev => prev.filter(n => n.id !== noteId));
    setActiveSession(prev => prev ? { ...prev, noteCount: prev.noteCount - 1 } : prev);
  }

  return (
    <ReadingSessionContext.Provider value={{ activeSession, notes, loading, start, end, captureNote, removeNote }}>
      {children}
    </ReadingSessionContext.Provider>
  );
}

export function useReadingSession(): ReadingSessionContextValue {
  const ctx = useContext(ReadingSessionContext);
  if (!ctx) throw new Error('useReadingSession must be used within ReadingSessionProvider');
  return ctx;
}
