import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Box, Typography, Button, TextField, Divider, Alert,
  FormControlLabel, Switch,
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { useAuth } from '../context/AuthContext';
import { useEditMode } from '../context/EditModeContext';
import { useThemeMode } from '../context/ThemeModeContext';
import { updateDisplayName, changePassword } from '../api/usersApi';

export default function UserSettingsPage() {
  const navigate = useNavigate();
  const { email, displayName, updateDisplayName: setCtxDisplayName } = useAuth();
  const { isEditMode, toggleEditMode } = useEditMode();
  const { mode, toggleThemeMode } = useThemeMode();

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
    if (newPassword !== confirmPassword) {
      setPasswordError('New passwords do not match.');
      return;
    }
    if (newPassword.length < 8) {
      setPasswordError('New password must be at least 8 characters.');
      return;
    }
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
      <TextField
        label="Email"
        value={email ?? ''}
        fullWidth
        disabled
        sx={{ mb: 3 }}
      />
      <TextField
        label="Display name"
        value={newDisplayName}
        onChange={e => { setNewDisplayName(e.target.value); setDisplayNameSuccess(false); }}
        fullWidth
        sx={{ mb: 1 }}
      />
      {displayNameError && <Alert severity="error" sx={{ mb: 1 }}>{displayNameError}</Alert>}
      {displayNameSuccess && <Alert severity="success" sx={{ mb: 1 }}>Display name updated.</Alert>}
      <Button
        variant="contained"
        onClick={handleDisplayNameSave}
        disabled={displayNameSaving}
        sx={{ mb: 4 }}
      >
        Save display name
      </Button>

      <Divider sx={{ mb: 4 }} />

      {/* Change password */}
      <Typography variant="h6" fontWeight={600} mb={2}>Change Password</Typography>
      <Box display="flex" flexDirection="column" gap={2}>
        <TextField
          label="Current password"
          type="password"
          value={currentPassword}
          onChange={e => { setCurrentPassword(e.target.value); setPasswordSuccess(false); }}
          fullWidth
        />
        <TextField
          label="New password"
          type="password"
          value={newPassword}
          onChange={e => { setNewPassword(e.target.value); setPasswordSuccess(false); }}
          fullWidth
        />
        <TextField
          label="Confirm new password"
          type="password"
          value={confirmPassword}
          onChange={e => { setConfirmPassword(e.target.value); setPasswordSuccess(false); }}
          fullWidth
        />
        {passwordError && <Alert severity="error">{passwordError}</Alert>}
        {passwordSuccess && <Alert severity="success">Password changed successfully.</Alert>}
        <Button
          variant="contained"
          onClick={handlePasswordChange}
          disabled={passwordSaving || !currentPassword || !newPassword || !confirmPassword}
        >
          Change password
        </Button>
      </Box>
    </Box>
  );
}
