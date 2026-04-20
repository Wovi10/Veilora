import { useState, useEffect } from 'react';
import {
  Box, Typography, CircularProgress, Alert, Button, Chip,
  Card, CardContent, CardActions, Divider, Collapse, IconButton,
} from '@mui/material';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import ExpandLessIcon from '@mui/icons-material/ExpandLess';
import DeleteOutlineIcon from '@mui/icons-material/DeleteOutline';
import StopCircleOutlinedIcon from '@mui/icons-material/StopCircleOutlined';
import MenuBookIcon from '@mui/icons-material/MenuBook';
import { getAllSessions, getNotes, deleteNote } from '../api/readingSessionsApi';
import type { ReadingSessionDto, ReadingNoteDto } from '../types/readingSession';
import { useReadingSession } from '../context/ReadingSessionContext';

export default function ReadingPage() {
  const { activeSession, end } = useReadingSession();
  const [sessions, setSessions] = useState<ReadingSessionDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    getAllSessions()
      .then(setSessions)
      .catch(() => setError('Failed to load sessions'))
      .finally(() => setLoading(false));
  }, []);

  async function handleEnd() {
    await end();
    setSessions(prev => prev.map(s =>
      s.id === activeSession?.id ? { ...s, endedAt: new Date().toISOString() } : s
    ));
  }

  if (loading) return <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>;
  if (error) return <Alert severity="error" sx={{ m: 3 }}>{error}</Alert>;

  const pastSessions = sessions.filter(s => !!s.endedAt);

  return (
    <Box sx={{ maxWidth: 800, mx: 'auto', px: 3, py: 4 }}>
      <Typography variant="h4" fontWeight={700} mb={4}>Reading Sessions</Typography>

      {/* Active session */}
      {activeSession && (
        <Box mb={4}>
          <Typography variant="overline" color="text.secondary">Active</Typography>
          <ActiveSessionCard session={activeSession} onEnd={handleEnd} />
        </Box>
      )}

      {!activeSession && sessions.length === 0 && (
        <Box display="flex" flexDirection="column" alignItems="center" gap={2} mt={8} color="text.secondary">
          <MenuBookIcon sx={{ fontSize: 48, opacity: 0.3 }} />
          <Typography variant="body1" fontStyle="italic">
            No sessions yet. Use the reading button to start one.
          </Typography>
        </Box>
      )}

      {/* Past sessions */}
      {pastSessions.length > 0 && (
        <Box>
          <Typography variant="overline" color="text.secondary">Past sessions</Typography>
          <Box display="flex" flexDirection="column" gap={2} mt={1}>
            {pastSessions.map(session => (
              <SessionCard key={session.id} session={session} />
            ))}
          </Box>
        </Box>
      )}
    </Box>
  );
}

function ActiveSessionCard({ session, onEnd }: { session: ReadingSessionDto; onEnd: () => void }) {
  const started = new Date(session.startedAt);
  return (
    <Card sx={{ borderRadius: 2, border: '1px solid', borderColor: 'primary.main' }}>
      <CardContent>
        <Box display="flex" justifyContent="space-between" alignItems="flex-start">
          <Box>
            <Typography variant="h6" fontWeight={600}>{session.worldName}</Typography>
            <Typography variant="body2" color="text.secondary">
              Started {started.toLocaleDateString()} at {started.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
            </Typography>
          </Box>
          <Chip label={`${session.noteCount} ${session.noteCount === 1 ? 'note' : 'notes'}`} size="small" color="primary" />
        </Box>
      </CardContent>
      <CardActions sx={{ px: 2, pb: 2 }}>
        <Button
          size="small"
          color="error"
          startIcon={<StopCircleOutlinedIcon />}
          onClick={onEnd}
        >
          End Session
        </Button>
      </CardActions>
    </Card>
  );
}

function SessionCard({ session }: { session: ReadingSessionDto }) {
  const [open, setOpen] = useState(false);
  const [notes, setNotes] = useState<ReadingNoteDto[] | null>(null);
  const [loadingNotes, setLoadingNotes] = useState(false);

  async function toggleNotes() {
    if (!open && notes === null) {
      setLoadingNotes(true);
      try {
        const fetched = await getNotes(session.id);
        setNotes(fetched);
      } finally {
        setLoadingNotes(false);
      }
    }
    setOpen(prev => !prev);
  }

  async function handleDeleteNote(noteId: string) {
    await deleteNote(noteId);
    setNotes(prev => prev?.filter(n => n.id !== noteId) ?? null);
  }

  const started = new Date(session.startedAt);
  const ended = session.endedAt ? new Date(session.endedAt) : null;

  return (
    <Card sx={{ borderRadius: 2 }}>
      <CardContent sx={{ pb: '8px !important' }}>
        <Box display="flex" justifyContent="space-between" alignItems="flex-start">
          <Box>
            <Typography variant="subtitle1" fontWeight={600}>{session.worldName}</Typography>
            <Typography variant="body2" color="text.secondary">
              {started.toLocaleDateString()}
              {ended && ` · ${Math.round((ended.getTime() - started.getTime()) / 60000)} min`}
            </Typography>
          </Box>
          <Box display="flex" alignItems="center" gap={1}>
            <Chip label={`${session.noteCount}`} size="small" />
            <IconButton size="small" onClick={toggleNotes} disabled={session.noteCount === 0}>
              {loadingNotes
                ? <CircularProgress size={16} />
                : open ? <ExpandLessIcon fontSize="small" /> : <ExpandMoreIcon fontSize="small" />}
            </IconButton>
          </Box>
        </Box>
      </CardContent>

      <Collapse in={open}>
        <Divider />
        <Box sx={{ px: 2, py: 1.5, display: 'flex', flexDirection: 'column', gap: 1 }}>
          {notes?.map(note => (
            <NoteRow key={note.id} note={note} onDelete={() => handleDeleteNote(note.id)} />
          ))}
        </Box>
      </Collapse>
    </Card>
  );
}

function NoteRow({ note, onDelete }: { note: ReadingNoteDto; onDelete: () => void }) {
  const time = new Date(note.createdAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  return (
    <Box display="flex" alignItems="flex-start" gap={1}>
      <Typography variant="caption" color="text.secondary" sx={{ mt: 0.25, minWidth: 40 }}>{time}</Typography>
      <Box flex={1}>
        <Typography variant="body2">{note.text}</Typography>
        {note.tags.length > 0 && (
          <Box display="flex" gap={0.5} mt={0.5} flexWrap="wrap">
            {note.tags.map(tag => (
              <Chip key={tag} label={`#${tag}`} size="small" variant="outlined" sx={{ height: 18, fontSize: '0.7rem' }} />
            ))}
          </Box>
        )}
      </Box>
      <IconButton size="small" onClick={onDelete} sx={{ opacity: 0.5, '&:hover': { opacity: 1 } }}>
        <DeleteOutlineIcon fontSize="small" />
      </IconButton>
    </Box>
  );
}
