'use client';

import Link from 'next/link';
import { useEffect, useState } from 'react';
import NavBar from '../components/NavBar';
import { getStoredToken } from '../lib/auth';

export default function HomePage() {
  const [isAuthed, setIsAuthed] = useState(false);

  useEffect(() => {
    setIsAuthed(!!getStoredToken());
  }, []);

  return (
    <div className="grid" style={{ gap: 24 }}>
      <NavBar />
      <header className="card" style={{ display: 'flex', flexWrap: 'wrap', gap: 16, alignItems: 'center' }}>
        <div style={{ flex: 1, minWidth: 260 }}>
          <div style={{ fontSize: 32, fontWeight: 800, marginBottom: 8 }}>Conference Portal</div>
          <p style={{ color: '#475569', margin: 0 }}>
            Manage conference speakers, talks, and your registrations. Start by creating an account or signing in.
          </p>
          <div style={{ display: 'flex', gap: 10, marginTop: 16, flexWrap: 'wrap' }}>
            <Link className="btn" href={isAuthed ? '/talks' : '/login'}>
              {isAuthed ? 'Go to talks' : 'Login'}
            </Link>
            <Link className="btn secondary" href="/register">
              Register
            </Link>
          </div>
        </div>
        <div className="card" style={{ minWidth: 260, maxWidth: 360, background: '#ecfdf3' }}>
          <div style={{ fontWeight: 700, marginBottom: 6 }}>At a glance</div>
          <ul style={{ margin: 0, paddingLeft: 20, color: '#475569', lineHeight: 1.6 }}>
            <li>Create and edit speakers and talks.</li>
            <li>Register for talks and track your spots.</li>
            <li>Manage your account and password.</li>
          </ul>
        </div>
      </header>

      <section className="grid" style={{ gridTemplateColumns: 'repeat(auto-fit, minmax(240px, 1fr))', gap: 16 }}>
        <div className="card">
          <div style={{ fontWeight: 700, marginBottom: 6 }}>Talks</div>
          <p style={{ margin: 0, color: '#475569' }}>Browse all talks and register with one click.</p>
          <Link href="/talks" className="btn" style={{ marginTop: 12 }}>
            View talks
          </Link>
        </div>
        <div className="card">
          <div style={{ fontWeight: 700, marginBottom: 6 }}>Speakers</div>
          <p style={{ margin: 0, color: '#475569' }}>See every speaker and their sessions.</p>
          <Link href="/speakers" className="btn" style={{ marginTop: 12 }}>
            View speakers
          </Link>
        </div>
        <div className="card">
          <div style={{ fontWeight: 700, marginBottom: 6 }}>My registrations</div>
          <p style={{ margin: 0, color: '#475569' }}>Keep track of the talks you have registered for.</p>
          <Link href="/my-registrations" className="btn" style={{ marginTop: 12 }}>
            My registrations
          </Link>
        </div>
        <div className="card">
          <div style={{ fontWeight: 700, marginBottom: 6 }}>Account</div>
          <p style={{ margin: 0, color: '#475569' }}>Update your password or delete your account.</p>
          <Link href="/account" className="btn" style={{ marginTop: 12 }}>
            Account settings
          </Link>
        </div>
      </section>
    </div>
  );
}

