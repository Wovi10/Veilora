import { useState } from 'react';
import {
  Alert, Button, Dialog, DialogActions, DialogContent, DialogTitle,
  FormControl, InputLabel, MenuItem, Select, TextField, Typography,
} from '@mui/material';
import { createRelationship } from '../api/relationshipsApi';
import type { RelationshipDto, RelationshipType } from '../types/relationship';
import type { CharacterDto } from '../types/character';

interface Props {
  open: boolean;
  character1: CharacterDto;
  character2: CharacterDto;
  onClose: () => void;
  onCreated: (rel: RelationshipDto) => void;
}

const RELATIONSHIP_TYPES: RelationshipType[] = [
  'Spouse', 'Partner', 'Godparent', 'Guardian', 'CloseFriend',
];

export default function AddRelationshipDialog({ open, character1, character2, onClose, onCreated }: Props) {
  const [relationshipType, setRelationshipType] = useState<RelationshipType | ''>('');
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');
  const [notes, setNotes] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  function handleClose() {
    setRelationshipType(''); setStartDate(''); setEndDate('');
    setNotes(''); setError(null);
    onClose();
  }

  async function handleSubmit() {
    if (!relationshipType) { setError('Relationship type is required'); return; }
    setSubmitting(true);
    setError(null);
    try {
      const rel = await createRelationship({
        character1Id: character1.id,
        character2Id: character2.id,
        relationshipType,
        startDate: startDate || undefined,
        endDate: endDate || undefined,
        notes: notes.trim() || undefined,
      });
      onCreated(rel);
      handleClose();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to create relationship');
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <Dialog open={open} onClose={handleClose} fullWidth maxWidth="sm">
      <DialogTitle>Add Relationship</DialogTitle>
      <DialogContent sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: '16px !important' }}>
        {error && <Alert severity="error">{error}</Alert>}
        <Typography variant="body2" color="text.secondary">
          <strong>Character 1:</strong> {character1.name}
        </Typography>
        <Typography variant="body2" color="text.secondary">
          <strong>Character 2:</strong> {character2.name}
        </Typography>
        <FormControl fullWidth required>
          <InputLabel>Relationship Type</InputLabel>
          <Select
            value={relationshipType}
            label="Relationship Type"
            onChange={e => setRelationshipType(e.target.value as RelationshipType)}
            autoFocus
          >
            {RELATIONSHIP_TYPES.map(t => <MenuItem key={t} value={t}>{t}</MenuItem>)}
          </Select>
        </FormControl>
        <TextField label="Start Date" type="date" value={startDate} onChange={e => setStartDate(e.target.value)} fullWidth slotProps={{ inputLabel: { shrink: true } }} />
        <TextField label="End Date" type="date" value={endDate} onChange={e => setEndDate(e.target.value)} fullWidth slotProps={{ inputLabel: { shrink: true } }} />
        <TextField label="Notes" value={notes} onChange={e => setNotes(e.target.value)} fullWidth multiline rows={2} />
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
