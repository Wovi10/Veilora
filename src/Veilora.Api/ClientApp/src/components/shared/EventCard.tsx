import { Box, Card, CardActionArea, CardContent, IconButton, Typography } from '@mui/material';
import EditIcon from '@mui/icons-material/Edit';
import type { EventDto } from '../../types/event';

export default function EventCard({ event, canEdit = false, onEdit, onClick }: { event: EventDto; canEdit?: boolean; onEdit?: () => void; onClick?: () => void }) {
  return (
    <Card sx={{ borderRadius: 2, height: '100%', transition: 'box-shadow 0.2s', '&:hover': { boxShadow: 3 } }}>
      {onClick ? (
        <CardActionArea onClick={onClick} sx={{ height: '100%' }}>
          <CardContent sx={{ pb: '12px !important' }}>
            <Typography variant="subtitle1" fontWeight={600} noWrap>{event.name}</Typography>
            {event.description && (
              <Typography
                variant="body2"
                color="text.secondary"
                sx={{ mt: 0.5, display: '-webkit-box', WebkitLineClamp: 2, WebkitBoxOrient: 'vertical', overflow: 'hidden' }}
              >
                {event.description}
              </Typography>
            )}
            <Typography variant="caption" color="text.secondary" display="block" mt={0.5}>
              Updated {new Date(event.updatedAt).toLocaleDateString('en-GB')}
            </Typography>
          </CardContent>
        </CardActionArea>
      ) : (
        <CardContent sx={{ pb: '12px !important' }}>
          <Box display="flex" justifyContent="space-between" alignItems="flex-start">
            <Typography variant="subtitle1" fontWeight={600} noWrap sx={{ flex: 1 }}>{event.name}</Typography>
            {canEdit && (
              <IconButton size="small" onClick={onEdit} sx={{ ml: 0.5, mt: -0.5 }}>
                <EditIcon fontSize="small" />
              </IconButton>
            )}
          </Box>
          {event.description && (
            <Typography
              variant="body2"
              color="text.secondary"
              sx={{ mt: 0.5, display: '-webkit-box', WebkitLineClamp: 2, WebkitBoxOrient: 'vertical', overflow: 'hidden' }}
            >
              {event.description}
            </Typography>
          )}
          <Typography variant="caption" color="text.secondary" display="block" mt={0.5}>
            Updated {new Date(event.updatedAt).toLocaleDateString('en-GB')}
          </Typography>
        </CardContent>
      )}
    </Card>
  );
}
