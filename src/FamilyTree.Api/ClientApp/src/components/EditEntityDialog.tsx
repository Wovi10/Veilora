import { useState, useEffect } from 'react';
import {
  Dialog, DialogTitle, DialogContent, DialogActions, Button,
  TextField, MenuItem, Box, Select, FormControl, InputLabel, Typography,
} from '@mui/material';
import { updateEntity } from '../api/entitiesApi';
import type { EntityDto, Gender } from '../types/entity';

interface Props {
  open: boolean;
  entity: EntityDto;
  treeEntities: EntityDto[];
  onClose: () => void;
  onSaved: (entity: EntityDto) => void;
}

const GENDERS: Gender[] = ['Male', 'Female', 'Other', 'Unknown'];

export default function EditEntityDialog({ open, entity, treeEntities, onClose, onSaved }: Props) {
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [species, setSpecies] = useState('');
  const [gender, setGender] = useState<Gender>('Unknown');
  const [birthDate, setBirthDate] = useState('');
  const [birthDateSuffix, setBirthDateSuffix] = useState('');
  const [deathDate, setDeathDate] = useState('');
  const [deathDateSuffix, setDeathDateSuffix] = useState('');
  const [description, setDescription] = useState('');
  const [parent1Id, setParent1Id] = useState('');
  const [parent2Id, setParent2Id] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    if (entity) {
      setFirstName(entity.firstName ?? '');
      setLastName(entity.lastName ?? '');
      setSpecies(entity.species ?? '');
      setGender(entity.gender ?? 'Unknown');
      setBirthDate(entity.birthDate ?? '');
      setBirthDateSuffix(entity.birthDateSuffix ?? '');
      setDeathDate(entity.deathDate ?? '');
      setDeathDateSuffix(entity.deathDateSuffix ?? '');
      setDescription(entity.description ?? '');
      setParent1Id(entity.parent1Id ?? '');
      setParent2Id(entity.parent2Id ?? '');
    }
  }, [entity]);

  const handleSubmit = async () => {
    const effectiveName = (firstName.trim() || lastName.trim())
      ? [firstName.trim(), lastName.trim()].filter(Boolean).join(' ')
      : entity.name;
    setSubmitting(true);
    try {
      const updated = await updateEntity(entity.id, {
        name: effectiveName,
        type: entity.type,
        description: description.trim() || undefined,
        firstName: firstName.trim() || undefined,
        lastName: lastName.trim() || undefined,
        species: species.trim() || undefined,
        gender,
        birthDate: birthDate || undefined,
        birthDateSuffix: birthDateSuffix.trim() || undefined,
        deathDate: deathDate || undefined,
        deathDateSuffix: deathDateSuffix.trim() || undefined,
        parent1Id: parent1Id || null,
        parent2Id: parent2Id || null,
      });
      onSaved(updated);
      setError('');
    } catch {
      setError('Failed to save changes');
    } finally {
      setSubmitting(false);
    }
  };

  const others = treeEntities.filter(e => e.id !== entity.id && e.type === 'Character');

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>Edit {entity.type}</DialogTitle>
      <DialogContent>
        <Box display="flex" flexDirection="column" gap={2} pt={1}>
          {entity.type === 'Character' ? (
            <>
              <Box display="flex" gap={2}>
                <TextField label="First Name" value={firstName} onChange={e => setFirstName(e.target.value)} fullWidth />
                <TextField label="Last Name" value={lastName} onChange={e => setLastName(e.target.value)} fullWidth />
              </Box>
              <TextField label="Species / Race" value={species} onChange={e => setSpecies(e.target.value)} />
              <FormControl fullWidth>
                <InputLabel>Gender</InputLabel>
                <Select value={gender} label="Gender" onChange={e => setGender(e.target.value as Gender)}>
                  {GENDERS.map(g => <MenuItem key={g} value={g}>{g}</MenuItem>)}
                </Select>
              </FormControl>
              <Box display="flex" gap={2}>
                <TextField label="Birth Date" type="date" value={birthDate} onChange={e => setBirthDate(e.target.value)} slotProps={{ inputLabel: { shrink: true } }} fullWidth />
                <TextField label="Birth Era / Suffix" placeholder="e.g. TA, SA, FA" value={birthDateSuffix} onChange={e => setBirthDateSuffix(e.target.value)} sx={{ maxWidth: 160 }} />
              </Box>
              <Box display="flex" gap={2}>
                <TextField label="Death Date" type="date" value={deathDate} onChange={e => setDeathDate(e.target.value)} slotProps={{ inputLabel: { shrink: true } }} fullWidth />
                <TextField label="Death Era / Suffix" placeholder="e.g. TA, SA, FA" value={deathDateSuffix} onChange={e => setDeathDateSuffix(e.target.value)} sx={{ maxWidth: 160 }} />
              </Box>
              <FormControl fullWidth>
                <InputLabel>Parent 1</InputLabel>
                <Select value={parent1Id} label="Parent 1" onChange={e => setParent1Id(e.target.value)}>
                  <MenuItem value="">None</MenuItem>
                  {others.map(e => <MenuItem key={e.id} value={e.id}>{e.name}</MenuItem>)}
                </Select>
              </FormControl>
              <FormControl fullWidth>
                <InputLabel>Parent 2</InputLabel>
                <Select value={parent2Id} label="Parent 2" onChange={e => setParent2Id(e.target.value)}>
                  <MenuItem value="">None</MenuItem>
                  {others.filter(e => e.id !== parent1Id).map(e => <MenuItem key={e.id} value={e.id}>{e.name}</MenuItem>)}
                </Select>
              </FormControl>
            </>
          ) : (
            <TextField label="Description" value={description} onChange={e => setDescription(e.target.value)} multiline rows={3} />
          )}
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
