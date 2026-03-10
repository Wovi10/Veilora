import { useState } from 'react';
import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  TextField,
} from '@mui/material';
import { createTree } from '../api/treesApi';
import type { TreeDto } from '../types/tree';

interface Props {
  open: boolean;
  onClose: () => void;
  onCreated: (tree: TreeDto) => void;
}

export default function NewTreeDialog({ open, onClose, onCreated }: Props) {
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [nameError, setNameError] = useState('');
  const [submitting, setSubmitting] = useState(false);

  function handleClose() {
    setName('');
    setDescription('');
    setNameError('');
    onClose();
  }

  async function handleSubmit() {
    if (!name.trim()) {
      setNameError('Tree name is required');
      return;
    }
    if (name.length > 200) {
      setNameError('Tree name must not exceed 200 characters');
      return;
    }
    setSubmitting(true);
    try {
      const tree = await createTree(name.trim(), description.trim() || undefined);
      onCreated(tree);
      handleClose();
    } catch (err) {
      setNameError(err instanceof Error ? err.message : 'Failed to create tree');
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <Dialog open={open} onClose={handleClose} fullWidth maxWidth="sm">
      <DialogTitle>New Family Tree</DialogTitle>
      <DialogContent sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: '16px !important' }}>
        <TextField
          label="Name"
          value={name}
          onChange={(e) => { setName(e.target.value); setNameError(''); }}
          error={!!nameError}
          helperText={nameError}
          required
          autoFocus
          fullWidth
        />
        <TextField
          label="Description"
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          multiline
          rows={3}
          fullWidth
        />
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose} disabled={submitting}>Cancel</Button>
        <Button variant="contained" onClick={handleSubmit} disabled={submitting}>
          {submitting ? 'Creating…' : 'Create'}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
