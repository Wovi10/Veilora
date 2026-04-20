import { useNavigate } from 'react-router-dom';
import {
  Box, Typography, Button, Chip, Divider, IconButton, CircularProgress, Stack,
} from '@mui/material';
import DeleteOutlineIcon from '@mui/icons-material/DeleteOutline';
import PauseCircleOutlineIcon from '@mui/icons-material/PauseCircleOutline';
import PlayArrowIcon from '@mui/icons-material/PlayArrow';
import DeleteForeverIcon from '@mui/icons-material/DeleteForever';
import MenuBookIcon from '@mui/icons-material/MenuBook';
import type { ReadingNoteDto } from '../types/readingSession';
import { useReadingSession } from '../context/ReadingSessionContext';

export default function ReadingPage() {
  const { session, notes, loading, pause, resume, clear, removeNote } = useReadingSession();
  const navigate = useNavigate();

  if (loading) {
    return <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>;
  }

  if (!session) {
    return (
      <Box display="flex" flexDirection="column" alignItems="center" gap={2} mt={12} color="text.secondary">
        <MenuBookIcon sx={{ fontSize: 56, opacity: 0.25 }} />
        <Typography variant="body1" fontStyle="italic">
          No active session. Use the reading button to start one.
        </Typography>
      </Box>
    );
  }

  const started = new Date(session.startedAt);

  async function handleClear() {
    const worldId = await clear();
    navigate(`/worlds/${worldId}`);
  }

  return (
    <Box sx={{ height: 'calc(100vh - 64px)', display: 'flex', flexDirection: 'column' }}>
      {/* Session header */}
      <Box display="flex" justifyContent="space-between" alignItems="center" px={3} py={2} borderBottom={1} borderColor="divider">
        <Box>
          <Box display="flex" alignItems="center" gap={1}>
            <Typography variant="h6" fontWeight={700}>{session.worldName}</Typography>
            {!session.isActive && (
              <Chip label="Paused" size="small" color="warning" variant="outlined" />
            )}
          </Box>
          <Typography variant="body2" color="text.secondary">
            Started {started.toLocaleDateString()} at {started.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
          </Typography>
        </Box>

        <Box display="flex" alignItems="center" gap={1.5}>
          <Chip
            label={`${session.noteCount} ${session.noteCount === 1 ? 'note' : 'notes'}`}
            size="small"
            color="primary"
          />
          {session.isActive ? (
            <Button
              size="small"
              startIcon={<PauseCircleOutlineIcon />}
              onClick={pause}
              sx={{ textTransform: 'none' }}
            >
              Pause session
            </Button>
          ) : (
            <>
              <Button
                size="small"
                variant="contained"
                startIcon={<PlayArrowIcon />}
                onClick={resume}
                sx={{ textTransform: 'none' }}
              >
                Resume
              </Button>
              <Button
                size="small"
                color="error"
                startIcon={<DeleteForeverIcon />}
                onClick={handleClear}
                sx={{ textTransform: 'none' }}
              >
                Clear session
              </Button>
            </>
          )}
        </Box>
      </Box>

      {/* Two-column body */}
      <Box sx={{ flex: 1, display: 'flex', overflow: 'hidden' }}>
        {/* Left — notes */}
        <Box sx={{ width: '50%', overflowY: 'auto', px: 3, py: 2 }}>
          {notes.length === 0 ? (
            <Typography variant="body2" color="text.secondary" fontStyle="italic" mt={2}>
              No notes yet. Start writing in the panel below.
            </Typography>
          ) : (
            <Stack gap={0.5}>
              {notes.map(note => (
                <NoteRow
                  key={note.id}
                  note={note}
                  onDelete={session.isActive ? () => removeNote(note.id) : undefined}
                />
              ))}
            </Stack>
          )}
        </Box>

        <Divider orientation="vertical" flexItem />

        {/* Right — placeholder for Layer 2 */}
        <Box sx={{ width: '50%', overflowY: 'auto', px: 3, py: 2 }} />
      </Box>
    </Box>
  );
}

function NoteRow({ note, onDelete }: { note: ReadingNoteDto; onDelete?: () => void }) {
  const time = new Date(note.createdAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  return (
    <Box
      display="flex"
      alignItems="flex-start"
      gap={1.5}
      sx={{ py: 1, px: 1, borderRadius: 1, '&:hover': { bgcolor: 'action.hover' } }}
    >
      <Typography variant="caption" color="text.secondary" sx={{ mt: 0.3, minWidth: 36, flexShrink: 0 }}>
        {time}
      </Typography>
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
      {onDelete && (
        <IconButton size="small" onClick={onDelete} sx={{ opacity: 0.4, '&:hover': { opacity: 1 }, flexShrink: 0 }}>
          <DeleteOutlineIcon fontSize="small" />
        </IconButton>
      )}
    </Box>
  );
}
