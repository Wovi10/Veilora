import { Paper, Typography, MenuItem, Divider } from '@mui/material';
import type { WorldSearchResult, WorldSearchItem } from '../../types/worldSearch';

interface Props {
  worldId: string;
  results: WorldSearchResult;
  onNavigate: (path: string) => void;
}

interface Section {
  key: keyof WorldSearchResult;
  label: string;
  getPath: (worldId: string, item: WorldSearchItem) => string;
}

const SECTIONS: Section[] = [
  { key: 'characters',  label: 'Characters',   getPath: (wid, i) => `/worlds/${wid}/characters/${i.id}` },
  { key: 'locations',   label: 'Locations',    getPath: (wid, i) => `/worlds/${wid}/locations/${i.id}` },
  { key: 'groups',      label: 'Groups',       getPath: (wid)    => `/worlds/${wid}/entities/Group` },
  { key: 'events',      label: 'Events',       getPath: (wid)    => `/worlds/${wid}/entities/Event` },
  { key: 'concepts',    label: 'Concepts',     getPath: (wid)    => `/worlds/${wid}/entities/Concept` },
  { key: 'familyTrees', label: 'Family Trees', getPath: (wid, i) => `/worlds/${wid}/family-trees/${i.id}` },
];

export default function WorldSearchDropdown({ worldId, results, onNavigate }: Props) {
  const nonEmptySections = SECTIONS.filter(s => results[s.key].length > 0);
  const hasResults = nonEmptySections.length > 0;

  return (
    <Paper
      elevation={4}
      sx={{
        position: 'absolute',
        top: '100%',
        left: 0,
        right: 0,
        zIndex: 1300,
        maxHeight: 400,
        overflowY: 'auto',
        mt: 0.5,
        borderRadius: 1,
      }}
    >
      {!hasResults ? (
        <Typography variant="body2" color="text.secondary" sx={{ px: 2, py: 1.5 }}>
          No results found
        </Typography>
      ) : (
        nonEmptySections.map((section, idx) => (
          <div key={section.key}>
            {idx > 0 && <Divider />}
            <Typography
              variant="caption"
              color="text.secondary"
              sx={{ display: 'block', px: 2, pt: 1, pb: 0.5, fontWeight: 600, textTransform: 'uppercase', letterSpacing: 0.5 }}
            >
              {section.label}
            </Typography>
            {(results[section.key] as WorldSearchItem[]).map(item => (
              <MenuItem
                key={item.id}
                dense
                onClick={() => onNavigate(section.getPath(worldId, item))}
                sx={{ px: 2, py: 0.75 }}
              >
                {item.name}
              </MenuItem>
            ))}
          </div>
        ))
      )}
    </Paper>
  );
}
