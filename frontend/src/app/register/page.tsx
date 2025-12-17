'use client';

import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { FormEvent, useState } from 'react';
import { RegisterRequest } from '../../../generated-source/MyProjectModels';
import { apiClient } from '../../lib/apiClient';

export default function RegisterPage() {
  const router = useRouter();
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirm, setConfirm] = useState('');
  const [status, setStatus] = useState<'idle' | 'loading' | 'error' | 'success'>('idle');
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    if (password !== confirm) {
      setError('Passwords do not match.');
      return;
    }
    setStatus('loading');
    setError(null);
    try {
      const body = new RegisterRequest({ username, email, password });
      await apiClient.registerPOST(body);
      setStatus('success');
      router.replace('/login');
    } catch (err: any) {
      console.error(err);
      setStatus('error');
      setError(err?.response?.data?.message || 'Registration failed.');
    }
  };

  return (
    <div className="grid" style={{ gap: 20, maxWidth: 560, margin: '0 auto', paddingTop: 40 }}>
      <div className="card" style={{ maxWidth: 520, margin: '0 auto' }}>
        <h1 style={{ margin: '4px 0 12px' }}>Create account</h1>
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
            <span style={{ fontWeight: 600 }}>Email</span>
            <input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              autoComplete="email"
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
              autoComplete="new-password"
              style={{ padding: 10, borderRadius: 10, border: '1px solid #cbd5e1' }}
            />
          </label>
          <label className="grid" style={{ gap: 6 }}>
            <span style={{ fontWeight: 600 }}>Confirm password</span>
            <input
              type="password"
              value={confirm}
              onChange={(e) => setConfirm(e.target.value)}
              required
              autoComplete="new-password"
              style={{ padding: 10, borderRadius: 10, border: '1px solid #cbd5e1' }}
            />
          </label>
          {error && <div style={{ color: '#b91c1c', fontWeight: 600 }}>{error}</div>}
          <button className="btn" type="submit" disabled={status === 'loading'}>
            {status === 'loading' ? 'Creating account…' : 'Register'}
          </button>
        </form>
        <p style={{ marginTop: 12, color: '#475569' }}>
          Already registered?{' '}
          <Link className="link-underline-hover" href="/login">
            Login here
          </Link>
          .
        </p>
      </div>
    </div>
  );
}

