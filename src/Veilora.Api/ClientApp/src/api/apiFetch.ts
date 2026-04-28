const TOKEN_KEY = 'auth_token';
const USER_ID_KEY = 'auth_user_id';
const EMAIL_KEY = 'auth_email';
const DISPLAY_NAME_KEY = 'auth_display_name';

export function getToken(): string | null {
  return localStorage.getItem(TOKEN_KEY);
}

export function setToken(token: string): void {
  localStorage.setItem(TOKEN_KEY, token);
}

export function clearToken(): void {
  localStorage.removeItem(TOKEN_KEY);
}

export function getUserId(): string | null {
  return localStorage.getItem(USER_ID_KEY);
}

export function setUserId(id: string): void {
  localStorage.setItem(USER_ID_KEY, id);
}

export function clearUserId(): void {
  localStorage.removeItem(USER_ID_KEY);
}

export function getEmail(): string | null {
  return localStorage.getItem(EMAIL_KEY);
}

export function setEmail(email: string): void {
  localStorage.setItem(EMAIL_KEY, email);
}

export function clearEmail(): void {
  localStorage.removeItem(EMAIL_KEY);
}

export function getDisplayName(): string | null {
  return localStorage.getItem(DISPLAY_NAME_KEY);
}

export function setDisplayName(name: string | null): void {
  if (name) localStorage.setItem(DISPLAY_NAME_KEY, name);
  else localStorage.removeItem(DISPLAY_NAME_KEY);
}

export function clearDisplayName(): void {
  localStorage.removeItem(DISPLAY_NAME_KEY);
}

export async function apiFetch(input: string, init?: RequestInit): Promise<Response> {
  const token = getToken();
  const headers = new Headers(init?.headers);
  if (token) {
    headers.set('Authorization', `Bearer ${token}`);
  }
  const res = await fetch(input, { ...init, headers });
  if (res.status === 401) {
    clearToken();
    window.location.href = '/login';
    return res;
  }
  if (!res.ok) {
    throw new Error(`HTTP ${res.status}: ${res.statusText}`);
  }
  return res;
}
