import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box, Typography, CircularProgress, Alert, Button,
  Card, CardContent, CardActionArea, Chip, Divider, Grid2,
} from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import AccountTreeIcon from '@mui/icons-material/AccountTree';
import { getWorld } from '../api/worldsApi';
import { getEntities } from '../api/entitiesApi';
import { getFamilyTrees } from '../api/familyTreesApi';
import type { WorldDto } from '../types/world';
import type { EntityDto, EntityType } from '../types/entity';
import type { FamilyTreeDto } from '../types/familyTree';
import { useEditMode } from '../context/EditModeContext';
import AddEntityDialog from '../components/AddEntityDialog';
import NewFamilyTreeDialog from '../components/NewFamilyTreeDialog';

const ENTITY_SECTIONS: { type: EntityType; plural: string }[] = [
  { type: 'Character', plural: 'Characters' },
  { type: 'Place',     plural: 'Places'     },
  { type: 'Group',     plural: 'Groups'     },
  { type: 'Event',     plural: 'Events'     },
  { type: 'Concept',   plural: 'Concepts'   },
];

export default function WorldPage() {
  const { worldId } = useParams<{ worldId: string }>();
  const navigate = useNavigate();
  const { isEditMode } = useEditMode();

  const [world, setWorld] = useState<WorldDto | null>(null);
  const [entities, setEntities] = useState<EntityDto[]>([]);
  const [familyTrees, setFamilyTrees] = useState<FamilyTreeDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const [addEntityOpen, setAddEntityOpen] = useState(false);
  const [addEntityType, setAddEntityType] = useState<EntityType>('Character');
  const [newFamilyTreeOpen, setNewFamilyTreeOpen] = useState(false);

  useEffect(() => {
    if (!worldId) return;
    Promise.all([getWorld(worldId), getEntities(), getFamilyTrees()])
      .then(([w, allEntities, allTrees]) => {
        setWorld(w);
        setEntities(allEntities.filter(e => e.worldId === worldId));
        setFamilyTrees(allTrees.filter(t => t.worldId === worldId));
      })
      .catch(() => setError('Failed to load world'))
      .finally(() => setLoading(false));
  }, [worldId]);

  if (loading) return <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>;
  if (error)   return <Alert severity="error" sx={{ m: 3 }}>{error}</Alert>;
  if (!world)  return null;

  const openAddEntity = (type: EntityType) => {
    setAddEntityType(type);
    setAddEntityOpen(true);
  };

  return (
    <Box sx={{ maxWidth: 1200, mx: 'auto', px: 3, py: 4 }}>
      {/* Header */}
      <Box display="flex" alignItems="flex-start" gap={2} mb={5}>
        <Button startIcon={<ArrowBackIcon />} onClick={() => navigate('/')} sx={{ mt: 0.5, flexShrink: 0 }}>
          Worlds
        </Button>
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

      {/* Entity sections */}
      {ENTITY_SECTIONS.map(({ type, plural }) => {
        const sectionEntities = entities.filter(e => e.type === type);
        return (
          <Box key={type} mb={5}>
            <Box display="flex" alignItems="center" justifyContent="space-between" mb={2}>
              <Typography variant="h5" fontWeight={600}>{plural}</Typography>
              {isEditMode && (
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
          {isEditMode && (
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
        <AddEntityDialog
          open={addEntityOpen}
          defaultType={addEntityType}
          worldId={worldId}
          onClose={() => setAddEntityOpen(false)}
          onCreated={entity => { setEntities(prev => [...prev, entity]); setAddEntityOpen(false); }}
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

function EntityCard({ entity }: { entity: EntityDto }) {
  return (
    <Card sx={{ borderRadius: 2, height: '100%', transition: 'box-shadow 0.2s', '&:hover': { boxShadow: 3 } }}>
      <CardContent sx={{ pb: '12px !important' }}>
        <Typography variant="subtitle1" fontWeight={600} noWrap>{entity.name}</Typography>
        {entity.species && (
          <Chip label={entity.species} size="small" variant="outlined" sx={{ mt: 0.5, mr: 0.5 }} />
        )}
        {entity.birthDate && (
          <Typography variant="caption" color="text.secondary" display="block" mt={0.5}>
            °&nbsp;{new Date(entity.birthDate).toLocaleDateString('en-GB')}
            {entity.deathDate && ` — †\u00a0${new Date(entity.deathDate).toLocaleDateString('en-GB')}`}
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
      </CardContent>
    </Card>
  );
}
