import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box, Typography, CircularProgress, Alert, Button, Chip, Divider, IconButton, Tooltip,
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import EditIcon from '@mui/icons-material/Edit';
import { getCharacter, getCharactersByWorld } from '../api/charactersApi';
import { getEntities } from '../api/entitiesApi';
import { getWorld } from '../api/worldsApi';
import type { CharacterDto } from '../types/character';
import type { EntityDto } from '../types/entity';
import type { EntityRefDto } from '../types/entityRef';
import type { WorldDto } from '../types/world';
import { useEditMode } from '../context/EditModeContext';
import { useAuth } from '../context/AuthContext';
import EditCharacterDialog from '../components/EditCharacterDialog';

function formatDate(date: string, suffix?: string) {
  const formatted = new Date(date).toLocaleDateString('en-GB');
  return suffix ? `${formatted} ${suffix}` : formatted;
}

function Section({ title, children }: { title: string; children: React.ReactNode }) {
  return (
    <Box mb={4}>
      <Typography variant="h6" fontWeight={600} mb={1.5}>{title}</Typography>
      <Divider sx={{ mb: 2 }} />
      {children}
    </Box>
  );
}

function CharacterLink({ entity, worldId }: { entity: EntityRefDto; worldId: string }) {
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
  const [character, setCharacter] = useState<CharacterDto | null>(null);
  const [worldCharacters, setWorldCharacters] = useState<CharacterDto[]>([]);
  const [worldEntities, setWorldEntities] = useState<EntityDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [editOpen, setEditOpen] = useState(false);

  useEffect(() => {
    if (!worldId || !entityId) return;
    Promise.all([getWorld(worldId), getCharacter(entityId), getCharactersByWorld(worldId), getEntities()])
      .then(([w, char, chars, allEntities]) => {
        setWorld(w);
        setCharacter(char);
        setWorldCharacters(chars);
        setWorldEntities(allEntities.filter(e => e.worldId === worldId));
      })
      .catch(() => setError('Failed to load character'))
      .finally(() => setLoading(false));
  }, [worldId, entityId]);

  if (loading) return <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>;
  if (error)   return <Alert severity="error" sx={{ m: 3 }}>{error}</Alert>;
  if (!character || !world) return null;

  const isOwner = !!userId && world.createdById === userId;
  const canEdit = isEditMode && isOwner;

  const charMap = new Map(worldCharacters.map(c => [c.id, c]));
  const parent1 = character.parent1Id ? charMap.get(character.parent1Id) : undefined;
  const parent2 = character.parent2Id ? charMap.get(character.parent2Id) : undefined;

  const fullName = [character.firstName, character.middleName, character.lastName]
    .filter(Boolean)
    .join(' ') || character.name;

  const detailRows = [
    character.otherNames && { label: 'Also known as', value: character.otherNames },
    character.position   && { label: 'Position',      value: character.position },
    character.height     && { label: 'Height',         value: character.height },
    character.hairColour && { label: 'Hair',           value: character.hairColour },
  ].filter(Boolean) as { label: string; value: string }[];

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
              <>
                Born {formatDate(character.birthDate, character.birthDateSuffixAbbreviation)}
                {character.birthPlaceLocationName && ` in ${character.birthPlaceLocationName}`}
              </>
            )}
            {character.birthDate && character.deathDate && ' · '}
            {character.deathDate && (
              <>
                Died {formatDate(character.deathDate, character.deathDateSuffixAbbreviation)}
                {character.deathPlaceLocationName && ` in ${character.deathPlaceLocationName}`}
              </>
            )}
          </Typography>
        )}
        {character.residence && (
          <Typography variant="body2" color="text.secondary" mt={0.5}>
            Resides in {character.residence}
          </Typography>
        )}
      </Box>

      {/* Detail fields */}
      {detailRows.length > 0 && (
        <Section title="Details">
          <Box display="flex" flexDirection="column" gap={0.75}>
            {detailRows.map(({ label, value }) => (
              <Box key={label} display="flex" gap={1.5}>
                <Typography variant="body2" color="text.secondary" sx={{ minWidth: 110 }}>{label}</Typography>
                <Typography variant="body2">{value}</Typography>
              </Box>
            ))}
          </Box>
        </Section>
      )}

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
      {(parent1 || parent2 || character.spouses?.length > 0 || character.children?.length > 0) && (
        <Section title="Family">
          {(parent1 || parent2) && (
            <Box mb={2}>
              <Typography variant="body2" color="text.secondary" mb={0.5}>Parents</Typography>
              <Box display="flex" gap={2} flexWrap="wrap">
                {parent1 && <CharacterLink entity={{ id: parent1.id, name: parent1.name }} worldId={worldId!} />}
                {parent2 && <CharacterLink entity={{ id: parent2.id, name: parent2.name }} worldId={worldId!} />}
              </Box>
            </Box>
          )}
          {character.spouses?.length > 0 && (
            <Box mb={2}>
              <Typography variant="body2" color="text.secondary" mb={0.5}>
                {character.spouses.length === 1 ? 'Spouse' : 'Spouses'}
              </Typography>
              <Box display="flex" gap={2} flexWrap="wrap">
                {character.spouses.map(s => (
                  <CharacterLink key={s.id} entity={s} worldId={worldId!} />
                ))}
              </Box>
            </Box>
          )}
          {character.children?.length > 0 && (
            <Box>
              <Typography variant="body2" color="text.secondary" mb={0.5}>
                {character.children.length === 1 ? 'Child' : 'Children'}
              </Typography>
              <Box display="flex" gap={2} flexWrap="wrap">
                {character.children.map(c => (
                  <CharacterLink key={c.id} entity={c} worldId={worldId!} />
                ))}
              </Box>
            </Box>
          )}
        </Section>
      )}

      {/* Affiliations */}
      {character.affiliations?.length > 0 && (
        <Section title="Affiliations">
          <Box display="flex" gap={1} flexWrap="wrap">
            {character.affiliations.map(a => (
              <Chip key={a.id} label={a.name} variant="outlined" />
            ))}
          </Box>
        </Section>
      )}

      {/* Locations */}
      {character.locations?.length > 0 && (
        <Section title="Locations">
          <Box display="flex" gap={1} flexWrap="wrap">
            {character.locations.map(l => (
              <Chip key={l.id} label={l.name} variant="outlined" />
            ))}
          </Box>
        </Section>
      )}

      {/* Languages */}
      {character.languages?.length > 0 && (
        <Section title="Languages">
          <Box display="flex" gap={1} flexWrap="wrap">
            {character.languages.map(l => (
              <Chip key={l.id} label={l.name} size="small" />
            ))}
          </Box>
        </Section>
      )}

      {editOpen && (
        <EditCharacterDialog
          open={editOpen}
          character={character}
          worldCharacters={worldCharacters}
          worldEntities={worldEntities}
          worldId={worldId!}
          onClose={() => setEditOpen(false)}
          onSaved={updated => {
            setCharacter(updated);
            setEditOpen(false);
          }}
          onDeleted={() => navigate(`/worlds/${worldId}/entities/Character`)}
        />
      )}
    </Box>
  );
}
