import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box, Typography, CircularProgress, Alert, Button, Chip, Divider, IconButton, Tooltip,
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import EditIcon from '@mui/icons-material/Edit';
import { getEntity, getEntities } from '../api/entitiesApi';
import { getRelationships } from '../api/relationshipsApi';
import { getWorld } from '../api/worldsApi';
import type { EntityDto } from '../types/entity';
import type { RelationshipDto, RelationshipType } from '../types/relationship';
import type { WorldDto } from '../types/world';
import { useEditMode } from '../context/EditModeContext';
import { useAuth } from '../context/AuthContext';
import EditEntityDialog from '../components/EditEntityDialog';

const RELATIONSHIP_LABEL: Record<RelationshipType, string> = {
  Spouse: 'Spouse',
  Partner: 'Partner',
  Godparent: 'Godparent',
  Guardian: 'Guardian',
  CloseFriend: 'Close Friend',
};

function Section({ title, children }: { title: string; children: React.ReactNode }) {
  return (
    <Box mb={4}>
      <Typography variant="h6" fontWeight={600} mb={1.5}>{title}</Typography>
      <Divider sx={{ mb: 2 }} />
      {children}
    </Box>
  );
}

function EntityLink({ entity, worldId }: { entity: EntityDto; worldId: string }) {
  const navigate = useNavigate();
  return (
    <Typography
      component="span"
      sx={{ cursor: 'pointer', color: 'primary.main', '&:hover': { textDecoration: 'underline' } }}
      onClick={() => navigate(`/worlds/${worldId}/characters/${entity.id}`)}
    >
      {entity.name}
    </Typography>
  );
}

export default function CharacterPage() {
  const { worldId, entityId } = useParams<{ worldId: string; entityId: string }>();
  const navigate = useNavigate();
  const { isEditMode } = useEditMode();
  const { userId } = useAuth();

  const [world, setWorld] = useState<WorldDto | null>(null);
  const [character, setCharacter] = useState<EntityDto | null>(null);
  const [worldEntities, setWorldEntities] = useState<EntityDto[]>([]);
  const [relationships, setRelationships] = useState<RelationshipDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [editOpen, setEditOpen] = useState(false);

  useEffect(() => {
    if (!worldId || !entityId) return;
    Promise.all([getWorld(worldId), getEntity(entityId), getEntities(), getRelationships()])
      .then(([w, char, allEntities, allRels]) => {
        setWorld(w);
        setCharacter(char);
        setWorldEntities(allEntities.filter(e => e.worldId === worldId));
        setRelationships(allRels.filter(r => r.entity1Id === entityId || r.entity2Id === entityId));
      })
      .catch(() => setError('Failed to load character'))
      .finally(() => setLoading(false));
  }, [worldId, entityId]);

  if (loading) return <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>;
  if (error)   return <Alert severity="error" sx={{ m: 3 }}>{error}</Alert>;
  if (!character || !world) return null;

  const isOwner = !!userId && world.createdById === userId;
  const canEdit = isEditMode && isOwner;

  const entityMap = new Map(worldEntities.map(e => [e.id, e]));
  const parent1 = character.parent1Id ? entityMap.get(character.parent1Id) : undefined;
  const parent2 = character.parent2Id ? entityMap.get(character.parent2Id) : undefined;
  const children = worldEntities.filter(
    e => e.parent1Id === character.id || e.parent2Id === character.id
  );

  const fullName = [character.firstName, character.middleName, character.lastName]
    .filter(Boolean)
    .join(' ') || character.name;

  return (
    <Box sx={{ maxWidth: 900, mx: 'auto', px: 3, py: 4 }}>
      {/* Nav */}
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={4}>
        <Button startIcon={<ArrowBackIcon />} onClick={() => navigate(`/worlds/${worldId}/entities/Character`)}>
          Characters
        </Button>
        {canEdit && (
          <Tooltip title="Edit character">
            <IconButton onClick={() => setEditOpen(true)}><EditIcon /></IconButton>
          </Tooltip>
        )}
      </Box>

      {/* Hero */}
      <Box mb={5}>
        <Typography variant="h3" fontWeight={700} lineHeight={1.1}>{fullName}</Typography>
        {character.maidenName && (
          <Typography variant="body1" color="text.secondary" fontStyle="italic" mt={0.5}>
            née {character.maidenName}
          </Typography>
        )}
        <Box display="flex" gap={1} mt={1.5} flexWrap="wrap" alignItems="center">
          {character.species && <Chip label={character.species} size="small" />}
          {character.gender && character.gender !== 'Unknown' && (
            <Chip label={character.gender} size="small" variant="outlined" />
          )}
        </Box>
        {(character.birthDate || character.deathDate) && (
          <Typography variant="body2" color="text.secondary" mt={1.5}>
            {character.birthDate && (
              <>Born {new Date(character.birthDate).toLocaleDateString('en-GB')}
                {character.birthPlace && ` in ${character.birthPlace}`}</>
            )}
            {character.birthDate && character.deathDate && ' · '}
            {character.deathDate && <>Died {new Date(character.deathDate).toLocaleDateString('en-GB')}</>}
          </Typography>
        )}
        {character.residence && !character.birthDate && !character.deathDate && (
          <Typography variant="body2" color="text.secondary" mt={1}>
            Lives in {character.residence}
          </Typography>
        )}
        {character.residence && (character.birthDate || character.deathDate) && (
          <Typography variant="body2" color="text.secondary">
            Resides in {character.residence}
          </Typography>
        )}
      </Box>

      {/* Description */}
      {character.description && (
        <Section title="About">
          <Typography variant="body1">{character.description}</Typography>
        </Section>
      )}

      {/* Biography */}
      {character.biography && (
        <Section title="Biography">
          <Typography variant="body1" sx={{ whiteSpace: 'pre-wrap', lineHeight: 1.8 }}>
            {character.biography}
          </Typography>
        </Section>
      )}

      {/* Family */}
      {(parent1 || parent2 || children.length > 0) && (
        <Section title="Family">
          {(parent1 || parent2) && (
            <Box mb={children.length > 0 ? 2 : 0}>
              <Typography variant="body2" color="text.secondary" mb={0.5}>Parents</Typography>
              <Box display="flex" gap={2} flexWrap="wrap">
                {parent1 && <EntityLink entity={parent1} worldId={worldId!} />}
                {parent2 && <EntityLink entity={parent2} worldId={worldId!} />}
              </Box>
            </Box>
          )}
          {children.length > 0 && (
            <Box>
              <Typography variant="body2" color="text.secondary" mb={0.5}>
                {children.length === 1 ? 'Child' : 'Children'}
              </Typography>
              <Box display="flex" gap={2} flexWrap="wrap">
                {children.map(child => (
                  <EntityLink key={child.id} entity={child} worldId={worldId!} />
                ))}
              </Box>
            </Box>
          )}
        </Section>
      )}

      {/* Relationships */}
      {relationships.length > 0 && (
        <Section title="Relationships">
          <Box display="flex" flexDirection="column" gap={1.5}>
            {relationships.map(rel => {
              const partnerId = rel.entity1Id === character.id ? rel.entity2Id : rel.entity1Id;
              const partner = entityMap.get(partnerId);
              return (
                <Box key={rel.id} display="flex" alignItems="center" gap={2}>
                  <Chip
                    label={RELATIONSHIP_LABEL[rel.relationshipType]}
                    size="small"
                    variant="outlined"
                    sx={{ minWidth: 90 }}
                  />
                  {partner ? (
                    <EntityLink entity={partner} worldId={worldId!} />
                  ) : (
                    <Typography variant="body2" color="text.secondary">Unknown</Typography>
                  )}
                  {(rel.startDate || rel.endDate) && (
                    <Typography variant="caption" color="text.secondary">
                      {rel.startDate && new Date(rel.startDate).toLocaleDateString('en-GB')}
                      {rel.startDate && rel.endDate && ' – '}
                      {rel.endDate && new Date(rel.endDate).toLocaleDateString('en-GB')}
                    </Typography>
                  )}
                </Box>
              );
            })}
          </Box>
        </Section>
      )}

      {editOpen && (
        <EditEntityDialog
          open={editOpen}
          entity={character}
          treeEntities={worldEntities}
          onClose={() => setEditOpen(false)}
          onSaved={updated => {
            setCharacter(updated);
            setEditOpen(false);
          }}
        />
      )}
    </Box>
  );
}
