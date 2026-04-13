import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Box, Typography, Button, CircularProgress, Alert,
  Card, CardActionArea,
} from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import { getWorlds } from '../api/worldsApi';
import type { WorldDto } from '../types/world';
import { useEditMode } from '../context/EditModeContext';
import NewWorldDialog from '../components/NewWorldDialog';

export default function HomePage() {
  const navigate = useNavigate();
  const { isEditMode } = useEditMode();
  const [worlds, setWorlds] = useState<WorldDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [newWorldOpen, setNewWorldOpen] = useState(false);

  useEffect(() => {
    // order the worlds by update date DESC
    getWorlds()
      .then(worlds => worlds.sort((a, b) => new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime()))
      .then(setWorlds)
      .catch(() => setError('Failed to load worlds'))
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>;
  if (error) return <Alert severity="error" sx={{ m: 3 }}>{error}</Alert>;

  return (
    <Box sx={{ maxWidth: 1200, mx: 'auto', px: 3, py: 4 }}>
      <Box display="flex" alignItems="center" justifyContent="space-between" mb={4}>
        <Typography variant="h4" fontWeight={700}>My Worlds</Typography>
        {isEditMode && (
          <Button variant="contained" startIcon={<AddIcon />} onClick={() => setNewWorldOpen(true)}>
            New World
          </Button>
        )}
      </Box>

      {worlds.length === 0 ? (
        <Box textAlign="center" mt={8}>
          <Typography variant="h6" color="text.secondary">No worlds yet.</Typography>
          {isEditMode && (
            <Button variant="contained" startIcon={<AddIcon />} sx={{ mt: 2 }} onClick={() => setNewWorldOpen(true)}>
              Create your first world
            </Button>
          )}
        </Box>
      ) : (
        <Box display="flex" flexDirection="column" gap={2}>
          {worlds.map(world => (
            <Card
              key={world.id}
              sx={{
                borderRadius: 3,
                overflow: 'hidden',
                transition: 'box-shadow 0.2s, transform 0.15s',
                '&:hover': { boxShadow: 8, transform: 'translateY(-1px)' },
              }}
            >
              <CardActionArea onClick={() => navigate(`/worlds/${world.id}`)} sx={{ p: 0 }}>
                <Box
                  sx={{
                    height: 160,
                    px: 5,
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'space-between',
                    background: 'linear-gradient(135deg, #1a1a2e 0%, #16213e 50%, #0f3460 100%)',
                    color: 'white',
                  }}
                >
                  <Box>
                    <Typography variant="h3" fontWeight={700} sx={{ letterSpacing: '-0.5px', lineHeight: 1.1 }}>
                      {world.name}
                    </Typography>
                    {world.author && (
                      <Typography variant="subtitle1" sx={{ mt: 0.5, opacity: 0.6, fontStyle: 'italic' }}>
                        by {world.author}
                      </Typography>
                    )}
                  </Box>
                  <Box textAlign="right">
                    <Typography variant="caption" sx={{ opacity: 0.4, textTransform: 'uppercase', letterSpacing: 1 }}>
                      Last updated
                    </Typography>
                    <Typography variant="body2" sx={{ opacity: 0.75 }}>
                      {new Date(world.updatedAt).toLocaleDateString('en-GB', {
                        day: 'numeric', month: 'long', year: 'numeric',
                      })}
                    </Typography>
                  </Box>
                </Box>
              </CardActionArea>
            </Card>
          ))}
        </Box>
      )}

      <NewWorldDialog
        open={newWorldOpen}
        onClose={() => setNewWorldOpen(false)}
        onCreated={world => { setWorlds(prev => [...prev, world]); setNewWorldOpen(false); }}
      />
    </Box>
  );
}
