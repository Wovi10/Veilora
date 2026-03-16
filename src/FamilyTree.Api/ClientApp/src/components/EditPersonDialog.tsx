import { useState } from 'react';
import {
  Alert,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  FormControl,
  InputLabel,
  MenuItem,
  Select,
  TextField,
} from '@mui/material';
import { updatePerson } from '../api/personsApi';
import type { Gender, PersonDto } from '../types/person';

interface Props {
  open: boolean;
  person: PersonDto;
  persons: PersonDto[];
  onClose: () => void;
  onSaved: (person: PersonDto) => void;
}

const fullName = (p: PersonDto) =>
  [p.firstName, p.middleName, p.lastName].filter(Boolean).join(' ');

export default function EditPersonDialog({ open, person, persons, onClose, onSaved }: Props) {
  const [firstName, setFirstName] = useState(person.firstName);
  const [lastName, setLastName] = useState(person.lastName);
  const [gender, setGender] = useState<Gender>(person.gender);
  const [birthDate, setBirthDate] = useState(person.birthDate ?? '');
  const [deathDate, setDeathDate] = useState(person.deathDate ?? '');
  const [parent1Id, setParent1Id] = useState<string | null>(person.parent1Id ?? null);
  const [parent2Id, setParent2Id] = useState<string | null>(person.parent2Id ?? null);

  const otherPersons = persons.filter(p => p.id !== person.id);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  function handleClose() {
    setError(null);
    onClose();
  }

  async function handleSubmit() {
    if (!firstName.trim() || !lastName.trim()) {
      setError('First name and last name are required');
      return;
    }
    setSubmitting(true);
    setError(null);
    try {
      const updated = await updatePerson(person.id, {
        firstName: firstName.trim(),
        lastName: lastName.trim(),
        gender,
        birthDate: birthDate || undefined,
        deathDate: deathDate || undefined,
        parent1Id,
        parent2Id,
      });
      onSaved(updated);
      handleClose();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to update person');
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <Dialog open={open} onClose={handleClose} fullWidth maxWidth="sm">
      <DialogTitle>Edit Person</DialogTitle>
      <DialogContent sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: '16px !important' }}>
        {error && <Alert severity="error">{error}</Alert>}
        <TextField
          label="First Name"
          value={firstName}
          onChange={(e) => setFirstName(e.target.value)}
          required
          autoFocus
          fullWidth
        />
        <TextField
          label="Last Name"
          value={lastName}
          onChange={(e) => setLastName(e.target.value)}
          required
          fullWidth
        />
        <FormControl fullWidth>
          <InputLabel>Gender</InputLabel>
          <Select
            value={gender}
            label="Gender"
            onChange={(e) => setGender(e.target.value as Gender)}
          >
            <MenuItem value="Male">Male</MenuItem>
            <MenuItem value="Female">Female</MenuItem>
            <MenuItem value="Other">Other</MenuItem>
            <MenuItem value="Unknown">Unknown</MenuItem>
          </Select>
        </FormControl>
        <TextField
          label="Birth Date"
          type="date"
          value={birthDate}
          onChange={(e) => setBirthDate(e.target.value)}
          fullWidth
          slotProps={{ inputLabel: { shrink: true } }}
        />
        <TextField
          label="Death Date"
          type="date"
          value={deathDate}
          onChange={(e) => setDeathDate(e.target.value)}
          fullWidth
          slotProps={{ inputLabel: { shrink: true } }}
        />
        <FormControl fullWidth>
          <InputLabel>Parent 1</InputLabel>
          <Select
            value={parent1Id ?? ''}
            label="Parent 1"
            onChange={(e) => setParent1Id(e.target.value || null)}
          >
            <MenuItem value=""><em>Unknown</em></MenuItem>
            {otherPersons.map(p => (
              <MenuItem key={p.id} value={p.id}>{fullName(p)}</MenuItem>
            ))}
          </Select>
        </FormControl>
        <FormControl fullWidth>
          <InputLabel>Parent 2</InputLabel>
          <Select
            value={parent2Id ?? ''}
            label="Parent 2"
            onChange={(e) => setParent2Id(e.target.value || null)}
          >
            <MenuItem value=""><em>Unknown</em></MenuItem>
            {otherPersons.map(p => (
              <MenuItem key={p.id} value={p.id}>{fullName(p)}</MenuItem>
            ))}
          </Select>
        </FormControl>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose} disabled={submitting}>Cancel</Button>
        <Button variant="contained" onClick={handleSubmit} disabled={submitting}>
          {submitting ? 'Saving…' : 'Save'}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
