import { useState, useEffect } from 'react';
import {
  Box, TextField, Typography, Button, CircularProgress, Divider,
  FormControl, InputLabel, Select, MenuItem, Autocomplete, Chip, IconButton,
} from '@mui/material';
import CheckIcon from '@mui/icons-material/Check';
import CloseIcon from '@mui/icons-material/Close';
import { getCharacter, updateCharacter } from '../../api/charactersApi';
import { getLocationsByWorld } from '../../api/locationsApi';
import { getLanguagesByWorld, getOrCreateLanguage } from '../../api/languagesApi';
import { getDateSuffixesByWorld } from '../../api/dateSuffixesApi';
import { getCharactersByWorld } from '../../api/charactersApi';
import { getEntitiesByWorldAndType } from '../../api/entitiesApi';
import type { CharacterDto, Gender } from '../../types/character';
import type { EntityRefDto } from '../../types/entityRef';
import type { LanguageDto } from '../../types/language';
import type { LocationDto } from '../../types/location';
import type { DateSuffixDto } from '../../types/dateSuffix';
import type { EntityDto } from '../../types/entity';

const GENDERS: Gender[] = ['Male', 'Female', 'Other', 'Unknown'];

interface Props {
  characterId: string;
  worldId: string;
  onClose: () => void;
}

function SectionLabel({ children }: { children: React.ReactNode }) {
  return (
    <Typography variant="caption" sx={{ textTransform: 'uppercase', letterSpacing: '0.07em', fontSize: '0.62rem', fontWeight: 700, color: 'text.disabled' }}>
      {children}
    </Typography>
  );
}

export default function CharacterEditor({ characterId, worldId, onClose }: Props) {
  const [character, setCharacter] = useState<CharacterDto | null>(null);
  const [loadingChar, setLoadingChar] = useState(true);

  // Reference data
  const [availableLocations, setAvailableLocations] = useState<LocationDto[]>([]);
  const [availableLanguages, setAvailableLanguages] = useState<LanguageDto[]>([]);
  const [availableDateSuffixes, setAvailableDateSuffixes] = useState<DateSuffixDto[]>([]);
  const [availableCharacters, setAvailableCharacters] = useState<CharacterDto[]>([]);
  const [availableGroups, setAvailableGroups] = useState<EntityDto[]>([]);

  // Form state
  const [firstName, setFirstName] = useState('');
  const [middleName, setMiddleName] = useState('');
  const [lastName, setLastName] = useState('');
  const [maidenName, setMaidenName] = useState('');
  const [otherNames, setOtherNames] = useState('');
  const [position, setPosition] = useState('');
  const [species, setSpecies] = useState('');
  const [gender, setGender] = useState<Gender>('Unknown');
  const [height, setHeight] = useState('');
  const [hairColour, setHairColour] = useState('');
  const [birthDate, setBirthDate] = useState('');
  const [birthDateSuffixId, setBirthDateSuffixId] = useState('');
  const [birthPlace, setBirthPlace] = useState<EntityRefDto | string | null>(null);
  const [birthPlaceInput, setBirthPlaceInput] = useState('');
  const [deathDate, setDeathDate] = useState('');
  const [deathDateSuffixId, setDeathDateSuffixId] = useState('');
  const [deathPlace, setDeathPlace] = useState<EntityRefDto | string | null>(null);
  const [deathPlaceInput, setDeathPlaceInput] = useState('');
  const [residence, setResidence] = useState('');
  const [selectedAffiliations, setSelectedAffiliations] = useState<Array<EntityRefDto | string>>([]);
  const [selectedLocations, setSelectedLocations] = useState<Array<EntityRefDto | string>>([]);
  const [selectedLanguages, setSelectedLanguages] = useState<Array<LanguageDto | string>>([]);
  const [selectedSpouses, setSelectedSpouses] = useState<EntityRefDto[]>([]);
  const [selectedChildren, setSelectedChildren] = useState<EntityRefDto[]>([]);
  const [parent1Id, setParent1Id] = useState('');
  const [parent2Id, setParent2Id] = useState('');
  const [biography, setBiography] = useState('');
  const [description, setDescription] = useState('');

  const [saving, setSaving] = useState(false);
  const [saved, setSaved] = useState(false);
  const [error, setError] = useState('');

  // Load character + reference data
  useEffect(() => {
    setLoadingChar(true);
    Promise.all([
      getCharacter(characterId),
      getLocationsByWorld(worldId),
      getLanguagesByWorld(worldId),
      getDateSuffixesByWorld(worldId),
      getCharactersByWorld(worldId),
      getEntitiesByWorldAndType(worldId, 'Group'),
    ]).then(([char, locs, langs, suffixes, chars, groups]) => {
      setCharacter(char);
      setAvailableLocations(locs);
      setAvailableLanguages(langs);
      setAvailableDateSuffixes(suffixes);
      setAvailableCharacters(chars.filter(c => c.id !== characterId));
      setAvailableGroups(groups);
      populateForm(char, locs);
    }).catch(() => setError('Failed to load character')).finally(() => setLoadingChar(false));
  }, [characterId, worldId]);

  function populateForm(char: CharacterDto, locs: LocationDto[]) {
    setFirstName(char.firstName ?? '');
    setMiddleName(char.middleName ?? '');
    setLastName(char.lastName ?? '');
    setMaidenName(char.maidenName ?? '');
    setOtherNames(char.otherNames ?? '');
    setPosition(char.position ?? '');
    setSpecies(char.species ?? '');
    setGender((char.gender as Gender) ?? 'Unknown');
    setHeight(char.height ?? '');
    setHairColour(char.hairColour ?? '');
    setBirthDate(char.birthDate ?? '');
    setBirthDateSuffixId(char.birthDateSuffixId ?? '');
    setDeathDate(char.deathDate ?? '');
    setDeathDateSuffixId(char.deathDateSuffixId ?? '');
    setResidence(char.residence ?? '');
    setBiography(char.biography ?? '');
    setDescription(char.description ?? '');
    setParent1Id(char.parent1Id ?? '');
    setParent2Id(char.parent2Id ?? '');

    const locMap = Object.fromEntries(locs.map(l => [l.id, l]));
    if (char.birthPlaceLocationId && locMap[char.birthPlaceLocationId]) {
      const loc = locMap[char.birthPlaceLocationId];
      setBirthPlace({ id: loc.id, name: loc.name });
      setBirthPlaceInput(loc.name);
    } else if (char.birthPlaceLocationName) {
      setBirthPlace(char.birthPlaceLocationName);
      setBirthPlaceInput(char.birthPlaceLocationName);
    }
    if (char.deathPlaceLocationId && locMap[char.deathPlaceLocationId]) {
      const loc = locMap[char.deathPlaceLocationId];
      setDeathPlace({ id: loc.id, name: loc.name });
      setDeathPlaceInput(loc.name);
    } else if (char.deathPlaceLocationName) {
      setDeathPlace(char.deathPlaceLocationName);
      setDeathPlaceInput(char.deathPlaceLocationName);
    }

    setSelectedAffiliations(char.affiliations.map(a => ({ id: a.id, name: a.name })));
    setSelectedLocations(char.locations.map(l => ({ id: l.id, name: l.name })));
    setSelectedLanguages(char.languages.map(l => ({ id: l.id, name: l.name, worldId })));
    setSelectedSpouses(char.spouses.map(s => ({ id: s.id, name: s.name })));
    setSelectedChildren(char.children.map(c => ({ id: c.id, name: c.name })));
  }

  async function handleSave() {
    if (!character) return;
    setSaving(true);
    setError('');
    try {
      const resolvedLanguageIds: string[] = [];
      for (const lang of selectedLanguages) {
        if (typeof lang === 'string') {
          const resolved = await getOrCreateLanguage(lang.trim(), worldId);
          resolvedLanguageIds.push(resolved.id);
        } else {
          resolvedLanguageIds.push(lang.id);
        }
      }

      const name = [firstName.trim(), lastName.trim()].filter(Boolean).join(' ') || character.name;

      const updated = await updateCharacter(characterId, {
        name,
        description: description.trim() || undefined,
        firstName: firstName.trim() || undefined,
        middleName: middleName.trim() || undefined,
        lastName: lastName.trim() || undefined,
        maidenName: maidenName.trim() || undefined,
        otherNames: otherNames.trim() || undefined,
        position: position.trim() || undefined,
        species: species.trim() || undefined,
        gender,
        height: height.trim() || undefined,
        hairColour: hairColour.trim() || undefined,
        birthDate: birthDate || undefined,
        birthDateSuffixId: birthDateSuffixId || null,
        birthPlaceLocationId: typeof birthPlace !== 'string' ? (birthPlace?.id ?? null) : null,
        birthPlaceName: typeof birthPlace === 'string' ? birthPlace.trim() : (birthPlaceInput.trim() && !birthPlace ? birthPlaceInput.trim() : undefined),
        deathDate: deathDate || undefined,
        deathDateSuffixId: deathDateSuffixId || null,
        deathPlaceLocationId: typeof deathPlace !== 'string' ? (deathPlace?.id ?? null) : null,
        deathPlaceName: typeof deathPlace === 'string' ? deathPlace.trim() : (deathPlaceInput.trim() && !deathPlace ? deathPlaceInput.trim() : undefined),
        residence: residence.trim() || undefined,
        biography: biography.trim() || undefined,
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
      setSaved(true);
      setTimeout(() => setSaved(false), 2000);
    } catch {
      setError('Failed to save changes');
    } finally {
      setSaving(false);
    }
  }

  const locationOptions: EntityRefDto[] = availableLocations.map(l => ({ id: l.id, name: l.name }));
  const groupOptions: EntityRefDto[] = availableGroups.map(e => ({ id: e.id, name: e.name }));
  const characterOptions: EntityRefDto[] = availableCharacters.map(c => ({ id: c.id, name: c.name }));

  if (loadingChar) {
    return <Box sx={{ display: 'flex', justifyContent: 'center', pt: 6 }}><CircularProgress size={24} /></Box>;
  }

  if (!character) {
    return <Box sx={{ px: 3, pt: 3 }}><Typography color="error" variant="body2">{error || 'Character not found.'}</Typography></Box>;
  }

  return (
    <Box sx={{ flex: 1, overflowY: 'auto' }}>
      {/* Header */}
      <Box sx={{ px: 3, pt: 2.5, pb: 1.5, display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
        <Typography variant="subtitle2" fontWeight={700} noWrap sx={{ flex: 1 }}>
          {character.name}
        </Typography>
        <IconButton size="small" onClick={onClose}>
          <CloseIcon fontSize="small" />
        </IconButton>
      </Box>

      <Box sx={{ px: 3, pb: 3, display: 'flex', flexDirection: 'column', gap: 2 }}>

        {/* Name */}
        <Box>
          <SectionLabel>Name</SectionLabel>
          <Box sx={{ display: 'flex', gap: 1.5, mt: 1, flexWrap: 'wrap' }}>
            <TextField label="First" size="small" value={firstName} onChange={e => setFirstName(e.target.value)} sx={{ flex: 1, minWidth: 100 }} />
            <TextField label="Middle" size="small" value={middleName} onChange={e => setMiddleName(e.target.value)} sx={{ flex: 1, minWidth: 100 }} />
            <TextField label="Last" size="small" value={lastName} onChange={e => setLastName(e.target.value)} sx={{ flex: 1, minWidth: 100 }} />
            <TextField label="Maiden" size="small" value={maidenName} onChange={e => setMaidenName(e.target.value)} sx={{ flex: 1, minWidth: 100 }} />
          </Box>
          <Box sx={{ display: 'flex', gap: 1.5, mt: 1.5 }}>
            <TextField label="Other names / aliases" size="small" value={otherNames} onChange={e => setOtherNames(e.target.value)} fullWidth />
          </Box>
        </Box>

        <Divider />

        {/* Identity */}
        <Box>
          <SectionLabel>Identity</SectionLabel>
          <Box sx={{ display: 'flex', gap: 1.5, mt: 1 }}>
            <TextField label="Position / Title" size="small" value={position} onChange={e => setPosition(e.target.value)} sx={{ flex: 1 }} />
            <TextField label="Species / Race" size="small" value={species} onChange={e => setSpecies(e.target.value)} sx={{ flex: 1 }} />
          </Box>
          <FormControl fullWidth size="small" sx={{ mt: 1.5 }}>
            <InputLabel>Gender</InputLabel>
            <Select value={gender} label="Gender" onChange={e => setGender(e.target.value as Gender)}>
              {GENDERS.map(g => <MenuItem key={g} value={g}>{g}</MenuItem>)}
            </Select>
          </FormControl>
        </Box>

        <Divider />

        {/* Appearance */}
        <Box>
          <SectionLabel>Appearance</SectionLabel>
          <Box sx={{ display: 'flex', gap: 1.5, mt: 1 }}>
            <TextField label="Height" size="small" value={height} onChange={e => setHeight(e.target.value)} sx={{ flex: 1 }} />
            <TextField label="Hair Colour" size="small" value={hairColour} onChange={e => setHairColour(e.target.value)} sx={{ flex: 1 }} />
          </Box>
        </Box>

        <Divider />

        {/* Birth */}
        <Box>
          <SectionLabel>Birth</SectionLabel>
          <Box sx={{ display: 'flex', gap: 1.5, mt: 1 }}>
            <TextField label="Birth Date" type="date" size="small" value={birthDate} onChange={e => setBirthDate(e.target.value)} slotProps={{ inputLabel: { shrink: true } }} sx={{ flex: 1 }} />
            {availableDateSuffixes.length > 0 && (
              <FormControl size="small" sx={{ minWidth: 130 }}>
                <InputLabel>Era</InputLabel>
                <Select value={birthDateSuffixId} label="Era" onChange={e => setBirthDateSuffixId(e.target.value)}>
                  <MenuItem value="">None</MenuItem>
                  {availableDateSuffixes.map(s => <MenuItem key={s.id} value={s.id}>{s.abbreviation}</MenuItem>)}
                </Select>
              </FormControl>
            )}
          </Box>
          <Autocomplete
            freeSolo size="small"
            options={locationOptions}
            getOptionLabel={opt => typeof opt === 'string' ? opt : opt.name}
            value={birthPlace}
            inputValue={birthPlaceInput}
            onChange={(_, val) => setBirthPlace(val)}
            onInputChange={(_, val) => setBirthPlaceInput(val)}
            renderInput={params => <TextField {...params} label="Birth Place" sx={{ mt: 1.5 }} />}
          />
        </Box>

        <Divider />

        {/* Death */}
        <Box>
          <SectionLabel>Death</SectionLabel>
          <Box sx={{ display: 'flex', gap: 1.5, mt: 1 }}>
            <TextField label="Death Date" type="date" size="small" value={deathDate} onChange={e => setDeathDate(e.target.value)} slotProps={{ inputLabel: { shrink: true } }} sx={{ flex: 1 }} />
            {availableDateSuffixes.length > 0 && (
              <FormControl size="small" sx={{ minWidth: 130 }}>
                <InputLabel>Era</InputLabel>
                <Select value={deathDateSuffixId} label="Era" onChange={e => setDeathDateSuffixId(e.target.value)}>
                  <MenuItem value="">None</MenuItem>
                  {availableDateSuffixes.map(s => <MenuItem key={s.id} value={s.id}>{s.abbreviation}</MenuItem>)}
                </Select>
              </FormControl>
            )}
          </Box>
          <Autocomplete
            freeSolo size="small"
            options={locationOptions}
            getOptionLabel={opt => typeof opt === 'string' ? opt : opt.name}
            value={deathPlace}
            inputValue={deathPlaceInput}
            onChange={(_, val) => setDeathPlace(val)}
            onInputChange={(_, val) => setDeathPlaceInput(val)}
            renderInput={params => <TextField {...params} label="Death Place" sx={{ mt: 1.5 }} />}
          />
        </Box>

        <Divider />

        {/* Residence */}
        <TextField label="Residence" size="small" value={residence} onChange={e => setResidence(e.target.value)} fullWidth />

        <Divider />

        {/* Connections */}
        <Box>
          <SectionLabel>Connections</SectionLabel>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1.5, mt: 1 }}>
            <Autocomplete
              multiple freeSolo size="small"
              options={groupOptions}
              getOptionLabel={opt => typeof opt === 'string' ? opt : opt.name}
              value={selectedAffiliations}
              onChange={(_, val) => setSelectedAffiliations(val as Array<EntityRefDto | string>)}
              isOptionEqualToValue={(opt, val) => (typeof opt !== 'string' && typeof val !== 'string') ? opt.id === val.id : opt === val}
              renderTags={(val, getTagProps) => val.map((opt, i) => (
                <Chip {...getTagProps({ index: i })} key={typeof opt === 'string' ? opt : opt.id} label={typeof opt === 'string' ? opt : opt.name} size="small" />
              ))}
              renderInput={params => <TextField {...params} label="Affiliations" />}
            />
            <Autocomplete
              multiple freeSolo size="small"
              options={locationOptions}
              getOptionLabel={opt => typeof opt === 'string' ? opt : opt.name}
              value={selectedLocations}
              onChange={(_, val) => setSelectedLocations(val as Array<EntityRefDto | string>)}
              isOptionEqualToValue={(opt, val) => (typeof opt !== 'string' && typeof val !== 'string') ? opt.id === val.id : opt === val}
              renderTags={(val, getTagProps) => val.map((opt, i) => (
                <Chip {...getTagProps({ index: i })} key={typeof opt === 'string' ? opt : opt.id} label={typeof opt === 'string' ? opt : opt.name} size="small" />
              ))}
              renderInput={params => <TextField {...params} label="Locations" />}
            />
            <Autocomplete
              multiple freeSolo size="small"
              options={availableLanguages}
              getOptionLabel={opt => typeof opt === 'string' ? opt : opt.name}
              value={selectedLanguages}
              onChange={(_, val) => setSelectedLanguages(val as Array<LanguageDto | string>)}
              isOptionEqualToValue={(opt, val) => (typeof opt !== 'string' && typeof val !== 'string') ? opt.id === val.id : opt === val}
              renderTags={(val, getTagProps) => val.map((opt, i) => (
                <Chip {...getTagProps({ index: i })} key={typeof opt === 'string' ? opt : opt.id} label={typeof opt === 'string' ? opt : opt.name} size="small" />
              ))}
              renderInput={params => <TextField {...params} label="Languages" />}
            />
            <Autocomplete
              multiple size="small"
              options={characterOptions}
              getOptionLabel={opt => opt.name}
              value={selectedSpouses}
              onChange={(_, val) => setSelectedSpouses(val)}
              isOptionEqualToValue={(opt, val) => opt.id === val.id}
              renderTags={(val, getTagProps) => val.map((opt, i) => (
                <Chip {...getTagProps({ index: i })} key={opt.id} label={opt.name} size="small" />
              ))}
              renderInput={params => <TextField {...params} label="Spouses" />}
            />
            <Autocomplete
              multiple size="small"
              options={characterOptions}
              getOptionLabel={opt => opt.name}
              value={selectedChildren}
              onChange={(_, val) => setSelectedChildren(val)}
              isOptionEqualToValue={(opt, val) => opt.id === val.id}
              renderTags={(val, getTagProps) => val.map((opt, i) => (
                <Chip {...getTagProps({ index: i })} key={opt.id} label={opt.name} size="small" />
              ))}
              renderInput={params => <TextField {...params} label="Children" />}
            />
            <FormControl fullWidth size="small">
              <InputLabel>Parent 1</InputLabel>
              <Select value={parent1Id} label="Parent 1" onChange={e => setParent1Id(e.target.value)}>
                <MenuItem value="">None</MenuItem>
                {availableCharacters.map(c => <MenuItem key={c.id} value={c.id}>{c.name}</MenuItem>)}
              </Select>
            </FormControl>
            <FormControl fullWidth size="small">
              <InputLabel>Parent 2</InputLabel>
              <Select value={parent2Id} label="Parent 2" onChange={e => setParent2Id(e.target.value)}>
                <MenuItem value="">None</MenuItem>
                {availableCharacters.filter(c => c.id !== parent1Id).map(c => <MenuItem key={c.id} value={c.id}>{c.name}</MenuItem>)}
              </Select>
            </FormControl>
          </Box>
        </Box>

        <Divider />

        {/* Biography */}
        <Box>
          <SectionLabel>Biography & Notes</SectionLabel>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1.5, mt: 1 }}>
            <TextField label="Biography" size="small" value={biography} onChange={e => setBiography(e.target.value)} multiline minRows={3} maxRows={8} fullWidth />
            <TextField label="Description" size="small" value={description} onChange={e => setDescription(e.target.value)} multiline minRows={2} maxRows={5} fullWidth />
          </Box>
        </Box>

        {/* Actions */}
        {error && <Typography variant="caption" color="error">{error}</Typography>}
        <Button
          variant="contained"
          size="small"
          onClick={handleSave}
          disabled={saving}
          color={saved ? 'success' : 'primary'}
          startIcon={saving ? <CircularProgress size={14} color="inherit" /> : saved ? <CheckIcon fontSize="small" /> : undefined}
          sx={{ textTransform: 'none', alignSelf: 'flex-start' }}
        >
          {saved ? 'Saved' : 'Save'}
        </Button>

      </Box>
    </Box>
  );
}
