import { useState, useEffect } from 'react';
import {
  Dialog, DialogTitle, DialogContent, DialogActions, Button,
  TextField, Box, Typography,
} from '@mui/material';
import { updateLocation, deleteLocation } from '../api/locationsApi';
import type { LocationDto } from '../types/location';

interface Props {
  open: boolean;
  location: LocationDto;
  onClose: () => void;
  onSaved: (location: LocationDto) => void;
  onDeleted?: () => void;
}

export default function EditLocationDialog({ open, location, onClose, onSaved, onDeleted }: Props) {
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [deleting, setDeleting] = useState(false);
  const [confirmDelete, setConfirmDelete] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    if (!location) return;
    setName(location.name);
    setDescription(location.description ?? '');
    setConfirmDelete(false);
    setError('');
  }, [location]);

  const handleSubmit = async () => {
    if (!name.trim()) { setError('Name is required'); return; }
    setSubmitting(true);
    setError('');
    try {
      const updated = await updateLocation(location.id, {
        name: name.trim(),
        description: description.trim() || undefined,
      });
      onSaved(updated);
    } catch {
      setError('Failed to save changes');
    } finally {
      setSubmitting(false);
    }
  };

  const handleDelete = async () => {
    setDeleting(true);
    try {
      await deleteLocation(location.id);
      onDeleted?.();
    } catch {
      setError('Failed to delete');
      setConfirmDelete(false);
    } finally {
      setDeleting(false);
    }
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>Edit Location</DialogTitle>
      <DialogContent>
        <Box display="flex" flexDirection="column" gap={2} pt={1}>
          <TextField label="Name" value={name} onChange={e => setName(e.target.value)} autoFocus />
          <TextField label="Description" value={description} onChange={e => setDescription(e.target.value)} multiline rows={3} />
          {error && <Typography color="error" variant="caption">{error}</Typography>}
        </Box>
      </DialogContent>
      <DialogActions sx={{ justifyContent: 'space-between' }}>
        {!confirmDelete ? (
          <Button color="error" onClick={() => setConfirmDelete(true)} disabled={submitting || deleting}>
            Delete
          </Button>
        ) : (
          <Box display="flex" alignItems="center" gap={1}>
            <Typography variant="body2" color="text.secondary">Delete this location?</Typography>
            <Button color="error" variant="contained" onClick={handleDelete} disabled={deleting} size="small">
              {deleting ? 'Deleting…' : 'Yes, Delete'}
            </Button>
            <Button onClick={() => setConfirmDelete(false)} disabled={deleting} size="small">No</Button>
          </Box>
        )}
        {!confirmDelete && (
          <Box display="flex" gap={1}>
            <Button onClick={onClose} disabled={submitting}>Cancel</Button>
            <Button onClick={handleSubmit} variant="contained" disabled={submitting}>Save</Button>
          </Box>
        )}
      </DialogActions>
    </Dialog>
  );
}
