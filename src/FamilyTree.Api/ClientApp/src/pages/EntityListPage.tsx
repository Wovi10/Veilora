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
import { getCharactersByWorld } from '../api/charactersApi';
import type { WorldDto } from '../types/world';
import type { EntityDto, EntityType } from '../types/entity';
import type { CharacterDto } from '../types/character';
import { useEditMode } from '../context/EditModeContext';
import { useAuth } from '../context/AuthContext';
import AddCharacterDialog from '../components/AddCharacterDialog';
import AddEntityDialog from '../components/AddEntityDialog';

const PLURAL: Record<string, string> = {
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

  const isCharacterType = entityType === 'Character';
  const plural = PLURAL[entityType ?? ''] ?? entityType ?? '';

  const [world, setWorld] = useState<WorldDto | null>(null);
  const [characters, setCharacters] = useState<CharacterDto[]>([]);
  const [entities, setEntities] = useState<EntityDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [addOpen, setAddOpen] = useState(false);

  useEffect(() => {
    if (!worldId || !entityType) return;
    if (isCharacterType) {
      Promise.all([getWorld(worldId), getCharactersByWorld(worldId)])
        .then(([w, chars]) => {
          setWorld(w);
          setCharacters(chars.sort((a, b) => new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime()));
        })
        .catch(() => setError('Failed to load'))
        .finally(() => setLoading(false));
    } else {
      Promise.all([getWorld(worldId), getEntities()])
        .then(([w, allEntities]) => {
          setWorld(w);
          const filtered = allEntities
            .filter(e => e.worldId === worldId && e.type === entityType)
            .sort((a, b) => new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime());
          setEntities(filtered);
        })
        .catch(() => setError('Failed to load'))
        .finally(() => setLoading(false));
    }
  }, [worldId, entityType, isCharacterType]);

  if (loading) return <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>;
  if (error)   return <Alert severity="error" sx={{ m: 3 }}>{error}</Alert>;
  if (!world)  return null;

  const isOwner = !!userId && world.createdById === userId;
  const canEdit = isEditMode && isOwner;
  const count = isCharacterType ? characters.length : entities.length;

  return (
    <Box sx={{ maxWidth: 1200, mx: 'auto', px: 3, py: 4 }}>
      {/* Header */}
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
      ) : isCharacterType ? (
        <Grid2 container spacing={2}>
          {characters.map(character => (
            <Grid2 key={character.id} size={{ xs: 12, sm: 6, md: 4, lg: 3 }}>
              <CharacterCard
                character={character}
                onClick={() => navigate(`/worlds/${worldId}/characters/${character.id}`)}
              />
            </Grid2>
          ))}
        </Grid2>
      ) : (
        <Grid2 container spacing={2}>
          {entities.map(entity => (
            <Grid2 key={entity.id} size={{ xs: 12, sm: 6, md: 4, lg: 3 }}>
              <EntityCard entity={entity} />
            </Grid2>
          ))}
        </Grid2>
      )}

      {worldId && isCharacterType && (
        <AddCharacterDialog
          open={addOpen}
          worldId={worldId}
          onClose={() => setAddOpen(false)}
          onCreated={character => {
            setCharacters(prev =>
              [character, ...prev].sort(
                (a, b) => new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime()
              )
            );
            setAddOpen(false);
          }}
        />
      )}

      {worldId && !isCharacterType && (
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
    </Box>
  );
}

function CharacterCard({ character, onClick }: { character: CharacterDto; onClick: () => void }) {
  return (
    <Card sx={{ borderRadius: 2, height: '100%', transition: 'box-shadow 0.2s', '&:hover': { boxShadow: 3 } }}>
      <CardActionArea onClick={onClick} sx={{ height: '100%' }}>
        <CardContent sx={{ pb: '12px !important' }}>
          <Typography variant="subtitle1" fontWeight={600} noWrap>{character.name}</Typography>
          {character.species && (
            <Chip label={character.species} size="small" variant="outlined" sx={{ mt: 0.5, mr: 0.5 }} />
          )}
          {character.birthDate && (
            <Typography variant="caption" color="text.secondary" display="block" mt={0.5}>
              °&nbsp;{new Date(character.birthDate).toLocaleDateString('en-GB')}{character.birthDateSuffix && ` ${character.birthDateSuffix}`}
              {character.deathDate && ` — †\u00a0${new Date(character.deathDate).toLocaleDateString('en-GB')}${character.deathDateSuffix ? ` ${character.deathDateSuffix}` : ''}`}
            </Typography>
          )}
          {character.description && !character.birthDate && (
            <Typography
              variant="body2"
              color="text.secondary"
              sx={{ mt: 0.5, display: '-webkit-box', WebkitLineClamp: 2, WebkitBoxOrient: 'vertical', overflow: 'hidden' }}
            >
              {character.description}
            </Typography>
          )}
          <Typography variant="caption" color="text.secondary" display="block" mt={0.5}>
            Updated {new Date(character.updatedAt).toLocaleDateString('en-GB')}
          </Typography>
        </CardContent>
      </CardActionArea>
    </Card>
  );
}

function EntityCard({ entity }: { entity: EntityDto }) {
  return (
    <Card sx={{ borderRadius: 2, height: '100%', transition: 'box-shadow 0.2s', '&:hover': { boxShadow: 3 } }}>
      <CardContent sx={{ pb: '12px !important' }}>
        <Typography variant="subtitle1" fontWeight={600} noWrap>{entity.name}</Typography>
        {entity.description && (
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
    </Card>
  );
}
