import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box, Typography, CircularProgress, Alert, Button, Divider, IconButton,
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import EditIcon from '@mui/icons-material/Edit';
import { getLocation } from '../../api/locationsApi';
import { getWorld } from '../../api/worldsApi';
import type { LocationDto } from '../../types/location';
import type { WorldDto } from '../../types/world';
import { useEditMode } from '../../context/EditModeContext';
import { useAuth } from '../../context/AuthContext';
import { EditLocationDialog } from '../../components';

export default function Location() {
  const { worldId, locationId } = useParams<{ worldId: string; locationId: string }>();
  const navigate = useNavigate();
  const { isEditMode } = useEditMode();
  const { userId } = useAuth();

  const [world, setWorld] = useState<WorldDto | null>(null);
  const [location, setLocation] = useState<LocationDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [editOpen, setEditOpen] = useState(false);

  useEffect(() => {
    if (!worldId || !locationId) return;
    Promise.all([getWorld(worldId), getLocation(locationId)])
      .then(([w, loc]) => {
        setWorld(w);
        setLocation(loc);
      })
      .catch(() => setError('Failed to load'))
      .finally(() => setLoading(false));
  }, [worldId, locationId]);

  if (loading) return <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>;
  if (error)   return <Alert severity="error" sx={{ m: 3 }}>{error}</Alert>;
  if (!world || !location) return null;

  const isOwner = !!userId && world.createdById === userId;
  const canEdit = isEditMode && isOwner;

  return (
    <Box sx={{ maxWidth: 900, mx: 'auto', px: 3, py: 4 }}>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={4}>
        <Button startIcon={<ArrowBackIcon />} onClick={() => navigate(`/worlds/${worldId}/locations`)}>
          Locations
        </Button>
        {canEdit && (
          <IconButton onClick={() => setEditOpen(true)} size="small">
            <EditIcon />
          </IconButton>
        )}
      </Box>

      <Typography variant="h3" fontWeight={700} mb={0.5}>
        {location.name}
      </Typography>
      <Typography variant="caption" color="text.secondary">
        Updated {new Date(location.updatedAt).toLocaleDateString('en-GB')}
      </Typography>

      {location.description && (
        <Box mt={4}>
          <Typography variant="h6" fontWeight={600} mb={1.5}>Description</Typography>
          <Divider sx={{ mb: 2 }} />
          <Typography variant="body1" color="text.secondary" sx={{ whiteSpace: 'pre-wrap' }}>
            {location.description}
          </Typography>
        </Box>
      )}

      {editOpen && (
        <EditLocationDialog
          open
          location={location}
          onClose={() => setEditOpen(false)}
          onSaved={updated => {
            setLocation(updated);
            setEditOpen(false);
          }}
          onDeleted={() => navigate(`/worlds/${worldId}/locations`)}
        />
      )}
    </Box>
  );
}
