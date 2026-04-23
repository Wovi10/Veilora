import { useState } from 'react';
import {
  Box, TextField, Chip, IconButton, Button, Typography, CircularProgress,
} from '@mui/material';
import CheckIcon from '@mui/icons-material/Check';
import CloseIcon from '@mui/icons-material/Close';
import GroupsIcon from '@mui/icons-material/Groups';
import BoltIcon from '@mui/icons-material/Bolt';
import LightbulbIcon from '@mui/icons-material/Lightbulb';
import { updateEntity } from '../../api/entitiesApi';
import type { EntityDto, EntityType } from '../../types/entity';

const TYPE_META: Record<EntityType, { icon: React.ReactNode; color: 'primary' | 'warning' | 'secondary' }> = {
  Group: { icon: <GroupsIcon fontSize="small" />, color: 'primary' },
  Event: { icon: <BoltIcon fontSize="small" />, color: 'warning' },
  Concept: { icon: <LightbulbIcon fontSize="small" />, color: 'secondary' },
};

interface Props {
  entity: EntityDto;
  onSaved: (entity: EntityDto) => void;
  onClose: () => void;
}

export default function EntityEditor({ entity, onSaved, onClose }: Props) {
  const [name, setName] = useState(entity.name);
  const [description, setDescription] = useState(entity.description ?? '');
  const [saving, setSaving] = useState(false);
  const [saved, setSaved] = useState(false);
  const [error, setError] = useState('');

  const meta = TYPE_META[entity.type as EntityType];
  const isDirty = name.trim() !== entity.name || description.trim() !== (entity.description ?? '');

  async function handleSave() {
    if (!name.trim()) { setError('Name is required'); return; }
    setSaving(true);
    setError('');
    try {
      const updated = await updateEntity(entity.id, {
        name: name.trim(),
        type: entity.type,
        description: description.trim() || undefined,
      });
      onSaved(updated);
      setSaved(true);
      setTimeout(() => setSaved(false), 2000);
    } catch {
      setError('Failed to save changes');
    } finally {
      setSaving(false);
    }
  }

  return (
    <Box sx={{ flex: 1, overflowY: 'auto', display: 'flex', flexDirection: 'column' }}>
      {/* Header */}
      <Box sx={{ px: 3, pt: 2.5, pb: 1.5, display: 'flex', alignItems: 'flex-start', gap: 1 }}>
        <Box sx={{ flex: 1 }}>
          <TextField
            fullWidth
            size="small"
            value={name}
            onChange={e => { setName(e.target.value); setError(''); }}
            error={!!error}
            helperText={error || ''}
            inputProps={{ style: { fontWeight: 600 } }}
            variant="standard"
          />
        </Box>
        <Chip
          icon={<Box sx={{ display: 'flex', alignItems: 'center', pl: 0.5 }}>{meta.icon}</Box>}
          label={entity.type}
          size="small"
          color={meta.color}
          variant="outlined"
          sx={{ mt: 0.5, flexShrink: 0 }}
        />
        <IconButton size="small" onClick={onClose} sx={{ mt: 0.25, flexShrink: 0 }}>
          <CloseIcon fontSize="small" />
        </IconButton>
      </Box>

      {/* Description */}
      <Box sx={{ px: 3, pb: 2 }}>
        <Typography variant="caption" color="text.secondary" display="block" mb={0.5}>
          Description
        </Typography>
        <TextField
          fullWidth
          multiline
          minRows={4}
          maxRows={12}
          size="small"
          placeholder="Describe this entity…"
          value={description}
          onChange={e => setDescription(e.target.value)}
        />
      </Box>

      {/* Actions */}
      <Box sx={{ px: 3, pb: 2.5, display: 'flex', alignItems: 'center', gap: 1.5 }}>
        <Button
          variant="contained"
          size="small"
          onClick={handleSave}
          disabled={saving || !isDirty}
          color={saved ? 'success' : 'primary'}
          startIcon={saving ? <CircularProgress size={14} color="inherit" /> : saved ? <CheckIcon fontSize="small" /> : undefined}
          sx={{ textTransform: 'none' }}
        >
          {saved ? 'Saved' : 'Save'}
        </Button>
        {isDirty && (
          <Button
            size="small"
            onClick={() => { setName(entity.name); setDescription(entity.description ?? ''); setError(''); }}
            sx={{ textTransform: 'none', color: 'text.secondary' }}
          >
            Discard
          </Button>
        )}
      </Box>
    </Box>
  );
}
