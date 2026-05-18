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
import SettingsIcon from '@mui/icons-material/Settings';
import EditIcon from '@mui/icons-material/Edit';
import VisibilityIcon from '@mui/icons-material/Visibility';
import DarkModeIcon from '@mui/icons-material/DarkMode';
import LightModeIcon from '@mui/icons-material/LightMode';
import LogoutIcon from '@mui/icons-material/Logout';
import Badge from '@mui/material/Badge';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import { useAuth } from './context/AuthContext';
import { useEditMode } from './context/EditModeContext';
import { useThemeMode } from './context/ThemeModeContext';
import { ReadingSessionProvider, useReadingSession } from './context/ReadingSessionContext';
import ReadingFab from './components/ReadingFab/ReadingFab';
import { useIsMobile } from './hooks/useBreakpoint';

interface DrawerContentProps {
  navigate: ReturnType<typeof useNavigate>;
  setDrawerOpen: (open: boolean) => void;
  isMobile: boolean;
  isEditMode: boolean;
  toggleEditMode: () => void;
  mode: 'light' | 'dark';
  toggleThemeMode: () => void;
  handleLogout: () => void;
}

function DrawerContent({
  navigate, setDrawerOpen, isMobile, isEditMode, toggleEditMode, mode, toggleThemeMode, handleLogout,
}: DrawerContentProps) {
  return (
    <Box sx={{ width: 220, display: 'flex', flexDirection: 'column', height: '100%' }}>
      <Toolbar>
        <Typography variant="h6">Veilora</Typography>
      </Toolbar>
      <Divider />
      <List>
        <ListItemButton onClick={() => { navigate('/'); setDrawerOpen(false); }}>
          <ListItemIcon><HomeIcon /></ListItemIcon>
          <ListItemText primary="Worlds" />
        </ListItemButton>
        <ListItemButton onClick={() => { navigate('/reading'); setDrawerOpen(false); }}>
          <ListItemIcon><MenuBookIcon /></ListItemIcon>
          <ListItemText primary="Reading" />
        </ListItemButton>
      </List>
      <Box sx={{ flexGrow: 1 }} />
      <Divider />
      <List>
        <ListItemButton onClick={() => { navigate('/settings'); setDrawerOpen(false); }}>
          <ListItemIcon><SettingsIcon /></ListItemIcon>
          <ListItemText primary="User Settings" />
        </ListItemButton>
        {isMobile && (
          <>
            <Divider />
            <ListItemButton onClick={() => { toggleEditMode(); setDrawerOpen(false); }}>
              <ListItemIcon>{isEditMode ? <VisibilityIcon /> : <EditIcon />}</ListItemIcon>
              <ListItemText primary={isEditMode ? 'View Mode' : 'Edit Mode'} />
            </ListItemButton>
            <ListItemButton onClick={() => { toggleThemeMode(); setDrawerOpen(false); }}>
              <ListItemIcon>{mode === 'dark' ? <LightModeIcon /> : <DarkModeIcon />}</ListItemIcon>
              <ListItemText primary={mode === 'dark' ? 'Light Mode' : 'Dark Mode'} />
            </ListItemButton>
            <ListItemButton onClick={handleLogout}>
              <ListItemIcon><LogoutIcon /></ListItemIcon>
              <ListItemText primary="Logout" />
            </ListItemButton>
          </>
        )}
      </List>
    </Box>
  );
}

function LogoutButton() {
  const { session, pause } = useReadingSession();
  const { logout } = useAuth();
  const navigate = useNavigate();

  async function handleLogout() {
    if (session?.isActive) await pause();
    logout();
    navigate('/login', { replace: true });
  }

  return (
    <Button color="inherit" onClick={handleLogout} sx={{ display: { xs: 'none', sm: 'flex' } }}>Logout</Button>
  );
}

interface NotesButtonProps {
  iconOnly?: boolean;
}

function NotesButton({ iconOnly }: NotesButtonProps) {
  const { session } = useReadingSession();
  const navigate = useNavigate();
  const count = session?.noteCount ?? 0;

  if (iconOnly) {
    return (
      <IconButton color="inherit" onClick={() => navigate('/reading')}>
        <Badge
          badgeContent={count || undefined}
          color="error"
          max={99}
          sx={{ '& .MuiBadge-badge': { right: -10 } }}
        >
          <MenuBookIcon />
        </Badge>
      </IconButton>
    );
  }

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
  const { isEditMode, toggleEditMode } = useEditMode();
  const isMobile = useIsMobile();

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
          <NotesButton iconOnly={isMobile} />
          <Button
            color="inherit"
            variant={isEditMode ? 'outlined' : 'text'}
            startIcon={isEditMode ? <VisibilityIcon /> : <EditIcon />}
            onClick={toggleEditMode}
            sx={{
              mr: 1,
              display: { xs: 'none', sm: 'flex' },
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
          <LogoutButton />
        </Toolbar>
      </AppBar>

      <Drawer anchor="left" open={drawerOpen} onClose={() => setDrawerOpen(false)}>
        <DrawerContentWithLogout
          navigate={navigate}
          setDrawerOpen={setDrawerOpen}
          isMobile={isMobile}
          isEditMode={isEditMode}
          toggleEditMode={toggleEditMode}
          mode={mode}
          toggleThemeMode={toggleThemeMode}
        />
      </Drawer>

      <Box component="main">
        <Outlet />
      </Box>
      <ReadingFab />
    </ThemeProvider>
    </ReadingSessionProvider>
  );
}

function DrawerContentWithLogout({
  navigate, setDrawerOpen, isMobile, isEditMode, toggleEditMode, mode, toggleThemeMode,
}: Omit<DrawerContentProps, 'handleLogout'>) {
  const { session, pause } = useReadingSession();
  const { logout } = useAuth();

  async function handleLogout() {
    if (session?.isActive) await pause();
    logout();
    navigate('/login', { replace: true });
    setDrawerOpen(false);
  }

  return (
    <DrawerContent
      navigate={navigate}
      setDrawerOpen={setDrawerOpen}
      isMobile={isMobile}
      isEditMode={isEditMode}
      toggleEditMode={toggleEditMode}
      mode={mode}
      toggleThemeMode={toggleThemeMode}
      handleLogout={handleLogout}
    />
  );
}
