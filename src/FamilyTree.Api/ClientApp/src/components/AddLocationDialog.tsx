import { useState } from 'react';
import {
  Dialog, DialogTitle, DialogContent, DialogActions, Button,
  TextField, Box,
} from '@mui/material';
import { createLocation } from '../api/locationsApi';
import type { LocationDto } from '../types/location';

interface Props {
  open: boolean;
  worldId: string;
  onClose: () => void;
  onCreated: (location: LocationDto) => void;
}

export default function AddLocationDialog({ open, worldId, onClose, onCreated }: Props) {
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');

  const resetForm = () => { setName(''); setDescription(''); setError(''); };
  const handleClose = () => { resetForm(); onClose(); };

  const handleSubmit = async () => {
    if (!name.trim()) { setError('Name is required'); return; }
    setSubmitting(true);
    try {
      const location = await createLocation({
        name: name.trim(),
        worldId,
        description: description.trim() || undefined,
      });
      onCreated(location);
      resetForm();
    } catch {
      setError('Failed to create location');
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>Add Location</DialogTitle>
      <DialogContent>
        <Box display="flex" flexDirection="column" gap={2} pt={1}>
          <TextField
            label="Name"
            value={name}
            onChange={e => setName(e.target.value)}
            required
            error={!!error}
            helperText={error || ''}
            autoFocus
          />
          <TextField
            label="Description"
            value={description}
            onChange={e => setDescription(e.target.value)}
            multiline
            rows={2}
          />
        </Box>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose}>Cancel</Button>
        <Button onClick={handleSubmit} variant="contained" disabled={submitting}>Add</Button>
      </DialogActions>
    </Dialog>
  );
}
