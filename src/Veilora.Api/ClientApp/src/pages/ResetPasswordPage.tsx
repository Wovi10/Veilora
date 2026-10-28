import { useState } from 'react';
import { Link as RouterLink, useNavigate, useSearchParams } from 'react-router-dom';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Paper from '@mui/material/Paper';
import TextField from '@mui/material/TextField';
import Typography from '@mui/material/Typography';
import Link from '@mui/material/Link';
import { resetPassword } from '../api/authApi';

export default function ResetPasswordPage() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const token = searchParams.get('token') ?? '';
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError('');

    if (newPassword !== confirmPassword) {
      setError('Passwords do not match.');
      return;
    }
    if (newPassword.length < 8) {
      setError('Password must be at least 8 characters.');
      return;
    }

    setLoading(true);
    try {
      await resetPassword({ token, newPassword });
      navigate('/login', { replace: true });
    } catch {
      setError('This reset link is invalid or has expired. Please request a new one.');
    } finally {
      setLoading(false);
    }
  }

  return (
    <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '100vh' }}>
      <Paper sx={{ p: 4, width: { xs: '100%', sm: 360 }, mx: { xs: 1, sm: 'auto' } }}>
        <Typography variant="h5" gutterBottom>Reset password</Typography>
        {!token ? (
          <>
            <Typography color="error" variant="body2" sx={{ mt: 2 }}>
              This reset link is missing its token. Please request a new one.
            </Typography>
            <Typography variant="body2" textAlign="center" sx={{ mt: 3 }}>
              <Link component={RouterLink} to="/forgot-password">Request a new link</Link>
            </Typography>
          </>
        ) : (
          <Box component="form" onSubmit={handleSubmit} sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 1 }}>
            <TextField label="New password" type="password" value={newPassword} onChange={e => setNewPassword(e.target.value)} required autoFocus />
            <TextField label="Confirm new password" type="password" value={confirmPassword} onChange={e => setConfirmPassword(e.target.value)} required />
            {error && <Typography color="error" variant="body2">{error}</Typography>}
            <Button type="submit" variant="contained" disabled={loading}>
              {loading ? 'Resetting…' : 'Reset password'}
            </Button>
            <Typography variant="body2" textAlign="center">
              <Link component={RouterLink} to="/login">Back to login</Link>
            </Typography>
          </Box>
        )}
      </Paper>
    </Box>
  );
}
