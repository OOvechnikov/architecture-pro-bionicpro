const AUTH_BASE = process.env.REACT_APP_API_URL!;

export async function checkSession(): Promise<boolean> {
  console.log('check session');
  const res = await fetch(`${AUTH_BASE}/auth/me`, {
    credentials: 'include'
  });
  return res.ok;
}

export function login() {
  console.log('LOGIN redirect');
  window.location.href = `${AUTH_BASE}/auth/login`;
}

export function logout() {
  console.log('logout redirect');
  window.location.href = `${AUTH_BASE}/auth/logout`;
}
