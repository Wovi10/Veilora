import { useState, useEffect } from 'react';
import {
  Dialog, DialogTitle, DialogContent, DialogActions, Button,
  TextField, MenuItem, Box, Select, FormControl, InputLabel, Typography,
  Autocomplete, Chip, Collapse,
} from '@mui/material';
import { updateCharacter, deleteCharacter } from '../../api/charactersApi';
import { getLanguagesByWorld, getOrCreateLanguage } from '../../api/languagesApi';
import { getLocationsByWorld } from '../../api/locationsApi';
import { getDateSuffixesByWorld } from '../../api/dateSuffixesApi';
import type { CharacterDto, Gender, UpdateCharacterDto } from '../../types/character';
import type { EntityDto } from '../../types/entity';
import type { EntityRefDto } from '../../types/entityRef';
import type { LanguageDto } from '../../types/language';
import type { LocationDto } from '../../types/location';
import type { DateSuffixDto } from '../../types/dateSuffix';

interface Props {
  open: boolean;
  character: CharacterDto;
  worldCharacters: CharacterDto[];
  worldEntities: EntityDto[];
  worldId: string;
  onClose: () => void;
  onSaved: (character: CharacterDto) => void;
  onDeleted?: () => void;
}

type LocationOrString = EntityRefDto | string;

const GENDERS: Gender[] = ['Male', 'Female', 'Other', 'Unknown'];

export default function EditCharacterDialog({ open, character, worldCharacters, worldEntities, worldId, onClose, onSaved, onDeleted }: Props) {
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [species, setSpecies] = useState('');
  const [gender, setGender] = useState<Gender>('Unknown');
  const [birthDate, setBirthDate] = useState('');
  const [birthDateSuffixId, setBirthDateSuffixId] = useState('');
  const [deathDate, setDeathDate] = useState('');
  const [deathDateSuffixId, setDeathDateSuffixId] = useState('');
  const [description, setDescription] = useState('');
  const [parent1Id, setParent1Id] = useState('');
  const [parent2Id, setParent2Id] = useState('');
  const [otherNames, setOtherNames] = useState('');
  const [position, setPosition] = useState('');
  const [height, setHeight] = useState('');
  const [hairColour, setHairColour] = useState('');

  const [birthPlaceLocation, setBirthPlaceLocation] = useState<EntityRefDto | string | null>(null);
  const [birthPlaceInput, setBirthPlaceInput] = useState('');
  const [deathPlaceLocation, setDeathPlaceLocation] = useState<EntityRefDto | string | null>(null);
  const [deathPlaceInput, setDeathPlaceInput] = useState('');
  const [selectedLocations, setSelectedLocations] = useState<Array<EntityRefDto | string>>([]);
  const [selectedAffiliations, setSelectedAffiliations] = useState<Array<EntityRefDto | string>>([]);
  const [selectedSpouses, setSelectedSpouses] = useState<EntityRefDto[]>([]);
  const [selectedChildren, setSelectedChildren] = useState<EntityRefDto[]>([]);

  const [selectedLanguages, setSelectedLanguages] = useState<Array<LanguageDto | string>>([]);
  const [availableLanguages, setAvailableLanguages] = useState<LanguageDto[]>([]);
  const [availableLocations, setAvailableLocations] = useState<LocationDto[]>([]);
  const [availableDateSuffixes, setAvailableDateSuffixes] = useState<DateSuffixDto[]>([]);

  const [expanded, setExpanded] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [deleting, setDeleting] = useState(false);
  const [confirmDelete, setConfirmDelete] = useState(false);
  const [error, setError] = useState('');

  const locationOptions: EntityRefDto[] = availableLocations.map(l => ({ id: l.id, name: l.name }));
  const groupOptions: EntityRefDto[] = worldEntities
    .filter(e => e.type === 'Group')
    .map(e => ({ id: e.id, name: e.name }));
  const characterOptions: EntityRefDto[] = worldCharacters
    .filter(c => c.id !== character.id)
    .map(c => ({ id: c.id, name: c.name }));

  useEffect(() => {
    if (!character) return;
    setConfirmDelete(false);
    setExpanded(false);
    setError('');
    setFirstName(character.firstName ?? '');
    setLastName(character.lastName ?? '');
    setSpecies(character.species ?? '');
    setGender(character.gender ?? 'Unknown');
    setBirthDate(character.birthDate ?? '');
    setBirthDateSuffixId(character.birthDateSuffixId ?? '');
    setDeathDate(character.deathDate ?? '');
    setDeathDateSuffixId(character.deathDateSuffixId ?? '');
    setDescription(character.description ?? '');
    setParent1Id(character.parent1Id ?? '');
    setParent2Id(character.parent2Id ?? '');
    setOtherNames(character.otherNames ?? '');
    setPosition(character.position ?? '');
    setHeight(character.height ?? '');
    setHairColour(character.hairColour ?? '');
    const birthPlace = character.birthPlaceLocationId
      ? { id: character.birthPlaceLocationId, name: character.birthPlaceLocationName ?? '' }
      : null;
    setBirthPlaceLocation(birthPlace);
    setBirthPlaceInput(birthPlace?.name ?? '');
    const deathPlace = character.deathPlaceLocationId
      ? { id: character.deathPlaceLocationId, name: character.deathPlaceLocationName ?? '' }
      : null;
    setDeathPlaceLocation(deathPlace);
    setDeathPlaceInput(deathPlace?.name ?? '');
    setSelectedLocations(character.locations ?? [] as Array<EntityRefDto | string>);
    setSelectedAffiliations(character.affiliations ?? [] as Array<EntityRefDto | string>);
    setSelectedLanguages(character.languages ?? []);
    setSelectedSpouses(character.spouses ?? []);
    setSelectedChildren(character.children ?? []);
  }, [character]);

  useEffect(() => {
    if (!open || !worldId) return;
    getLanguagesByWorld(worldId).then(setAvailableLanguages).catch(() => {});
    getLocationsByWorld(worldId).then(setAvailableLocations).catch(() => {});
    getDateSuffixesByWorld(worldId).then(setAvailableDateSuffixes).catch(() => {});
  }, [open, worldId]);

  const handleDelete = async () => {
    setDeleting(true);
    try {
      await deleteCharacter(character.id);
      onDeleted?.();
    } catch {
      setError('Failed to delete character');
      setConfirmDelete(false);
    } finally {
      setDeleting(false);
    }
  };

  const handleSubmit = async () => {
    setSubmitting(true);
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

      const effectiveName = (firstName.trim() || lastName.trim())
        ? [firstName.trim(), lastName.trim()].filter(Boolean).join(' ')
        : character.name;

      const dto: UpdateCharacterDto = {
        name: effectiveName,
        description: description.trim() || undefined,
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
      };
      const updated = await updateCharacter(character.id, dto);
      onSaved(updated);
    } catch {
      setError('Failed to save changes');
    } finally {
      setSubmitting(false);
    }
  };

  const others = worldCharacters.filter(c => c.id !== character.id);

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>Edit Character</DialogTitle>
      <DialogContent>
        <Box display="flex" flexDirection="column" gap={2} pt={1}>
          <Box display="flex" gap={2}>
            <TextField label="First Name" value={firstName} onChange={e => setFirstName(e.target.value)} fullWidth />
            <TextField label="Last Name" value={lastName} onChange={e => setLastName(e.target.value)} fullWidth />
          </Box>
          <Box display="flex" gap={2}>
            <TextField label="Other Names" value={otherNames} onChange={e => setOtherNames(e.target.value)} fullWidth placeholder="Aliases, nicknames…" />
            <TextField label="Position / Title" value={position} onChange={e => setPosition(e.target.value)} fullWidth placeholder="King, Knight…" />
          </Box>
          <Box display="flex" gap={2}>
            <TextField label="Species / Race" value={species} onChange={e => setSpecies(e.target.value)} fullWidth />
            <FormControl fullWidth>
              <InputLabel>Gender</InputLabel>
              <Select value={gender} label="Gender" onChange={e => setGender(e.target.value as Gender)}>
                {GENDERS.map(g => <MenuItem key={g} value={g}>{g}</MenuItem>)}
              </Select>
            </FormControl>
          </Box>
          <TextField label="Description" value={description} onChange={e => setDescription(e.target.value)} multiline rows={4} placeholder="Brief description of this character…" />
          <Button
            size="small"
            onClick={() => setExpanded(e => !e)}
            sx={{ alignSelf: 'flex-start', color: 'text.secondary' }}
          >
            {expanded ? '▴ Less details' : '▾ More details'}
          </Button>
          <Collapse in={expanded}>
            <Box display="flex" flexDirection="column" gap={2}>
              <Box display="flex" gap={2}>
                <TextField label="Height" value={height} onChange={e => setHeight(e.target.value)} fullWidth placeholder="e.g. 6'2&quot;" />
                <TextField label="Hair Colour" value={hairColour} onChange={e => setHairColour(e.target.value)} fullWidth />
              </Box>
              <Box display="flex" gap={2}>
                <TextField label="Birth Date" type="date" value={birthDate} onChange={e => setBirthDate(e.target.value)} slotProps={{ inputLabel: { shrink: true } }} fullWidth />
                {availableDateSuffixes.length > 0 && (
                  <FormControl sx={{ minWidth: 140 }}>
                    <InputLabel>Birth Era</InputLabel>
                    <Select value={birthDateSuffixId} label="Birth Era" onChange={e => setBirthDateSuffixId(e.target.value)}>
                      <MenuItem value="">None</MenuItem>
                      {availableDateSuffixes.map(s => <MenuItem key={s.id} value={s.id}>{s.abbreviation} — {s.name}</MenuItem>)}
                    </Select>
                  </FormControl>
                )}
              </Box>
              <Autocomplete
                freeSolo
                options={locationOptions}
                getOptionLabel={opt => typeof opt === 'string' ? opt : opt.name}
                value={birthPlaceLocation}
                inputValue={birthPlaceInput}
                onChange={(_, val) => setBirthPlaceLocation(val as LocationOrString | null)}
                onInputChange={(_, val) => setBirthPlaceInput(val)}
                renderInput={params => <TextField {...params} label="Birth Place" placeholder="Select or type to create…" />}
              />
              <Box display="flex" gap={2}>
                <TextField label="Death Date" type="date" value={deathDate} onChange={e => setDeathDate(e.target.value)} slotProps={{ inputLabel: { shrink: true } }} fullWidth />
                {availableDateSuffixes.length > 0 && (
                  <FormControl sx={{ minWidth: 140 }}>
                    <InputLabel>Death Era</InputLabel>
                    <Select value={deathDateSuffixId} label="Death Era" onChange={e => setDeathDateSuffixId(e.target.value)}>
                      <MenuItem value="">None</MenuItem>
                      {availableDateSuffixes.map(s => <MenuItem key={s.id} value={s.id}>{s.abbreviation} — {s.name}</MenuItem>)}
                    </Select>
                  </FormControl>
                )}
              </Box>
              <Autocomplete
                freeSolo
                options={locationOptions}
                getOptionLabel={opt => typeof opt === 'string' ? opt : opt.name}
                value={deathPlaceLocation}
                inputValue={deathPlaceInput}
                onChange={(_, val) => setDeathPlaceLocation(val as LocationOrString | null)}
                onInputChange={(_, val) => setDeathPlaceInput(val)}
                renderInput={params => <TextField {...params} label="Death Place" placeholder="Select or type to create…" />}
              />
              <Autocomplete
                multiple
                freeSolo
                options={locationOptions}
                getOptionLabel={opt => typeof opt === 'string' ? opt : opt.name}
                value={selectedLocations}
                onChange={(_, val) => setSelectedLocations(val as Array<LocationOrString>)}
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
                onChange={(_, val) => setSelectedAffiliations(val as Array<LocationOrString>)}
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
                  {others.map(c => <MenuItem key={c.id} value={c.id}>{c.name}</MenuItem>)}
                </Select>
              </FormControl>
              <FormControl fullWidth>
                <InputLabel>Parent 2</InputLabel>
                <Select value={parent2Id} label="Parent 2" onChange={e => setParent2Id(e.target.value)}>
                  <MenuItem value="">None</MenuItem>
                  {others.filter(c => c.id !== parent1Id).map(c => <MenuItem key={c.id} value={c.id}>{c.name}</MenuItem>)}
                </Select>
              </FormControl>
            </Box>
          </Collapse>
          {error && <Typography color="error" variant="caption">{error}</Typography>}
        </Box>
      </DialogContent>
      <DialogActions sx={{ justifyContent: 'space-between' }}>
        {!confirmDelete ? (
          <Button color="error" onClick={() => setConfirmDelete(true)} disabled={submitting || deleting}>
            Delete
          </Button>
        ) : (
          <Box display="flex" alignItems="center" gap={1}>
            <Typography variant="body2" color="text.secondary">Delete this character?</Typography>
            <Button color="error" variant="contained" onClick={handleDelete} disabled={deleting} size="small">
              {deleting ? 'Deleting…' : 'Yes, Delete'}
            </Button>
            <Button onClick={() => setConfirmDelete(false)} disabled={deleting} size="small">
              No
            </Button>
          </Box>
        )}
        {!confirmDelete && (
          <Box display="flex" gap={1}>
            <Button onClick={onClose} disabled={submitting}>Cancel</Button>
            <Button onClick={handleSubmit} variant="contained" disabled={submitting}>Save</Button>
          </Box>
        )}
      </DialogActions>
    </Dialog>
  );
}
