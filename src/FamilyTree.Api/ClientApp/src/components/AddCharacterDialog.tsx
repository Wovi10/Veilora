import { useState } from 'react';
import {
  Dialog, DialogTitle, DialogContent, DialogActions, Button,
  TextField, MenuItem, Box, Select, FormControl, InputLabel,
} from '@mui/material';
import { createCharacter } from '../api/charactersApi';
import type { CharacterDto } from '../types/character';
import type { Gender } from '../types/character';

interface Props {
  open: boolean;
  worldId: string;
  onClose: () => void;
  onCreated: (character: CharacterDto) => void;
}

const GENDERS: Gender[] = ['Male', 'Female', 'Other', 'Unknown'];

export default function AddCharacterDialog({ open, worldId, onClose, onCreated }: Props) {
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [species, setSpecies] = useState('');
  const [gender, setGender] = useState<Gender>('Unknown');
  const [birthDate, setBirthDate] = useState('');
  const [deathDate, setDeathDate] = useState('');
  const [description, setDescription] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');

  const resetForm = () => {
    setFirstName(''); setLastName(''); setSpecies('');
    setGender('Unknown'); setBirthDate(''); setDeathDate('');
    setDescription(''); setError('');
  };

  const handleClose = () => { resetForm(); onClose(); };

  const handleSubmit = async () => {
    const name = [firstName.trim(), lastName.trim()].filter(Boolean).join(' ');
    if (!name) { setError('Name is required'); return; }
    setSubmitting(true);
    try {
      const character = await createCharacter({
        name,
        worldId,
        description: description.trim() || undefined,
        firstName: firstName.trim() || undefined,
        lastName: lastName.trim() || undefined,
        species: species.trim() || undefined,
        gender,
        birthDate: birthDate || undefined,
        deathDate: deathDate || undefined,
      });
      onCreated(character);
      resetForm();
    } catch {
      setError('Failed to create character');
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>Add Character</DialogTitle>
      <DialogContent>
        <Box display="flex" flexDirection="column" gap={2} pt={1}>
          <Box display="flex" gap={2}>
            <TextField label="First Name" value={firstName} onChange={e => setFirstName(e.target.value)} fullWidth autoFocus />
            <TextField label="Last Name" value={lastName} onChange={e => setLastName(e.target.value)} fullWidth />
          </Box>
          <TextField label="Species / Race" value={species} onChange={e => setSpecies(e.target.value)} placeholder="e.g. Elf, Hobbit" />
          <FormControl fullWidth>
            <InputLabel>Gender</InputLabel>
            <Select value={gender} label="Gender" onChange={e => setGender(e.target.value as Gender)}>
              {GENDERS.map(g => <MenuItem key={g} value={g}>{g}</MenuItem>)}
            </Select>
          </FormControl>
          <Box display="flex" gap={2}>
            <TextField label="Birth Date" type="date" value={birthDate} onChange={e => setBirthDate(e.target.value)} slotProps={{ inputLabel: { shrink: true } }} fullWidth />
            <TextField label="Death Date" type="date" value={deathDate} onChange={e => setDeathDate(e.target.value)} slotProps={{ inputLabel: { shrink: true } }} fullWidth />
          </Box>
          <TextField label="Description" value={description} onChange={e => setDescription(e.target.value)} multiline rows={2} />
          {error && <Box color="error.main" fontSize={12}>{error}</Box>}
        </Box>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose}>Cancel</Button>
        <Button onClick={handleSubmit} variant="contained" disabled={submitting}>Add</Button>
      </DialogActions>
    </Dialog>
  );
}
