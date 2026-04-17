import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box, Typography, CircularProgress, Alert, Button, Grid2,
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import AddIcon from '@mui/icons-material/Add';
import { getWorld } from '../../api/worldsApi';
import { getEntities } from '../../api/entitiesApi';
import { getLocationsByWorld } from '../../api/locationsApi';
import type { WorldDto } from '../../types/world';
import type { EntityDto, EntityType } from '../../types/entity';
import type { LocationDto } from '../../types/location';
import { useEditMode } from '../../context/EditModeContext';
import { useAuth } from '../../context/AuthContext';
import {
  AddEntityDialog, EditEntityDialog,
  AddLocationDialog, EditLocationDialog, LocationCard, EntityCard,
} from '../../components';

const PLURAL: Record<string, string> = {
  Place: 'Locations',
  Group: 'Groups',
  Event: 'Events',
  Concept: 'Concepts',
};

export default function EntityListPage() {
  const { worldId, entityType } = useParams<{ worldId: string; entityType: string }>();
  const navigate = useNavigate();
  const { isEditMode } = useEditMode();
  const { userId } = useAuth();

  const isLocationType = entityType === 'Place';
  const plural = PLURAL[entityType ?? ''] ?? entityType ?? '';

  const [world, setWorld] = useState<WorldDto | null>(null);
  const [entities, setEntities] = useState<EntityDto[]>([]);
  const [locations, setLocations] = useState<LocationDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [addOpen, setAddOpen] = useState(false);
  const [editingEntity, setEditingEntity] = useState<EntityDto | null>(null);
  const [editingLocation, setEditingLocation] = useState<LocationDto | null>(null);

  useEffect(() => {
    if (!worldId || !entityType) return;
    if (isLocationType) {
      Promise.all([getWorld(worldId), getLocationsByWorld(worldId)])
        .then(([w, locs]) => {
          setWorld(w);
          setLocations(locs.sort((a, b) => new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime()));
        })
        .catch(() => setError('Failed to load'))
        .finally(() => setLoading(false));
    } else {
      Promise.all([getWorld(worldId), getEntities()])
        .then(([w, allEntities]) => {
          setWorld(w);
          setEntities(
            allEntities
              .filter(e => e.worldId === worldId && e.type === entityType)
              .sort((a, b) => new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime())
          );
        })
        .catch(() => setError('Failed to load'))
        .finally(() => setLoading(false));
    }
  }, [worldId, entityType, isLocationType]);

  if (loading) return <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>;
  if (error)   return <Alert severity="error" sx={{ m: 3 }}>{error}</Alert>;
  if (!world)  return null;

  const isOwner = !!userId && world.createdById === userId;
  const canEdit = isEditMode && isOwner;
  const count = isLocationType ? locations.length : entities.length;

  return (
    <Box sx={{ maxWidth: 1200, mx: 'auto', px: 3, py: 4 }}>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={4}>
        <Button startIcon={<ArrowBackIcon />} onClick={() => navigate(`/worlds/${worldId}`)}>
          {world.name}
        </Button>
        {canEdit && (
          <Button size="small" startIcon={<AddIcon />} onClick={() => setAddOpen(true)}>
            Add {entityType}
          </Button>
        )}
      </Box>

      <Typography variant="h4" fontWeight={700} mb={3}>
        {plural}
        <Typography component="span" variant="h6" color="text.secondary" fontWeight={400} ml={1.5}>
          {count}
        </Typography>
      </Typography>

      {count === 0 ? (
        <Typography variant="body2" color="text.secondary" fontStyle="italic">
          No {plural.toLowerCase()} yet.
        </Typography>
      ) : isLocationType ? (
        <Grid2 container spacing={2}>
          {locations.map(location => (
            <Grid2 key={location.id} size={{ xs: 12, sm: 6, md: 4, lg: 3 }}>
              <LocationCard
                location={location}
                canEdit={canEdit}
                onEdit={() => setEditingLocation(location)}
              />
            </Grid2>
          ))}
        </Grid2>
      ) : (
        <Grid2 container spacing={2}>
          {entities.map(entity => (
            <Grid2 key={entity.id} size={{ xs: 12, sm: 6, md: 4, lg: 3 }}>
              <EntityCard
                entity={entity}
                canEdit={canEdit}
                onEdit={() => setEditingEntity(entity)}
              />
            </Grid2>
          ))}
        </Grid2>
      )}

      {worldId && isLocationType && (
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

      {worldId && !isLocationType && (
        <AddEntityDialog
          open={addOpen}
          defaultType={entityType as EntityType}
          worldId={worldId}
          onClose={() => setAddOpen(false)}
          onCreated={entity => {
            setEntities(prev =>
              [entity, ...prev].sort(
                (a, b) => new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime()
              )
            );
            setAddOpen(false);
          }}
        />
      )}

      {editingEntity && (
        <EditEntityDialog
          open
          entity={editingEntity}
          onClose={() => setEditingEntity(null)}
          onSaved={updated => {
            setEntities(prev => prev.map(e => e.id === updated.id ? updated : e));
            setEditingEntity(null);
          }}
          onDeleted={() => {
            setEntities(prev => prev.filter(e => e.id !== editingEntity.id));
            setEditingEntity(null);
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
