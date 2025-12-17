const TOKEN_KEY = 'access_token';
const REFRESH_KEY = 'refresh_token';

export function getStoredToken(): string | null {
  if (typeof window === 'undefined') return null;
  return localStorage.getItem(TOKEN_KEY);
}

export function storeToken(token: string) {
  if (typeof window === 'undefined') return;
  localStorage.setItem(TOKEN_KEY, token);
}

export function clearStoredToken() {
  if (typeof window === 'undefined') return;
  localStorage.removeItem(TOKEN_KEY);
}

export function getStoredRefreshToken(): string | null {
  if (typeof window === 'undefined') return null;
  return localStorage.getItem(REFRESH_KEY);
}

export function storeRefreshToken(token: string) {
  if (typeof window === 'undefined') return;
  localStorage.setItem(REFRESH_KEY, token);
}

export function clearStoredRefreshToken() {
  if (typeof window === 'undefined') return;
  localStorage.removeItem(REFRESH_KEY);
}

export function clearAllTokens() {
  clearStoredToken();
  clearStoredRefreshToken();
}

