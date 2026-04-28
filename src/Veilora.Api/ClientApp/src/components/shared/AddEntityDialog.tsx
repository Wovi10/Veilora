import { useState } from 'react';
import {
  Dialog, DialogTitle, DialogContent, DialogActions, Button,
  TextField, MenuItem, Box, Select, FormControl, InputLabel,
} from '@mui/material';
import { createEntity } from '../../api/entitiesApi';
import type { EntityDto, EntityType } from '../../types/entity';

interface Props {
  open: boolean;
  worldId: string;
  defaultType: EntityType;
  onClose: () => void;
  onCreated: (entity: EntityDto) => void;
}

const ENTITY_TYPES: EntityType[] = ['Group', 'Concept'];

export default function AddEntityDialog({ open, worldId, defaultType, onClose, onCreated }: Props) {
  const [type, setType] = useState<EntityType>(defaultType);
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');

  const resetForm = () => {
    setName(''); setDescription(''); setError('');
    setType(defaultType);
  };

  const handleClose = () => { resetForm(); onClose(); };

  const handleSubmit = async () => {
    if (!name.trim()) { setError('Name is required'); return; }
    setSubmitting(true);
    try {
      const entity = await createEntity({
        name: name.trim(),
        type,
        worldId,
        description: description.trim() || undefined,
      });
      onCreated(entity);
      resetForm();
    } catch {
      setError('Failed to create entity');
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>Add {type}</DialogTitle>
      <DialogContent>
        <Box display="flex" flexDirection="column" gap={2} pt={1}>
          <FormControl fullWidth>
            <InputLabel>Type</InputLabel>
            <Select value={type} label="Type" onChange={e => setType(e.target.value as EntityType)}>
              {ENTITY_TYPES.map(t => <MenuItem key={t} value={t}>{t}</MenuItem>)}
            </Select>
          </FormControl>
          <TextField
            label="Name"
            value={name}
            onChange={e => setName(e.target.value)}
            required
            error={!!error}
            helperText={error || ''}
            autoFocus
          />
          <TextField label="Description" value={description} onChange={e => setDescription(e.target.value)} multiline rows={2} />
        </Box>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose}>Cancel</Button>
        <Button onClick={handleSubmit} variant="contained" disabled={submitting}>Add</Button>
      </DialogActions>
    </Dialog>
  );
}
