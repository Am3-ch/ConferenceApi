'use client';

import Link from 'next/link';

export default function Footer() {
  return (
    <footer className="footer">
      <div className="footer-content">
        <p>© 2024 Conference API. All rights reserved.</p>
        <div className="footer-links">
          <Link href="/about">About</Link>
          <Link href="/privacy">Privacy Policy</Link>
          <Link href="/terms">Terms of Service</Link>
          <Link href="/contact">Contact</Link>
        </div>
      </div>
    </footer>
  );
}

