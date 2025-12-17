'use client';

import Link from 'next/link';
import { useEffect, useMemo, useState } from 'react';
import NavBar from '../../components/NavBar';
import { apiClient } from '../../lib/apiClient';
import { useAuthGuard } from '../../lib/useAuthGuard';
import type { TalkResponseDto } from '../../../generated-source/MyProjectModels';

export default function TalksPage() {
  const { ready } = useAuthGuard();
  const [talks, setTalks] = useState<TalkResponseDto[]>([]);
  const [status, setStatus] = useState<'idle' | 'loading' | 'error'>('idle');
  const [error, setError] = useState<string | null>(null);

  const sortedTalks = useMemo(() => {
    return [...talks].sort((a, b) => {
      const timeA = a.scheduledAt ? new Date(a.scheduledAt).getTime() : 0;
      const timeB = b.scheduledAt ? new Date(b.scheduledAt).getTime() : 0;
      return timeA - timeB;
    });
  }, [talks]);

  useEffect(() => {
    if (!ready) return;
    void loadTalks();
  }, [ready]);

  const loadTalks = async () => {
    setStatus('loading');
    setError(null);
    try {
      const data = await apiClient.talksAll(undefined, undefined, undefined);
      setTalks(data ?? []);
      setStatus('idle');
    } catch (err: any) {
      console.error(err);
      setStatus('error');
      setError(err?.response?.data?.message || 'Unable to load talks.');
    }
  };

  const toggleRegistration = async (talk: TalkResponseDto) => {
    try {
      if (talk.isUserRegistered) {
        await apiClient.registerDELETE(talk.id!);
      } else {
        await apiClient.registerPOST2(talk.id!);
      }
      await loadTalks();
    } catch (err: any) {
      console.error(err);
      setError(err?.response?.data?.message || 'Could not update registration.');
    }
  };

  if (!ready) return null;

  return (
    <div className="grid" style={{ gap: 20 }}>
      <NavBar />
      <div className="card" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <h1 style={{ margin: 0 }}>Talks</h1>
        <Link href="/talks/new" className="btn">
          Create talk
        </Link>
      </div>
      {error && <div className="card" style={{ color: '#b91c1c', fontWeight: 600 }}>{error}</div>}
      {status === 'loading' && <div className="card">Loading talks…</div>}
      {sortedTalks.length === 0 && status === 'idle' && <div className="card">No talks yet.</div>}
      <div className="grid" style={{ gap: 14 }}>
        {sortedTalks.map((talk) => (
          <div key={talk.id} className="card" style={{ borderColor: '#d1fae5' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', gap: 12 }}>
              <div>
                <div style={{ fontWeight: 700 }}>{talk.title}</div>
                <div style={{ color: '#166534', fontSize: 14 }}>{formatDate(talk.scheduledAt)}</div>
              </div>
              <button className="btn secondary" onClick={() => toggleRegistration(talk)}>
                {talk.isUserRegistered ? 'Unregister' : 'Register'}
              </button>
            </div>
            <p style={{ marginTop: 10, marginBottom: 10, color: '#475569' }}>{talk.description}</p>
            <div style={{ display: 'flex', gap: 8, flexWrap: 'wrap', color: '#475569' }}>
              {talk.level && <Badge label={talk.level} />}
              {talk.category && <Badge label={talk.category} />}
              {talk.room && <Badge label={`Room ${talk.room}`} />}
              {talk.status && <Badge label={talk.status} />}
            </div>
            {talk.speaker && (
              <div style={{ marginTop: 8, color: '#0f172a' }}>
                Speaker: <strong>{talk.speaker.fullName}</strong>
              </div>
            )}
            <div style={{ color: '#475569', fontSize: 14, marginTop: 8 }}>
              {talk.currentAttendees ?? 0} registered
              {talk.maxAttendees ? ` / ${talk.maxAttendees} max` : ''}
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

