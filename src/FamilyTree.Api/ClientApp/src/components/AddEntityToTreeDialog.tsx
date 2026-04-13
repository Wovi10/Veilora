import { useState, useEffect } from 'react';
import {
  Dialog, DialogTitle, DialogContent, DialogActions, Button,
  Box, List, ListItemButton, ListItemText, Typography,
  CircularProgress, TextField, Divider,
} from '@mui/material';
import { getEntities, createEntity } from '../api/entitiesApi';
import { addEntityToFamilyTree } from '../api/familyTreesApi';
import type { EntityDto } from '../types/entity';

interface Props {
  open: boolean;
  familyTreeId: string;
  worldId: string;
  existingEntityIds: string[];
  onClose: () => void;
  onAdded: (entity: EntityDto) => void;
}

export default function AddEntityToTreeDialog({
  open, familyTreeId, worldId, existingEntityIds, onClose, onAdded,
}: Props) {
  const [worldEntities, setWorldEntities] = useState<EntityDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [newFirstName, setNewFirstName] = useState('');
  const [newLastName, setNewLastName] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    if (!open) return;
    setLoading(true);
    getEntities()
      .then(all => setWorldEntities(all.filter(e => e.worldId === worldId && !existingEntityIds.includes(e.id))))
      .catch(() => setError('Failed to load entities'))
      .finally(() => setLoading(false));
  }, [open, worldId, existingEntityIds]);

  const handleSelectExisting = async (entity: EntityDto) => {
    setSubmitting(true);
    try {
      await addEntityToFamilyTree(familyTreeId, entity.id);
      onAdded(entity);
      handleClose();
    } catch {
      setError('Failed to add entity to tree');
    } finally {
      setSubmitting(false);
    }
  };

  const handleCreateNew = async () => {
    const name = [newFirstName.trim(), newLastName.trim()].filter(Boolean).join(' ');
    if (!name) { setError('Name is required'); return; }
    setSubmitting(true);
    try {
      const entity = await createEntity({
        name,
        type: 'Character',
        worldId,
        firstName: newFirstName.trim() || undefined,
        lastName: newLastName.trim() || undefined,
      });
      await addEntityToFamilyTree(familyTreeId, entity.id);
      onAdded(entity);
      handleClose();
    } catch {
      setError('Failed to create character');
    } finally {
      setSubmitting(false);
    }
  };

  const handleClose = () => {
    setNewFirstName(''); setNewLastName(''); setError('');
    onClose();
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>Add to Tree</DialogTitle>
      <DialogContent>
        <Box display="flex" flexDirection="column" gap={2} pt={1}>
          <Typography variant="subtitle2" color="text.secondary">Create new character</Typography>
          <Box display="flex" gap={2}>
            <TextField label="First Name" value={newFirstName} onChange={e => setNewFirstName(e.target.value)} fullWidth autoFocus />
            <TextField label="Last Name" value={newLastName} onChange={e => setNewLastName(e.target.value)} fullWidth />
          </Box>
          <Button variant="outlined" onClick={handleCreateNew} disabled={submitting}>
            Create & Add
          </Button>

          {worldEntities.length > 0 && (
            <>
              <Divider><Typography variant="caption">or pick from world</Typography></Divider>
              {loading ? (
                <CircularProgress size={24} sx={{ alignSelf: 'center' }} />
              ) : (
                <List dense disablePadding sx={{ maxHeight: 240, overflow: 'auto', border: '1px solid', borderColor: 'divider', borderRadius: 1 }}>
                  {worldEntities.map(entity => (
                    <ListItemButton key={entity.id} onClick={() => handleSelectExisting(entity)} disabled={submitting}>
                      <ListItemText primary={entity.name} secondary={entity.species || entity.type} />
                    </ListItemButton>
                  ))}
                </List>
              )}
            </>
          )}

          {error && <Typography color="error" variant="caption">{error}</Typography>}
        </Box>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose}>Cancel</Button>
      </DialogActions>
    </Dialog>
  );
}
