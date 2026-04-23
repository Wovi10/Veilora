import { useState, useEffect, useRef } from 'react';
import {
  Box, TextField, InputAdornment, CircularProgress, Typography, Divider, ButtonBase,
} from '@mui/material';
import SearchIcon from '@mui/icons-material/Search';
import GroupsIcon from '@mui/icons-material/Groups';
import BoltIcon from '@mui/icons-material/Bolt';
import LightbulbIcon from '@mui/icons-material/Lightbulb';
import PersonIcon from '@mui/icons-material/Person';
import PlaceIcon from '@mui/icons-material/Place';
import AddCircleOutlineIcon from '@mui/icons-material/AddCircleOutline';
import { searchWorld } from '../../api/worldSearchApi';
import type { EntityDto, EntityType } from '../../types/entity';
import type { EntitySearchItem, NamedItem } from '../../api/worldSearchApi';

const ENTITY_TYPE_META: Record<EntityType, { icon: React.ReactNode; color: string }> = {
  Group:   { icon: <GroupsIcon sx={{ fontSize: 14 }} />,    color: 'primary.main' },
  Event:   { icon: <BoltIcon sx={{ fontSize: 14 }} />,      color: 'warning.main' },
  Concept: { icon: <LightbulbIcon sx={{ fontSize: 14 }} />, color: 'secondary.main' },
};

const ENTITY_TYPES: EntityType[] = ['Group', 'Event', 'Concept'];

const SECTION_LABEL_SX = {
  textTransform: 'uppercase' as const,
  letterSpacing: '0.07em',
  fontSize: '0.62rem',
  fontWeight: 700,
  color: 'text.disabled',
  mb: 0.75,
};

interface Props {
  worldId: string;
  hasSelection: boolean;
  onSelectEntity: (entity: EntityDto) => void;
  onSelectCharacter: (id: string) => void;
  onSelectLocation: (id: string) => void;
  onStartCreate: (name: string) => void;
  onDeselect: () => void;
}

export default function EntitySearchBar({ worldId, hasSelection, onSelectEntity, onSelectCharacter, onSelectLocation, onStartCreate, onDeselect }: Props) {
  const [query, setQuery] = useState('');
  const [entities, setEntities] = useState<EntitySearchItem[]>([]);
  const [characters, setCharacters] = useState<NamedItem[]>([]);
  const [locations, setLocations] = useState<NamedItem[]>([]);
  const [searching, setSearching] = useState(false);
  const [showResults, setShowResults] = useState(false);
  const debounceRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  const hasResults = entities.length > 0 || characters.length > 0 || locations.length > 0;

  useEffect(() => {
    if (debounceRef.current) clearTimeout(debounceRef.current);
    const trimmed = query.trim();
    if (!trimmed) {
      setEntities([]); setCharacters([]); setLocations([]);
      setShowResults(false);
      return;
    }
    debounceRef.current = setTimeout(async () => {
      setSearching(true);
      try {
        const result = await searchWorld(worldId, trimmed);
        setEntities(result.entities);
        setCharacters(result.characters);
        setLocations(result.locations);
        setShowResults(true);
      } catch (err) {
        console.error('[EntitySearch] error:', err);
        setEntities([]); setCharacters([]); setLocations([]);
      } finally {
        setSearching(false);
      }
    }, 300);
    return () => { if (debounceRef.current) clearTimeout(debounceRef.current); };
  }, [query, worldId]);

  function handleSelectEntity(entity: EntityDto) {
    setQuery('');
    setShowResults(false);
    setPendingType(null);
    onSelectEntity(entity);
  }

  function handleSelectCharacter(id: string) {
    setQuery('');
    setShowResults(false);
    setPendingType(null);
    onSelectCharacter(id);
  }

  function handleSelectLocation(id: string) {
    setQuery('');
    setShowResults(false);
    setPendingType(null);
    onSelectLocation(id);
  }

  function handleQueryChange(value: string) {
    setQuery(value);
    if (hasSelection) onDeselect();
  }

  // Group entities by type for display
  const entityGroups = ENTITY_TYPES
    .map(type => ({ type, items: entities.filter(e => e.type === type) }))
    .filter(g => g.items.length > 0);

  return (
    <Box>
      <TextField
        fullWidth
        size="small"
        placeholder="Search or create entity…"
        value={query}
        onChange={e => handleQueryChange(e.target.value)}
        onFocus={() => { if (hasResults) setShowResults(true); }}
        InputProps={{
          startAdornment: (
            <InputAdornment position="start">
              {searching
                ? <CircularProgress size={16} />
                : <SearchIcon fontSize="small" sx={{ color: 'text.disabled' }} />
              }
            </InputAdornment>
          ),
        }}
        sx={{
          '& .MuiOutlinedInput-root': { borderRadius: 0 },
          '& fieldset': { border: 'none', borderBottom: 1, borderColor: 'divider' },
        }}
      />

      {showResults && query.trim() && (
        <Box sx={{ borderBottom: 1, borderColor: 'divider', maxHeight: 360, overflowY: 'auto', px: 2, py: 1.5, display: 'flex', flexDirection: 'column', gap: 1.5 }}>

          {/* Entity groups — one row per type */}
          {entityGroups.map(({ type, items }) => {
            const meta = ENTITY_TYPE_META[type];
            return (
              <Box key={type}>
                <Typography variant="caption" sx={SECTION_LABEL_SX}>{type}s</Typography>
                <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.75 }}>
                  {items.map(entity => (
                    <EntityNode
                      key={entity.id}
                      label={entity.name}
                      icon={meta.icon}
                      accentColor={meta.color}
                      onClick={() => handleSelectEntity({ id: entity.id, name: entity.name, type: entity.type, worldId, createdAt: '', updatedAt: '' })}
                    />
                  ))}
                </Box>
              </Box>
            );
          })}

          {/* Characters */}
          {characters.length > 0 && (
            <Box>
              <Typography variant="caption" sx={SECTION_LABEL_SX}>Characters</Typography>
              <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.75 }}>
                {characters.map(c => (
                  <EntityNode key={c.id} label={c.name} icon={<PersonIcon sx={{ fontSize: 14 }} />} accentColor="info.main" onClick={() => handleSelectCharacter(c.id)} />
                ))}
              </Box>
            </Box>
          )}

          {/* Locations */}
          {locations.length > 0 && (
            <Box>
              <Typography variant="caption" sx={SECTION_LABEL_SX}>Locations</Typography>
              <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.75 }}>
                {locations.map(l => (
                  <EntityNode key={l.id} label={l.name} icon={<PlaceIcon sx={{ fontSize: 14 }} />} accentColor="success.main" onClick={() => handleSelectLocation(l.id)} />
                ))}
              </Box>
            </Box>
          )}

          {/* Create */}
          {hasResults && <Divider />}
          <Box
            component={ButtonBase}
            onClick={() => { setShowResults(false); onStartCreate(query.trim()); }}
            sx={{
              display: 'flex', alignItems: 'center', gap: 0.75, alignSelf: 'flex-start',
              color: 'text.secondary', py: 0.25, borderRadius: 1,
              '&:hover': { color: 'text.primary' },
            }}
          >
            <AddCircleOutlineIcon sx={{ fontSize: 15 }} />
            <Typography variant="body2" fontStyle="italic">
              Create "{query.trim()}" as entity
            </Typography>
          </Box>
        </Box>
      )}
    </Box>
  );
}

function EntityNode({ label, icon, accentColor, onClick }: {
  label: string;
  icon: React.ReactNode;
  accentColor: string;
  onClick?: () => void;
}) {
  return (
    <Box
      component={onClick ? ButtonBase : Box}
      onClick={onClick}
      sx={{
        display: 'flex', alignItems: 'center', gap: 0.6,
        px: 1, py: 0.5,
        borderTop: 1, borderRight: 1, borderBottom: 1, borderColor: 'divider',
        borderLeft: 3, borderLeftColor: accentColor,
        borderRadius: 1.5,
        maxWidth: 160, minWidth: 0,
        cursor: onClick ? 'pointer' : 'default',
        bgcolor: 'background.paper',
        transition: 'background-color 0.15s',
        ...(onClick && { '&:hover': { bgcolor: 'action.hover' } }),
        ...(!onClick && { opacity: 0.65 }),
      }}
    >
      <Box sx={{ color: accentColor, display: 'flex', alignItems: 'center', flexShrink: 0 }}>
        {icon}
      </Box>
      <Typography variant="caption" noWrap sx={{ fontWeight: 500, lineHeight: 1.3 }}>
        {label}
      </Typography>
    </Box>
  );
}
