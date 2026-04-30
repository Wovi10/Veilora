import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box, Typography, CircularProgress, Alert, Button, Chip, IconButton, Tooltip,
  TextField, Select, MenuItem, FormControl, Autocomplete,
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import EditIcon from '@mui/icons-material/Edit';
import CheckIcon from '@mui/icons-material/Check';
import CloseIcon from '@mui/icons-material/Close';
import { getCharacter, getCharactersByWorld, updateCharacter } from '../../api/charactersApi';
import { getEntities } from '../../api/entitiesApi';
import { getWorld } from '../../api/worldsApi';
import { getLanguagesByWorld, getOrCreateLanguage } from '../../api/languagesApi';
import { getLocationsByWorld } from '../../api/locationsApi';
import { getDateSuffixesByWorld } from '../../api/dateSuffixesApi';
import type { CharacterDto, Gender } from '../../types/character';
import type { EntityDto } from '../../types/entity';
import type { EntityRefDto } from '../../types/entityRef';
import type { WorldDto } from '../../types/world';
import type { LanguageDto } from '../../types/language';
import type { LocationDto } from '../../types/location';
import type { DateSuffixDto } from '../../types/dateSuffix';
import { useEditMode } from '../../context/EditModeContext';
import { useAuth } from '../../context/AuthContext';
import RichTextEditor from '../../components/shared/RichTextEditor';

const GENDERS: Gender[] = ['Male', 'Female', 'Other', 'Unknown'];

type LocationOrString = EntityRefDto | string;

function formatDate(date: string, suffix?: string) {
  const formatted = new Date(date).toLocaleDateString('en-GB');
  return suffix ? `${formatted} ${suffix}` : formatted;
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

function InfoLabel({ children }: { children: React.ReactNode }) {
  return (
    <Typography variant="caption" color="text.secondary" display="block" mb={0.25}>
      {children}
    </Typography>
  );
}

function SectionHeader({ children }: { children: React.ReactNode }) {
  return (
    <Box sx={{ bgcolor: 'action.selected', px: 1.5, py: 0.5 }}>
      <Typography variant="caption" fontWeight={600} color="text.secondary" sx={{ textTransform: 'uppercase', letterSpacing: 0.5 }}>
        {children}
      </Typography>
    </Box>
  );
}

export default function CharacterPage() {
  const { worldId, entityId } = useParams<{ worldId: string; entityId: string }>();
  const navigate = useNavigate();
  const { isEditMode } = useEditMode();
  const { userId } = useAuth();

  // Page data
  const [world, setWorld] = useState<WorldDto | null>(null);
  const [character, setCharacter] = useState<CharacterDto | null>(null);
  const [worldCharacters, setWorldCharacters] = useState<CharacterDto[]>([]);
  const [worldEntities, setWorldEntities] = useState<EntityDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  // Inline edit mode
  const [editing, setEditing] = useState(false);
  const [saving, setSaving] = useState(false);

  // Field drafts
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [otherNames, setOtherNames] = useState('');
  const [position, setPosition] = useState('');
  const [species, setSpecies] = useState('');
  const [gender, setGender] = useState<Gender>('Unknown');
  const [birthDate, setBirthDate] = useState('');
  const [birthDateSuffixId, setBirthDateSuffixId] = useState('');
  const [birthPlaceLocation, setBirthPlaceLocation] = useState<LocationOrString | null>(null);
  const [birthPlaceInput, setBirthPlaceInput] = useState('');
  const [deathDate, setDeathDate] = useState('');
  const [deathDateSuffixId, setDeathDateSuffixId] = useState('');
  const [deathPlaceLocation, setDeathPlaceLocation] = useState<LocationOrString | null>(null);
  const [deathPlaceInput, setDeathPlaceInput] = useState('');
  const [height, setHeight] = useState('');
  const [hairColour, setHairColour] = useState('');
  const [parent1Id, setParent1Id] = useState('');
  const [parent2Id, setParent2Id] = useState('');
  const [selectedSpouses, setSelectedSpouses] = useState<EntityRefDto[]>([]);
  const [selectedChildren, setSelectedChildren] = useState<EntityRefDto[]>([]);
  const [selectedAffiliations, setSelectedAffiliations] = useState<LocationOrString[]>([]);
  const [selectedLocations, setSelectedLocations] = useState<LocationOrString[]>([]);
  const [selectedLanguages, setSelectedLanguages] = useState<Array<LanguageDto | string>>([]);
  const [descriptionDraft, setDescriptionDraft] = useState('');

  // Available options for dropdowns
  const [availableLanguages, setAvailableLanguages] = useState<LanguageDto[]>([]);
  const [availableLocations, setAvailableLocations] = useState<LocationDto[]>([]);
  const [availableDateSuffixes, setAvailableDateSuffixes] = useState<DateSuffixDto[]>([]);

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

  useEffect(() => {
    if (!editing || !worldId) return;
    Promise.all([getLanguagesByWorld(worldId), getLocationsByWorld(worldId), getDateSuffixesByWorld(worldId)])
      .then(([langs, locs, suffixes]) => {
        setAvailableLanguages(langs);
        setAvailableLocations(locs);
        setAvailableDateSuffixes(suffixes);
      })
      .catch(() => {});
  }, [editing, worldId]);

  if (loading) return <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>;
  if (error)   return <Alert severity="error" sx={{ m: 3 }}>{error}</Alert>;
  if (!character || !world) return null;

  const isOwner = !!userId && world.createdById === userId;
  const canEdit = isEditMode && isOwner;

  const handleStartEditing = () => {
    setFirstName(character.firstName ?? '');
    setLastName(character.lastName ?? '');
    setOtherNames(character.otherNames ?? '');
    setPosition(character.position ?? '');
    setSpecies(character.species ?? '');
    setGender(character.gender ?? 'Unknown');
    setBirthDate(character.birthDate ?? '');
    setBirthDateSuffixId(character.birthDateSuffixId ?? '');
    const birthPlace = character.birthPlaceLocationId
      ? { id: character.birthPlaceLocationId, name: character.birthPlaceLocationName ?? '' }
      : null;
    setBirthPlaceLocation(birthPlace);
    setBirthPlaceInput(birthPlace?.name ?? '');
    setDeathDate(character.deathDate ?? '');
    setDeathDateSuffixId(character.deathDateSuffixId ?? '');
    const deathPlace = character.deathPlaceLocationId
      ? { id: character.deathPlaceLocationId, name: character.deathPlaceLocationName ?? '' }
      : null;
    setDeathPlaceLocation(deathPlace);
    setDeathPlaceInput(deathPlace?.name ?? '');
    setHeight(character.height ?? '');
    setHairColour(character.hairColour ?? '');
    setParent1Id(character.parent1Id ?? '');
    setParent2Id(character.parent2Id ?? '');
    setSelectedSpouses(character.spouses ?? []);
    setSelectedChildren(character.children ?? []);
    setSelectedAffiliations(character.affiliations ?? []);
    setSelectedLocations(character.locations ?? []);
    setSelectedLanguages(character.languages ?? []);
    setDescriptionDraft(character.description ?? '');
    setEditing(true);
  };

  const handleCancel = () => setEditing(false);

  const handleSave = async () => {
    setSaving(true);
    try {
      const resolvedLanguageIds: string[] = [];
      for (const lang of selectedLanguages) {
        if (typeof lang === 'string') {
          const resolved = await getOrCreateLanguage(lang.trim(), worldId!);
          resolvedLanguageIds.push(resolved.id);
        } else {
          resolvedLanguageIds.push(lang.id);
        }
      }

      const effectiveName = (firstName.trim() || lastName.trim())
        ? [firstName.trim(), lastName.trim()].filter(Boolean).join(' ')
        : character.name;

      const updated = await updateCharacter(character.id, {
        name: effectiveName,
        description: descriptionDraft || undefined,
        firstName: firstName.trim() || undefined,
        lastName: lastName.trim() || undefined,
        species: species.trim() || undefined,
        gender,
        birthDate: birthDate || undefined,
        birthDateSuffixId: birthDateSuffixId || null,
        birthPlaceLocationId: typeof birthPlaceLocation !== 'string' ? (birthPlaceLocation?.id ?? null) : null,
        birthPlaceName: typeof birthPlaceLocation === 'string' ? birthPlaceLocation.trim() : (birthPlaceInput.trim() && !birthPlaceLocation ? birthPlaceInput.trim() : undefined),
        deathDate: deathDate || undefined,
        deathDateSuffixId: deathDateSuffixId || null,
        deathPlaceLocationId: typeof deathPlaceLocation !== 'string' ? (deathPlaceLocation?.id ?? null) : null,
        deathPlaceName: typeof deathPlaceLocation === 'string' ? deathPlaceLocation.trim() : (deathPlaceInput.trim() && !deathPlaceLocation ? deathPlaceInput.trim() : undefined),
        otherNames: otherNames.trim() || undefined,
        position: position.trim() || undefined,
        height: height.trim() || undefined,
        hairColour: hairColour.trim() || undefined,
        parent1Id: parent1Id || null,
        parent2Id: parent2Id || null,
        locationIds: selectedLocations.filter((l): l is EntityRefDto => typeof l !== 'string').map(l => l.id),
        locationNames: selectedLocations.filter((l): l is string => typeof l === 'string'),
        affiliationIds: selectedAffiliations.filter((a): a is EntityRefDto => typeof a !== 'string').map(a => a.id),
        affiliationNames: selectedAffiliations.filter((a): a is string => typeof a === 'string'),
        languageIds: resolvedLanguageIds,
        spouseIds: selectedSpouses.map(s => s.id),
        childIds: selectedChildren.map(c => c.id),
      });
      setCharacter(updated);
      setEditing(false);
    } finally {
      setSaving(false);
    }
  };

  const charMap = new Map(worldCharacters.map(c => [c.id, c]));
  const parent1 = character.parent1Id ? charMap.get(character.parent1Id) : undefined;
  const parent2 = character.parent2Id ? charMap.get(character.parent2Id) : undefined;

  const fullName = [character.firstName, character.middleName, character.lastName]
    .filter(Boolean)
    .join(' ') || character.name;

  const locationOptions: EntityRefDto[] = availableLocations.map(l => ({ id: l.id, name: l.name }));
  const groupOptions: EntityRefDto[] = worldEntities.filter(e => e.type === 'Group').map(e => ({ id: e.id, name: e.name }));
  const characterOptions: EntityRefDto[] = worldCharacters.filter(c => c.id !== character.id).map(c => ({ id: c.id, name: c.name }));

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
      </Box>

      {/* Title */}
      <Box mb={3}>
        {editing ? (
          <Box display="flex" gap={1} alignItems="center" flexWrap="wrap">
            <TextField size="small" label="First name" value={firstName} onChange={e => setFirstName(e.target.value)} sx={{ width: 160 }} />
            <TextField size="small" label="Last name" value={lastName} onChange={e => setLastName(e.target.value)} sx={{ width: 160 }} />
            <Tooltip title="Save changes">
              <IconButton size="small" color="primary" onClick={handleSave} disabled={saving}>
                <CheckIcon fontSize="small" />
              </IconButton>
            </Tooltip>
            <Tooltip title="Cancel">
              <IconButton size="small" onClick={handleCancel} disabled={saving}>
                <CloseIcon fontSize="small" />
              </IconButton>
            </Tooltip>
          </Box>
        ) : (
          <Box display="flex" alignItems="center" gap={1}>
            <Typography variant="h3" fontWeight={700} lineHeight={1.1}>{fullName}</Typography>
            {canEdit && (
              <Tooltip title="Edit character">
                <IconButton size="small" onClick={handleStartEditing} sx={{ color: 'text.secondary', mt: 0.5 }}>
                  <EditIcon fontSize="small" />
                </IconButton>
              </Tooltip>
            )}
          </Box>
        )}
        {character.maidenName && !editing && (
          <Typography variant="body1" color="text.secondary" fontStyle="italic" mt={0.5}>
            née {character.maidenName}
          </Typography>
        )}
      </Box>

      {/* Wiki layout: body + infobox */}
      <Box display="flex" gap={4} alignItems="stretch">

        {/* Main body */}
        <Box flex={1} minWidth={0}>
          {editing ? (
            <RichTextEditor content={descriptionDraft} onChange={setDescriptionDraft} />
          ) : character.description ? (
            <Box
              sx={{
                lineHeight: 1.9,
                '& h2': { fontSize: '1.25rem', fontWeight: 600, mt: 2, mb: 0.5 },
                '& p': { my: 0.5 },
                '& ul, & ol': { pl: 3 },
                '& blockquote': {
                  borderLeft: '3px solid',
                  borderColor: 'divider',
                  pl: 2,
                  ml: 0,
                  color: 'text.secondary',
                  fontStyle: 'italic',
                },
              }}
              dangerouslySetInnerHTML={{ __html: character.description }}
            />
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

          {/* ── Biographical information ── */}
          {(editing || hasBio) && (
            <>
              <SectionHeader>Biographical information</SectionHeader>
              <Box display="flex" flexDirection="column" sx={{ px: 1.5, py: 1, gap: 1 }}>
                {editing ? (
                  <>
                    <Box>
                      <InfoLabel>Also known as</InfoLabel>
                      <TextField size="small" fullWidth value={otherNames} onChange={e => setOtherNames(e.target.value)} placeholder="Aliases, nicknames…" />
                    </Box>
                    <Box>
                      <InfoLabel>Position / Title</InfoLabel>
                      <TextField size="small" fullWidth value={position} onChange={e => setPosition(e.target.value)} placeholder="King, Knight…" />
                    </Box>
                    <Box display="flex" gap={1}>
                      <Box flex={1}>
                        <InfoLabel>Species</InfoLabel>
                        <TextField size="small" fullWidth value={species} onChange={e => setSpecies(e.target.value)} />
                      </Box>
                      <Box flex={1}>
                        <InfoLabel>Gender</InfoLabel>
                        <FormControl size="small" fullWidth>
                          <Select value={gender} onChange={e => setGender(e.target.value as Gender)}>
                            {GENDERS.map(g => <MenuItem key={g} value={g}>{g}</MenuItem>)}
                          </Select>
                        </FormControl>
                      </Box>
                    </Box>
                    <Box>
                      <InfoLabel>Birth date</InfoLabel>
                      <Box display="flex" gap={1}>
                        <TextField size="small" type="date" value={birthDate} onChange={e => setBirthDate(e.target.value)} slotProps={{ inputLabel: { shrink: true } }} sx={{ flex: 1 }} />
                        {availableDateSuffixes.length > 0 && (
                          <FormControl size="small" sx={{ minWidth: 90 }}>
                            <Select value={birthDateSuffixId} onChange={e => setBirthDateSuffixId(e.target.value)} displayEmpty>
                              <MenuItem value="">—</MenuItem>
                              {availableDateSuffixes.map(s => <MenuItem key={s.id} value={s.id}>{s.abbreviation}</MenuItem>)}
                            </Select>
                          </FormControl>
                        )}
                      </Box>
                    </Box>
                    <Box>
                      <InfoLabel>Birth place</InfoLabel>
                      <Autocomplete
                        size="small"
                        freeSolo
                        options={locationOptions}
                        getOptionLabel={opt => typeof opt === 'string' ? opt : opt.name}
                        value={birthPlaceLocation}
                        inputValue={birthPlaceInput}
                        onChange={(_, val) => setBirthPlaceLocation(val as LocationOrString | null)}
                        onInputChange={(_, val) => setBirthPlaceInput(val)}
                        renderInput={params => <TextField {...params} placeholder="Select or type…" />}
                      />
                    </Box>
                    <Box>
                      <InfoLabel>Death date</InfoLabel>
                      <Box display="flex" gap={1}>
                        <TextField size="small" type="date" value={deathDate} onChange={e => setDeathDate(e.target.value)} slotProps={{ inputLabel: { shrink: true } }} sx={{ flex: 1 }} />
                        {availableDateSuffixes.length > 0 && (
                          <FormControl size="small" sx={{ minWidth: 90 }}>
                            <Select value={deathDateSuffixId} onChange={e => setDeathDateSuffixId(e.target.value)} displayEmpty>
                              <MenuItem value="">—</MenuItem>
                              {availableDateSuffixes.map(s => <MenuItem key={s.id} value={s.id}>{s.abbreviation}</MenuItem>)}
                            </Select>
                          </FormControl>
                        )}
                      </Box>
                    </Box>
                    <Box>
                      <InfoLabel>Death place</InfoLabel>
                      <Autocomplete
                        size="small"
                        freeSolo
                        options={locationOptions}
                        getOptionLabel={opt => typeof opt === 'string' ? opt : opt.name}
                        value={deathPlaceLocation}
                        inputValue={deathPlaceInput}
                        onChange={(_, val) => setDeathPlaceLocation(val as LocationOrString | null)}
                        onInputChange={(_, val) => setDeathPlaceInput(val)}
                        renderInput={params => <TextField {...params} placeholder="Select or type…" />}
                      />
                    </Box>
                    <Box>
                      <InfoLabel>Affiliations</InfoLabel>
                      <Autocomplete
                        size="small"
                        multiple
                        freeSolo
                        options={groupOptions}
                        getOptionLabel={opt => typeof opt === 'string' ? opt : opt.name}
                        value={selectedAffiliations}
                        onChange={(_, val) => setSelectedAffiliations(val as LocationOrString[])}
                        renderTags={(val, getTagProps) =>
                          val.map((opt, i) => (
                            <Chip {...getTagProps({ index: i })} key={typeof opt === 'string' ? opt : opt.id} label={typeof opt === 'string' ? opt : opt.name} size="small" />
                          ))
                        }
                        renderInput={params => <TextField {...params} placeholder="Add or type…" />}
                      />
                    </Box>
                    <Box>
                      <InfoLabel>Locations</InfoLabel>
                      <Autocomplete
                        size="small"
                        multiple
                        freeSolo
                        options={locationOptions}
                        getOptionLabel={opt => typeof opt === 'string' ? opt : opt.name}
                        value={selectedLocations}
                        onChange={(_, val) => setSelectedLocations(val as LocationOrString[])}
                        renderTags={(val, getTagProps) =>
                          val.map((opt, i) => (
                            <Chip {...getTagProps({ index: i })} key={typeof opt === 'string' ? opt : opt.id} label={typeof opt === 'string' ? opt : opt.name} size="small" />
                          ))
                        }
                        renderInput={params => <TextField {...params} placeholder="Add or type…" />}
                      />
                    </Box>
                    <Box>
                      <InfoLabel>Languages</InfoLabel>
                      <Autocomplete
                        size="small"
                        multiple
                        freeSolo
                        options={availableLanguages}
                        getOptionLabel={opt => typeof opt === 'string' ? opt : opt.name}
                        value={selectedLanguages}
                        onChange={(_, val) => setSelectedLanguages(val as Array<LanguageDto | string>)}
                        renderTags={(val, getTagProps) =>
                          val.map((opt, i) => (
                            <Chip {...getTagProps({ index: i })} key={typeof opt === 'string' ? opt : opt.id} label={typeof opt === 'string' ? opt : opt.name} size="small" />
                          ))
                        }
                        renderInput={params => <TextField {...params} placeholder="Type to add…" />}
                      />
                    </Box>
                  </>
                ) : (
                  <>
                    {bioRows.map(({ label, value }) => (
                      <Box key={label}>
                        <InfoLabel>{label}</InfoLabel>
                        <Typography variant="body2">{value}</Typography>
                      </Box>
                    ))}
                    {character.affiliations?.length > 0 && (
                      <Box>
                        <InfoLabel>Affiliations</InfoLabel>
                        <Box display="flex" gap={0.5} flexWrap="wrap" mt={0.25}>
                          {character.affiliations.map(a => <Chip key={a.id} label={a.name} size="small" variant="outlined" />)}
                        </Box>
                      </Box>
                    )}
                    {character.locations?.length > 0 && (
                      <Box>
                        <InfoLabel>Locations</InfoLabel>
                        <Box display="flex" gap={0.5} flexWrap="wrap" mt={0.25}>
                          {character.locations.map(l => <Chip key={l.id} label={l.name} size="small" variant="outlined" />)}
                        </Box>
                      </Box>
                    )}
                    {character.languages?.length > 0 && (
                      <Box>
                        <InfoLabel>Languages</InfoLabel>
                        <Box display="flex" gap={0.5} flexWrap="wrap" mt={0.25}>
                          {character.languages.map(l => <Chip key={l.id} label={l.name} size="small" />)}
                        </Box>
                      </Box>
                    )}
                  </>
                )}
              </Box>
            </>
          )}

          {/* ── Family ── */}
          {(editing || hasFamily) && (
            <>
              <SectionHeader>Family</SectionHeader>
              <Box display="flex" flexDirection="column" sx={{ px: 1.5, py: 1, gap: 1 }}>
                {editing ? (
                  <>
                    <Box>
                      <InfoLabel>Parent 1</InfoLabel>
                      <FormControl size="small" fullWidth>
                        <Select value={parent1Id} onChange={e => setParent1Id(e.target.value)} displayEmpty>
                          <MenuItem value="">None</MenuItem>
                          {characterOptions.map(c => <MenuItem key={c.id} value={c.id}>{c.name}</MenuItem>)}
                        </Select>
                      </FormControl>
                    </Box>
                    <Box>
                      <InfoLabel>Parent 2</InfoLabel>
                      <FormControl size="small" fullWidth>
                        <Select value={parent2Id} onChange={e => setParent2Id(e.target.value)} displayEmpty>
                          <MenuItem value="">None</MenuItem>
                          {characterOptions.filter(c => c.id !== parent1Id).map(c => <MenuItem key={c.id} value={c.id}>{c.name}</MenuItem>)}
                        </Select>
                      </FormControl>
                    </Box>
                    <Box>
                      <InfoLabel>Spouse(s)</InfoLabel>
                      <Autocomplete
                        size="small"
                        multiple
                        options={characterOptions}
                        getOptionLabel={opt => opt.name}
                        value={selectedSpouses}
                        onChange={(_, val) => setSelectedSpouses(val)}
                        isOptionEqualToValue={(opt, val) => opt.id === val.id}
                        renderTags={(val, getTagProps) =>
                          val.map((opt, i) => <Chip {...getTagProps({ index: i })} key={opt.id} label={opt.name} size="small" />)
                        }
                        renderInput={params => <TextField {...params} placeholder="Add spouse…" />}
                      />
                    </Box>
                    <Box>
                      <InfoLabel>Children</InfoLabel>
                      <Autocomplete
                        size="small"
                        multiple
                        options={characterOptions}
                        getOptionLabel={opt => opt.name}
                        value={selectedChildren}
                        onChange={(_, val) => setSelectedChildren(val)}
                        isOptionEqualToValue={(opt, val) => opt.id === val.id}
                        renderTags={(val, getTagProps) =>
                          val.map((opt, i) => <Chip {...getTagProps({ index: i })} key={opt.id} label={opt.name} size="small" />)
                        }
                        renderInput={params => <TextField {...params} placeholder="Add child…" />}
                      />
                    </Box>
                  </>
                ) : (
                  <>
                    {(parent1 || parent2) && (
                      <Box>
                        <InfoLabel>Parents</InfoLabel>
                        <Box display="flex" flexDirection="column">
                          {parent1 && <CharacterLink entity={{ id: parent1.id, name: parent1.name }} worldId={worldId!} />}
                          {parent2 && <CharacterLink entity={{ id: parent2.id, name: parent2.name }} worldId={worldId!} />}
                        </Box>
                      </Box>
                    )}
                    {character.spouses?.length > 0 && (
                      <Box>
                        <InfoLabel>{character.spouses.length === 1 ? 'Spouse' : 'Spouses'}</InfoLabel>
                        <Box display="flex" flexDirection="column">
                          {character.spouses.map(s => <CharacterLink key={s.id} entity={s} worldId={worldId!} />)}
                        </Box>
                      </Box>
                    )}
                    {character.children?.length > 0 && (
                      <Box>
                        <InfoLabel>{character.children.length === 1 ? 'Child' : 'Children'}</InfoLabel>
                        <Box display="flex" flexDirection="column">
                          {character.children.map(c => <CharacterLink key={c.id} entity={c} worldId={worldId!} />)}
                        </Box>
                      </Box>
                    )}
                  </>
                )}
              </Box>
            </>
          )}

          {/* ── Physical description ── */}
          {(editing || physicalRows.length > 0) && (
            <>
              <SectionHeader>Physical description</SectionHeader>
              <Box display="flex" flexDirection="column" sx={{ px: 1.5, py: 1, gap: 1 }}>
                {editing ? (
                  <>
                    <Box>
                      <InfoLabel>Height</InfoLabel>
                      <TextField size="small" fullWidth value={height} onChange={e => setHeight(e.target.value)} placeholder="e.g. 6'2&quot;" />
                    </Box>
                    <Box>
                      <InfoLabel>Hair colour</InfoLabel>
                      <TextField size="small" fullWidth value={hairColour} onChange={e => setHairColour(e.target.value)} />
                    </Box>
                  </>
                ) : (
                  physicalRows.map(({ label, value }) => (
                    <Box key={label}>
                      <InfoLabel>{label}</InfoLabel>
                      <Typography variant="body2">{value}</Typography>
                    </Box>
                  ))
                )}
              </Box>
            </>
          )}
        </Box>

      </Box>

    </Box>
  );
}
