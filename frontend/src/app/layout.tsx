import './globals.css';
import type { Metadata } from 'next';
import React from 'react';
import Footer from '../components/Footer';

export const metadata: Metadata = {
  title: 'Conference Portal',
  description: 'Browse talks and speakers after signing in.',
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en">
      <body>
        <div style={{ maxWidth: 1000, margin: '0 auto', padding: '32px 20px' }}>
          {children}
          <Footer />
        </div>
      </body>
    </html>
  );
}

