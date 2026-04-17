import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box, Typography, CircularProgress, Alert, Button, Paper,
  Table, TableBody, TableCell, TableContainer, TableHead, TableRow,
  Switch, IconButton, TextField, Divider, Dialog, DialogTitle,
  DialogContent, DialogContentText, DialogActions, Checkbox,
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import DeleteIcon from '@mui/icons-material/Delete';
import EditIcon from '@mui/icons-material/Edit';
import CheckIcon from '@mui/icons-material/Check';
import CloseIcon from '@mui/icons-material/Close';
import AddIcon from '@mui/icons-material/Add';
import { getWorld, updateWorld, deleteWorld, transferOwnership } from '../api/worldsApi';
import {
  addPermissionByEmail,
  upsertPermission,
  deletePermission,
  NotFoundError,
  getPermissions
} from '../api/worldPermissionsApi';
import { getDateSuffixesByWorld, createDateSuffix, updateDateSuffix, deleteDateSuffix } from '../api/dateSuffixesApi';
import type { WorldPermissionDto } from '../types/worldPermission';
import type { DateSuffixDto } from '../types/dateSuffix';
import { useAuth } from '../context/AuthContext';

interface EraDraft {
  abbreviation: string;
  name: string;
  anchorYear: string;
  scale: string;
  isReversed: boolean;
  isDefault: boolean;
}

const emptyDraft: EraDraft = { abbreviation: '', name: '', anchorYear: '0', scale: '1', isReversed: false, isDefault: false };

export default function WorldSettingsPage() {
  const { worldId } = useParams<{ worldId: string }>();
  const navigate = useNavigate();
  const { userId } = useAuth();

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  // World fields
  const [name, setName] = useState('');
  const [author, setAuthor] = useState('');
  const [description, setDescription] = useState('');
  const [saving, setSaving] = useState(false);
  const [saveSuccess, setSaveSuccess] = useState(false);
  const [saveError, setSaveError] = useState('');

  // Date eras
  const [eras, setEras] = useState<DateSuffixDto[]>([]);
  const [editingEraId, setEditingEraId] = useState<string | 'new' | null>(null);
  const [eraDraft, setEraDraft] = useState<EraDraft>(emptyDraft);
  const [eraSaving, setEraSaving] = useState(false);
  const [eraError, setEraError] = useState('');

  // Permissions
  const [permissions, setPermissions] = useState<WorldPermissionDto[]>([]);
  const [newEmail, setNewEmail] = useState('');
  const [newCanEdit, setNewCanEdit] = useState(false);
  const [addError, setAddError] = useState('');
  const [adding, setAdding] = useState(false);

  // Transfer ownership
  const [transferEmail, setTransferEmail] = useState('');
  const [transferDialogOpen, setTransferDialogOpen] = useState(false);
  const [transferring, setTransferring] = useState(false);
  const [transferError, setTransferError] = useState('');

  // Delete
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [deleting, setDeleting] = useState(false);

  useEffect(() => {
    if (!worldId) return;
    Promise.all([getWorld(worldId), getPermissions(worldId), getDateSuffixesByWorld(worldId)])
      .then(([world, perms, suffixes]) => {
        if (world.createdById !== userId) {
          navigate(`/worlds/${worldId}`, { replace: true });
          return;
        }
        setName(world.name);
        setAuthor(world.author ?? '');
        setDescription(world.description ?? '');
        setPermissions(perms);
        setEras(suffixes);
      })
      .catch(() => setError('Failed to load settings.'))
      .finally(() => setLoading(false));
  }, [worldId, userId, navigate]);

  async function handleSave(e: React.FormEvent) {
    e.preventDefault();
    if (!worldId) return;
    setSaving(true);
    setSaveError('');
    setSaveSuccess(false);
    try {
      await updateWorld(worldId, { name, author: author || undefined, description: description || undefined });
      setSaveSuccess(true);
    } catch {
      setSaveError('Failed to save changes.');
    } finally {
      setSaving(false);
    }
  }

  function startEditEra(era: DateSuffixDto) {
    setEditingEraId(era.id);
    setEraDraft({
      abbreviation: era.abbreviation,
      name: era.name,
      anchorYear: String(era.anchorYear),
      scale: String(era.scale),
      isReversed: era.isReversed,
      isDefault: era.isDefault,
    });
    setEraError('');
  }

  function startAddEra() {
    setEditingEraId('new');
    setEraDraft(emptyDraft);
    setEraError('');
  }

  async function saveEra() {
    if (!worldId) return;
    const anchorYear = parseInt(eraDraft.anchorYear, 10);
    const scale = parseFloat(eraDraft.scale);
    if (!eraDraft.abbreviation.trim() || !eraDraft.name.trim()) {
      setEraError('Suffix and name are required.');
      return;
    }
    if (isNaN(anchorYear)) { setEraError('Anchor year must be an integer.'); return; }
    if (isNaN(scale) || scale <= 0) { setEraError('Scale must be a positive number.'); return; }

    setEraSaving(true);
    setEraError('');
    try {
      if (editingEraId === 'new') {
        const created = await createDateSuffix({
          worldId, abbreviation: eraDraft.abbreviation.trim(), name: eraDraft.name.trim(),
          anchorYear, scale, isReversed: eraDraft.isReversed, isDefault: eraDraft.isDefault,
        });
        setEras(prev => {
          const cleared = eraDraft.isDefault ? prev.map(e => ({ ...e, isDefault: false })) : prev;
          return [...cleared, created].sort((a, b) => a.anchorYear - b.anchorYear);
        });
      } else {
        const updated = await updateDateSuffix(editingEraId!, {
          abbreviation: eraDraft.abbreviation.trim(), name: eraDraft.name.trim(),
          anchorYear, scale, isReversed: eraDraft.isReversed, isDefault: eraDraft.isDefault,
        });
        setEras(prev =>
          prev.map(e => e.id === editingEraId ? updated : (eraDraft.isDefault ? { ...e, isDefault: false } : e))
            .sort((a, b) => a.anchorYear - b.anchorYear)
        );
      }
      setEditingEraId(null);
    } catch {
      setEraError('Failed to save era.');
    } finally {
      setEraSaving(false);
    }
  }

  async function handleDeleteEra(era: DateSuffixDto) {
    if (!worldId) return;
    await deleteDateSuffix(era.id);
    setEras(prev => prev.filter(e => e.id !== era.id));
  }

  async function handleToggleCanEdit(perm: WorldPermissionDto) {
    if (!worldId) return;
    const updated = await upsertPermission(worldId, perm.userId, { canEdit: !perm.canEdit });
    setPermissions(prev => prev.map(p => p.id === perm.id ? updated : p));
  }

  async function handleDelete(perm: WorldPermissionDto) {
    if (!worldId) return;
    await deletePermission(worldId, perm.userId);
    setPermissions(prev => prev.filter(p => p.id !== perm.id));
  }

  async function handleAdd(e: React.FormEvent) {
    e.preventDefault();
    if (!worldId || !newEmail.trim()) return;
    setAddError('');
    setAdding(true);
    try {
      const added = await addPermissionByEmail(worldId, newEmail.trim(), newCanEdit);
      setPermissions(prev => {
        const exists = prev.find(p => p.id === added.id);
        return exists ? prev.map(p => p.id === added.id ? added : p) : [...prev, added];
      });
      setNewEmail('');
      setNewCanEdit(false);
    } catch (err) {
      setAddError(err instanceof NotFoundError ? 'No user found with that email address.' : 'Failed to add user.');
    } finally {
      setAdding(false);
    }
  }

  if (loading) return <Box sx={{ display: 'flex', justifyContent: 'center', mt: 8 }}><CircularProgress /></Box>;
  if (error) return <Box sx={{ maxWidth: 800, mx: 'auto', px: 3, py: 4 }}><Alert severity="error">{error}</Alert></Box>;

  const patchDraft = (patch: Partial<EraDraft>) => setEraDraft(d => ({ ...d, ...patch }));

  function EraEditCells() {
    return (
      <>
        <TableCell sx={{ py: 0.5 }}>
          <TextField size="small" value={eraDraft.abbreviation} onChange={e => patchDraft({ abbreviation: e.target.value })}
            placeholder="e.g. TA" sx={{ width: 80 }} inputProps={{ maxLength: 20 }} />
        </TableCell>
        <TableCell sx={{ py: 0.5 }}>
          <TextField size="small" value={eraDraft.name} onChange={e => patchDraft({ name: e.target.value })}
            placeholder="e.g. Third Age" fullWidth inputProps={{ maxLength: 100 }} />
        </TableCell>
        <TableCell align="right" sx={{ py: 0.5 }}>
          <TextField size="small" type="number" value={eraDraft.anchorYear}
            onChange={e => patchDraft({ anchorYear: e.target.value })}
            sx={{ width: 110 }} inputProps={{ style: { textAlign: 'right' } }} />
        </TableCell>
        <TableCell align="right" sx={{ py: 0.5 }}>
          <TextField size="small" type="number" value={eraDraft.scale}
            onChange={e => patchDraft({ scale: e.target.value })}
            sx={{ width: 90 }} inputProps={{ step: '0.001', min: '0', style: { textAlign: 'right' } }} />
        </TableCell>
        <TableCell align="center" sx={{ py: 0.5 }}>
          <Checkbox size="small" checked={eraDraft.isReversed} onChange={e => patchDraft({ isReversed: e.target.checked })} />
        </TableCell>
        <TableCell align="center" sx={{ py: 0.5 }}>
          <Checkbox size="small" checked={eraDraft.isDefault} onChange={e => patchDraft({ isDefault: e.target.checked })} />
        </TableCell>
        <TableCell align="right" sx={{ py: 0.5 }}>
          <IconButton size="small" onClick={saveEra} disabled={eraSaving}><CheckIcon fontSize="small" /></IconButton>
          <IconButton size="small" onClick={() => { setEditingEraId(null); setEraError(''); }}><CloseIcon fontSize="small" /></IconButton>
        </TableCell>
      </>
    );
  }

  return (
    <Box sx={{ maxWidth: 800, mx: 'auto', px: 3, py: 4 }}>
      <Button startIcon={<ArrowBackIcon />} onClick={() => navigate(`/worlds/${worldId}`)} sx={{ mb: 3 }}>
        Back to {name}
      </Button>

      <Typography variant="h4" fontWeight={700} mb={4}>World Settings</Typography>

      {/* World details */}
      <Typography variant="h6" fontWeight={600} mb={2}>Details</Typography>
      <Box component="form" onSubmit={handleSave} sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
        <TextField label="Name" value={name} onChange={e => setName(e.target.value)} required />
        <TextField label="Author" value={author} onChange={e => setAuthor(e.target.value)} />
        <TextField
          label="Description"
          value={description}
          onChange={e => setDescription(e.target.value)}
          multiline
          minRows={3}
        />
        {saveSuccess && <Alert severity="success" onClose={() => setSaveSuccess(false)}>Changes saved.</Alert>}
        {saveError && <Alert severity="error">{saveError}</Alert>}
        <Box>
          <Button type="submit" variant="contained" disabled={saving}>
            {saving ? 'Saving…' : 'Save changes'}
          </Button>
        </Box>
      </Box>

      <Divider sx={{ my: 5 }} />

      {/* Date eras */}
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={1}>
        <Typography variant="h6" fontWeight={600}>Date eras / suffixes</Typography>
        {editingEraId === null && (
          <Button size="small" startIcon={<AddIcon />} onClick={startAddEra}>Add era</Button>
        )}
      </Box>
      <Typography variant="body2" color="text.secondary" mb={2}>
        Define the eras used in this world. The anchor year is the absolute year at which the era begins; scale converts era-years to absolute years (e.g. 0.5 means 1 era-year = 0.5 absolute years). Reversed means year numbers count down (like BCE).
      </Typography>

      {(eras.length > 0 || editingEraId !== null) && (
        <TableContainer component={Paper} variant="outlined" sx={{ mb: eraError ? 1 : 3 }}>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Suffix</TableCell>
                <TableCell>Name</TableCell>
                <TableCell align="right">Anchor year</TableCell>
                <TableCell align="right">Scale</TableCell>
                <TableCell align="center">Reversed</TableCell>
                <TableCell align="center">Default</TableCell>
                <TableCell align="right" />
              </TableRow>
            </TableHead>
            <TableBody>
              {eras.map(era => (
                <TableRow key={era.id}>
                  {editingEraId === era.id ? (
                    <EraEditCells />
                  ) : (
                    <>
                      <TableCell sx={{ fontFamily: 'monospace', fontWeight: 600 }}>{era.abbreviation}</TableCell>
                      <TableCell>{era.name}</TableCell>
                      <TableCell align="right">{era.anchorYear}</TableCell>
                      <TableCell align="right">{era.scale}</TableCell>
                      <TableCell align="center">
                        {era.isReversed && <CheckIcon sx={{ fontSize: 16, color: 'text.secondary' }} />}
                      </TableCell>
                      <TableCell align="center">
                        {era.isDefault && <CheckIcon sx={{ fontSize: 16, color: 'primary.main' }} />}
                      </TableCell>
                      <TableCell align="right">
                        <IconButton size="small" onClick={() => startEditEra(era)} disabled={editingEraId !== null}>
                          <EditIcon fontSize="small" />
                        </IconButton>
                        <IconButton size="small" onClick={() => handleDeleteEra(era)} disabled={editingEraId !== null}>
                          <DeleteIcon fontSize="small" />
                        </IconButton>
                      </TableCell>
                    </>
                  )}
                </TableRow>
              ))}
              {editingEraId === 'new' && (
                <TableRow>
                  <EraEditCells />
                </TableRow>
              )}
            </TableBody>
          </Table>
        </TableContainer>
      )}

      {eraError && <Alert severity="error" sx={{ mb: 3 }}>{eraError}</Alert>}

      {eras.length === 0 && editingEraId === null && (
        <Typography color="text.secondary" variant="body2" mb={3}>No eras defined yet.</Typography>
      )}

      <Divider sx={{ my: 5 }} />

      {/* Permissions */}
      <Typography variant="h6" fontWeight={600} mb={2}>Shared with</Typography>

      {permissions.length === 0 ? (
        <Typography color="text.secondary" mb={3}>This world is not shared with anyone yet.</Typography>
      ) : (
        <TableContainer component={Paper} variant="outlined" sx={{ mb: 4 }}>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>User</TableCell>
                <TableCell>User ID</TableCell>
                <TableCell align="center">Can edit</TableCell>
                <TableCell align="right" />
              </TableRow>
            </TableHead>
            <TableBody>
              {permissions.map(perm => (
                <TableRow key={perm.id}>
                  <TableCell>{perm.username ?? '—'}</TableCell>
                  <TableCell sx={{ fontFamily: 'monospace', fontSize: '0.75rem', color: 'text.secondary' }}>
                    {perm.userId}
                  </TableCell>
                  <TableCell align="center">
                    <Switch checked={perm.canEdit} onChange={() => handleToggleCanEdit(perm)} size="small" />
                  </TableCell>
                  <TableCell align="right">
                    <IconButton size="small" onClick={() => handleDelete(perm)}>
                      <DeleteIcon fontSize="small" />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}

      <Typography variant="h6" fontWeight={600} mb={2}>Add user</Typography>
      <Box component="form" onSubmit={handleAdd} sx={{ display: 'flex', alignItems: 'center', gap: 2, flexWrap: 'wrap' }}>
        <TextField
          label="User ID"
          value={newEmail}
          onChange={e => setNewEmail(e.target.value)}
          size="small"
          sx={{ flexGrow: 1, minWidth: 280 }}
          required
        />
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
          <Typography variant="body2">Can edit</Typography>
          <Switch checked={newCanEdit} onChange={e => setNewCanEdit(e.target.checked)} size="small" />
        </Box>
        <Button type="submit" variant="contained" disabled={adding}>Add</Button>
      </Box>
      {addError && <Alert severity="error" sx={{ mt: 2 }}>{addError}</Alert>}

      <Divider sx={{ my: 5 }} />

      {/* Danger zone */}
      <Paper variant="outlined" sx={{ borderColor: 'error.main', borderRadius: 1 }}>
        <Box sx={{ px: 2, py: 1.5, borderBottom: '1px solid', borderColor: 'error.main' }}>
          <Typography variant="subtitle1" fontWeight={600} color="error">Danger zone</Typography>
        </Box>
        <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', gap: 2, px: 2, py: 2, flexWrap: 'wrap' }}>
          <Box>
            <Typography variant="body2" fontWeight={600}>Change creator</Typography>
            <Typography variant="body2" color="text.secondary">
              Transfer ownership of this world to another user.
            </Typography>
          </Box>
          <Button variant="outlined" color="error" size="small" sx={{ whiteSpace: 'nowrap', flexShrink: 0 }} onClick={() => { setTransferError(''); setTransferDialogOpen(true); }}>
            Change creator
          </Button>
        </Box>
        <Divider />
        <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', gap: 2, px: 2, py: 2, flexWrap: 'wrap' }}>
          <Box>
            <Typography variant="body2" fontWeight={600}>Delete this world</Typography>
            <Typography variant="body2" color="text.secondary">
              Once you delete a world, there is no going back. Please be certain.
            </Typography>
          </Box>
          <Button variant="outlined" color="error" size="small" sx={{ whiteSpace: 'nowrap', flexShrink: 0 }} onClick={() => setDeleteDialogOpen(true)}>
            Delete this world
          </Button>
        </Box>
      </Paper>

      <Dialog open={transferDialogOpen} onClose={() => !transferring && setTransferDialogOpen(false)} fullWidth maxWidth="xs">
        <DialogTitle>Change creator of "{name}"</DialogTitle>
        <DialogContent sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: '16px !important' }}>
          <DialogContentText>
            Enter the email address of the new creator. You will lose ownership of this world.
          </DialogContentText>
          <TextField
            label="Email address"
            type="email"
            value={transferEmail}
            onChange={e => { setTransferEmail(e.target.value); setTransferError(''); }}
            size="small"
            autoFocus
            disabled={transferring}
          />
          {transferError && <Alert severity="error">{transferError}</Alert>}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setTransferDialogOpen(false)} disabled={transferring}>Cancel</Button>
          <Button
            color="error"
            variant="contained"
            disabled={transferring || !transferEmail.trim()}
            onClick={async () => {
              if (!worldId) return;
              setTransferring(true);
              setTransferError('');
              try {
                const found = await transferOwnership(worldId, transferEmail.trim());
                if (!found) {
                  setTransferError('No user found with that email address.');
                } else {
                  setTransferDialogOpen(false);
                  setTransferEmail('');
                  navigate('/', { replace: true });
                }
              } catch {
                setTransferError('Something went wrong. Please try again.');
              } finally {
                setTransferring(false);
              }
            }}
          >
            {transferring ? 'Transferring…' : 'Change creator'}
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog open={deleteDialogOpen} onClose={() => !deleting && setDeleteDialogOpen(false)}>
        <DialogTitle>Delete "{name}"?</DialogTitle>
        <DialogContent>
          <DialogContentText>
            This will permanently delete the world and everything in it — all entities, family trees, and notes. This action cannot be undone.
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteDialogOpen(false)} disabled={deleting}>Cancel</Button>
          <Button
            color="error"
            variant="contained"
            disabled={deleting}
            onClick={async () => {
              if (!worldId) return;
              setDeleting(true);
              try {
                await deleteWorld(worldId);
                navigate('/', { replace: true });
              } catch {
                setDeleting(false);
                setDeleteDialogOpen(false);
              }
            }}
          >
            {deleting ? 'Deleting…' : 'Delete'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
