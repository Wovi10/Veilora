import { useState, useEffect } from 'react';
import {
  Dialog, DialogTitle, DialogContent, DialogActions, Button,
  TextField, MenuItem, Box, Select, FormControl, InputLabel, Typography,
  Autocomplete, Chip,
} from '@mui/material';
import { updateCharacter, deleteCharacter } from '../api/charactersApi';
import { getLanguagesByWorld, getOrCreateLanguage } from '../api/languagesApi';
import type { CharacterDto, Gender, UpdateCharacterDto } from '../types/character';
import type { EntityDto } from '../types/entity';
import type { EntityRefDto } from '../types/entityRef';
import type { LanguageDto } from '../types/language';

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

type PlaceOrString = EntityRefDto | string;

const GENDERS: Gender[] = ['Male', 'Female', 'Other', 'Unknown'];

export default function EditCharacterDialog({ open, character, worldCharacters, worldEntities, worldId, onClose, onSaved, onDeleted }: Props) {
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [species, setSpecies] = useState('');
  const [gender, setGender] = useState<Gender>('Unknown');
  const [birthDate, setBirthDate] = useState('');
  const [birthDateSuffix, setBirthDateSuffix] = useState('');
  const [deathDate, setDeathDate] = useState('');
  const [deathDateSuffix, setDeathDateSuffix] = useState('');
  const [description, setDescription] = useState('');
  const [parent1Id, setParent1Id] = useState('');
  const [parent2Id, setParent2Id] = useState('');
  const [otherNames, setOtherNames] = useState('');
  const [position, setPosition] = useState('');
  const [height, setHeight] = useState('');
  const [hairColour, setHairColour] = useState('');

  const [birthPlaceEntity, setBirthPlaceEntity] = useState<EntityRefDto | string | null>(null);
  const [birthPlaceInput, setBirthPlaceInput] = useState('');
  const [deathPlaceEntity, setDeathPlaceEntity] = useState<EntityRefDto | string | null>(null);
  const [deathPlaceInput, setDeathPlaceInput] = useState('');
  const [selectedLocations, setSelectedLocations] = useState<Array<EntityRefDto | string>>([]);
  const [selectedAffiliations, setSelectedAffiliations] = useState<Array<EntityRefDto | string>>([]);
  const [selectedSpouses, setSelectedSpouses] = useState<EntityRefDto[]>([]);
  const [selectedChildren, setSelectedChildren] = useState<EntityRefDto[]>([]);

  const [selectedLanguages, setSelectedLanguages] = useState<Array<LanguageDto | string>>([]);
  const [availableLanguages, setAvailableLanguages] = useState<LanguageDto[]>([]);

  const [submitting, setSubmitting] = useState(false);
  const [deleting, setDeleting] = useState(false);
  const [confirmDelete, setConfirmDelete] = useState(false);
  const [error, setError] = useState('');

  const placeOptions: EntityRefDto[] = worldEntities
    .filter(e => e.type === 'Place')
    .map(e => ({ id: e.id, name: e.name }));
  const groupOptions: EntityRefDto[] = worldEntities
    .filter(e => e.type === 'Group')
    .map(e => ({ id: e.id, name: e.name }));
  const characterOptions: EntityRefDto[] = worldCharacters
    .filter(c => c.id !== character.id)
    .map(c => ({ id: c.id, name: c.name }));

  useEffect(() => {
    if (!character) return;
    setConfirmDelete(false);
    setError('');
    setFirstName(character.firstName ?? '');
    setLastName(character.lastName ?? '');
    setSpecies(character.species ?? '');
    setGender(character.gender ?? 'Unknown');
    setBirthDate(character.birthDate ?? '');
    setBirthDateSuffix(character.birthDateSuffix ?? '');
    setDeathDate(character.deathDate ?? '');
    setDeathDateSuffix(character.deathDateSuffix ?? '');
    setDescription(character.description ?? '');
    setParent1Id(character.parent1Id ?? '');
    setParent2Id(character.parent2Id ?? '');
    setOtherNames(character.otherNames ?? '');
    setPosition(character.position ?? '');
    setHeight(character.height ?? '');
    setHairColour(character.hairColour ?? '');
    const birthPlace = character.birthPlaceEntityId
      ? { id: character.birthPlaceEntityId, name: character.birthPlaceEntityName ?? '' }
      : null;
    setBirthPlaceEntity(birthPlace);
    setBirthPlaceInput(birthPlace?.name ?? '');
    const deathPlace = character.deathPlaceEntityId
      ? { id: character.deathPlaceEntityId, name: character.deathPlaceEntityName ?? '' }
      : null;
    setDeathPlaceEntity(deathPlace);
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
        birthDateSuffix: birthDateSuffix.trim() || undefined,
        birthPlaceEntityId: typeof birthPlaceEntity !== 'string' ? (birthPlaceEntity?.id ?? null) : null,
        birthPlaceName: typeof birthPlaceEntity === 'string' ? birthPlaceEntity.trim() : (birthPlaceInput.trim() && !birthPlaceEntity ? birthPlaceInput.trim() : undefined),
        deathDate: deathDate || undefined,
        deathDateSuffix: deathDateSuffix.trim() || undefined,
        deathPlaceEntityId: typeof deathPlaceEntity !== 'string' ? (deathPlaceEntity?.id ?? null) : null,
        deathPlaceName: typeof deathPlaceEntity === 'string' ? deathPlaceEntity.trim() : (deathPlaceInput.trim() && !deathPlaceEntity ? deathPlaceInput.trim() : undefined),
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
      console.log(dto)
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
          <TextField label="Species / Race" value={species} onChange={e => setSpecies(e.target.value)} />
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
            options={placeOptions}
            getOptionLabel={opt => typeof opt === 'string' ? opt : opt.name}
            value={birthPlaceEntity}
            inputValue={birthPlaceInput}
            onChange={(_, val) => setBirthPlaceEntity(val as PlaceOrString | null)}
            onInputChange={(_, val) => setBirthPlaceInput(val)}
            renderInput={params => <TextField {...params} label="Birth Place" placeholder="Select or type to create…" />}
          />
          <Box display="flex" gap={2}>
            <TextField label="Death Date" type="date" value={deathDate} onChange={e => setDeathDate(e.target.value)} slotProps={{ inputLabel: { shrink: true } }} fullWidth />
            <TextField label="Death Era / Suffix" placeholder="e.g. TA, SA, FA" value={deathDateSuffix} onChange={e => setDeathDateSuffix(e.target.value)} sx={{ maxWidth: 160 }} />
          </Box>
          <Autocomplete
            freeSolo
            options={placeOptions}
            getOptionLabel={opt => typeof opt === 'string' ? opt : opt.name}
            value={deathPlaceEntity}
            inputValue={deathPlaceInput}
            onChange={(_, val) => setDeathPlaceEntity(val as PlaceOrString | null)}
            onInputChange={(_, val) => setDeathPlaceInput(val)}
            renderInput={params => <TextField {...params} label="Death Place" placeholder="Select or type to create…" />}
          />
          <Autocomplete
            multiple
            freeSolo
            options={placeOptions}
            getOptionLabel={opt => typeof opt === 'string' ? opt : opt.name}
            value={selectedLocations}
            onChange={(_, val) => setSelectedLocations(val as Array<PlaceOrString>)}
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
            onChange={(_, val) => setSelectedAffiliations(val as Array<PlaceOrString>)}
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
