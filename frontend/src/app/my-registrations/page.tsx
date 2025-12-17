'use client';

import { useEffect, useState } from 'react';
import NavBar from '../../components/NavBar';
import { apiClient } from '../../lib/apiClient';
import { useAuthGuard } from '../../lib/useAuthGuard';
import type { TalkResponseDto } from '../../../generated-source/MyProjectModels';

export default function MyRegistrationsPage() {
  const { ready } = useAuthGuard();
  const [talks, setTalks] = useState<TalkResponseDto[]>([]);
  const [status, setStatus] = useState<'idle' | 'loading' | 'error'>('idle');
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!ready) return;
    void load();
  }, [ready]);

  const load = async () => {
    setStatus('loading');
    setError(null);
    try {
      const data = await apiClient.myRegistrations();
      setTalks(data ?? []);
      setStatus('idle');
    } catch (err: any) {
      console.error(err);
      setStatus('error');
      setError(err?.response?.data?.message || 'Unable to load your registrations.');
    }
  };

  if (!ready) return null;

  return (
    <div className="grid" style={{ gap: 20 }}>
      <NavBar />
      <div className="card">
        <h1 style={{ margin: 0 }}>My registrations</h1>
      </div>
      {error && <div className="card" style={{ color: '#b91c1c', fontWeight: 600 }}>{error}</div>}
      {status === 'loading' && <div className="card">Loading your registrations…</div>}
      {talks.length === 0 && status === 'idle' && <div className="card">You have not registered for any talks yet.</div>}
      <div className="grid" style={{ gap: 14 }}>
        {talks.map((talk) => (
          <div key={talk.id} className="card">
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
              <div>
                <div style={{ fontWeight: 700 }}>{talk.title}</div>
                <div style={{ color: '#166534', fontSize: 14 }}>{formatDate(talk.scheduledAt)}</div>
              </div>
            </div>
            <p style={{ marginTop: 8, marginBottom: 10, color: '#475569' }}>{talk.description}</p>
            <div style={{ display: 'flex', gap: 8, flexWrap: 'wrap', color: '#475569' }}>
              {talk.level && <Badge label={talk.level} />}
              {talk.category && <Badge label={talk.category} />}
              {talk.room && <Badge label={`Room ${talk.room}`} />}
              {talk.status && <Badge label={talk.status} />}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}

function Badge({ label }: { label: string }) {
  return (
    <span
      style={{
        background: '#ecfdf3',
        color: '#166534',
        padding: '4px 10px',
        borderRadius: 999,
        fontWeight: 600,
      }}
    >
      {label}
    </span>
  );
}

function formatDate(date?: Date) {
  if (!date) return 'TBD';
  const d = new Date(date);
  return d.toLocaleString(undefined, {
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  });
}

