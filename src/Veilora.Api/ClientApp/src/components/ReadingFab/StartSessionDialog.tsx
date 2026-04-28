import { useState, useEffect } from 'react';
import {
  Dialog, DialogTitle, DialogContent, DialogActions,
  Button, FormControl, InputLabel, Select, MenuItem, CircularProgress,
} from '@mui/material';
import { getWorlds } from '../../api/worldsApi';
import type { WorldDto } from '../../types/world';
import { useReadingSession } from '../../context/ReadingSessionContext';

interface Props {
  open: boolean;
  onClose: () => void;
  onStarted: () => void;
}

export default function StartSessionDialog({ open, onClose, onStarted }: Props) {
  const { start } = useReadingSession();
  const [worlds, setWorlds] = useState<WorldDto[]>([]);
  const [worldId, setWorldId] = useState('');
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    if (open) getWorlds().then(setWorlds).catch(() => {});
  }, [open]);

  async function handleStart() {
    if (!worldId) return;
    setSaving(true);
    try {
      await start(worldId);
      onClose();
      onStarted();
    } finally {
      setSaving(false);
    }
  }

  return (
    <Dialog open={open} onClose={onClose} maxWidth="xs" fullWidth>
      <DialogTitle>Start Reading Session</DialogTitle>
      <DialogContent>
        <FormControl fullWidth sx={{ mt: 1 }}>
          <InputLabel>World</InputLabel>
          <Select value={worldId} label="World" onChange={e => setWorldId(e.target.value)}>
            {worlds.map(w => (
              <MenuItem key={w.id} value={w.id}>{w.name}</MenuItem>
            ))}
          </Select>
        </FormControl>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Cancel</Button>
        <Button
          variant="contained"
          onClick={handleStart}
          disabled={!worldId || saving}
          startIcon={saving ? <CircularProgress size={16} /> : undefined}
        >
          Start Reading
        </Button>
      </DialogActions>
    </Dialog>
  );
}
