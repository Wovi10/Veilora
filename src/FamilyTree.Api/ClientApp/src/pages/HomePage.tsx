import { useEffect, useState } from 'react';
import {
  Alert,
  Box,
  Button,
  Card,
  CardActions,
  CardContent,
  CircularProgress,
  Container,
  Grid,
  Typography,
} from '@mui/material';
import { getTrees } from '../api/treesApi';
import type { TreeDto } from '../types/tree';
import NewTreeDialog from '../components/NewTreeDialog';

export default function HomePage() {
  const [trees, setTrees] = useState<TreeDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [dialogOpen, setDialogOpen] = useState(false);

  useEffect(() => {
    getTrees()
      .then(setTrees)
      .catch((err: unknown) =>
        setError(err instanceof Error ? err.message : 'Unexpected error')
      )
      .finally(() => setLoading(false));
  }, []);

  return (
    <Container maxWidth="xl" sx={{ py: 5, px: { xs: 3, sm: 4, md: 5 } }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 4 }}>
        <Typography variant="h5" component="h1">
          My Trees
        </Typography>
        <Button variant="contained" onClick={() => setDialogOpen(true)}>
          New Tree
        </Button>
      </Box>

      <NewTreeDialog
        open={dialogOpen}
        onClose={() => setDialogOpen(false)}
        onCreated={(tree) => setTrees((prev) => [...prev, tree])}
      />

      {loading && (
        <Box sx={{ display: 'flex', justifyContent: 'center', mt: 8 }}>
          <CircularProgress />
        </Box>
      )}

      {error && (
        <Alert severity="error" sx={{ mt: 2 }}>
          {error}
        </Alert>
      )}

      {!loading && !error && trees.length === 0 && (
        <Typography color="text.secondary" sx={{ mt: 4, textAlign: 'center' }}>
          No family trees yet. Create one to get started.
        </Typography>
      )}

      {!loading && !error && trees.length > 0 && (
        <Grid container spacing={3}>
          {trees.map((tree) => (
            <Grid key={tree.id} size={{ xs: 12, sm: 6, md: 6, lg: 4, xl: 3 }}>
              <Card variant="outlined" sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
                <CardContent sx={{ flexGrow: 1 }}>
                  <Typography variant="h6" gutterBottom>
                    {tree.name}
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    {tree.description ?? 'No description'}
                  </Typography>
                </CardContent>
                <CardActions sx={{ justifyContent: 'space-between', px: 2, pb: 2 }}>
                  <Typography variant="caption" color="text.secondary">
                    Created {new Date(tree.createdAt).toLocaleDateString()}
                  </Typography>
                  <Button size="small" variant="outlined" disabled>
                    Open
                  </Button>
                </CardActions>
              </Card>
            </Grid>
          ))}
        </Grid>
      )}
    </Container>
  );
}
