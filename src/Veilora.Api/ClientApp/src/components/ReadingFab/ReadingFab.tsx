import { useState, useRef, useEffect } from 'react';
import { useLocation } from 'react-router-dom';
import {
  Box, Button, Paper, TextField, Typography,
  IconButton, Tooltip, CircularProgress,
} from '@mui/material';
import MenuBookIcon from '@mui/icons-material/MenuBook';
import RemoveIcon from '@mui/icons-material/Remove';
import CloseIcon from '@mui/icons-material/Close';
import PlayArrowIcon from '@mui/icons-material/PlayArrow';
import { useReadingSession } from '../../context/ReadingSessionContext';
import StartSessionDialog from './StartSessionDialog';

export default function ReadingFab() {
  const { session, loading, pause, resume, captureNote } = useReadingSession();
  const [minimized, setMinimized] = useState(true);
  const location = useLocation();

  useEffect(() => {
    if (location.pathname === '/reading') setMinimized(true);
  }, [location.pathname]);
  const [startOpen, setStartOpen] = useState(false);
  const [text, setText] = useState('');
  const [saving, setSaving] = useState(false);
  const inputRef = useRef<HTMLInputElement>(null);

  if (loading) return null;

  async function handleKeyDown(e: React.KeyboardEvent) {
    if (e.key === 'Escape') { setMinimized(true); return; }
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      const trimmed = text.trim();
      if (!trimmed || saving) return;
      setSaving(true);
      try {
        await captureNote(trimmed);
        setText('');
      } finally {
        setSaving(false);
        inputRef.current?.focus();
      }
    }
  }

  function handleOpen() {
    setMinimized(false);
    setTimeout(() => inputRef.current?.focus(), 50);
  }

  async function handlePause(e: React.MouseEvent) {
    e.stopPropagation();
    await pause();
  }

  return (
    <>
      <Box sx={{ position: 'fixed', bottom: 0, right: 24, zIndex: 1400 }}>
        {/* No session */}
        {!session && (
          <Button
            variant="contained"
            startIcon={<MenuBookIcon />}
            onClick={() => setStartOpen(true)}
            sx={{ borderRadius: '8px 8px 0 0', px: 3, py: 1.25, boxShadow: 4, textTransform: 'none', fontWeight: 600 }}
          >
            Start Reading
          </Button>
        )}

        {/* Active or paused session */}
        {session && (
          <Paper elevation={8} sx={{ width: 340, borderRadius: '8px 8px 0 0', overflow: 'hidden' }}>
            {/* Header */}
            <Box
              onClick={minimized ? handleOpen : () => setMinimized(true)}
              sx={{
                bgcolor: 'grey.900', px: 2, py: 1,
                display: 'flex', alignItems: 'center', justifyContent: 'space-between',
                cursor: 'pointer', userSelect: 'none',
              }}
            >
              <Typography variant="body2" sx={{ color: 'common.white', fontWeight: 600 }} noWrap>
                {session.worldName}
                <Typography component="span" variant="body2" sx={{ color: 'grey.400', fontWeight: 400, ml: 1 }}>
                  {session.noteCount} {session.noteCount === 1 ? 'note' : 'notes'}
                  {!session.isActive && ' · paused'}
                </Typography>
              </Typography>
              <Box display="flex" gap={0.5}>
                <IconButton
                  size="small"
                  onClick={e => { e.stopPropagation(); minimized ? handleOpen() : setMinimized(true); }}
                  sx={{ color: 'grey.300', '&:hover': { bgcolor: 'rgba(255,255,255,0.1)' } }}
                >
                  <RemoveIcon fontSize="small" />
                </IconButton>
                {session.isActive && (
                  <Tooltip title="Pause session">
                    <IconButton
                      size="small"
                      onClick={handlePause}
                      sx={{ color: 'grey.300', '&:hover': { bgcolor: 'rgba(255,255,255,0.1)' } }}
                    >
                      <CloseIcon fontSize="small" />
                    </IconButton>
                  </Tooltip>
                )}
              </Box>
            </Box>

            {/* Body — active: note input */}
            {!minimized && session.isActive && (
              <Box sx={{ p: 1.5, display: 'flex', flexDirection: 'column', gap: 1 }}>
                <TextField
                  inputRef={inputRef}
                  multiline
                  minRows={9}
                  maxRows={12}
                  fullWidth
                  autoFocus
                  placeholder="Jot a note… (Enter to save)"
                  value={text}
                  onChange={e => setText(e.target.value)}
                  onKeyDown={handleKeyDown}
                  variant="standard"
                  slotProps={{
                    input: {
                      disableUnderline: true,
                      endAdornment: saving
                        ? <CircularProgress size={14} sx={{ alignSelf: 'flex-start', mt: 0.5 }} />
                        : undefined,
                    },
                  }}
                  sx={{ '& textarea': { fontSize: '0.9rem', lineHeight: 1.6 } }}
                />
                <Box display="flex" justifyContent="flex-end">
                  <Button size="small" color="error" onClick={handlePause} sx={{ textTransform: 'none' }}>
                    Pause session
                  </Button>
                </Box>
              </Box>
            )}

            {/* Body — paused: resume prompt */}
            {!minimized && !session.isActive && (
              <Box sx={{ p: 2, display: 'flex', flexDirection: 'column', gap: 1.5 }}>
                <Typography variant="body2" color="text.secondary">
                  {session.noteCount} {session.noteCount === 1 ? 'note' : 'notes'} captured
                </Typography>
                <Button
                  variant="outlined"
                  size="small"
                  startIcon={<PlayArrowIcon />}
                  onClick={async () => { await resume(); handleOpen(); }}
                  sx={{ textTransform: 'none' }}
                >
                  Resume
                </Button>
              </Box>
            )}
          </Paper>
        )}
      </Box>

      <StartSessionDialog
        open={startOpen}
        onClose={() => setStartOpen(false)}
        onStarted={handleOpen}
      />
    </>
  );
}
