import { useState } from 'react';
import {
  Dialog, DialogTitle, DialogContent, DialogActions,
  Button, TextField, Box,
} from '@mui/material';
import { createFamilyTree } from '../api/familyTreesApi';
import type { FamilyTreeDto } from '../types/familyTree';

interface Props {
  open: boolean;
  worldId: string;
  onClose: () => void;
  onCreated: (tree: FamilyTreeDto) => void;
}

export default function NewFamilyTreeDialog({ open, worldId, onClose, onCreated }: Props) {
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');

  const handleClose = () => {
    setName(''); setDescription(''); setError('');
    onClose();
  };

  const handleSubmit = async () => {
    if (!name.trim()) { setError('Name is required'); return; }
    setSubmitting(true);
    try {
      const tree = await createFamilyTree({
        name: name.trim(),
        description: description.trim() || undefined,
        worldId,
      });
      onCreated(tree);
      handleClose();
    } catch {
      setError('Failed to create family tree');
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>New Family Tree</DialogTitle>
      <DialogContent>
        <Box display="flex" flexDirection="column" gap={2} pt={1}>
          <TextField
            label="Name"
            value={name}
            onChange={e => setName(e.target.value)}
            required
            error={!!error}
            helperText={error}
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
        <Button onClick={handleSubmit} variant="contained" disabled={submitting}>Create</Button>
      </DialogActions>
    </Dialog>
  );
}
