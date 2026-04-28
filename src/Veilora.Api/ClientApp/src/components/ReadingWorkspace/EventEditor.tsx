import { useState, useEffect } from 'react';
import {
  Box, TextField, Typography, Button, CircularProgress, IconButton,
} from '@mui/material';
import CheckIcon from '@mui/icons-material/Check';
import CloseIcon from '@mui/icons-material/Close';
import { getEvent, updateEvent } from '../../api/eventsApi';
import type { EventDto } from '../../types/event';

interface Props {
  eventId: string;
  onClose: () => void;
}

function SectionLabel({ children }: { children: React.ReactNode }) {
  return (
    <Typography variant="caption" sx={{ textTransform: 'uppercase', letterSpacing: '0.07em', fontSize: '0.62rem', fontWeight: 700, color: 'text.disabled' }}>
      {children}
    </Typography>
  );
}

export default function EventEditor({ eventId, onClose }: Props) {
  const [event, setEvent] = useState<EventDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [saving, setSaving] = useState(false);
  const [saved, setSaved] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    setLoading(true);
    getEvent(eventId)
      .then(ev => {
        setEvent(ev);
        setName(ev.name ?? '');
        setDescription(ev.description ?? '');
      })
      .catch(() => setError('Failed to load event'))
      .finally(() => setLoading(false));
  }, [eventId]);

  async function handleSave() {
    if (!event) return;
    setSaving(true);
    setError('');
    try {
      const updated = await updateEvent(eventId, {
        name: name.trim() || event.name,
        description: description.trim() || undefined,
      });
      setEvent(updated);
      setSaved(true);
      setTimeout(() => setSaved(false), 2000);
    } catch {
      setError('Failed to save changes');
    } finally {
      setSaving(false);
    }
  }

  if (loading) {
    return <Box sx={{ display: 'flex', justifyContent: 'center', pt: 6 }}><CircularProgress size={24} /></Box>;
  }

  if (!event) {
    return <Box sx={{ px: 3, pt: 3 }}><Typography color="error" variant="body2">{error || 'Event not found.'}</Typography></Box>;
  }

  return (
    <Box sx={{ flex: 1, overflowY: 'auto' }}>
      <Box sx={{ px: 3, pt: 2.5, pb: 1.5, display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
        <Typography variant="subtitle2" fontWeight={700} noWrap sx={{ flex: 1 }}>
          {event.name}
        </Typography>
        <IconButton size="small" onClick={onClose}>
          <CloseIcon fontSize="small" />
        </IconButton>
      </Box>

      <Box sx={{ px: 3, pb: 3, display: 'flex', flexDirection: 'column', gap: 2 }}>
        <Box>
          <SectionLabel>Name</SectionLabel>
          <TextField
            size="small"
            fullWidth
            value={name}
            onChange={e => setName(e.target.value)}
            sx={{ mt: 1 }}
          />
        </Box>

        <Box>
          <SectionLabel>Description</SectionLabel>
          <TextField
            size="small"
            fullWidth
            multiline
            minRows={4}
            maxRows={12}
            value={description}
            onChange={e => setDescription(e.target.value)}
            sx={{ mt: 1 }}
          />
        </Box>

        {error && <Typography variant="caption" color="error">{error}</Typography>}
        <Button
          variant="contained"
          size="small"
          onClick={handleSave}
          disabled={saving}
          color={saved ? 'success' : 'primary'}
          startIcon={saving ? <CircularProgress size={14} color="inherit" /> : saved ? <CheckIcon fontSize="small" /> : undefined}
          sx={{ textTransform: 'none', alignSelf: 'flex-start' }}
        >
          {saved ? 'Saved' : 'Save'}
        </Button>
      </Box>
    </Box>
  );
}
