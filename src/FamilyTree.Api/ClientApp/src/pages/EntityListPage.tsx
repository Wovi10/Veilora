import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box, Typography, CircularProgress, Alert, Button,
  Card, CardActionArea, CardContent, Chip, Grid2,
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import AddIcon from '@mui/icons-material/Add';
import { getWorld } from '../api/worldsApi';
import { getEntities } from '../api/entitiesApi';
import type { WorldDto } from '../types/world';
import type { EntityDto, EntityType } from '../types/entity';
import { useEditMode } from '../context/EditModeContext';
import { useAuth } from '../context/AuthContext';
import AddEntityDialog from '../components/AddEntityDialog';

const PLURAL: Record<EntityType, string> = {
  Character: 'Characters',
  Place: 'Places',
  Group: 'Groups',
  Event: 'Events',
  Concept: 'Concepts',
};

export default function EntityListPage() {
  const { worldId, entityType } = useParams<{ worldId: string; entityType: string }>();
  const navigate = useNavigate();
  const { isEditMode } = useEditMode();
  const { userId } = useAuth();

  const type = entityType as EntityType;
  const plural = PLURAL[type] ?? type;

  const [world, setWorld] = useState<WorldDto | null>(null);
  const [entities, setEntities] = useState<EntityDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [addEntityOpen, setAddEntityOpen] = useState(false);

  useEffect(() => {
    if (!worldId) return;
    Promise.all([getWorld(worldId), getEntities()])
      .then(([w, allEntities]) => {
        setWorld(w);
        const filtered = allEntities
          .filter(e => e.worldId === worldId && e.type === type)
          .sort((a, b) => new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime());
        setEntities(filtered);
      })
      .catch(() => setError('Failed to load'))
      .finally(() => setLoading(false));
  }, [worldId, type]);

  if (loading) return <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>;
  if (error)   return <Alert severity="error" sx={{ m: 3 }}>{error}</Alert>;
  if (!world)  return null;

  const isOwner = !!userId && world.createdById === userId;
  const canEdit = isEditMode && isOwner;

  return (
    <Box sx={{ maxWidth: 1200, mx: 'auto', px: 3, py: 4 }}>
      {/* Header */}
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={4}>
        <Button startIcon={<ArrowBackIcon />} onClick={() => navigate(`/worlds/${worldId}`)}>
          {world.name}
        </Button>
        {canEdit && (
          <Button size="small" startIcon={<AddIcon />} onClick={() => setAddEntityOpen(true)}>
            Add {type}
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
                onClick={entity.type === 'Character'
                  ? () => navigate(`/worlds/${worldId}/characters/${entity.id}`)
                  : undefined}
              />
            </Grid2>
          ))}
        </Grid2>
      )}

      {worldId && (
        <AddEntityDialog
          open={addEntityOpen}
          defaultType={type}
          worldId={worldId}
          onClose={() => setAddEntityOpen(false)}
          onCreated={entity => {
            setEntities(prev =>
              [entity, ...prev].sort(
                (a, b) => new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime()
              )
            );
            setAddEntityOpen(false);
          }}
        />
      )}
    </Box>
  );
}

function EntityCard({ entity, onClick }: { entity: EntityDto; onClick?: () => void }) {
  const content = (
    <CardContent sx={{ pb: '12px !important' }}>
      <Typography variant="subtitle1" fontWeight={600} noWrap>{entity.name}</Typography>
      {entity.species && (
        <Chip label={entity.species} size="small" variant="outlined" sx={{ mt: 0.5, mr: 0.5 }} />
      )}
      {entity.birthDate && (
        <Typography variant="caption" color="text.secondary" display="block" mt={0.5}>
          °&nbsp;{new Date(entity.birthDate).toLocaleDateString('en-GB')}{entity.birthDateSuffix && ` ${entity.birthDateSuffix}`}
          {entity.deathDate && ` — †\u00a0${new Date(entity.deathDate).toLocaleDateString('en-GB')}${entity.deathDateSuffix ? ` ${entity.deathDateSuffix}` : ''}`}
        </Typography>
      )}
      {entity.description && !entity.birthDate && (
        <Typography
          variant="body2"
          color="text.secondary"
          sx={{ mt: 0.5, display: '-webkit-box', WebkitLineClamp: 2, WebkitBoxOrient: 'vertical', overflow: 'hidden' }}
        >
          {entity.description}
        </Typography>
      )}
      <Typography variant="caption" color="text.secondary" display="block" mt={0.5}>
        Updated {new Date(entity.updatedAt).toLocaleDateString('en-GB')}
      </Typography>
    </CardContent>
  );
  return (
    <Card sx={{ borderRadius: 2, height: '100%', transition: 'box-shadow 0.2s', '&:hover': { boxShadow: 3 } }}>
      {onClick ? <CardActionArea onClick={onClick} sx={{ height: '100%' }}>{content}</CardActionArea> : content}
    </Card>
  );
}
