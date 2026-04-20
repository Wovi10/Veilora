import {
  Box, Typography, Button, Chip, Divider, IconButton, CircularProgress, Stack,
} from '@mui/material';
import DeleteOutlineIcon from '@mui/icons-material/DeleteOutline';
import StopCircleOutlinedIcon from '@mui/icons-material/StopCircleOutlined';
import MenuBookIcon from '@mui/icons-material/MenuBook';
import type { ReadingNoteDto } from '../types/readingSession';
import { useReadingSession } from '../context/ReadingSessionContext';

export default function ReadingPage() {
  const { activeSession, notes, loading, end, removeNote } = useReadingSession();

  if (loading) {
    return <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>;
  }

  if (!activeSession) {
    return (
      <Box display="flex" flexDirection="column" alignItems="center" gap={2} mt={12} color="text.secondary">
        <MenuBookIcon sx={{ fontSize: 56, opacity: 0.25 }} />
        <Typography variant="body1" fontStyle="italic">
          No active session. Use the reading button to start one.
        </Typography>
      </Box>
    );
  }

  const started = new Date(activeSession.startedAt);

  return (
    <Box sx={{ height: 'calc(100vh - 64px)', display: 'flex', flexDirection: 'column' }}>
      {/* Session header */}
      <Box display="flex" justifyContent="space-between" alignItems="center" px={3} py={2} borderBottom={1} borderColor="divider">
        <Box>
          <Typography variant="h6" fontWeight={700}>{activeSession.worldName}</Typography>
          <Typography variant="body2" color="text.secondary">
            Started {started.toLocaleDateString()} at {started.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
          </Typography>
        </Box>
        <Box display="flex" alignItems="center" gap={1.5}>
          <Chip
            label={`${activeSession.noteCount} ${activeSession.noteCount === 1 ? 'note' : 'notes'}`}
            size="small"
            color="primary"
          />
          <Button
            size="small"
            color="error"
            startIcon={<StopCircleOutlinedIcon />}
            onClick={end}
            sx={{ textTransform: 'none' }}
          >
            End session
          </Button>
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
                <NoteRow key={note.id} note={note} onDelete={() => removeNote(note.id)} />
              ))}
            </Stack>
          )}
        </Box>

        <Divider orientation="vertical" flexItem />

        {/* Right — placeholder for Layer 2 (convert to entities) */}
        <Box sx={{ width: '50%', overflowY: 'auto', px: 3, py: 2 }} />
      </Box>
    </Box>
  );
}

function NoteRow({ note, onDelete }: { note: ReadingNoteDto; onDelete: () => void }) {
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
      <IconButton size="small" onClick={onDelete} sx={{ opacity: 0.4, '&:hover': { opacity: 1 }, flexShrink: 0 }}>
        <DeleteOutlineIcon fontSize="small" />
      </IconButton>
    </Box>
  );
}
