'use client';

import { FormEvent, useState } from 'react';
import NavBar from '../../components/NavBar';
import { apiClient } from '../../lib/apiClient';
import { useAuthGuard } from '../../lib/useAuthGuard';
import { DeleteAccountRequest, UpdatePasswordRequest } from '../../../generated-source/MyProjectModels';
import { clearAllTokens } from '../../lib/auth';
import { useRouter } from 'next/navigation';
import { setAuthToken } from '../../lib/apiClient';

export default function AccountPage() {
  const { ready } = useAuthGuard();
  const router = useRouter();

  const [currentPassword, setCurrentPassword] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [deletePassword, setDeletePassword] = useState('');
  const [status, setStatus] = useState<'idle' | 'loading' | 'error' | 'success'>('idle');
  const [error, setError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  if (!ready) return null;

  const handlePasswordUpdate = async (e: FormEvent) => {
    e.preventDefault();
    if (newPassword !== confirmPassword) {
      setError('New passwords do not match.');
      return;
    }
    setStatus('loading');
    setError(null);
    setSuccessMessage(null);
    try {
      const body = new UpdatePasswordRequest({
        currentPassword,
        newPassword,
        confirmNewPassword: confirmPassword,
      });
      await apiClient.password(body);
      setStatus('success');
      setSuccessMessage('Password updated.');
      setCurrentPassword('');
      setNewPassword('');
      setConfirmPassword('');
    } catch (err: any) {
      console.error(err);
      setStatus('error');
      setError(err?.response?.data?.message || 'Unable to update password.');
    }
  };

  const handleDeleteAccount = async (e: FormEvent) => {
    e.preventDefault();
    if (!deletePassword) {
      setError('Enter your password to confirm account deletion.');
      return;
    }
    setStatus('loading');
    setError(null);
    setSuccessMessage(null);
    try {
      const body = new DeleteAccountRequest({ password: deletePassword });
      await apiClient.account(body);
      clearAllTokens();
      setAuthToken(null);
      router.replace('/register');
    } catch (err: any) {
      console.error(err);
      setStatus('error');
      setError(err?.response?.data?.message || 'Unable to delete account.');
    }
  };

  return (
    <div className="grid" style={{ gap: 20 }}>
      <NavBar />
      <div className="grid" style={{ gridTemplateColumns: 'repeat(auto-fit, minmax(320px, 1fr))', gap: 16 }}>
        <div className="card">
          <h2 style={{ marginTop: 0 }}>Update password</h2>
          <form className="grid" style={{ gap: 12 }} onSubmit={handlePasswordUpdate}>
            <TextField
              label="Current password"
              type="password"
              value={currentPassword}
              onChange={setCurrentPassword}
              required
            />
            <TextField label="New password" type="password" value={newPassword} onChange={setNewPassword} required />
            <TextField
              label="Confirm new password"
              type="password"
              value={confirmPassword}
              onChange={setConfirmPassword}
              required
            />
            {error && <div style={{ color: '#b91c1c', fontWeight: 600 }}>{error}</div>}
            {successMessage && <div style={{ color: '#15803d', fontWeight: 600 }}>{successMessage}</div>}
            <button className="btn" type="submit" disabled={status === 'loading'}>
              {status === 'loading' ? 'Saving…' : 'Save password'}
            </button>
          </form>
        </div>

        <div className="card" style={{ borderColor: '#fecdd3', background: '#fff1f2' }}>
          <h2 style={{ marginTop: 0, color: '#be123c' }}>Delete account</h2>
          <p style={{ color: '#9f1239' }}>This action is permanent.</p>
          <form className="grid" style={{ gap: 12 }} onSubmit={handleDeleteAccount}>
            <TextField
              label="Password"
              type="password"
              value={deletePassword}
              onChange={setDeletePassword}
              required
            />
            <button className="btn secondary" type="submit" disabled={status === 'loading'}>
              {status === 'loading' ? 'Deleting…' : 'Delete account'}
            </button>
          </form>
        </div>
      </div>
    </div>
  );
}

function TextField({
  label,
  value,
  onChange,
  type = 'text',
  required,
}: {
  label: string;
  value: string;
  onChange: (value: string) => void;
  type?: string;
  required?: boolean;
}) {
  return (
    <label className="grid" style={{ gap: 6 }}>
      <span style={{ fontWeight: 600 }}>{label}</span>
      <input
        type={type}
        value={value}
        onChange={(e) => onChange(e.target.value)}
        required={required}
        style={{ padding: 10, borderRadius: 10, border: '1px solid #cbd5e1' }}
      />
    </label>
  );
}

