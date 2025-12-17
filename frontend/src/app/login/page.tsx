'use client';

import axios from 'axios';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { FormEvent, useState } from 'react';
import { storeRefreshToken, storeToken } from '../../lib/auth';
import { setAuthToken } from '../../lib/apiClient';

type LoginResponse = {
  token: string;
  refreshToken?: string;
  username?: string;
  expiresAt?: string;
};

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL ?? 'http://127.0.0.1:8080';

export default function LoginPage() {
  const router = useRouter();
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [status, setStatus] = useState<'idle' | 'loading' | 'error' | 'success'>('idle');
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setStatus('loading');
    setError(null);
    try {
      const res = await axios.post<LoginResponse>(`${API_BASE_URL}/api/Auth/login`, {
        username,
        password,
      });
      const accessToken = res.data.token;
      const refreshToken = res.data.refreshToken;
      storeToken(accessToken);
      if (refreshToken) {
        storeRefreshToken(refreshToken);
      }
      setAuthToken(accessToken);
      setStatus('success');
      router.replace('/talks');
    } catch (err: any) {
      console.error(err);
      setStatus('error');
      setError(err?.response?.data?.message || 'Login failed. Check your credentials.');
    }
  };

  return (
    <div className="grid" style={{ gap: 20, maxWidth: 520, margin: '0 auto', paddingTop: 40 }}>
      <div className="card" style={{ maxWidth: 480, margin: '0 auto' }}>
        <h1 style={{ margin: '4px 0 12px' }}>Login</h1>
        <form onSubmit={handleSubmit} className="grid" style={{ gap: 12 }}>
          <label className="grid" style={{ gap: 6 }}>
            <span style={{ fontWeight: 600 }}>Username</span>
            <input
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              required
              autoComplete="username"
              style={{ padding: 10, borderRadius: 10, border: '1px solid #cbd5e1' }}
            />
          </label>
          <label className="grid" style={{ gap: 6 }}>
            <span style={{ fontWeight: 600 }}>Password</span>
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              autoComplete="current-password"
              style={{ padding: 10, borderRadius: 10, border: '1px solid #cbd5e1' }}
            />
          </label>
          {error && <div style={{ color: '#b91c1c', fontWeight: 600 }}>{error}</div>}
          <button className="btn" type="submit" disabled={status === 'loading'}>
            {status === 'loading' ? 'Signing in…' : 'Login'}
          </button>
        </form>
        <p style={{ marginTop: 12, color: '#475569' }}>
          Don&apos;t have an account?{' '}
          <Link className="link-underline-hover" href="/register">
            Register here
          </Link>
          .
        </p>
      </div>
    </div>
  );
}

