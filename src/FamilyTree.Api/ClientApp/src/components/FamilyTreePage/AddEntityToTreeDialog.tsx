import { useState, useEffect } from 'react';
import {
  Dialog, DialogTitle, DialogContent, DialogActions, Button,
  Box, List, ListItemButton, ListItemText, Typography,
  CircularProgress, TextField, Divider,
} from '@mui/material';
import { getCharactersByWorld, createCharacter } from '../api/charactersApi';
import { addCharacterToFamilyTree } from '../api/familyTreesApi';
import type { CharacterDto } from '../types/character';

interface Props {
  open: boolean;
  familyTreeId: string;
  worldId: string;
  existingCharacterIds: string[];
  onClose: () => void;
  onAdded: (character: CharacterDto) => void;
}

export default function AddEntityToTreeDialog({
  open, familyTreeId, worldId, existingCharacterIds, onClose, onAdded,
}: Props) {
  const [worldCharacters, setWorldCharacters] = useState<CharacterDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [newFirstName, setNewFirstName] = useState('');
  const [newLastName, setNewLastName] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    if (!open) return;
    setLoading(true);
    getCharactersByWorld(worldId)
      .then(all => setWorldCharacters(all.filter(c => !existingCharacterIds.includes(c.id))))
      .catch(() => setError('Failed to load characters'))
      .finally(() => setLoading(false));
  }, [open, worldId, existingCharacterIds]);

  const handleSelectExisting = async (character: CharacterDto) => {
    setSubmitting(true);
    try {
      await addCharacterToFamilyTree(familyTreeId, character.id);
      onAdded(character);
      handleClose();
    } catch (err) {
      setError(`Failed to add character to tree: ${err instanceof Error ? err.message : 'unknown error'}`);
    } finally {
      setSubmitting(false);
    }
  };

  const handleCreateNew = async () => {
    const name = [newFirstName.trim(), newLastName.trim()].filter(Boolean).join(' ');
    if (!name) { setError('Name is required'); return; }
    setSubmitting(true);
    let character: CharacterDto | undefined;
    try {
      character = await createCharacter({
        name,
        worldId,
        firstName: newFirstName.trim() || undefined,
        lastName: newLastName.trim() || undefined,
      });
    } catch (err) {
      setError(`Failed to create character: ${err instanceof Error ? err.message : 'unknown error'}`);
      setSubmitting(false);
      return;
    }
    try {
      await addCharacterToFamilyTree(familyTreeId, character.id);
      onAdded(character);
      handleClose();
    } catch (err) {
      setError(`Character created but failed to add to tree: ${err instanceof Error ? err.message : 'unknown error'}`);
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

          {worldCharacters.length > 0 && (
            <>
              <Divider><Typography variant="caption">or pick from world</Typography></Divider>
              {loading ? (
                <CircularProgress size={24} sx={{ alignSelf: 'center' }} />
              ) : (
                <List dense disablePadding sx={{ maxHeight: 240, overflow: 'auto', border: '1px solid', borderColor: 'divider', borderRadius: 1 }}>
                  {worldCharacters.map(character => (
                    <ListItemButton key={character.id} onClick={() => handleSelectExisting(character)} disabled={submitting}>
                      <ListItemText primary={character.name} secondary={character.species} />
                    </ListItemButton>
                  ))}
                </List>
              )}
            </>
          )}

          {error && <Typography color="error" variant="body2">{error}</Typography>}
        </Box>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose}>Cancel</Button>
      </DialogActions>
    </Dialog>
  );
}
