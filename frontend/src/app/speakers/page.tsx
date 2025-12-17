'use client';

import Link from 'next/link';
import { useEffect, useState } from 'react';
import NavBar from '../../components/NavBar';
import { apiClient } from '../../lib/apiClient';
import { useAuthGuard } from '../../lib/useAuthGuard';
import type { SpeakerResponseDto } from '../../../generated-source/MyProjectModels';

export default function SpeakersPage() {
  const { ready } = useAuthGuard();
  const [speakers, setSpeakers] = useState<SpeakerResponseDto[]>([]);
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
      const data = await apiClient.speakersAll();
      setSpeakers(data ?? []);
      setStatus('idle');
    } catch (err: any) {
      console.error(err);
      setStatus('error');
      setError(err?.response?.data?.message || 'Unable to load speakers.');
    }
  };

  if (!ready) return null;

  return (
    <div className="grid" style={{ gap: 20 }}>
      <NavBar />
      <div className="card" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <h1 style={{ margin: 0 }}>Speakers</h1>
        <Link href="/speakers/new" className="btn">
          Add speaker
        </Link>
      </div>
      {error && <div className="card" style={{ color: '#b91c1c', fontWeight: 600 }}>{error}</div>}
      {status === 'loading' && <div className="card">Loading speakers…</div>}
      {speakers.length === 0 && status === 'idle' && <div className="card">No speakers yet.</div>}
      <div className="grid" style={{ gap: 14 }}>
        {speakers.map((speaker) => (
          <div key={speaker.id} className="card">
            <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
              <div
                style={{
                  width: 38,
                  height: 38,
                  borderRadius: '50%',
                  background: '#15803d',
                  color: '#fff',
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'center',
                  fontWeight: 800,
                  fontSize: 16,
                }}
              >
                {getInitial(speaker.fullName)}
              </div>
              <div style={{ fontWeight: 700 }}>{speaker.fullName}</div>
            </div>
            {speaker.jobTitle && (
              <div style={{ color: '#475569' }}>
                {speaker.jobTitle}
                {speaker.company ? ` · ${speaker.company}` : ''}
              </div>
            )}
            <p style={{ marginTop: 8, marginBottom: 8, color: '#475569' }}>{speaker.bio}</p>
            <div style={{ color: '#475569', fontSize: 14 }}>
              Total talks: {speaker.totalTalks ?? 0}
              {speaker.twitterHandle ? ` · @${speaker.twitterHandle}` : ''}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}

function getInitial(name?: string) {
  if (!name) return '?';
  const trimmed = name.trim();
  return trimmed ? trimmed[0].toUpperCase() : '?';
}

