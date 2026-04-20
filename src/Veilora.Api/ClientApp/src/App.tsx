import { useState, useMemo } from 'react';
import { Outlet, useNavigate } from 'react-router-dom';
import AppBar from '@mui/material/AppBar';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import CssBaseline from '@mui/material/CssBaseline';
import Divider from '@mui/material/Divider';
import Drawer from '@mui/material/Drawer';
import IconButton from '@mui/material/IconButton';
import List from '@mui/material/List';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import Toolbar from '@mui/material/Toolbar';
import Typography from '@mui/material/Typography';
import HomeIcon from '@mui/icons-material/Home';
import MenuBookIcon from '@mui/icons-material/MenuBook';
import MenuIcon from '@mui/icons-material/Menu';
import EditIcon from '@mui/icons-material/Edit';
import VisibilityIcon from '@mui/icons-material/Visibility';
import DarkModeIcon from '@mui/icons-material/DarkMode';
import LightModeIcon from '@mui/icons-material/LightMode';
import Badge from '@mui/material/Badge';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import { useAuth } from './context/AuthContext';
import { useEditMode } from './context/EditModeContext';
import { useThemeMode } from './context/ThemeModeContext';
import { ReadingSessionProvider, useReadingSession } from './context/ReadingSessionContext';
import ReadingFab from './components/ReadingFab/ReadingFab';

function NotesButton() {
  const { session } = useReadingSession();
  const navigate = useNavigate();
  const count = session?.noteCount ?? 0;

  return (
    <Button
      color="inherit"
      onClick={() => navigate('/reading')}
      sx={{ mr: 1, textTransform: 'none', pr: 2 }}
    >
      <Badge
        badgeContent={count || undefined}
        color="error"
        max={99}
        sx={{ '& .MuiBadge-badge': { right: -10 } }}
      >
        Notes
      </Badge>
    </Button>
  );
}

export default function App() {
  const { mode, toggleThemeMode } = useThemeMode();
  const theme = useMemo(() => createTheme({ palette: { mode } }), [mode]);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const navigate = useNavigate();
  const { logout } = useAuth();
  const { isEditMode, toggleEditMode } = useEditMode();

  return (
    <ReadingSessionProvider>
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <AppBar position="sticky">
        <Toolbar>
          <IconButton
            color="inherit"
            edge="start"
            onClick={() => setDrawerOpen(true)}
            sx={{ mr: 2 }}
          >
            <MenuIcon />
          </IconButton>
          <Typography
            variant="h6"
            component="div"
            sx={{ flexGrow: 1 }}
          >
            <Box
              component="span"
              onClick={() => navigate('/')}
              sx={{ cursor: 'pointer', '&:hover': { opacity: 0.85 } }}
            >
              Veilora
            </Box>
          </Typography>
          <NotesButton />
          <Button
            color="inherit"
            variant={isEditMode ? 'outlined' : 'text'}
            startIcon={isEditMode ? <VisibilityIcon /> : <EditIcon />}
            onClick={toggleEditMode}
            sx={{
              mr: 1,
              borderColor: 'rgba(255,255,255,0.5)',
              ...(isEditMode && {
                bgcolor: 'rgba(255,255,255,0.15)',
                '&:hover': { bgcolor: 'rgba(255,255,255,0.25)' },
              }),
            }}
          >
            {isEditMode ? 'View Mode' : 'Edit'}
          </Button>
          <IconButton color="inherit" onClick={toggleThemeMode} sx={{ mr: 1 }}>
            {mode === 'dark' ? <LightModeIcon /> : <DarkModeIcon />}
          </IconButton>
          <Button color="inherit" onClick={() => { logout(); navigate('/login', { replace: true }); }}>
            Logout
          </Button>
        </Toolbar>
      </AppBar>

      <Drawer anchor="left" open={drawerOpen} onClose={() => setDrawerOpen(false)}>
        <Toolbar>
          <Typography variant="h6">Veilora</Typography>
        </Toolbar>
        <Divider />
        <List sx={{ width: 220 }}>
          <ListItemButton onClick={() => { navigate('/'); setDrawerOpen(false); }}>
            <ListItemIcon><HomeIcon /></ListItemIcon>
            <ListItemText primary="Worlds" />
          </ListItemButton>
          <ListItemButton onClick={() => { navigate('/reading'); setDrawerOpen(false); }}>
            <ListItemIcon><MenuBookIcon /></ListItemIcon>
            <ListItemText primary="Reading" />
          </ListItemButton>
        </List>
      </Drawer>

      <Box component="main">
        <Outlet />
      </Box>
      <ReadingFab />
    </ThemeProvider>
    </ReadingSessionProvider>
  );
}
