import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box, Typography, CircularProgress, Alert, Button,
  Chip, Grid2,
  Checkbox, FormControl, InputLabel, InputAdornment, ListItemText, MenuItem, Select, TextField,
} from '@mui/material';
import SearchIcon from '@mui/icons-material/Search';
import type { SelectChangeEvent } from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import AddIcon from '@mui/icons-material/Add';
import { getWorld } from '../../api/worldsApi';
import { getEntities } from '../../api/entitiesApi';
import { getCharactersByWorld } from '../../api/charactersApi';
import type { WorldDto } from '../../types/world';
import type { EntityDto } from '../../types/entity';
import type { CharacterDto } from '../../types/character';
import { useEditMode } from '../../context/EditModeContext';
import { useAuth } from '../../context/AuthContext';
import { AddCharacterDialog, CharacterCard } from '../../components';

export default function Characters() {
  const { worldId } = useParams<{ worldId: string }>();
  const navigate = useNavigate();
  const { isEditMode } = useEditMode();
  const { userId } = useAuth();

  const [world, setWorld] = useState<WorldDto | null>(null);
  const [characters, setCharacters] = useState<CharacterDto[]>([]);
  const [entities, setEntities] = useState<EntityDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [addOpen, setAddOpen] = useState(false);
  const [speciesFilter, setSpeciesFilter] = useState<string[]>([]);
  const [languageFilter, setLanguageFilter] = useState<string[]>([]);
  const [searchText, setSearchText] = useState('');

  useEffect(() => {
    if (!worldId) return;
    Promise.all([getWorld(worldId), getCharactersByWorld(worldId), getEntities()])
      .then(([w, chars, allEntities]) => {
        setWorld(w);
        setCharacters(chars);
        setEntities(allEntities.filter(e => e.worldId === worldId));
      })
      .catch(() => setError('Failed to load'))
      .finally(() => setLoading(false));
  }, [worldId]);

  if (loading) return <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>;
  if (error)   return <Alert severity="error" sx={{ m: 3 }}>{error}</Alert>;
  if (!world)  return null;

  const isOwner = !!userId && world.createdById === userId;
  const canEdit = isEditMode && isOwner;

  const speciesOptions = [...new Set(characters.map(c => c.species).filter((s): s is string => !!s))].sort();
  const languageOptions = [...new Map(characters.flatMap(c => c.languages).map(l => [l.id, l])).values()]
    .sort((a, b) => a.name.localeCompare(b.name));

  const searchLower = searchText.trim().toLowerCase();
  const visibleCharacters = characters.filter(c => {
    if (speciesFilter.length > 0 && !(c.species && speciesFilter.includes(c.species))) return false;
    if (languageFilter.length > 0 && !c.languages.some(l => languageFilter.includes(l.id))) return false;
    if (searchLower && !c.name.toLowerCase().includes(searchLower) && !(c.otherNames?.toLowerCase().includes(searchLower))) return false;
    return true;
  });

  const isFiltered = speciesFilter.length > 0 || languageFilter.length > 0 || !!searchLower;

  return (
    <Box sx={{ maxWidth: 1200, mx: 'auto', px: 3, py: 4 }}>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={4}>
        <Button startIcon={<ArrowBackIcon />} onClick={() => navigate(`/worlds/${worldId}`)}>
          {world.name}
        </Button>
        {canEdit && (
          <Button size="small" startIcon={<AddIcon />} onClick={() => setAddOpen(true)}>
            Add Character
          </Button>
        )}
      </Box>

      <Typography variant="h4" fontWeight={700} mb={2}>
        Characters
        <Typography component="span" variant="h6" color="text.secondary" fontWeight={400} ml={1.5}>
          {visibleCharacters.length}
        </Typography>
      </Typography>

      <Box sx={{ display: 'flex', gap: 2, flexWrap: 'wrap', alignItems: 'center', mb: 3 }}>
        <TextField
          size="small"
          placeholder="Search by name…"
          value={searchText}
          onChange={e => setSearchText(e.target.value)}
          sx={{ minWidth: 220 }}
          slotProps={{
            input: {
              startAdornment: (
                <InputAdornment position="start">
                  <SearchIcon fontSize="small" />
                </InputAdornment>
              ),
            },
          }}
        />
        {speciesOptions.length > 0 && (
          <FormControl size="small" sx={{ minWidth: 220 }}>
            <InputLabel>Species / Race</InputLabel>
            <Select
              multiple
              value={speciesFilter}
              label="Species / Race"
              onChange={(e: SelectChangeEvent<string[]>) =>
                setSpeciesFilter(typeof e.target.value === 'string' ? [e.target.value] : e.target.value)
              }
              renderValue={selected => (
                <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
                  {(selected as string[]).map(s => <Chip key={s} label={s} size="small" />)}
                </Box>
              )}
            >
              {speciesOptions.map(species => (
                <MenuItem key={species} value={species}>
                  <Checkbox checked={speciesFilter.includes(species)} size="small" />
                  <ListItemText primary={species} />
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        )}
        {languageOptions.length > 0 && (
          <FormControl size="small" sx={{ minWidth: 220 }}>
            <InputLabel>Language</InputLabel>
            <Select
              multiple
              value={languageFilter}
              label="Language"
              onChange={(e: SelectChangeEvent<string[]>) =>
                setLanguageFilter(typeof e.target.value === 'string' ? [e.target.value] : e.target.value)
              }
              renderValue={selected => (
                <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
                  {(selected as string[]).map(id => {
                    const lang = languageOptions.find(l => l.id === id);
                    return <Chip key={id} label={lang?.name ?? id} size="small" />;
                  })}
                </Box>
              )}
            >
              {languageOptions.map(lang => (
                <MenuItem key={lang.id} value={lang.id}>
                  <Checkbox checked={languageFilter.includes(lang.id)} size="small" />
                  <ListItemText primary={lang.name} />
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        )}
      </Box>

      {visibleCharacters.length === 0 ? (
        <Typography variant="body2" color="text.secondary" fontStyle="italic">
          {isFiltered ? 'No characters matching this filter.' : 'No characters yet.'}
        </Typography>
      ) : (
        <Grid2 container spacing={2}>
          {visibleCharacters.map(character => (
            <Grid2 key={character.id} size={{ xs: 12, sm: 6, md: 4, lg: 3 }}>
              <CharacterCard
                character={character}
                onClick={() => navigate(`/worlds/${worldId}/characters/${character.id}`)}
              />
            </Grid2>
          ))}
        </Grid2>
      )}

      {worldId && (
        <AddCharacterDialog
          open={addOpen}
          worldId={worldId}
          worldCharacters={characters}
          worldEntities={entities}
          onClose={() => setAddOpen(false)}
          onCreated={character => {
            setCharacters(prev =>
              [...prev, character].sort((a, b) => {
                const lastA = a.lastName ?? '\uffff';
                const lastB = b.lastName ?? '\uffff';
                if (lastA !== lastB) return lastA.localeCompare(lastB);
                if (!a.birthDate && !b.birthDate) return 0;
                if (!a.birthDate) return 1;
                if (!b.birthDate) return -1;
                return a.birthDate < b.birthDate ? -1 : a.birthDate > b.birthDate ? 1 : 0;
              })
            );
            setAddOpen(false);
          }}
        />
      )}
    </Box>
  );
}
