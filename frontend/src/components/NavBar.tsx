'use client';

import Link from 'next/link';
import { usePathname, useRouter } from 'next/navigation';
import { clearStoredToken, getStoredToken } from '../lib/auth';
import { setAuthToken } from '../lib/apiClient';

const links = [
  { href: '/talks', label: 'Talks' },
  { href: '/speakers', label: 'Speakers' },
  { href: '/my-registrations', label: 'My registrations' },
  { href: '/account', label: 'Account' },
];

export default function NavBar() {
  const pathname = usePathname();
  const router = useRouter();
  const isAuthed = !!getStoredToken();

  const handleLogout = () => {
    clearStoredToken();
    setAuthToken(null);
    router.push('/login');
  };

  return (
    <nav className="card" style={{ marginBottom: 16, display: 'flex', alignItems: 'center', gap: 14 }}>
      <Link href="/conference" style={{ fontWeight: 800, fontSize: 18, marginRight: 12 }}>
        Conference
      </Link>
      <div style={{ display: 'flex', gap: 10, flexWrap: 'wrap' }}>
        {links.map((link) => (
          <Link
            key={link.href}
            href={link.href}
            style={{
              padding: '8px 12px',
              borderRadius: 10,
              fontWeight: 600,
              color: pathname?.startsWith(link.href) ? '#fff' : '#166534',
              background: pathname?.startsWith(link.href) ? '#15803d' : '#ecfdf3',
              border: '1px solid #cbd5e1',
            }}
          >
            {link.label}
          </Link>
        ))}
      </div>
      <div style={{ flex: 1 }} />
      {isAuthed ? (
        <button className="btn secondary" onClick={handleLogout}>
          Logout
        </button>
      ) : (
        <div style={{ display: 'flex', gap: 8 }}>
          <Link className="btn secondary" href="/login">
            Login
          </Link>
          <Link className="btn" href="/register">
            Register
          </Link>
        </div>
      )}
    </nav>
  );
}

