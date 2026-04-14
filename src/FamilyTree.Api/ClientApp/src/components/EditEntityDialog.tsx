import { useState, useEffect } from 'react';
import {
  Dialog, DialogTitle, DialogContent, DialogActions, Button,
  TextField, Box, Typography,
} from '@mui/material';
import { updateEntity } from '../api/entitiesApi';
import type { EntityDto } from '../types/entity';

interface Props {
  open: boolean;
  entity: EntityDto;
  onClose: () => void;
  onSaved: (entity: EntityDto) => void;
}

export default function EditEntityDialog({ open, entity, onClose, onSaved }: Props) {
  const [description, setDescription] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    if (!entity) return;
    setDescription(entity.description ?? '');
  }, [entity]);

  const handleSubmit = async () => {
    setSubmitting(true);
    setError('');
    try {
      const updated = await updateEntity(entity.id, {
        name: entity.name,
        type: entity.type,
        description: description.trim() || undefined,
      });
      onSaved(updated);
    } catch {
      setError('Failed to save changes');
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>Edit {entity.type}</DialogTitle>
      <DialogContent>
        <Box display="flex" flexDirection="column" gap={2} pt={1}>
          <TextField label="Description" value={description} onChange={e => setDescription(e.target.value)} multiline rows={3} />
          {error && <Typography color="error" variant="caption">{error}</Typography>}
        </Box>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Cancel</Button>
        <Button onClick={handleSubmit} variant="contained" disabled={submitting}>Save</Button>
      </DialogActions>
    </Dialog>
  );
}
