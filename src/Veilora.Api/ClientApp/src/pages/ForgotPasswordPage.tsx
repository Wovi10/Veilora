import { useState } from 'react';
import { Link as RouterLink } from 'react-router-dom';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Paper from '@mui/material/Paper';
import TextField from '@mui/material/TextField';
import Typography from '@mui/material/Typography';
import Link from '@mui/material/Link';
import { forgotPassword } from '../api/authApi';

export default function ForgotPasswordPage() {
  const [email, setEmail] = useState('');
  const [submitted, setSubmitted] = useState(false);
  const [loading, setLoading] = useState(false);

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setLoading(true);
    try {
      await forgotPassword({ email });
    } finally {
      setLoading(false);
      setSubmitted(true);
    }
  }

  return (
    <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '100vh' }}>
      <Paper sx={{ p: 4, width: { xs: '100%', sm: 360 }, mx: { xs: 1, sm: 'auto' } }}>
        <Typography variant="h5" gutterBottom>Forgot password</Typography>
        {submitted ? (
          <>
            <Typography variant="body2" sx={{ mt: 2 }}>
              If an account exists for that email, we&apos;ve sent a link to reset your password.
            </Typography>
            <Typography variant="body2" textAlign="center" sx={{ mt: 3 }}>
              <Link component={RouterLink} to="/login">Back to login</Link>
            </Typography>
          </>
        ) : (
          <Box component="form" onSubmit={handleSubmit} sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 1 }}>
            <Typography variant="body2">
              Enter your account email and we&apos;ll send you a link to reset your password.
            </Typography>
            <TextField label="Email" type="email" value={email} onChange={e => setEmail(e.target.value)} required autoFocus />
            <Button type="submit" variant="contained" disabled={loading}>
              {loading ? 'Sending…' : 'Send reset link'}
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
