import { useState } from 'react';
import { Box, Typography } from '@mui/material';
import CategoryIcon from '@mui/icons-material/Category';
import type { EntityDto } from '../../types/entity';
import { useReadingSession } from '../../context/ReadingSessionContext';
import EntitySearchBar from './EntitySearchBar';
import EntityEditor from './EntityEditor';
import CharacterEditor from './CharacterEditor';

type Selection =
  | { kind: 'entity'; data: EntityDto }
  | { kind: 'character'; id: string }
  | null;

export default function ReadingWorkspace() {
  const { session } = useReadingSession();
  const [selection, setSelection] = useState<Selection>(null);

  if (!session) return null;

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', height: '100%', overflow: 'hidden' }}>
      <EntitySearchBar
        worldId={session.worldId}
        hasSelection={selection !== null}
        onSelectEntity={entity => setSelection({ kind: 'entity', data: entity })}
        onSelectCharacter={id => setSelection({ kind: 'character', id })}
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
          onClose={() => setSelection(null)}
        />
      )}

      {selection?.kind === 'character' && (
        <CharacterEditor
          characterId={selection.id}
          worldId={session.worldId}
          onClose={() => setSelection(null)}
        />
      )}
    </Box>
  );
}
