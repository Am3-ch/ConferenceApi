'use client';

import { useRouter } from 'next/navigation';
import { FormEvent, useState } from 'react';
import NavBar from '../../../components/NavBar';
import { apiClient } from '../../../lib/apiClient';
import { useAuthGuard } from '../../../lib/useAuthGuard';
import { CreateSpeakerDto } from '../../../../generated-source/MyProjectModels';

export default function NewSpeakerPage() {
  const router = useRouter();
  const { ready } = useAuthGuard();
  const [fullName, setFullName] = useState('');
  const [bio, setBio] = useState('');
  const [company, setCompany] = useState('');
  const [jobTitle, setJobTitle] = useState('');
  const [profileImageUrl, setProfileImageUrl] = useState('');
  const [twitterHandle, setTwitterHandle] = useState('');
  const [linkedInUrl, setLinkedInUrl] = useState('');
  const [websiteUrl, setWebsiteUrl] = useState('');
  const [status, setStatus] = useState<'idle' | 'loading' | 'error' | 'success'>('idle');
  const [error, setError] = useState<string | null>(null);

  if (!ready) return null;

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setStatus('loading');
    setError(null);
    try {
      const body = new CreateSpeakerDto({
        fullName,
        bio,
        company: company || undefined,
        jobTitle: jobTitle || undefined,
        profileImageUrl: profileImageUrl || undefined,
        twitterHandle: twitterHandle || undefined,
        linkedInUrl: linkedInUrl || undefined,
        websiteUrl: websiteUrl || undefined,
      });
      await apiClient.speakersPOST(body);
      setStatus('success');
      router.replace('/speakers');
    } catch (err: any) {
      console.error(err);
      setStatus('error');
      setError(err?.response?.data?.message || 'Could not create speaker.');
    }
  };

  return (
    <div className="grid" style={{ gap: 20 }}>
      <NavBar />
      <div className="card" style={{ maxWidth: 720 }}>
        <h1 style={{ margin: '4px 0 12px' }}>Add speaker</h1>
        <form className="grid" style={{ gap: 12 }} onSubmit={handleSubmit}>
          <TextField label="Full name" value={fullName} onChange={setFullName} required />
          <TextArea label="Bio" value={bio} onChange={setBio} required />
          <TextField label="Company" value={company} onChange={setCompany} />
          <TextField label="Job title" value={jobTitle} onChange={setJobTitle} />
          <TextField label="Profile image URL" value={profileImageUrl} onChange={setProfileImageUrl} />
          <TextField label="Twitter handle" value={twitterHandle} onChange={setTwitterHandle} placeholder="@handle" />
          <TextField label="LinkedIn URL" value={linkedInUrl} onChange={setLinkedInUrl} />
          <TextField label="Website URL" value={websiteUrl} onChange={setWebsiteUrl} />
          {error && <div style={{ color: '#b91c1c', fontWeight: 600 }}>{error}</div>}
          <div style={{ display: 'flex', gap: 10 }}>
            <button className="btn" type="submit" disabled={status === 'loading'}>
              {status === 'loading' ? 'Saving…' : 'Save speaker'}
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

