import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box, Typography, CircularProgress, Alert, Button, Grid2,
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import AddIcon from '@mui/icons-material/Add';
import { getWorld } from '../../api/worldsApi';
import { getEntities } from '../../api/entitiesApi';
import type { WorldDto } from '../../types/world';
import type { EntityDto, EntityType } from '../../types/entity';
import { useEditMode } from '../../context/EditModeContext';
import { useAuth } from '../../context/AuthContext';
import { AddEntityDialog, EditEntityDialog, EntityCard } from '../../components';

const PLURAL: Record<string, string> = {
  Group: 'Groups',
  Event: 'Events',
  Concept: 'Concepts',
};

export default function EntityListPage() {
  const { worldId, entityType } = useParams<{ worldId: string; entityType: string }>();
  const navigate = useNavigate();
  const { isEditMode } = useEditMode();
  const { userId } = useAuth();

  const plural = PLURAL[entityType ?? ''] ?? entityType ?? '';

  const [world, setWorld] = useState<WorldDto | null>(null);
  const [entities, setEntities] = useState<EntityDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [addOpen, setAddOpen] = useState(false);
  const [editingEntity, setEditingEntity] = useState<EntityDto | null>(null);

  useEffect(() => {
    if (!worldId || !entityType) return;
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
  }, [worldId, entityType]);

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
            Add {entityType}
          </Button>
        )}
      </Box>

      <Typography variant="h4" fontWeight={700} mb={3}>
        {plural}
        <Typography component="span" variant="h6" color="text.secondary" fontWeight={400} ml={1.5}>
          {entities.length}
        </Typography>
      </Typography>

      {entities.length === 0 ? (
        <Typography variant="body2" color="text.secondary" fontStyle="italic">
          No {plural.toLowerCase()} yet.
        </Typography>
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

      {worldId && (
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
    </Box>
  );
}
