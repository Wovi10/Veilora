import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box, Typography, CircularProgress, Alert, Button, Chip, Divider, IconButton, Tooltip,
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import EditIcon from '@mui/icons-material/Edit';
import { getCharacter, getCharactersByWorld } from '../../api/charactersApi';
import { getEntities } from '../../api/entitiesApi';
import { getWorld } from '../../api/worldsApi';
import type { CharacterDto } from '../../types/character';
import type { EntityDto } from '../../types/entity';
import type { EntityRefDto } from '../../types/entityRef';
import type { WorldDto } from '../../types/world';
import { useEditMode } from '../../context/EditModeContext';
import { useAuth } from '../../context/AuthContext';
import { EditCharacterDialog } from '../../components';

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

  const bioRows = [
    character.otherNames && { label: 'Also known as', value: character.otherNames },
    character.position   && { label: 'Position',      value: character.position },
    character.species    && { label: 'Species',        value: character.species },
    character.gender && character.gender !== 'Unknown' && { label: 'Gender', value: character.gender },
    character.birthDate  && {
      label: 'Born',
      value: [formatDate(character.birthDate, character.birthDateSuffixAbbreviation), character.birthPlaceLocationName].filter(Boolean).join(', '),
    },
    character.deathDate  && {
      label: 'Died',
      value: [formatDate(character.deathDate, character.deathDateSuffixAbbreviation), character.deathPlaceLocationName].filter(Boolean).join(', '),
    },
    character.residence  && { label: 'Residence',     value: character.residence },
  ].filter(Boolean) as { label: string; value: string }[];

  const physicalRows = [
    character.height     && { label: 'Height', value: character.height },
    character.hairColour && { label: 'Hair',   value: character.hairColour },
  ].filter(Boolean) as { label: string; value: string }[];

  const hasFamily = parent1 || parent2 || character.spouses?.length > 0 || character.children?.length > 0;
  const hasBio = bioRows.length > 0 || character.affiliations?.length > 0 || character.locations?.length > 0 || character.languages?.length > 0;

  return (
    <Box sx={{ maxWidth: 1100, mx: 'auto', px: 3, py: 4 }}>
      {/* Nav */}
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={4}>
        <Button startIcon={<ArrowBackIcon />} onClick={() => navigate(`/worlds/${worldId}/characters`)}>
          Characters
        </Button>
        {canEdit && (
          <Tooltip title="Edit character">
            <IconButton onClick={() => setEditOpen(true)}><EditIcon /></IconButton>
          </Tooltip>
        )}
      </Box>

      {/* Title */}
      <Box mb={3}>
        <Typography variant="h3" fontWeight={700} lineHeight={1.1}>{fullName}</Typography>
        {character.maidenName && (
          <Typography variant="body1" color="text.secondary" fontStyle="italic" mt={0.5}>
            née {character.maidenName}
          </Typography>
        )}
      </Box>

      {/* Wiki layout: body + infobox */}
      <Box display="flex" gap={4} alignItems="stretch">

        {/* Main body */}
        <Box flex={1} minWidth={0}>
          {character.description ? (
            <Typography variant="body1" sx={{ whiteSpace: 'pre-wrap', lineHeight: 1.9 }}>
              {character.description}
            </Typography>
          ) : (
            <Typography variant="body1" color="text.disabled" fontStyle="italic">
              No description yet.
            </Typography>
          )}
        </Box>

        {/* Infobox */}
        <Box
          sx={{
            width: 300,
            flexShrink: 0,
            border: '1px solid',
            borderColor: 'divider',
            borderRadius: 1,
            overflow: 'hidden',
          }}
        >
          <Box sx={{ bgcolor: 'action.hover', px: 1.5, py: 1 }}>
            <Typography variant="subtitle2" fontWeight={700}>{fullName}</Typography>
          </Box>
          {/* Biographical information */}
          {hasBio && (
            <>
              <Box sx={{ bgcolor: 'action.selected', px: 1.5, py: 0.5 }}>
                <Typography variant="caption" fontWeight={600} color="text.secondary" sx={{ textTransform: 'uppercase', letterSpacing: 0.5 }}>
                  Biographical information
                </Typography>
              </Box>
              <Box display="flex" flexDirection="column" sx={{ px: 1.5, py: 1, gap: 0.75 }}>
                {bioRows.map(({ label, value }) => (
                  <Box key={label}>
                    <Typography variant="caption" color="text.secondary" display="block">{label}</Typography>
                    <Typography variant="body2">{value}</Typography>
                  </Box>
                ))}
                {character.affiliations?.length > 0 && (
                  <Box>
                    <Typography variant="caption" color="text.secondary" display="block">Affiliations</Typography>
                    <Box display="flex" gap={0.5} flexWrap="wrap" mt={0.25}>
                      {character.affiliations.map(a => <Chip key={a.id} label={a.name} size="small" variant="outlined" />)}
                    </Box>
                  </Box>
                )}
                {character.locations?.length > 0 && (
                  <Box>
                    <Typography variant="caption" color="text.secondary" display="block">Locations</Typography>
                    <Box display="flex" gap={0.5} flexWrap="wrap" mt={0.25}>
                      {character.locations.map(l => <Chip key={l.id} label={l.name} size="small" variant="outlined" />)}
                    </Box>
                  </Box>
                )}
                {character.languages?.length > 0 && (
                  <Box>
                    <Typography variant="caption" color="text.secondary" display="block">Languages</Typography>
                    <Box display="flex" gap={0.5} flexWrap="wrap" mt={0.25}>
                      {character.languages.map(l => <Chip key={l.id} label={l.name} size="small" />)}
                    </Box>
                  </Box>
                )}
              </Box>
            </>
          )}

          {/* Family */}
          {hasFamily && (
            <>
              <Box sx={{ bgcolor: 'action.selected', px: 1.5, py: 0.5 }}>
                <Typography variant="caption" fontWeight={600} color="text.secondary" sx={{ textTransform: 'uppercase', letterSpacing: 0.5 }}>
                  Family
                </Typography>
              </Box>
              <Box display="flex" flexDirection="column" sx={{ px: 1.5, py: 1, gap: 0.75 }}>
                {(parent1 || parent2) && (
                  <Box>
                    <Typography variant="caption" color="text.secondary" display="block">Parents</Typography>
                    <Box display="flex" flexDirection="column">
                      {parent1 && <CharacterLink entity={{ id: parent1.id, name: parent1.name }} worldId={worldId!} />}
                      {parent2 && <CharacterLink entity={{ id: parent2.id, name: parent2.name }} worldId={worldId!} />}
                    </Box>
                  </Box>
                )}
                {character.spouses?.length > 0 && (
                  <Box>
                    <Typography variant="caption" color="text.secondary" display="block">
                      {character.spouses.length === 1 ? 'Spouse' : 'Spouses'}
                    </Typography>
                    <Box display="flex" flexDirection="column">
                      {character.spouses.map(s => <CharacterLink key={s.id} entity={s} worldId={worldId!} />)}
                    </Box>
                  </Box>
                )}
                {character.children?.length > 0 && (
                  <Box>
                    <Typography variant="caption" color="text.secondary" display="block">
                      {character.children.length === 1 ? 'Child' : 'Children'}
                    </Typography>
                    <Box display="flex" flexDirection="column">
                      {character.children.map(c => <CharacterLink key={c.id} entity={c} worldId={worldId!} />)}
                    </Box>
                  </Box>
                )}
              </Box>
            </>
          )}

          {/* Physical description */}
          {physicalRows.length > 0 && (
            <>
              <Box sx={{ bgcolor: 'action.selected', px: 1.5, py: 0.5 }}>
                <Typography variant="caption" fontWeight={600} color="text.secondary" sx={{ textTransform: 'uppercase', letterSpacing: 0.5 }}>
                  Physical description
                </Typography>
              </Box>
              <Box display="flex" flexDirection="column" sx={{ px: 1.5, py: 1, gap: 0.75 }}>
                {physicalRows.map(({ label, value }) => (
                  <Box key={label}>
                    <Typography variant="caption" color="text.secondary" display="block">{label}</Typography>
                    <Typography variant="body2">{value}</Typography>
                  </Box>
                ))}
              </Box>
            </>
          )}
        </Box>

      </Box>

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
          onDeleted={() => navigate(`/worlds/${worldId}/characters`)}
        />
      )}
    </Box>
  );
}
