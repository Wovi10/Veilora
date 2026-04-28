import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Box, Typography, Button, TextField, Divider, Alert,
  FormControlLabel, Switch, CircularProgress,
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { useAuth } from '../context/AuthContext';
import { useEditMode } from '../context/EditModeContext';
import { useThemeMode } from '../context/ThemeModeContext';
import {
  getMe, updateDisplayName, changePassword,
  setBackupUser, removeBackupUser, deleteAccount,
  type UserMeDto,
} from '../api/usersApi';

export default function UserSettingsPage() {
  const navigate = useNavigate();
  const { email, displayName, updateDisplayName: setCtxDisplayName, logout } = useAuth();
  const { isEditMode, toggleEditMode } = useEditMode();
  const { mode, toggleThemeMode } = useThemeMode();

  const [me, setMe] = useState<UserMeDto | null>(null);

  const [newDisplayName, setNewDisplayName] = useState(displayName ?? '');
  const [displayNameSaving, setDisplayNameSaving] = useState(false);
  const [displayNameSuccess, setDisplayNameSuccess] = useState(false);
  const [displayNameError, setDisplayNameError] = useState('');

  const [currentPassword, setCurrentPassword] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [passwordSaving, setPasswordSaving] = useState(false);
  const [passwordSuccess, setPasswordSuccess] = useState(false);
  const [passwordError, setPasswordError] = useState('');

  const [backupEmail, setBackupEmail] = useState('');
  const [backupSaving, setBackupSaving] = useState(false);
  const [backupSuccess, setBackupSuccess] = useState(false);
  const [backupError, setBackupError] = useState('');

  const [confirmDelete, setConfirmDelete] = useState(false);
  const [deleting, setDeleting] = useState(false);
  const [deleteError, setDeleteError] = useState('');

  useEffect(() => {
    getMe().then(data => {
      setMe(data);
      setBackupEmail(data.backupUserEmail ?? '');
    });
  }, []);

  const handleDisplayNameSave = async () => {
    setDisplayNameSaving(true);
    setDisplayNameSuccess(false);
    setDisplayNameError('');
    try {
      const updated = await updateDisplayName(newDisplayName.trim() || null);
      setCtxDisplayName(updated.displayName);
      setDisplayNameSuccess(true);
    } catch {
      setDisplayNameError('Failed to update display name.');
    } finally {
      setDisplayNameSaving(false);
    }
  };

  const handlePasswordChange = async () => {
    if (newPassword !== confirmPassword) { setPasswordError('New passwords do not match.'); return; }
    if (newPassword.length < 8) { setPasswordError('New password must be at least 8 characters.'); return; }
    setPasswordSaving(true);
    setPasswordSuccess(false);
    setPasswordError('');
    try {
      await changePassword(currentPassword, newPassword);
      setCurrentPassword('');
      setNewPassword('');
      setConfirmPassword('');
      setPasswordSuccess(true);
    } catch {
      setPasswordError('Failed to change password. Check that your current password is correct.');
    } finally {
      setPasswordSaving(false);
    }
  };

  const handleSetBackup = async () => {
    setBackupSaving(true);
    setBackupSuccess(false);
    setBackupError('');
    try {
      const updated = await setBackupUser(backupEmail.trim());
      setMe(updated);
      setBackupSuccess(true);
    } catch {
      setBackupError('No user found with that email address.');
    } finally {
      setBackupSaving(false);
    }
  };

  const handleRemoveBackup = async () => {
    setBackupSaving(true);
    setBackupSuccess(false);
    setBackupError('');
    try {
      const updated = await removeBackupUser();
      setMe(updated);
      setBackupEmail('');
    } catch {
      setBackupError('Failed to remove backup user.');
    } finally {
      setBackupSaving(false);
    }
  };

  const handleDeleteAccount = async () => {
    setDeleting(true);
    setDeleteError('');
    try {
      await deleteAccount();
      logout();
      navigate('/login', { replace: true });
    } catch {
      setDeleteError('Failed to delete account.');
      setDeleting(false);
    }
  };

  return (
    <Box sx={{ maxWidth: 600, mx: 'auto', px: 3, py: 4 }}>
      <Button startIcon={<ArrowBackIcon />} onClick={() => navigate('/')} sx={{ mb: 3 }}>
        Home
      </Button>

      <Typography variant="h4" fontWeight={700} mb={4}>
        User Settings
      </Typography>

      {/* Preferences */}
      <Typography variant="h6" fontWeight={600} mb={2}>Preferences</Typography>
      <Box display="flex" flexDirection="column" gap={1} mb={4}>
        <FormControlLabel
          control={<Switch checked={isEditMode} onChange={toggleEditMode} />}
          label="Edit mode"
        />
        <FormControlLabel
          control={<Switch checked={mode === 'dark'} onChange={toggleThemeMode} />}
          label="Dark mode"
        />
      </Box>

      <Divider sx={{ mb: 4 }} />

      {/* Account */}
      <Typography variant="h6" fontWeight={600} mb={2}>Account</Typography>
      <TextField label="Email" value={email ?? ''} fullWidth disabled sx={{ mb: 3 }} />
      <TextField
        label="Display name"
        value={newDisplayName}
        onChange={e => { setNewDisplayName(e.target.value); setDisplayNameSuccess(false); }}
        fullWidth
        sx={{ mb: 1 }}
      />
      {displayNameError && <Alert severity="error" sx={{ mb: 1 }}>{displayNameError}</Alert>}
      {displayNameSuccess && <Alert severity="success" sx={{ mb: 1 }}>Display name updated.</Alert>}
      <Button variant="contained" onClick={handleDisplayNameSave} disabled={displayNameSaving} sx={{ mb: 4 }}>
        Save display name
      </Button>

      <Divider sx={{ mb: 4 }} />

      {/* Change password */}
      <Typography variant="h6" fontWeight={600} mb={2}>Change Password</Typography>
      <Box display="flex" flexDirection="column" gap={2} mb={4}>
        <TextField label="Current password" type="password" value={currentPassword} onChange={e => { setCurrentPassword(e.target.value); setPasswordSuccess(false); }} fullWidth />
        <TextField label="New password" type="password" value={newPassword} onChange={e => { setNewPassword(e.target.value); setPasswordSuccess(false); }} fullWidth />
        <TextField label="Confirm new password" type="password" value={confirmPassword} onChange={e => { setConfirmPassword(e.target.value); setPasswordSuccess(false); }} fullWidth />
        {passwordError && <Alert severity="error">{passwordError}</Alert>}
        {passwordSuccess && <Alert severity="success">Password changed successfully.</Alert>}
        <Button variant="contained" onClick={handlePasswordChange} disabled={passwordSaving || !currentPassword || !newPassword || !confirmPassword}>
          Change password
        </Button>
      </Box>

      <Divider sx={{ mb: 4 }} />

      {/* Backup user */}
      <Typography variant="h6" fontWeight={600} mb={1}>Backup Account</Typography>
      <Typography variant="body2" color="text.secondary" mb={2}>
        If you delete your account, all your worlds and content will be transferred to this user.
      </Typography>
      {me === null ? (
        <CircularProgress size={20} />
      ) : (
        <>
          {me.backupUserEmail && (
            <Alert severity="info" sx={{ mb: 2 }}>
              Current backup: <strong>{me.backupUserDisplayName ?? me.backupUserEmail}</strong> ({me.backupUserEmail})
            </Alert>
          )}
          <Box display="flex" gap={1} alignItems="flex-start" mb={1}>
            <TextField
              label="Backup user email"
              value={backupEmail}
              onChange={e => { setBackupEmail(e.target.value); setBackupSuccess(false); setBackupError(''); }}
              size="small"
              sx={{ flex: 1 }}
            />
            <Button variant="contained" onClick={handleSetBackup} disabled={backupSaving || !backupEmail.trim()}>
              Save
            </Button>
            {me.backupUserEmail && (
              <Button color="error" onClick={handleRemoveBackup} disabled={backupSaving}>
                Remove
              </Button>
            )}
          </Box>
          {backupError && <Alert severity="error" sx={{ mb: 1 }}>{backupError}</Alert>}
          {backupSuccess && <Alert severity="success" sx={{ mb: 1 }}>Backup user saved.</Alert>}
        </>
      )}

      <Divider sx={{ mt: 4, mb: 4 }} />

      {/* Danger zone */}
      <Typography variant="h6" fontWeight={600} color="error" mb={1}>Danger Zone</Typography>
      <Typography variant="body2" color="text.secondary" mb={2}>
        Deleting your account is permanent.{' '}
        {me?.backupUserEmail
          ? 'Your worlds will be transferred to your backup user.'
          : 'All your worlds and content will be permanently deleted.'}
      </Typography>
      {deleteError && <Alert severity="error" sx={{ mb: 2 }}>{deleteError}</Alert>}
      {!confirmDelete ? (
        <Button color="error" variant="outlined" onClick={() => setConfirmDelete(true)}>
          Delete account
        </Button>
      ) : (
        <Box display="flex" alignItems="center" gap={1}>
          <Typography variant="body2">Are you sure?</Typography>
          <Button color="error" variant="contained" onClick={handleDeleteAccount} disabled={deleting} size="small">
            {deleting ? 'Deleting…' : 'Yes, delete my account'}
          </Button>
          <Button onClick={() => setConfirmDelete(false)} disabled={deleting} size="small">Cancel</Button>
        </Box>
      )}
    </Box>
  );
}
