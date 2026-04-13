import { createBrowserRouter, Navigate, type RouteObject } from 'react-router-dom';
import App from './App';
import { getToken } from './api/apiFetch';
import HomePage from './pages/HomePage';
import LoginPage from './pages/LoginPage';
import WorldPage from './pages/WorldPage';
import FamilyTreePage from './pages/FamilyTreePage';

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
      { path: 'worlds/:worldId', element: <WorldPage /> },
      { path: 'family-trees/:familyTreeId', element: <FamilyTreePage /> },
    ],
  },
];

export const router = createBrowserRouter(routes);
