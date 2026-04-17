import { useState } from 'react';
import {
  Dialog, DialogTitle, DialogContent, DialogActions,
  Button, TextField, Box,
} from '@mui/material';
import { createWorld } from '../api/worldsApi';
import type { WorldDto } from '../types/world';

interface Props {
  open: boolean;
  onClose: () => void;
  onCreated: (world: WorldDto) => void;
}

export default function NewWorldDialog({ open, onClose, onCreated }: Props) {
  const [name, setName] = useState('');
  const [author, setAuthor] = useState('');
  const [description, setDescription] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');

  const handleClose = () => {
    setName(''); setAuthor(''); setDescription(''); setError('');
    onClose();
  };

  const handleSubmit = async () => {
    if (!name.trim()) { setError('Name is required'); return; }
    setSubmitting(true);
    try {
      const world = await createWorld({
        name: name.trim(),
        author: author.trim() || undefined,
        description: description.trim() || undefined,
      });
      onCreated(world);
      handleClose();
    } catch {
      setError('Failed to create world');
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>New World</DialogTitle>
      <DialogContent>
        <Box display="flex" flexDirection="column" gap={2} pt={1}>
          <TextField
            label="World Name"
            value={name}
            onChange={e => setName(e.target.value)}
            required
            error={!!error}
            helperText={error}
            autoFocus
          />
          <TextField
            label="Author"
            value={author}
            onChange={e => setAuthor(e.target.value)}
            placeholder="e.g. Tolkien"
          />
          <TextField
            label="Description"
            value={description}
            onChange={e => setDescription(e.target.value)}
            multiline
            rows={3}
          />
        </Box>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose}>Cancel</Button>
        <Button onClick={handleSubmit} variant="contained" disabled={submitting}>Create</Button>
      </DialogActions>
    </Dialog>
  );
}
