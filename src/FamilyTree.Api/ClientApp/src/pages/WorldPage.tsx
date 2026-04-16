import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box, Typography, CircularProgress, Alert, Button,
  Card, CardContent, CardActionArea, Chip, Divider, Grid2,
} from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import OpenInFullIcon from '@mui/icons-material/OpenInFull';
import AccountTreeIcon from '@mui/icons-material/AccountTree';
import SettingsIcon from '@mui/icons-material/Settings';
import { getWorld } from '../api/worldsApi';
import { getEntitiesByTypePaged } from '../api/entitiesApi';
import { getCharactersByWorldPaged } from '../api/charactersApi';
import { getFamilyTreesByWorldPaged } from '../api/familyTreesApi';
import { getLocationsByWorldPaged } from '../api/locationsApi';
import type { WorldDto } from '../types/world';
import type { EntityDto, EntityType } from '../types/entity';
import type { CharacterDto } from '../types/character';
import type { FamilyTreeDto } from '../types/familyTree';
import type { LocationDto } from '../types/location';
import { useEditMode } from '../context/EditModeContext';
import { useAuth } from '../context/AuthContext';
import AddCharacterDialog from '../components/AddCharacterDialog';
import AddEntityDialog from '../components/AddEntityDialog';
import AddLocationDialog from '../components/AddLocationDialog';
import NewFamilyTreeDialog from '../components/NewFamilyTreeDialog';

const ENTITY_SECTIONS: { type: EntityType; plural: string }[] = [
  { type: 'Group',    plural: 'Groups'   },
  { type: 'Event',    plural: 'Events'   },
  { type: 'Concept',  plural: 'Concepts' },
];

export default function WorldPage() {
  const { worldId } = useParams<{ worldId: string }>();
  const navigate = useNavigate();
  const { isEditMode } = useEditMode();
  const { userId } = useAuth();

  const [world, setWorld] = useState<WorldDto | null>(null);
  const [characters, setCharacters] = useState<CharacterDto[]>([]);
  const [entitiesByType, setEntitiesByType] = useState<Record<EntityType, EntityDto[]>>({} as Record<EntityType, EntityDto[]>);
  const [locations, setLocations] = useState<LocationDto[]>([]);
  const [familyTrees, setFamilyTrees] = useState<FamilyTreeDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const [addCharacterOpen, setAddCharacterOpen] = useState(false);
  const [addEntityOpen, setAddEntityOpen] = useState(false);
  const [addEntityType, setAddEntityType] = useState<EntityType>('Group');
  const [addLocationOpen, setAddLocationOpen] = useState(false);
  const [newFamilyTreeOpen, setNewFamilyTreeOpen] = useState(false);

  useEffect(() => {
    if (!worldId) return;
    Promise.all([
      getWorld(worldId),
      getCharactersByWorldPaged(worldId, 1, 4),
      getLocationsByWorldPaged(worldId, 1, 4),
      getEntitiesByTypePaged(worldId, 'Group', 1, 4),
      getEntitiesByTypePaged(worldId, 'Event', 1, 4),
      getEntitiesByTypePaged(worldId, 'Concept', 1, 4),
      getFamilyTreesByWorldPaged(worldId, 1, 4),
    ])
      .then(([w, chars, locs, groups, events, concepts, trees]) => {
        setWorld(w);
        setCharacters(chars.items);
        setLocations(locs.items);
        setEntitiesByType({
          Group: groups.items,
          Event: events.items,
          Concept: concepts.items,
        } as Record<EntityType, EntityDto[]>);
        setFamilyTrees(trees.items);
      })
      .catch(() => setError('Failed to load world'))
      .finally(() => setLoading(false));
  }, [worldId]);

  if (loading) return <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>;
  if (error)   return <Alert severity="error" sx={{ m: 3 }}>{error}</Alert>;
  if (!world)  return null;

  const isOwner = !!userId && world?.createdById === userId;
  const canEdit = isEditMode && isOwner;

  const openAddEntity = (type: EntityType) => {
    setAddEntityType(type);
    setAddEntityOpen(true);
  };

  return (
    <Box sx={{ maxWidth: 1200, mx: 'auto', px: 3, py: 4 }}>
      {/* Header */}
      <Box display="flex" flexDirection="column" mb={5}>
        <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
          <Button startIcon={<ArrowBackIcon />} onClick={() => navigate('/')}>
            Worlds
          </Button>
          {world.createdById === userId && canEdit && (
            <Button startIcon={<SettingsIcon />} onClick={() => navigate(`/worlds/${worldId}/settings`)}>
              Settings
            </Button>
          )}
        </Box>
        <Box>
          <Typography variant="h3" fontWeight={700} lineHeight={1.1}>{world.name}</Typography>
          {world.author && (
            <Typography variant="subtitle1" color="text.secondary" fontStyle="italic" mt={0.5}>
              by {world.author}
            </Typography>
          )}
          {world.description && (
            <Typography variant="body1" color="text.secondary" mt={1}>{world.description}</Typography>
          )}
        </Box>
      </Box>

      {/* Characters section */}
      <Box mb={5}>
        <Box display="flex" alignItems="center" justifyContent="space-between" mb={2}>
          <Box display="flex" alignItems="center" gap={1}>
            <Typography variant="h5" fontWeight={600}>Characters</Typography>
            <Button
              size="small"
              startIcon={<OpenInFullIcon fontSize="small" />}
              onClick={() => navigate(`/worlds/${worldId}/entities/Character`)}
              sx={{ textTransform: 'none', minWidth: 0 }}
            >
              View all
            </Button>
          </Box>
          {canEdit && (
            <Button size="small" startIcon={<AddIcon />} onClick={() => setAddCharacterOpen(true)}>
              Add Character
            </Button>
          )}
        </Box>
        {characters.length === 0 ? (
          <Typography variant="body2" color="text.secondary" fontStyle="italic">
            No characters yet.
          </Typography>
        ) : (
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
        )}
        <Divider sx={{ mt: 4 }} />
      </Box>

      {/* Places (Locations) section */}
      <Box mb={5}>
        <Box display="flex" alignItems="center" justifyContent="space-between" mb={2}>
          <Box display="flex" alignItems="center" gap={1}>
            <Typography variant="h5" fontWeight={600}>Places</Typography>
            <Button
              size="small"
              startIcon={<OpenInFullIcon fontSize="small" />}
              onClick={() => navigate(`/worlds/${worldId}/entities/Place`)}
              sx={{ textTransform: 'none', minWidth: 0 }}
            >
              View all
            </Button>
          </Box>
          {canEdit && (
            <Button size="small" startIcon={<AddIcon />} onClick={() => setAddLocationOpen(true)}>
              Add Place
            </Button>
          )}
        </Box>
        {locations.length === 0 ? (
          <Typography variant="body2" color="text.secondary" fontStyle="italic">
            No places yet.
          </Typography>
        ) : (
          <Grid2 container spacing={2}>
            {locations.map(location => (
              <Grid2 key={location.id} size={{ xs: 12, sm: 6, md: 4, lg: 3 }}>
                <LocationCard location={location} />
              </Grid2>
            ))}
          </Grid2>
        )}
        <Divider sx={{ mt: 4 }} />
      </Box>

      {/* Group / Event / Concept sections */}
      {ENTITY_SECTIONS.map(({ type, plural }) => {
        const sectionEntities = entitiesByType[type] ?? [];
        return (
          <Box key={type} mb={5}>
            <Box display="flex" alignItems="center" justifyContent="space-between" mb={2}>
              <Box display="flex" alignItems="center" gap={1}>
                <Typography variant="h5" fontWeight={600}>{plural}</Typography>
                <Button
                  size="small"
                  startIcon={<OpenInFullIcon fontSize="small" />}
                  onClick={() => navigate(`/worlds/${worldId}/entities/${type}`)}
                  sx={{ textTransform: 'none', minWidth: 0 }}
                >
                  View all
                </Button>
              </Box>
              {canEdit && (
                <Button size="small" startIcon={<AddIcon />} onClick={() => openAddEntity(type)}>
                  Add {type}
                </Button>
              )}
            </Box>
            {sectionEntities.length === 0 ? (
              <Typography variant="body2" color="text.secondary" fontStyle="italic">
                No {plural.toLowerCase()} yet.
              </Typography>
            ) : (
              <Grid2 container spacing={2}>
                {sectionEntities.map(entity => (
                  <Grid2 key={entity.id} size={{ xs: 12, sm: 6, md: 4, lg: 3 }}>
                    <EntityCard entity={entity} />
                  </Grid2>
                ))}
              </Grid2>
            )}
            <Divider sx={{ mt: 4 }} />
          </Box>
        );
      })}

      {/* Family Trees section */}
      <Box mb={5}>
        <Box display="flex" alignItems="center" justifyContent="space-between" mb={2}>
          <Typography variant="h5" fontWeight={600}>Family Trees</Typography>
          {canEdit && (
            <Button size="small" startIcon={<AddIcon />} onClick={() => setNewFamilyTreeOpen(true)}>
              New Family Tree
            </Button>
          )}
        </Box>
        {familyTrees.length === 0 ? (
          <Typography variant="body2" color="text.secondary" fontStyle="italic">
            No family trees yet.
          </Typography>
        ) : (
          <Grid2 container spacing={2}>
            {familyTrees.map(tree => (
              <Grid2 key={tree.id} size={{ xs: 12, sm: 6, md: 4 }}>
                <Card sx={{ borderRadius: 2, transition: 'box-shadow 0.2s', '&:hover': { boxShadow: 4 } }}>
                  <CardActionArea onClick={() => navigate(`/family-trees/${tree.id}`)} sx={{ p: 2 }}>
                    <Box display="flex" alignItems="center" gap={1.5}>
                      <AccountTreeIcon color="primary" />
                      <Box>
                        <Typography variant="subtitle1" fontWeight={600}>{tree.name}</Typography>
                        {tree.description && (
                          <Typography variant="body2" color="text.secondary">{tree.description}</Typography>
                        )}
                      </Box>
                    </Box>
                  </CardActionArea>
                </Card>
              </Grid2>
            ))}
          </Grid2>
        )}
      </Box>

      {worldId && (
        <AddCharacterDialog
          open={addCharacterOpen}
          worldId={worldId}
          worldCharacters={characters}
          worldEntities={entitiesByType['Group'] ?? []}
          onClose={() => setAddCharacterOpen(false)}
          onCreated={character => { setCharacters(prev => [character, ...prev]); setAddCharacterOpen(false); }}
        />
      )}

      {worldId && (
        <AddLocationDialog
          open={addLocationOpen}
          worldId={worldId}
          onClose={() => setAddLocationOpen(false)}
          onCreated={location => { setLocations(prev => [location, ...prev]); setAddLocationOpen(false); }}
        />
      )}

      {worldId && (
        <AddEntityDialog
          open={addEntityOpen}
          defaultType={addEntityType}
          worldId={worldId}
          onClose={() => setAddEntityOpen(false)}
          onCreated={entity => {
            setEntitiesByType(prev => ({
              ...prev,
              [entity.type]: [entity, ...(prev[entity.type as EntityType] ?? [])],
            }));
            setAddEntityOpen(false);
          }}
        />
      )}

      {worldId && (
        <NewFamilyTreeDialog
          open={newFamilyTreeOpen}
          worldId={worldId}
          onClose={() => setNewFamilyTreeOpen(false)}
          onCreated={tree => { setFamilyTrees(prev => [...prev, tree]); setNewFamilyTreeOpen(false); }}
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
              °&nbsp;{new Date(character.birthDate).toLocaleDateString('en-GB')}{character.birthDateSuffixAbbreviation && ` ${character.birthDateSuffixAbbreviation}`}
              {character.deathDate && ` — †\u00a0${new Date(character.deathDate).toLocaleDateString('en-GB')}${character.deathDateSuffixAbbreviation ? ` ${character.deathDateSuffixAbbreviation}` : ''}`}
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
        </CardContent>
      </CardActionArea>
    </Card>
  );
}

function LocationCard({ location }: { location: LocationDto }) {
  return (
    <Card sx={{ borderRadius: 2, height: '100%', transition: 'box-shadow 0.2s', '&:hover': { boxShadow: 3 } }}>
      <CardContent sx={{ pb: '12px !important' }}>
        <Typography variant="subtitle1" fontWeight={600} noWrap>{location.name}</Typography>
        {location.description && (
          <Typography
            variant="body2"
            color="text.secondary"
            sx={{ mt: 0.5, display: '-webkit-box', WebkitLineClamp: 2, WebkitBoxOrient: 'vertical', overflow: 'hidden' }}
          >
            {location.description}
          </Typography>
        )}
      </CardContent>
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
      </CardContent>
    </Card>
  );
}
