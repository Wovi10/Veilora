import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box, Typography, CircularProgress, Alert, Button, Grid2,
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import AddIcon from '@mui/icons-material/Add';
import { getWorld } from '../../api/worldsApi';
import { getLocationsByWorld } from '../../api/locationsApi';
import type { WorldDto } from '../../types/world';
import type { LocationDto } from '../../types/location';
import { useEditMode } from '../../context/EditModeContext';
import { useAuth } from '../../context/AuthContext';
import { AddLocationDialog, EditLocationDialog, LocationCard } from '../../components';

export default function Locations() {
  const { worldId } = useParams<{ worldId: string }>();
  const navigate = useNavigate();
  const { isEditMode } = useEditMode();
  const { userId } = useAuth();

  const [world, setWorld] = useState<WorldDto | null>(null);
  const [locations, setLocations] = useState<LocationDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [addOpen, setAddOpen] = useState(false);
  const [editingLocation, setEditingLocation] = useState<LocationDto | null>(null);

  useEffect(() => {
    if (!worldId) return;
    Promise.all([getWorld(worldId), getLocationsByWorld(worldId)])
      .then(([w, locs]) => {
        setWorld(w);
        setLocations(locs.sort((a, b) => new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime()));
      })
      .catch(() => setError('Failed to load'))
      .finally(() => setLoading(false));
  }, [worldId]);

  if (loading) return <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>;
  if (error)   return <Alert severity="error" sx={{ m: 3 }}>{error}</Alert>;
  if (!world)  return null;

  const isOwner = !!userId && world.createdById === userId;
  const canEdit = isEditMode && isOwner;

  return (
    <Box sx={{ maxWidth: 1200, mx: 'auto', px: 3, py: 4 }}>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={4}>
        <Button startIcon={<ArrowBackIcon />} onClick={() => navigate(`/worlds/${worldId}`)}>
          {world.name}
        </Button>
        {canEdit && (
          <Button size="small" startIcon={<AddIcon />} onClick={() => setAddOpen(true)}>
            Add Location
          </Button>
        )}
      </Box>

      <Typography variant="h4" fontWeight={700} mb={3}>
        Locations
        <Typography component="span" variant="h6" color="text.secondary" fontWeight={400} ml={1.5}>
          {locations.length}
        </Typography>
      </Typography>

      {locations.length === 0 ? (
        <Typography variant="body2" color="text.secondary" fontStyle="italic">
          No locations yet.
        </Typography>
      ) : (
        <Grid2 container spacing={2}>
          {locations.map(location => (
            <Grid2 key={location.id} size={{ xs: 12, sm: 6, md: 4, lg: 3 }}>
              <LocationCard
                location={location}
                onClick={() => navigate(`/worlds/${worldId}/locations/${location.id}`)}
                canEdit={canEdit}
                onEdit={() => setEditingLocation(location)}
              />
            </Grid2>
          ))}
        </Grid2>
      )}

      {worldId && (
        <AddLocationDialog
          open={addOpen}
          worldId={worldId}
          onClose={() => setAddOpen(false)}
          onCreated={location => {
            setLocations(prev =>
              [location, ...prev].sort(
                (a, b) => new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime()
              )
            );
            setAddOpen(false);
          }}
        />
      )}

      {editingLocation && (
        <EditLocationDialog
          open
          location={editingLocation}
          onClose={() => setEditingLocation(null)}
          onSaved={updated => {
            setLocations(prev => prev.map(l => l.id === updated.id ? updated : l));
            setEditingLocation(null);
          }}
          onDeleted={() => {
            setLocations(prev => prev.filter(l => l.id !== editingLocation.id));
            setEditingLocation(null);
          }}
        />
      )}
    </Box>
  );
}
