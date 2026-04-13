import { useState } from 'react';
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
import MenuIcon from '@mui/icons-material/Menu';
import EditIcon from '@mui/icons-material/Edit';
import VisibilityIcon from '@mui/icons-material/Visibility';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import { useAuth } from './context/AuthContext';
import { useEditMode } from './context/EditModeContext';

const theme = createTheme();

export default function App() {
  const [drawerOpen, setDrawerOpen] = useState(false);
  const navigate = useNavigate();
  const { logout } = useAuth();
  const { isEditMode, toggleEditMode } = useEditMode();

  return (
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
          <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
            Lorekeeper
          </Typography>
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
          <Button color="inherit" onClick={() => { logout(); navigate('/login', { replace: true }); }}>
            Logout
          </Button>
        </Toolbar>
      </AppBar>

      <Drawer anchor="left" open={drawerOpen} onClose={() => setDrawerOpen(false)}>
        <Toolbar>
          <Typography variant="h6">Lorekeeper</Typography>
        </Toolbar>
        <Divider />
        <List sx={{ width: 220 }}>
          <ListItemButton onClick={() => { navigate('/'); setDrawerOpen(false); }}>
            <ListItemIcon><HomeIcon /></ListItemIcon>
            <ListItemText primary="Worlds" />
          </ListItemButton>
        </List>
      </Drawer>

      <Box component="main">
        <Outlet />
      </Box>
    </ThemeProvider>
  );
}
