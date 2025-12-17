'use client';

import Link from 'next/link';
import NavBar from '../../components/NavBar';
import { useAuthGuard } from '../../lib/useAuthGuard';

export default function ConferencePage() {
  const { ready } = useAuthGuard();

  if (!ready) {
    return null;
  }

  return (
    <div className="grid" style={{ gap: 20 }}>
      <NavBar />
      <div className="card">
        <h1 style={{ margin: '4px 0 12px' }}>Welcome back</h1>
        <p style={{ marginTop: 0, color: '#475569' }}>
          Explore talks, manage speakers, and keep track of your registrations.
        </p>
        <div className="grid" style={{ gridTemplateColumns: 'repeat(auto-fit, minmax(220px, 1fr))', gap: 12 }}>
          <QuickLink href="/talks" title="Talks" description="Browse and register for talks." />
          <QuickLink href="/speakers" title="Speakers" description="View and manage speakers." />
          <QuickLink href="/my-registrations" title="My registrations" description="Everything you signed up for." />
          <QuickLink href="/account" title="Account" description="Password and account actions." />
        </div>
      </div>
    </div>
  );
}

function QuickLink({ href, title, description }: { href: string; title: string; description: string }) {
  return (
    <Link href={href} className="card" style={{ borderColor: '#e2e8f0', display: 'block' }}>
      <div style={{ fontWeight: 700, marginBottom: 6 }}>{title}</div>
      <p style={{ margin: 0, color: '#475569' }}>{description}</p>
    </Link>
  );
}

