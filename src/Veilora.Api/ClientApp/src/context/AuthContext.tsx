import { createContext, useContext, useState, type ReactNode } from 'react';
import {
  clearToken, clearUserId, clearEmail, clearDisplayName,
  getToken, getUserId, getEmail, getDisplayName,
  setToken, setUserId, setEmail, setDisplayName,
} from '../api/apiFetch';

interface AuthContextValue {
  token: string | null;
  userId: string | null;
  email: string | null;
  displayName: string | null;
  isAuthenticated: boolean;
  login: (token: string, userId: string, email: string, displayName: string | null) => void;
  logout: () => void;
  updateDisplayName: (name: string | null) => void;
}

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setTokenState] = useState<string | null>(getToken);
  const [userId, setUserIdState] = useState<string | null>(getUserId);
  const [email, setEmailState] = useState<string | null>(getEmail);
  const [displayName, setDisplayNameState] = useState<string | null>(getDisplayName);

  function login(newToken: string, newUserId: string, newEmail: string, newDisplayName: string | null) {
    setToken(newToken);
    setUserId(newUserId);
    setEmail(newEmail);
    setDisplayName(newDisplayName);
    setTokenState(newToken);
    setUserIdState(newUserId);
    setEmailState(newEmail);
    setDisplayNameState(newDisplayName);
  }

  function logout() {
    clearToken();
    clearUserId();
    clearEmail();
    clearDisplayName();
    setTokenState(null);
    setUserIdState(null);
    setEmailState(null);
    setDisplayNameState(null);
  }

  function updateDisplayName(name: string | null) {
    setDisplayName(name);
    setDisplayNameState(name);
  }

  return (
    <AuthContext.Provider value={{ token, userId, email, displayName, isAuthenticated: token !== null, login, logout, updateDisplayName }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used inside AuthProvider');
  return ctx;
}
