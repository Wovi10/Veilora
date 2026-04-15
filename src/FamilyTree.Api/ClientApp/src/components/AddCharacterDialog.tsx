import { useState, useEffect } from 'react';
import {
  Dialog, DialogTitle, DialogContent, DialogActions, Button,
  TextField, MenuItem, Box, Select, FormControl, InputLabel,
  Autocomplete, Chip,
} from '@mui/material';
import { createCharacter } from '../api/charactersApi';
import { getLanguagesByWorld, getOrCreateLanguage } from '../api/languagesApi';
import { getLocationsByWorld } from '../api/locationsApi';
import type { CharacterDto } from '../types/character';
import type { Gender } from '../types/character';
import type { EntityDto } from '../types/entity';
import type { EntityRefDto } from '../types/entityRef';
import type { LanguageDto } from '../types/language';
import type { LocationDto } from '../types/location';

interface Props {
  open: boolean;
  worldId: string;
  worldCharacters: CharacterDto[];
  worldEntities: EntityDto[];
  onClose: () => void;
  onCreated: (character: CharacterDto) => void;
}

const GENDERS: Gender[] = ['Male', 'Female', 'Other', 'Unknown'];

export default function AddCharacterDialog({ open, worldId, worldCharacters, worldEntities, onClose, onCreated }: Props) {
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [otherNames, setOtherNames] = useState('');
  const [position, setPosition] = useState('');
  const [species, setSpecies] = useState('');
  const [gender, setGender] = useState<Gender>('Unknown');
  const [height, setHeight] = useState('');
  const [hairColour, setHairColour] = useState('');
  const [birthDate, setBirthDate] = useState('');
  const [birthDateSuffix, setBirthDateSuffix] = useState('');
  const [birthPlaceLocation, setBirthPlaceLocation] = useState<EntityRefDto | string | null>(null);
  const [birthPlaceInput, setBirthPlaceInput] = useState('');
  const [deathDate, setDeathDate] = useState('');
  const [deathDateSuffix, setDeathDateSuffix] = useState('');
  const [deathPlaceLocation, setDeathPlaceLocation] = useState<EntityRefDto | string | null>(null);
  const [deathPlaceInput, setDeathPlaceInput] = useState('');
  const [selectedLocations, setSelectedLocations] = useState<Array<EntityRefDto | string>>([]);
  const [selectedAffiliations, setSelectedAffiliations] = useState<Array<EntityRefDto | string>>([]);
  const [selectedLanguages, setSelectedLanguages] = useState<Array<LanguageDto | string>>([]);
  const [availableLanguages, setAvailableLanguages] = useState<LanguageDto[]>([]);
  const [availableLocations, setAvailableLocations] = useState<LocationDto[]>([]);
  const [selectedSpouses, setSelectedSpouses] = useState<EntityRefDto[]>([]);
  const [selectedChildren, setSelectedChildren] = useState<EntityRefDto[]>([]);
  const [parent1Id, setParent1Id] = useState('');
  const [parent2Id, setParent2Id] = useState('');
  const [description, setDescription] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    if (!open || !worldId) return;
    getLanguagesByWorld(worldId).then(setAvailableLanguages).catch(() => {});
    getLocationsByWorld(worldId).then(setAvailableLocations).catch(() => {});
  }, [open, worldId]);

  const resetForm = () => {
    setFirstName(''); setLastName(''); setOtherNames(''); setPosition('');
    setSpecies(''); setGender('Unknown'); setHeight(''); setHairColour('');
    setBirthDate(''); setBirthDateSuffix(''); setBirthPlaceLocation(null); setBirthPlaceInput('');
    setDeathDate(''); setDeathDateSuffix(''); setDeathPlaceLocation(null); setDeathPlaceInput('');
    setSelectedLocations([]); setSelectedAffiliations([]); setSelectedLanguages([]);
    setSelectedSpouses([]); setSelectedChildren([]);
    setParent1Id(''); setParent2Id('');
    setDescription(''); setError('');
  };

  const handleClose = () => { resetForm(); onClose(); };

  const handleSubmit = async () => {
    const name = [firstName.trim(), lastName.trim()].filter(Boolean).join(' ');
    if (!name) { setError('Name is required'); return; }
    setSubmitting(true);
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

      const character = await createCharacter({
        name,
        worldId,
        description: description.trim() || undefined,
        firstName: firstName.trim() || undefined,
        lastName: lastName.trim() || undefined,
        otherNames: otherNames.trim() || undefined,
        position: position.trim() || undefined,
        species: species.trim() || undefined,
        gender,
        height: height.trim() || undefined,
        hairColour: hairColour.trim() || undefined,
        birthDate: birthDate || undefined,
        birthDateSuffix: birthDateSuffix.trim() || undefined,
        birthPlaceLocationId: typeof birthPlaceLocation !== 'string' ? birthPlaceLocation?.id : undefined,
        birthPlaceName: typeof birthPlaceLocation === 'string' ? birthPlaceLocation.trim() : (birthPlaceInput.trim() && !birthPlaceLocation ? birthPlaceInput.trim() : undefined),
        deathDate: deathDate || undefined,
        deathDateSuffix: deathDateSuffix.trim() || undefined,
        deathPlaceLocationId: typeof deathPlaceLocation !== 'string' ? deathPlaceLocation?.id : undefined,
        deathPlaceName: typeof deathPlaceLocation === 'string' ? deathPlaceLocation.trim() : (deathPlaceInput.trim() && !deathPlaceLocation ? deathPlaceInput.trim() : undefined),
        parent1Id: parent1Id || undefined,
        parent2Id: parent2Id || undefined,
        locationIds: selectedLocations.filter((l): l is EntityRefDto => typeof l !== 'string').map(l => l.id),
        locationNames: selectedLocations.filter((l): l is string => typeof l === 'string'),
        affiliationIds: selectedAffiliations.filter((a): a is EntityRefDto => typeof a !== 'string').map(a => a.id),
        affiliationNames: selectedAffiliations.filter((a): a is string => typeof a === 'string'),
        languageIds: resolvedLanguageIds,
        spouseIds: selectedSpouses.map(s => s.id),
        childIds: selectedChildren.map(c => c.id),
      });
      onCreated(character);
      resetForm();
    } catch {
      setError('Failed to create character');
    } finally {
      setSubmitting(false);
    }
  };

  const locationOptions: EntityRefDto[] = availableLocations.map(l => ({ id: l.id, name: l.name }));
  const groupOptions: EntityRefDto[] = worldEntities
    .filter(e => e.type === 'Group')
    .map(e => ({ id: e.id, name: e.name }));
  const characterOptions: EntityRefDto[] = worldCharacters
    .map(c => ({ id: c.id, name: c.name }));

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>Add Character</DialogTitle>
      <DialogContent>
        <Box display="flex" flexDirection="column" gap={2} pt={1}>
          <Box display="flex" gap={2}>
            <TextField label="First Name" value={firstName} onChange={e => setFirstName(e.target.value)} fullWidth autoFocus />
            <TextField label="Last Name" value={lastName} onChange={e => setLastName(e.target.value)} fullWidth />
          </Box>
          <Box display="flex" gap={2}>
            <TextField label="Other Names" value={otherNames} onChange={e => setOtherNames(e.target.value)} fullWidth placeholder="Aliases, nicknames…" />
            <TextField label="Position / Title" value={position} onChange={e => setPosition(e.target.value)} fullWidth placeholder="King, Knight…" />
          </Box>
          <TextField label="Species / Race" value={species} onChange={e => setSpecies(e.target.value)} placeholder="e.g. Elf, Hobbit" />
          <FormControl fullWidth>
            <InputLabel>Gender</InputLabel>
            <Select value={gender} label="Gender" onChange={e => setGender(e.target.value as Gender)}>
              {GENDERS.map(g => <MenuItem key={g} value={g}>{g}</MenuItem>)}
            </Select>
          </FormControl>
          <Box display="flex" gap={2}>
            <TextField label="Height" value={height} onChange={e => setHeight(e.target.value)} fullWidth placeholder="e.g. 6'2&quot;" />
            <TextField label="Hair Colour" value={hairColour} onChange={e => setHairColour(e.target.value)} fullWidth />
          </Box>
          <Box display="flex" gap={2}>
            <TextField label="Birth Date" type="date" value={birthDate} onChange={e => setBirthDate(e.target.value)} slotProps={{ inputLabel: { shrink: true } }} fullWidth />
            <TextField label="Birth Era / Suffix" placeholder="e.g. TA, SA, FA" value={birthDateSuffix} onChange={e => setBirthDateSuffix(e.target.value)} sx={{ maxWidth: 160 }} />
          </Box>
          <Autocomplete
            freeSolo
            options={locationOptions}
            getOptionLabel={opt => typeof opt === 'string' ? opt : opt.name}
            value={birthPlaceLocation}
            inputValue={birthPlaceInput}
            onChange={(_, val) => setBirthPlaceLocation(val)}
            onInputChange={(_, val) => setBirthPlaceInput(val)}
            renderInput={params => <TextField {...params} label="Birth Place" placeholder="Select or type to create…" />}
          />
          <Box display="flex" gap={2}>
            <TextField label="Death Date" type="date" value={deathDate} onChange={e => setDeathDate(e.target.value)} slotProps={{ inputLabel: { shrink: true } }} fullWidth />
            <TextField label="Death Era / Suffix" placeholder="e.g. TA, SA, FA" value={deathDateSuffix} onChange={e => setDeathDateSuffix(e.target.value)} sx={{ maxWidth: 160 }} />
          </Box>
          <Autocomplete
            freeSolo
            options={locationOptions}
            getOptionLabel={opt => typeof opt === 'string' ? opt : opt.name}
            value={deathPlaceLocation}
            inputValue={deathPlaceInput}
            onChange={(_, val) => setDeathPlaceLocation(val)}
            onInputChange={(_, val) => setDeathPlaceInput(val)}
            renderInput={params => <TextField {...params} label="Death Place" placeholder="Select or type to create…" />}
          />
          <Autocomplete
            multiple
            freeSolo
            options={locationOptions}
            getOptionLabel={opt => typeof opt === 'string' ? opt : opt.name}
            value={selectedLocations}
            onChange={(_, val) => setSelectedLocations(val as Array<EntityRefDto | string>)}
            renderTags={(val, getTagProps) =>
              val.map((opt, i) => (
                <Chip {...getTagProps({ index: i })} key={typeof opt === 'string' ? opt : opt.id} label={typeof opt === 'string' ? opt : opt.name} size="small" />
              ))
            }
            renderInput={params => <TextField {...params} label="Locations" placeholder="Add or type to create…" />}
          />
          <Autocomplete
            multiple
            freeSolo
            options={groupOptions}
            getOptionLabel={opt => typeof opt === 'string' ? opt : opt.name}
            value={selectedAffiliations}
            onChange={(_, val) => setSelectedAffiliations(val as Array<EntityRefDto | string>)}
            renderTags={(val, getTagProps) =>
              val.map((opt, i) => (
                <Chip {...getTagProps({ index: i })} key={typeof opt === 'string' ? opt : opt.id} label={typeof opt === 'string' ? opt : opt.name} size="small" />
              ))
            }
            renderInput={params => <TextField {...params} label="Affiliations" placeholder="Add or type to create…" />}
          />
          <Autocomplete
            multiple
            freeSolo
            options={availableLanguages}
            getOptionLabel={opt => typeof opt === 'string' ? opt : opt.name}
            value={selectedLanguages}
            onChange={(_, val) => setSelectedLanguages(val as Array<LanguageDto | string>)}
            renderTags={(val, getTagProps) =>
              val.map((opt, i) => (
                <Chip
                  {...getTagProps({ index: i })}
                  key={typeof opt === 'string' ? opt : opt.id}
                  label={typeof opt === 'string' ? opt : opt.name}
                  size="small"
                />
              ))
            }
            renderInput={params => <TextField {...params} label="Languages" placeholder="Type to add…" />}
          />
          <Autocomplete
            multiple
            options={characterOptions}
            getOptionLabel={opt => opt.name}
            value={selectedSpouses}
            onChange={(_, val) => setSelectedSpouses(val)}
            isOptionEqualToValue={(opt, val) => opt.id === val.id}
            renderTags={(val, getTagProps) =>
              val.map((opt, i) => <Chip {...getTagProps({ index: i })} key={opt.id} label={opt.name} size="small" />)
            }
            renderInput={params => <TextField {...params} label="Spouse(s)" placeholder="Add spouse…" />}
          />
          <Autocomplete
            multiple
            options={characterOptions}
            getOptionLabel={opt => opt.name}
            value={selectedChildren}
            onChange={(_, val) => setSelectedChildren(val)}
            isOptionEqualToValue={(opt, val) => opt.id === val.id}
            renderTags={(val, getTagProps) =>
              val.map((opt, i) => <Chip {...getTagProps({ index: i })} key={opt.id} label={opt.name} size="small" />)
            }
            renderInput={params => <TextField {...params} label="Children" placeholder="Add child…" />}
          />
          <FormControl fullWidth>
            <InputLabel>Parent 1</InputLabel>
            <Select value={parent1Id} label="Parent 1" onChange={e => setParent1Id(e.target.value)}>
              <MenuItem value="">None</MenuItem>
              {worldCharacters.map(c => <MenuItem key={c.id} value={c.id}>{c.name}</MenuItem>)}
            </Select>
          </FormControl>
          <FormControl fullWidth>
            <InputLabel>Parent 2</InputLabel>
            <Select value={parent2Id} label="Parent 2" onChange={e => setParent2Id(e.target.value)}>
              <MenuItem value="">None</MenuItem>
              {worldCharacters.filter(c => c.id !== parent1Id).map(c => <MenuItem key={c.id} value={c.id}>{c.name}</MenuItem>)}
            </Select>
          </FormControl>
          <TextField label="Description" value={description} onChange={e => setDescription(e.target.value)} multiline rows={2} />
          {error && <Box color="error.main" fontSize={12}>{error}</Box>}
        </Box>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose}>Cancel</Button>
        <Button onClick={handleSubmit} variant="contained" disabled={submitting}>Add</Button>
      </DialogActions>
    </Dialog>
  );
}
