import axios, { AxiosInstance, AxiosResponse } from 'axios';
import { Client } from '../../generated-source/MyProjectModels';
import {
  clearAllTokens,
  getStoredRefreshToken,
  getStoredToken,
  storeRefreshToken,
  storeToken,
} from './auth';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL ?? 'http://127.0.0.1:8080';

const axiosInstance: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
});

export const apiClient = new Client(API_BASE_URL, axiosInstance);

export function setAuthToken(token: string | null) {
  if (token) {
    axiosInstance.defaults.headers.common.Authorization = `Bearer ${token}`;
  } else {
    delete axiosInstance.defaults.headers.common.Authorization;
  }
}

// Attach persisted access token at start (browser only)
const initialToken = getStoredToken();
if (initialToken) {
  setAuthToken(initialToken);
}

type RefreshResponse = {
  token: string;
  refreshToken?: string;
  username?: string;
  expiresAt?: string;
};

let refreshPromise: Promise<string | null> | null = null;

async function refreshAccessToken(): Promise<string | null> {
  if (refreshPromise) return refreshPromise;
  const refreshToken = getStoredRefreshToken();
  if (!refreshToken) return null;

  refreshPromise = axiosInstance
    .post<RefreshResponse>('/api/Auth/refresh', { refreshToken })
    .then((res: AxiosResponse<RefreshResponse>) => {
      const newAccess = res.data.token;
      const newRefresh = res.data.refreshToken;
      if (newAccess) {
        storeToken(newAccess);
        setAuthToken(newAccess);
      }
      if (newRefresh) {
        storeRefreshToken(newRefresh);
      }
      return newAccess ?? null;
    })
    .catch((err) => {
      console.error('Refresh token failed', err);
      clearAllTokens();
      setAuthToken(null);
      return null;
    })
    .finally(() => {
      refreshPromise = null;
    });

  return refreshPromise;
}

axiosInstance.interceptors.response.use(
  (response) => response,
  async (error) => {
    const status = error?.response?.status;
    const originalRequest = error.config;

    if (status === 401 && originalRequest && !(originalRequest as any)._retry) {
      (originalRequest as any)._retry = true;
      const newAccess = await refreshAccessToken();
      if (newAccess) {
        originalRequest.headers = originalRequest.headers || {};
        originalRequest.headers.Authorization = `Bearer ${newAccess}`;
        return axiosInstance(originalRequest);
      }
    }

    return Promise.reject(error);
  }
);

