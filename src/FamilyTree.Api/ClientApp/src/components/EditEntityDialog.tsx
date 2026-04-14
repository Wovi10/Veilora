import { useState, useEffect } from 'react';
import {
  Dialog, DialogTitle, DialogContent, DialogActions, Button,
  TextField, MenuItem, Box, Select, FormControl, InputLabel, Typography,
  Autocomplete, Chip,
} from '@mui/material';
import { updateEntity } from '../api/entitiesApi';
import { getLanguagesByWorld, getOrCreateLanguage } from '../api/languagesApi';
import type { EntityDto, Gender } from '../types/entity';
import type { EntityRefDto } from '../types/entityRef';
import type { LanguageDto } from '../types/language';

interface Props {
  open: boolean;
  entity: EntityDto;
  treeEntities: EntityDto[];
  worldId: string;
  onClose: () => void;
  onSaved: (entity: EntityDto) => void;
}

const GENDERS: Gender[] = ['Male', 'Female', 'Other', 'Unknown'];

export default function EditEntityDialog({ open, entity, treeEntities, worldId, onClose, onSaved }: Props) {
  // Basic fields
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

  // New detail fields
  const [otherNames, setOtherNames] = useState('');
  const [position, setPosition] = useState('');
  const [height, setHeight] = useState('');
  const [hairColour, setHairColour] = useState('');

  // Entity-linked fields
  const [birthPlaceEntity, setBirthPlaceEntity] = useState<EntityRefDto | null>(null);
  const [deathPlaceEntity, setDeathPlaceEntity] = useState<EntityRefDto | null>(null);
  const [selectedLocations, setSelectedLocations] = useState<EntityRefDto[]>([]);
  const [selectedAffiliations, setSelectedAffiliations] = useState<EntityRefDto[]>([]);
  const [selectedSpouses, setSelectedSpouses] = useState<EntityRefDto[]>([]);
  const [selectedChildren, setSelectedChildren] = useState<EntityRefDto[]>([]);

  // Languages (freeSolo — can hold LanguageDto or plain string)
  const [selectedLanguages, setSelectedLanguages] = useState<Array<LanguageDto | string>>([]);
  const [availableLanguages, setAvailableLanguages] = useState<LanguageDto[]>([]);

  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');

  // Derived option lists from world entities
  const placeOptions: EntityRefDto[] = treeEntities
    .filter(e => e.type === 'Place')
    .map(e => ({ id: e.id, name: e.name }));
  const groupOptions: EntityRefDto[] = treeEntities
    .filter(e => e.type === 'Group')
    .map(e => ({ id: e.id, name: e.name }));
  const characterOptions: EntityRefDto[] = treeEntities
    .filter(e => e.type === 'Character' && e.id !== entity.id)
    .map(e => ({ id: e.id, name: e.name }));

  useEffect(() => {
    if (!entity) return;
    setFirstName(entity.firstName ?? '');
    setLastName(entity.lastName ?? '');
    setSpecies(entity.species ?? '');
    setGender(entity.gender ?? 'Unknown');
    setBirthDate(entity.birthDate ?? '');
    setBirthDateSuffix(entity.birthDateSuffix ?? '');
    setDeathDate(entity.deathDate ?? '');
    setDeathDateSuffix(entity.deathDateSuffix ?? '');
    setDescription(entity.description ?? '');
    setParent1Id(entity.parent1Id ?? '');
    setParent2Id(entity.parent2Id ?? '');
    setOtherNames(entity.otherNames ?? '');
    setPosition(entity.position ?? '');
    setHeight(entity.height ?? '');
    setHairColour(entity.hairColour ?? '');
    setBirthPlaceEntity(entity.birthPlaceEntityId
      ? { id: entity.birthPlaceEntityId, name: entity.birthPlaceEntityName ?? '' }
      : null);
    setDeathPlaceEntity(entity.deathPlaceEntityId
      ? { id: entity.deathPlaceEntityId, name: entity.deathPlaceEntityName ?? '' }
      : null);
    setSelectedLocations(entity.locations ?? []);
    setSelectedAffiliations(entity.affiliations ?? []);
    setSelectedLanguages(entity.languages ?? []);
    setSelectedSpouses(entity.spouses ?? []);
    setSelectedChildren(entity.children ?? []);
  }, [entity]);

  useEffect(() => {
    if (!open || !worldId) return;
    getLanguagesByWorld(worldId).then(setAvailableLanguages).catch(() => {});
  }, [open, worldId]);

  const handleSubmit = async () => {
    setSubmitting(true);
    setError('');
    try {
      // Resolve any plain-string language entries via getOrCreate
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
        : entity.name;

      const updated = await updateEntity(entity.id, {
        name: effectiveName,
        type: entity.type,
        description: description.trim() || undefined,
        firstName: firstName.trim() || undefined,
        lastName: lastName.trim() || undefined,
        species: species.trim() || undefined,
        gender,
        birthDate: birthDate || undefined,
        birthDateSuffix: birthDateSuffix.trim() || undefined,
        deathDate: deathDate || undefined,
        deathDateSuffix: deathDateSuffix.trim() || undefined,
        birthPlaceEntityId: birthPlaceEntity?.id ?? null,
        deathPlaceEntityId: deathPlaceEntity?.id ?? null,
        otherNames: otherNames.trim() || undefined,
        position: position.trim() || undefined,
        height: height.trim() || undefined,
        hairColour: hairColour.trim() || undefined,
        parent1Id: parent1Id || null,
        parent2Id: parent2Id || null,
        locationIds: selectedLocations.map(l => l.id),
        affiliationIds: selectedAffiliations.map(a => a.id),
        languageIds: resolvedLanguageIds,
        spouseIds: selectedSpouses.map(s => s.id),
        childIds: selectedChildren.map(c => c.id),
      });
      onSaved(updated);
    } catch {
      setError('Failed to save changes');
    } finally {
      setSubmitting(false);
    }
  };

  const others = treeEntities.filter(e => e.id !== entity.id && e.type === 'Character');

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>Edit {entity.type}</DialogTitle>
      <DialogContent>
        <Box display="flex" flexDirection="column" gap={2} pt={1}>
          {entity.type === 'Character' ? (
            <>
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
                options={placeOptions}
                getOptionLabel={opt => opt.name}
                value={birthPlaceEntity}
                onChange={(_, val) => setBirthPlaceEntity(val)}
                isOptionEqualToValue={(opt, val) => opt.id === val.id}
                renderInput={params => <TextField {...params} label="Birth Place" />}
              />
              <Box display="flex" gap={2}>
                <TextField label="Death Date" type="date" value={deathDate} onChange={e => setDeathDate(e.target.value)} slotProps={{ inputLabel: { shrink: true } }} fullWidth />
                <TextField label="Death Era / Suffix" placeholder="e.g. TA, SA, FA" value={deathDateSuffix} onChange={e => setDeathDateSuffix(e.target.value)} sx={{ maxWidth: 160 }} />
              </Box>
              <Autocomplete
                options={placeOptions}
                getOptionLabel={opt => opt.name}
                value={deathPlaceEntity}
                onChange={(_, val) => setDeathPlaceEntity(val)}
                isOptionEqualToValue={(opt, val) => opt.id === val.id}
                renderInput={params => <TextField {...params} label="Death Place" />}
              />
              <Autocomplete
                multiple
                options={placeOptions}
                getOptionLabel={opt => opt.name}
                value={selectedLocations}
                onChange={(_, val) => setSelectedLocations(val)}
                isOptionEqualToValue={(opt, val) => opt.id === val.id}
                renderTags={(val, getTagProps) =>
                  val.map((opt, i) => <Chip {...getTagProps({ index: i })} key={opt.id} label={opt.name} size="small" />)
                }
                renderInput={params => <TextField {...params} label="Locations" placeholder="Add location…" />}
              />
              <Autocomplete
                multiple
                options={groupOptions}
                getOptionLabel={opt => opt.name}
                value={selectedAffiliations}
                onChange={(_, val) => setSelectedAffiliations(val)}
                isOptionEqualToValue={(opt, val) => opt.id === val.id}
                renderTags={(val, getTagProps) =>
                  val.map((opt, i) => <Chip {...getTagProps({ index: i })} key={opt.id} label={opt.name} size="small" />)
                }
                renderInput={params => <TextField {...params} label="Affiliations" placeholder="Add group…" />}
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
                  {others.map(e => <MenuItem key={e.id} value={e.id}>{e.name}</MenuItem>)}
                </Select>
              </FormControl>
              <FormControl fullWidth>
                <InputLabel>Parent 2</InputLabel>
                <Select value={parent2Id} label="Parent 2" onChange={e => setParent2Id(e.target.value)}>
                  <MenuItem value="">None</MenuItem>
                  {others.filter(e => e.id !== parent1Id).map(e => <MenuItem key={e.id} value={e.id}>{e.name}</MenuItem>)}
                </Select>
              </FormControl>
            </>
          ) : (
            <TextField label="Description" value={description} onChange={e => setDescription(e.target.value)} multiline rows={3} />
          )}
          {error && <Typography color="error" variant="caption">{error}</Typography>}
        </Box>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Cancel</Button>
        <Button onClick={handleSubmit} variant="contained" disabled={submitting}>Save</Button>
      </DialogActions>
    </Dialog>
  );
}
