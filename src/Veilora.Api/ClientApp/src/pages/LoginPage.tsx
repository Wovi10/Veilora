import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Paper from '@mui/material/Paper';
import TextField from '@mui/material/TextField';
import Typography from '@mui/material/Typography';
import Link from '@mui/material/Link';
import { login as loginApi, register as registerApi } from '../api/authApi';
import { useAuth } from '../context/AuthContext';

export default function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const [mode, setMode] = useState<'login' | 'register'>('login');
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [displayName, setDisplayName] = useState('');
  const [usernameOrEmail, setUsernameOrEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      if (mode === 'register') {
        const res = await registerApi({ username, email, password, displayName: displayName || undefined });
        login(res.token, res.id, res.email, res.displayName ?? null);
      } else {
        const res = await loginApi({ usernameOrEmail, password });
        login(res.token, res.id, res.email, res.displayName ?? null);
      }
      navigate('/', { replace: true });
    } catch {
      setError(mode === 'register' ? 'Registration failed. Username or email may already be taken.' : 'Invalid username/email or password.');
    } finally {
      setLoading(false);
    }
  }

  function toggleMode() {
    setError('');
    setMode(m => m === 'login' ? 'register' : 'login');
  }

  return (
    <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '100vh' }}>
      <Paper sx={{ p: 4, width: 360 }}>
        <Typography variant="h5" gutterBottom>Veilora</Typography>
        <Box component="form" onSubmit={handleSubmit} sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 1 }}>
          {mode === 'register' ? (
            <>
              <TextField label="Username" value={username} onChange={e => setUsername(e.target.value)} required autoFocus />
              <TextField label="Email" type="email" value={email} onChange={e => setEmail(e.target.value)} required />
              <TextField label="Display name (optional)" value={displayName} onChange={e => setDisplayName(e.target.value)} />
            </>
          ) : (
            <TextField label="Username or email" value={usernameOrEmail} onChange={e => setUsernameOrEmail(e.target.value)} required autoFocus />
          )}
          <TextField label="Password" type="password" value={password} onChange={e => setPassword(e.target.value)} required />
          {error && <Typography color="error" variant="body2">{error}</Typography>}
          <Button type="submit" variant="contained" disabled={loading}>
            {loading ? (mode === 'register' ? 'Registering…' : 'Logging in…') : (mode === 'register' ? 'Register' : 'Log in')}
          </Button>
          <Typography variant="body2" textAlign="center">
            {mode === 'login' ? "Don't have an account? " : 'Already have an account? '}
            <Link component="button" type="button" onClick={toggleMode}>
              {mode === 'login' ? 'Register' : 'Log in'}
            </Link>
          </Typography>
        </Box>
      </Paper>
    </Box>
  );
}
