import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box, Typography, CircularProgress, Alert, Button, Grid2,
} from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { getFamilyTreesByWorldPaged } from '../../api/familyTreesApi';
import type { FamilyTreeDto } from '../../types/familyTree';
import { useEditMode } from '../../context/EditModeContext';
import { useAuth } from '../../context/AuthContext';
import { getWorld } from '../../api/worldsApi';
import type { WorldDto } from '../../types/world';
import { FamilyTreeCard, NewFamilyTreeDialog } from '../../components';

export default function FamilyTrees() {
  const { worldId } = useParams<{ worldId: string }>();
  const navigate = useNavigate();
  const { isEditMode } = useEditMode();
  const { userId } = useAuth();

  const [world, setWorld] = useState<WorldDto | null>(null);
  const [familyTrees, setFamilyTrees] = useState<FamilyTreeDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [newFamilyTreeOpen, setNewFamilyTreeOpen] = useState(false);

  useEffect(() => {
    if (!worldId) return;
    Promise.all([
      getWorld(worldId),
      getFamilyTreesByWorldPaged(worldId, 1, 100),
    ])
      .then(([w, trees]) => {
        setWorld(w);
        setFamilyTrees(trees.items);
      })
      .catch(() => setError('Failed to load family trees'))
      .finally(() => setLoading(false));
  }, [worldId]);

  if (loading) return <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>;
  if (error)   return <Alert severity="error" sx={{ m: 3 }}>{error}</Alert>;

  const canEdit = isEditMode && !!userId && world?.createdById === userId;

  return (
    <Box sx={{ maxWidth: 1200, mx: 'auto', px: 3, py: 4 }}>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={4}>
        <Button startIcon={<ArrowBackIcon />} onClick={() => navigate(`/worlds/${worldId}`)}>
          {world?.name ?? 'World'}
        </Button>
        {canEdit && (
          <Button size="small" startIcon={<AddIcon />} onClick={() => setNewFamilyTreeOpen(true)}>
            New Family Tree
          </Button>
        )}
      </Box>

      <Typography variant="h4" fontWeight={700} mb={2}>
        Family Trees
        <Typography component="span" variant="h6" color="text.secondary" fontWeight={400} ml={1.5}>
          {familyTrees.length}
        </Typography>
      </Typography>

      {familyTrees.length === 0 ? (
        <Typography variant="body2" color="text.secondary" fontStyle="italic">
          No family trees yet.
        </Typography>
      ) : (
        <Grid2 container spacing={2}>
          {familyTrees.map(tree => (
            <Grid2 key={tree.id} size={{ xs: 12, sm: 6, md: 4 }}>
              <FamilyTreeCard
                tree={tree}
                onClick={() => navigate(`/worlds/${worldId}/family-trees/${tree.id}`)}
              />
            </Grid2>
          ))}
        </Grid2>
      )}

      {worldId && (
        <NewFamilyTreeDialog
          open={newFamilyTreeOpen}
          worldId={worldId}
          onClose={() => setNewFamilyTreeOpen(false)}
          onCreated={tree => { setFamilyTrees(prev => [...prev, tree]); setNewFamilyTreeOpen(false); }}
        />
      )}
    </Box>
  );
}
