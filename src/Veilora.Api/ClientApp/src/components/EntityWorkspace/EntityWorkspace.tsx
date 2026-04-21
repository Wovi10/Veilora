import { useState } from 'react';
import { Box, Typography } from '@mui/material';
import CategoryIcon from '@mui/icons-material/Category';
import type { EntityDto } from '../../types/entity';
import { useReadingSession } from '../../context/ReadingSessionContext';
import EntitySearchBar from './EntitySearchBar';
import EntityEditor from './EntityEditor';

export default function EntityWorkspace() {
  const { session } = useReadingSession();
  const [selectedEntity, setSelectedEntity] = useState<EntityDto | null>(null);

  if (!session) return null;

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', height: '100%', overflow: 'hidden' }}>
      <EntitySearchBar
        worldId={session.worldId}
        onSelect={(entity) => setSelectedEntity(entity)}
        onDeselect={() => setSelectedEntity(null)}
        selectedEntity={selectedEntity}
      />

      {!selectedEntity && (
        <Box
          sx={{
            flex: 1,
            display: 'flex',
            flexDirection: 'column',
            alignItems: 'center',
            justifyContent: 'center',
            gap: 1,
            color: 'text.disabled',
          }}
        >
          <CategoryIcon sx={{ fontSize: 40, opacity: 0.3 }} />
          <Typography variant="body2" fontStyle="italic">
            Search for an entity to view or edit it.
          </Typography>
        </Box>
      )}

      {selectedEntity && (
        <EntityEditor
          entity={selectedEntity}
          onSaved={setSelectedEntity}
          onClose={() => setSelectedEntity(null)}
        />
      )}
    </Box>
  );
}
