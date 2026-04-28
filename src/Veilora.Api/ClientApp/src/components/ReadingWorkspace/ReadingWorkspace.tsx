import { useState } from 'react';
import { Box, Typography } from '@mui/material';
import CategoryIcon from '@mui/icons-material/Category';
import type { EntityDto } from '../../types/entity';
import { useReadingSession } from '../../context/ReadingSessionContext';
import { deleteEntity } from '../../api/entitiesApi';
import { deleteLocation } from '../../api/locationsApi';
import { deleteCharacter } from '../../api/charactersApi';
import { deleteEvent } from '../../api/eventsApi';
import EntitySearchBar from './EntitySearchBar';
import EntityEditor from './EntityEditor';
import CharacterEditor from './CharacterEditor';
import LocationEditor from './LocationEditor';
import EventEditor from './EventEditor';
import CreateEntityPanel from './CreateEntityPanel';

type Selection =
  | { kind: 'entity'; data: EntityDto; isNew?: boolean }
  | { kind: 'character'; id: string; isNew?: boolean }
  | { kind: 'location'; id: string; isNew?: boolean }
  | { kind: 'event'; id: string; isNew?: boolean }
  | { kind: 'create'; name: string }
  | null;

export default function ReadingWorkspace() {
  const { session } = useReadingSession();
  const [selection, setSelection] = useState<Selection>(null);

  if (!session) return null;

  async function handleClose() {
    if (selection?.kind === 'entity' && selection.isNew) await deleteEntity(selection.data.id).catch(() => {});
    else if (selection?.kind === 'location' && selection.isNew) await deleteLocation(selection.id).catch(() => {});
    else if (selection?.kind === 'character' && selection.isNew) await deleteCharacter(selection.id).catch(() => {});
    else if (selection?.kind === 'event' && selection.isNew) await deleteEvent(selection.id).catch(() => {});
    setSelection(null);
  }

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', height: '100%', overflow: 'hidden' }}>
      <EntitySearchBar
        worldId={session.worldId}
        hasSelection={selection !== null}
        onSelectEntity={entity => setSelection({ kind: 'entity', data: entity })}
        onSelectCharacter={id => setSelection({ kind: 'character', id })}
        onSelectLocation={id => setSelection({ kind: 'location', id })}
        onSelectEvent={id => setSelection({ kind: 'event', id })}
        onStartCreate={name => setSelection({ kind: 'create', name })}
        onDeselect={() => setSelection(null)}
      />

      {!selection && (
        <Box sx={{ flex: 1, display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', gap: 1, color: 'text.disabled' }}>
          <CategoryIcon sx={{ fontSize: 40, opacity: 0.3 }} />
          <Typography variant="body2" fontStyle="italic">
            Search for an entity or character to edit it.
          </Typography>
        </Box>
      )}

      {selection?.kind === 'entity' && (
        <EntityEditor
          entity={selection.data}
          onSaved={updated => setSelection({ kind: 'entity', data: updated })}
          onClose={selection.isNew ? handleClose : () => setSelection(null)}
        />
      )}

      {selection?.kind === 'character' && (
        <CharacterEditor
          characterId={selection.id}
          worldId={session.worldId}
          onClose={selection.isNew ? handleClose : () => setSelection(null)}
        />
      )}

      {selection?.kind === 'location' && (
        <LocationEditor
          locationId={selection.id}
          onClose={selection.isNew ? handleClose : () => setSelection(null)}
        />
      )}

      {selection?.kind === 'event' && (
        <EventEditor
          eventId={selection.id}
          onClose={selection.isNew ? handleClose : () => setSelection(null)}
        />
      )}

      {selection?.kind === 'create' && (
        <CreateEntityPanel
          initialName={selection.name}
          worldId={session.worldId}
          onCreated={result => {
            if (result.kind === 'entity') setSelection({ kind: 'entity', data: result.data, isNew: true });
            else if (result.kind === 'location') setSelection({ kind: 'location', id: result.data.id, isNew: true });
            else if (result.kind === 'event') setSelection({ kind: 'event', id: result.data.id, isNew: true });
            else setSelection({ kind: 'character', id: result.data.id, isNew: true });
          }}
          onClose={() => setSelection(null)}
        />
      )}
    </Box>
  );
}
