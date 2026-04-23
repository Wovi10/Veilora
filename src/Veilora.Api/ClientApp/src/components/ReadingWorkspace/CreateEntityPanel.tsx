import { useState } from 'react';
import {
  Box, TextField, Typography, Button, CircularProgress, IconButton,
  FormControl, InputLabel, Select, MenuItem,
} from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';
import { createEntity } from '../../api/entitiesApi';
import { createLocation } from '../../api/locationsApi';
import { createCharacter } from '../../api/charactersApi';
import { createEvent } from '../../api/eventsApi';
import type { EntityDto, EntityType as EntitySubType } from '../../types/entity';
import type { LocationDto } from '../../types/location';
import type { CharacterDto } from '../../types/character';
import type { EventDto } from '../../types/event';

type EntityType = EntitySubType | 'Location' | 'Character' | 'Event';

const TYPE_OPTIONS: { value: EntityType; label: string }[] = [
  { value: 'Group',     label: 'Group' },
  { value: 'Concept',   label: 'Concept' },
  { value: 'Event',     label: 'Event' },
  { value: 'Location',  label: 'Location' },
  { value: 'Character', label: 'Character' },
];

type CreatedResult =
  | { kind: 'entity'; data: EntityDto }
  | { kind: 'location'; data: LocationDto }
  | { kind: 'character'; data: CharacterDto }
  | { kind: 'event'; data: EventDto };

interface Props {
  initialName: string;
  worldId: string;
  onCreated: (result: CreatedResult) => void;
  onClose: () => void;
}

export default function CreateEntityPanel({ initialName, worldId, onCreated, onClose }: Props) {
  const [name, setName] = useState(initialName);
  const [type, setType] = useState<EntityType>('Group');
  const [creating, setCreating] = useState(false);
  const [error, setError] = useState('');

  async function handleCreate() {
    const trimmed = name.trim();
    if (!trimmed) { setError('Name is required'); return; }
    setCreating(true);
    setError('');
    try {
      if (type === 'Location') {
        const data = await createLocation({ name: trimmed, worldId });
        onCreated({ kind: 'location', data });
      } else if (type === 'Character') {
        const data = await createCharacter({ name: trimmed, worldId });
        onCreated({ kind: 'character', data });
      } else if (type === 'Event') {
        const data = await createEvent({ name: trimmed, worldId });
        onCreated({ kind: 'event', data });
      } else {
        const data = await createEntity({ name: trimmed, type, worldId });
        onCreated({ kind: 'entity', data });
      }
    } catch {
      setError('Failed to create');
    } finally {
      setCreating(false);
    }
  }

  return (
    <Box sx={{ flex: 1, overflowY: 'auto' }}>
      <Box sx={{ px: 3, pt: 2.5, pb: 1.5, display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
        <Typography variant="subtitle2" fontWeight={700}>New entity</Typography>
        <IconButton size="small" onClick={onClose}>
          <CloseIcon fontSize="small" />
        </IconButton>
      </Box>

      <Box sx={{ px: 3, pb: 3, display: 'flex', flexDirection: 'column', gap: 2.5 }}>
        <Box sx={{ display: 'flex', gap: 1.5, alignItems: 'flex-start' }}>
          <TextField
            label="Name"
            size="small"
            autoFocus
            value={name}
            onChange={e => { setName(e.target.value); setError(''); }}
            error={!!error}
            helperText={error || ''}
            onKeyDown={e => { if (e.key === 'Enter') handleCreate(); }}
            sx={{ flex: 1 }}
          />
          <FormControl size="small" sx={{ minWidth: 130 }}>
            <InputLabel>Type</InputLabel>
            <Select
              value={type}
              label="Type"
              onChange={e => setType(e.target.value as EntityType)}
            >
              {TYPE_OPTIONS.map(t => (
                <MenuItem key={t.value} value={t.value}>{t.label}</MenuItem>
              ))}
            </Select>
          </FormControl>
        </Box>

        <Button
          variant="contained"
          size="small"
          onClick={handleCreate}
          disabled={creating || !name.trim()}
          startIcon={creating ? <CircularProgress size={14} color="inherit" /> : undefined}
          sx={{ textTransform: 'none', alignSelf: 'flex-start' }}
        >
          Create
        </Button>
      </Box>
    </Box>
  );
}
