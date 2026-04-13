import { createContext, useContext, useState, type ReactNode } from 'react';
import { clearToken, clearUserId, getToken, getUserId, setToken, setUserId } from '../api/apiFetch';

interface AuthContextValue {
  token: string | null;
  userId: string | null;
  isAuthenticated: boolean;
  login: (token: string, userId: string) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setTokenState] = useState<string | null>(getToken);
  const [userId, setUserIdState] = useState<string | null>(getUserId);

  function login(newToken: string, newUserId: string) {
    setToken(newToken);
    setUserId(newUserId);
    setTokenState(newToken);
    setUserIdState(newUserId);
  }

  function logout() {
    clearToken();
    clearUserId();
    setTokenState(null);
    setUserIdState(null);
  }

  return (
    <AuthContext.Provider value={{ token, userId, isAuthenticated: token !== null, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used inside AuthProvider');
  return ctx;
}
