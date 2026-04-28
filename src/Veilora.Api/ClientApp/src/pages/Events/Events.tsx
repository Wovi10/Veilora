import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box, Typography, CircularProgress, Alert, Button, Grid2,
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import AddIcon from '@mui/icons-material/Add';
import { getWorld } from '../../api/worldsApi';
import { getEventsByWorld } from '../../api/eventsApi';
import type { WorldDto } from '../../types/world';
import type { EventDto } from '../../types/event';
import { useEditMode } from '../../context/EditModeContext';
import { useAuth } from '../../context/AuthContext';
import { AddEventDialog, EditEventDialog, EventCard } from '../../components';

export default function Events() {
  const { worldId } = useParams<{ worldId: string }>();
  const navigate = useNavigate();
  const { isEditMode } = useEditMode();
  const { userId } = useAuth();

  const [world, setWorld] = useState<WorldDto | null>(null);
  const [events, setEvents] = useState<EventDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [addOpen, setAddOpen] = useState(false);
  const [editingEvent, setEditingEvent] = useState<EventDto | null>(null);

  useEffect(() => {
    if (!worldId) return;
    Promise.all([getWorld(worldId), getEventsByWorld(worldId)])
      .then(([w, evs]) => {
        setWorld(w);
        setEvents(evs.sort((a, b) => new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime()));
      })
      .catch(() => setError('Failed to load'))
      .finally(() => setLoading(false));
  }, [worldId]);

  if (loading) return <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>;
  if (error)   return <Alert severity="error" sx={{ m: 3 }}>{error}</Alert>;
  if (!world)  return null;

  const isOwner = !!userId && world.createdById === userId;
  const canEdit = isEditMode && isOwner;

  return (
    <Box sx={{ maxWidth: 1200, mx: 'auto', px: 3, py: 4 }}>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={4}>
        <Button startIcon={<ArrowBackIcon />} onClick={() => navigate(`/worlds/${worldId}`)}>
          {world.name}
        </Button>
        {canEdit && (
          <Button size="small" startIcon={<AddIcon />} onClick={() => setAddOpen(true)}>
            Add Event
          </Button>
        )}
      </Box>

      <Typography variant="h4" fontWeight={700} mb={3}>
        Events
        <Typography component="span" variant="h6" color="text.secondary" fontWeight={400} ml={1.5}>
          {events.length}
        </Typography>
      </Typography>

      {events.length === 0 ? (
        <Typography variant="body2" color="text.secondary" fontStyle="italic">
          No events yet.
        </Typography>
      ) : (
        <Grid2 container spacing={2}>
          {events.map(event => (
            <Grid2 key={event.id} size={{ xs: 12, sm: 6, md: 4, lg: 3 }}>
              <EventCard
                event={event}
                canEdit={canEdit}
                onEdit={() => setEditingEvent(event)}
              />
            </Grid2>
          ))}
        </Grid2>
      )}

      {worldId && (
        <AddEventDialog
          open={addOpen}
          worldId={worldId}
          onClose={() => setAddOpen(false)}
          onCreated={event => {
            setEvents(prev =>
              [event, ...prev].sort(
                (a, b) => new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime()
              )
            );
            setAddOpen(false);
          }}
        />
      )}

      {editingEvent && (
        <EditEventDialog
          open
          event={editingEvent}
          onClose={() => setEditingEvent(null)}
          onSaved={updated => {
            setEvents(prev => prev.map(e => e.id === updated.id ? updated : e));
            setEditingEvent(null);
          }}
          onDeleted={() => {
            setEvents(prev => prev.filter(e => e.id !== editingEvent.id));
            setEditingEvent(null);
          }}
        />
      )}
    </Box>
  );
}
