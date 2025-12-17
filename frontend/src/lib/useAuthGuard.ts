import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { getStoredToken } from './auth';
import { setAuthToken } from './apiClient';

export function useAuthGuard() {
  const router = useRouter();
  const [ready, setReady] = useState(false);
  const [token, setToken] = useState<string | null>(null);

  useEffect(() => {
    const saved = getStoredToken();
    if (!saved) {
      router.replace('/login');
      return;
    }
    setToken(saved);
    setAuthToken(saved);
    setReady(true);
  }, [router]);

  return { ready, token };
}

