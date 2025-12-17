'use client';

import { useRouter } from 'next/navigation';
import { FormEvent, useState } from 'react';
import NavBar from '../../../components/NavBar';
import { apiClient } from '../../../lib/apiClient';
import { useAuthGuard } from '../../../lib/useAuthGuard';
import { CreateTalkDto } from '../../../../generated-source/MyProjectModels';

export default function NewTalkPage() {
  const router = useRouter();
  const { ready } = useAuthGuard();
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [scheduledAt, setScheduledAt] = useState('');
  const [durationMinutes, setDurationMinutes] = useState<number | undefined>(60);
  const [room, setRoom] = useState('');
  const [level, setLevel] = useState('');
  const [category, setCategory] = useState('');
  const [maxAttendees, setMaxAttendees] = useState<number | undefined>(undefined);
  const [status, setStatus] = useState<'idle' | 'loading' | 'error' | 'success'>('idle');
  const [error, setError] = useState<string | null>(null);

  if (!ready) return null;

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setStatus('loading');
    setError(null);
    try {
      const body = new CreateTalkDto({
        title,
        description,
        scheduledAt: scheduledAt ? new Date(scheduledAt) : new Date(),
        durationMinutes,
        room: room || undefined,
        level: level || undefined,
        category: category || undefined,
        maxAttendees,
      });
      await apiClient.talksPOST(body);
      setStatus('success');
      router.replace('/talks');
    } catch (err: any) {
      console.error(err);
      setStatus('error');
      setError(err?.response?.data?.message || 'Could not create talk.');
    }
  };

  return (
    <div className="grid" style={{ gap: 20 }}>
      <NavBar />
      <div className="card" style={{ maxWidth: 720 }}>
        <h1 style={{ margin: '4px 0 12px' }}>Create talk</h1>
        <form className="grid" style={{ gap: 12 }} onSubmit={handleSubmit}>
          <TextField label="Title" value={title} onChange={setTitle} required />
          <TextArea label="Description" value={description} onChange={setDescription} required />
          <label className="grid" style={{ gap: 6 }}>
            <span style={{ fontWeight: 600 }}>Scheduled at</span>
            <input
              type="datetime-local"
              value={scheduledAt}
              onChange={(e) => setScheduledAt(e.target.value)}
              required
              style={{ padding: 10, borderRadius: 10, border: '1px solid #cbd5e1' }}
            />
          </label>
          <TextField
            label="Duration (minutes)"
            type="number"
            value={durationMinutes?.toString() ?? ''}
            onChange={(val) => setDurationMinutes(val ? Number(val) : undefined)}
          />
          <TextField label="Room" value={room} onChange={setRoom} placeholder="e.g. A1" />
          <TextField label="Level" value={level} onChange={setLevel} placeholder="Beginner / Intermediate / Advanced" />
          <TextField label="Category" value={category} onChange={setCategory} placeholder="AI, Web, Cloud..." />
          <TextField
            label="Max attendees"
            type="number"
            value={maxAttendees?.toString() ?? ''}
            onChange={(val) => setMaxAttendees(val ? Number(val) : undefined)}
          />
          {error && <div style={{ color: '#b91c1c', fontWeight: 600 }}>{error}</div>}
          <div style={{ display: 'flex', gap: 10 }}>
            <button className="btn" type="submit" disabled={status === 'loading'}>
              {status === 'loading' ? 'Creating…' : 'Create talk'}
            </button>
            <button className="btn secondary" type="button" onClick={() => router.back()}>
              Cancel
            </button>
          </div>
        </form>
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
  placeholder,
}: {
  label: string;
  value: string;
  onChange: (value: string) => void;
  type?: string;
  required?: boolean;
  placeholder?: string;
}) {
  return (
    <label className="grid" style={{ gap: 6 }}>
      <span style={{ fontWeight: 600 }}>{label}</span>
      <input
        type={type}
        value={value}
        onChange={(e) => onChange(e.target.value)}
        required={required}
        placeholder={placeholder}
        style={{ padding: 10, borderRadius: 10, border: '1px solid #cbd5e1' }}
      />
    </label>
  );
}

function TextArea({
  label,
  value,
  onChange,
  required,
}: {
  label: string;
  value: string;
  onChange: (value: string) => void;
  required?: boolean;
}) {
  return (
    <label className="grid" style={{ gap: 6 }}>
      <span style={{ fontWeight: 600 }}>{label}</span>
      <textarea
        value={value}
        onChange={(e) => onChange(e.target.value)}
        required={required}
        rows={4}
        style={{ padding: 10, borderRadius: 10, border: '1px solid #cbd5e1', resize: 'vertical' }}
      />
    </label>
  );
}

