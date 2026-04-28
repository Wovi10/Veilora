import { createBrowserRouter, Navigate, type RouteObject } from 'react-router-dom';
import App from './App';
import { getToken } from './api/apiFetch';
import HomePage from './pages/HomePage';
import LoginPage from './pages/LoginPage';
import WorldPage from './pages/WorldPage';
import WorldSettingsPage from './pages/WorldSettingsPage';
import FamilyTreePage from './pages/FamilyTreePage';
import EntityListPage from './pages/EntityListPage';
import Characters from './pages/Characters/Characters';
import Character from './pages/Characters/Character';
import Locations from './pages/Locations/Locations';
import Location from './pages/Locations/Location';
import FamilyTrees from './pages/FamilyTrees/FamilyTrees';
import FamilyTree from './pages/FamilyTrees/FamilyTree';
import Events from './pages/Events/Events';
import ReadingPage from './pages/ReadingPage';
import UserSettingsPage from './pages/UserSettingsPage';

function ProtectedRoute({ children }: { children: React.ReactNode }) {
  // Read from localStorage directly — always in sync, no React state batching issues
  if (!getToken()) return <Navigate to="/login" replace />;
  return <>{children}</>;
}

const routes: RouteObject[] = [
  {
    path: '/login',
    element: <LoginPage />,
  },
  {
    path: '/',
    element: (
      <ProtectedRoute>
        <App />
      </ProtectedRoute>
    ),
    children: [
      { index: true, element: <HomePage /> },
      { path: 'worlds', element: <HomePage /> },
      { path: 'worlds/:worldId', element: <WorldPage /> },
      { path: 'worlds/:worldId/settings', element: <WorldSettingsPage /> },
      { path: 'worlds/:worldId/characters', element: <Characters /> },
      { path: 'worlds/:worldId/characters/:entityId', element: <Character /> },
      { path: 'worlds/:worldId/locations', element: <Locations /> },
      { path: 'worlds/:worldId/locations/:locationId', element: <Location /> },
      { path: 'worlds/:worldId/events', element: <Events /> },
      { path: 'worlds/:worldId/family-trees', element: <FamilyTrees /> },
      { path: 'worlds/:worldId/family-trees/:familyTreeId', element: <FamilyTree /> },
      { path: 'worlds/:worldId/entities/:entityType', element: <EntityListPage /> },
      { path: 'family-trees/:familyTreeId', element: <FamilyTreePage /> },
      { path: 'reading', element: <ReadingPage /> },
      { path: 'settings', element: <UserSettingsPage /> },
    ],
  },
];

export const router = createBrowserRouter(routes);
