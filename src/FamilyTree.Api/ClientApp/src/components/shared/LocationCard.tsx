import { Box, Card, CardActionArea, CardContent, IconButton, Typography } from '@mui/material';
import EditIcon from '@mui/icons-material/Edit';
import type { LocationDto } from '../types/location';

export default function LocationCard({ location, canEdit = false, onEdit, onClick }: { location: LocationDto; canEdit?: boolean; onEdit?: () => void; onClick?: () => void }) {
  return (
    <Card sx={{ borderRadius: 2, height: '100%', transition: 'box-shadow 0.2s', '&:hover': { boxShadow: 3 } }}>
      {onClick ? (
        <CardActionArea onClick={onClick} sx={{ height: '100%' }}>
          <CardContent sx={{ pb: '12px !important' }}>
            <Typography variant="subtitle1" fontWeight={600} noWrap>{location.name}</Typography>
            {location.description && (
              <Typography
                variant="body2"
                color="text.secondary"
                sx={{ mt: 0.5, display: '-webkit-box', WebkitLineClamp: 2, WebkitBoxOrient: 'vertical', overflow: 'hidden' }}
              >
                {location.description}
              </Typography>
            )}
            <Typography variant="caption" color="text.secondary" display="block" mt={0.5}>
              Updated {new Date(location.updatedAt).toLocaleDateString('en-GB')}
            </Typography>
          </CardContent>
        </CardActionArea>
      ) : (
      <CardContent sx={{ pb: '12px !important' }}>
        <Box display="flex" justifyContent="space-between" alignItems="flex-start">
          <Typography variant="subtitle1" fontWeight={600} noWrap sx={{ flex: 1 }}>{location.name}</Typography>
          {canEdit && (
            <IconButton size="small" onClick={onEdit} sx={{ ml: 0.5, mt: -0.5 }}>
              <EditIcon fontSize="small" />
            </IconButton>
          )}
        </Box>
        {location.description && (
          <Typography
            variant="body2"
            color="text.secondary"
            sx={{ mt: 0.5, display: '-webkit-box', WebkitLineClamp: 2, WebkitBoxOrient: 'vertical', overflow: 'hidden' }}
          >
            {location.description}
          </Typography>
        )}
        <Typography variant="caption" color="text.secondary" display="block" mt={0.5}>
          Updated {new Date(location.updatedAt).toLocaleDateString('en-GB')}
        </Typography>
      </CardContent>
      )}
    </Card>
  );
}
