import { useState } from 'react';
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
import ReadingWorkspace from '../components/ReadingWorkspace/ReadingWorkspace';

export default function ReadingPage() {
  const { session, notes, loading, pause, resume, clear, removeNote } = useReadingSession();
  const navigate = useNavigate();
  const [selectedNote, setSelectedNote] = useState<ReadingNoteDto | null>(null);

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

  function handleRemoveNote(noteId: string) {
    if (selectedNote?.id === noteId) setSelectedNote(null);
    removeNote(noteId);
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
        {/* Left — detail (top) + notes list (bottom) */}
        <Box sx={{ width: '50%', display: 'flex', flexDirection: 'column', overflow: 'hidden' }}>
          {/* Detail panel */}
          <Box sx={{ flex: 1, overflowY: 'auto', px: 3, py: 2 }}>
            {selectedNote == null ? (
              <Typography variant="body2" color="text.secondary" fontStyle="italic" mt={2}>
                Click a note to read it.
              </Typography>
            ) : (
              <Box>
                <Typography variant="caption" color="text.secondary" display="block" mb={1}>
                  {new Date(selectedNote.createdAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                </Typography>
                <Typography variant="body1" sx={{ whiteSpace: 'pre-wrap' }}>{selectedNote.text}</Typography>
                {selectedNote.tags.length > 0 && (
                  <Box display="flex" gap={0.5} mt={1.5} flexWrap="wrap">
                    {selectedNote.tags.map(tag => (
                      <Chip key={tag} label={`#${tag}`} size="small" variant="outlined" sx={{ height: 20, fontSize: '0.72rem' }} />
                    ))}
                  </Box>
                )}
              </Box>
            )}
          </Box>

          <Divider />

          {/* Notes list */}
          <Box sx={{ height: 240, overflowY: 'auto', px: 3, py: 1 }}>
            {notes.length === 0 ? (
              <Typography variant="body2" color="text.secondary" fontStyle="italic" mt={1}>
                No notes yet. Start writing in the panel below.
              </Typography>
            ) : (
              <Stack gap={0}>
                {notes.map(note => (
                  <NoteRow
                    key={note.id}
                    note={note}
                    selected={selectedNote?.id === note.id}
                    onSelect={() => setSelectedNote(note)}
                    onDelete={() => handleRemoveNote(note.id)}
                  />
                ))}
              </Stack>
            )}
          </Box>
        </Box>

        <Divider orientation="vertical" flexItem />

        {/* Right — entity workspace */}
        <Box sx={{ width: '50%', display: 'flex', flexDirection: 'column', overflow: 'hidden' }}>
          <ReadingWorkspace />
        </Box>
      </Box>
    </Box>
  );
}

function NoteRow({
  note, selected, onSelect, onDelete,
}: {
  note: ReadingNoteDto;
  selected: boolean;
  onSelect: () => void;
  onDelete?: () => void;
}) {
  const time = new Date(note.createdAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  return (
    <Box
      display="flex"
      alignItems="center"
      gap={1.5}
      onClick={onSelect}
      sx={{
        py: 0.75, px: 1, borderRadius: 1, cursor: 'pointer',
        bgcolor: selected ? 'action.selected' : 'transparent',
        '&:hover': { bgcolor: selected ? 'action.selected' : 'action.hover' },
      }}
    >
      <Typography variant="caption" color="text.secondary" sx={{ minWidth: 36, flexShrink: 0 }}>
        {time}
      </Typography>
      <Typography variant="body2" flex={1} noWrap>
        {note.text}
      </Typography>
      {onDelete && (
        <IconButton
          size="small"
          onClick={e => { e.stopPropagation(); onDelete(); }}
          sx={{ opacity: 0.4, '&:hover': { opacity: 1 }, flexShrink: 0 }}
        >
          <DeleteOutlineIcon fontSize="small" />
        </IconButton>
      )}
    </Box>
  );
}
