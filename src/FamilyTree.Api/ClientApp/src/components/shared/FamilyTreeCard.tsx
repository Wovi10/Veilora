import { Box, Card, CardActionArea, Typography } from '@mui/material';
import AccountTreeIcon from '@mui/icons-material/AccountTree';
import type { FamilyTreeDto } from '../../types/familyTree';

export default function FamilyTreeCard({ tree, onClick }: { tree: FamilyTreeDto; onClick: () => void }) {
  return (
    <Card sx={{ borderRadius: 2, transition: 'box-shadow 0.2s', '&:hover': { boxShadow: 4 } }}>
      <CardActionArea onClick={onClick} sx={{ p: 2 }}>
        <Box display="flex" alignItems="center" gap={1.5}>
          <AccountTreeIcon color="primary" />
          <Box>
            <Typography variant="subtitle1" fontWeight={600}>{tree.name}</Typography>
            {tree.description && (
              <Typography variant="body2" color="text.secondary">{tree.description}</Typography>
            )}
            <Typography variant="caption" color="text.secondary" display="block" mt={0.5}>
              Updated {new Date(tree.updatedAt).toLocaleDateString('en-GB')}
            </Typography>
          </Box>
        </Box>
      </CardActionArea>
    </Card>
  );
}
