import { useState } from 'react';
import {
  Dialog, DialogTitle, DialogContent, DialogActions, Button,
  TextField, MenuItem, Box, Select, FormControl, InputLabel,
} from '@mui/material';
import { createEntity } from '../api/entitiesApi';
import type { EntityDto, EntityType, Gender } from '../types/entity';

interface Props {
  open: boolean;
  worldId: string;
  defaultType: EntityType;
  onClose: () => void;
  onCreated: (entity: EntityDto) => void;
}

const ENTITY_TYPES: EntityType[] = ['Character', 'Place', 'Faction', 'Event', 'Concept'];
const GENDERS: Gender[] = ['Male', 'Female', 'Other', 'Unknown'];

export default function AddEntityDialog({ open, worldId, defaultType, onClose, onCreated }: Props) {
  const [type, setType] = useState<EntityType>(defaultType);
  const [name, setName] = useState('');
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
    setName(''); setFirstName(''); setLastName(''); setSpecies('');
    setGender('Unknown'); setBirthDate(''); setDeathDate('');
    setDescription(''); setError('');
    setType(defaultType);
  };

  const handleClose = () => { resetForm(); onClose(); };

  const handleSubmit = async () => {
    const effectiveName = type === 'Character' && (firstName.trim() || lastName.trim())
      ? [firstName.trim(), lastName.trim()].filter(Boolean).join(' ')
      : name.trim();

    if (!effectiveName) { setError('Name is required'); return; }
    setSubmitting(true);
    try {
      const entity = await createEntity({
        name: effectiveName,
        type,
        worldId,
        description: description.trim() || undefined,
        ...(type === 'Character' ? {
          firstName: firstName.trim() || undefined,
          lastName: lastName.trim() || undefined,
          species: species.trim() || undefined,
          gender,
          birthDate: birthDate || undefined,
          deathDate: deathDate || undefined,
        } : {}),
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

          {type === 'Character' ? (
            <>
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
            </>
          ) : (
            <TextField
              label="Name"
              value={name}
              onChange={e => setName(e.target.value)}
              required
              error={!!error}
              helperText={error || ''}
              autoFocus
            />
          )}

          <TextField label="Description" value={description} onChange={e => setDescription(e.target.value)} multiline rows={2} />
          {type === 'Character' && error && <Box color="error.main" fontSize={12}>{error}</Box>}
        </Box>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose}>Cancel</Button>
        <Button onClick={handleSubmit} variant="contained" disabled={submitting}>Add</Button>
      </DialogActions>
    </Dialog>
  );
}
