import { useState, useEffect } from 'react';
import { Box, TextField, IconButton, Alert, Tooltip } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import DeleteIcon from '@mui/icons-material/Delete';
import EditIcon from '@mui/icons-material/Edit';
import ArrowUpwardIcon from '@mui/icons-material/ArrowUpward';
import ArrowDownwardIcon from '@mui/icons-material/ArrowDownward';
import CheckIcon from '@mui/icons-material/Check';
import CloseIcon from '@mui/icons-material/Close';
import { getPhotos, addPhoto, updatePhoto, deletePhoto, reorderPhotos } from '../../api/photosApi';
import type { PhotoDto, PhotoTargetType } from '../../types/photo';

interface PhotoSectionProps {
  entityType: PhotoTargetType;
  entityId: string;
  canEdit: boolean;
}

export default function PhotoSection({ entityType, entityId, canEdit }: PhotoSectionProps) {
  const [photos, setPhotos] = useState<PhotoDto[]>([]);
  const [addUrl, setAddUrl] = useState('');
  const [error, setError] = useState('');
  const [editingId, setEditingId] = useState<string | null>(null);
  const [editUrl, setEditUrl] = useState('');

  useEffect(() => {
    getPhotos(entityType, entityId)
      .then(setPhotos)
      .catch(() => setError('Failed to load photos.'));
  }, [entityType, entityId]);

  if (photos.length === 0 && !canEdit) return null;

  const handleAdd = async () => {
    setError('');
    try {
      const created = await addPhoto(entityType, entityId, { url: addUrl });
      setPhotos(prev => [...prev, created]);
      setAddUrl('');
    } catch (e: unknown) {
      const msg = e instanceof Error ? e.message : 'Failed to add photo.';
      setError(msg);
    }
  };

  const handleDelete = async (photoId: string) => {
    setError('');
    try {
      await deletePhoto(entityType, entityId, photoId);
      setPhotos(prev => prev.filter(p => p.id !== photoId));
    } catch {
      setError('Failed to delete photo.');
    }
  };

  const handleMoveUp = async (index: number) => {
    if (index === 0) return;
    setError('');
    const reordered = [...photos];
    [reordered[index - 1], reordered[index]] = [reordered[index], reordered[index - 1]];
    try {
      const updated = await reorderPhotos(entityType, entityId, { orderedIds: reordered.map(p => p.id) });
      setPhotos(updated);
    } catch {
      setError('Failed to reorder photos.');
    }
  };

  const handleMoveDown = async (index: number) => {
    if (index === photos.length - 1) return;
    setError('');
    const reordered = [...photos];
    [reordered[index], reordered[index + 1]] = [reordered[index + 1], reordered[index]];
    try {
      const updated = await reorderPhotos(entityType, entityId, { orderedIds: reordered.map(p => p.id) });
      setPhotos(updated);
    } catch {
      setError('Failed to reorder photos.');
    }
  };

  const handleStartEdit = (photo: PhotoDto) => {
    setEditingId(photo.id);
    setEditUrl(photo.url);
    setError('');
  };

  const handleCancelEdit = () => {
    setEditingId(null);
    setEditUrl('');
  };

  const handleSaveEdit = async (photoId: string) => {
    setError('');
    try {
      const updated = await updatePhoto(entityType, entityId, photoId, { url: editUrl });
      setPhotos(prev => prev.map(p => (p.id === photoId ? updated : p)));
      setEditingId(null);
      setEditUrl('');
    } catch {
      setError('Failed to update photo.');
    }
  };

  return (
    <Box>
      {error && (
        <Alert severity="error" sx={{ mb: 1 }} onClose={() => setError('')}>
          {error}
        </Alert>
      )}
      <Box
        sx={{
          display: 'flex',
          flexDirection: 'row',
          gap: 1,
          overflowX: 'auto',
          pb: 1,
        }}
      >
        {photos.map((photo, index) => (
          <Box key={photo.id} sx={{ position: 'relative', flexShrink: 0 }}>
            {editingId === photo.id ? (
              <Box sx={{ display: 'flex', flexDirection: 'column', gap: 0.5, width: 200 }}>
                <TextField
                  size="small"
                  value={editUrl}
                  onChange={e => setEditUrl(e.target.value)}
                  placeholder="New URL…"
                  fullWidth
                />
                <Box display="flex" gap={0.5}>
                  <Tooltip title="Save">
                    <IconButton size="small" color="primary" onClick={() => handleSaveEdit(photo.id)}>
                      <CheckIcon fontSize="small" />
                    </IconButton>
                  </Tooltip>
                  <Tooltip title="Cancel">
                    <IconButton size="small" onClick={handleCancelEdit}>
                      <CloseIcon fontSize="small" />
                    </IconButton>
                  </Tooltip>
                </Box>
              </Box>
            ) : (
              <Box>
                <Box
                  component="img"
                  src={photo.url}
                  alt={`Photo ${index + 1}`}
                  sx={{
                    height: 120,
                    width: 'auto',
                    objectFit: 'cover',
                    borderRadius: 1,
                    display: 'block',
                  }}
                />
                {canEdit && (
                  <Box display="flex" gap={0.5} mt={0.5}>
                    <Tooltip title="Move up">
                      <span>
                        <IconButton size="small" onClick={() => handleMoveUp(index)} disabled={index === 0}>
                          <ArrowUpwardIcon fontSize="small" />
                        </IconButton>
                      </span>
                    </Tooltip>
                    <Tooltip title="Move down">
                      <span>
                        <IconButton size="small" onClick={() => handleMoveDown(index)} disabled={index === photos.length - 1}>
                          <ArrowDownwardIcon fontSize="small" />
                        </IconButton>
                      </span>
                    </Tooltip>
                    <Tooltip title="Edit URL">
                      <IconButton size="small" onClick={() => handleStartEdit(photo)}>
                        <EditIcon fontSize="small" />
                      </IconButton>
                    </Tooltip>
                    <Tooltip title="Delete">
                      <IconButton size="small" color="error" onClick={() => handleDelete(photo.id)}>
                        <DeleteIcon fontSize="small" />
                      </IconButton>
                    </Tooltip>
                  </Box>
                )}
              </Box>
            )}
          </Box>
        ))}
      </Box>
      {canEdit && (
        <Box display="flex" gap={1} mt={1} alignItems="center">
          <TextField
            size="small"
            value={addUrl}
            onChange={e => setAddUrl(e.target.value)}
            placeholder="Photo URL…"
            sx={{ flex: 1 }}
            onKeyDown={e => { if (e.key === 'Enter') handleAdd(); }}
          />
          <Tooltip title="Add photo">
            <IconButton color="primary" onClick={handleAdd} disabled={!addUrl.trim()}>
              <AddIcon />
            </IconButton>
          </Tooltip>
        </Box>
      )}
    </Box>
  );
}
