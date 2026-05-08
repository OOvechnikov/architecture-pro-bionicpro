const AUTH_BASE = process.env.REACT_APP_API_URL!;

export async function checkSession(): Promise<boolean> {
  const res = await fetch(`${AUTH_BASE}/auth/me`, {
    credentials: 'include'
  });
  return res.ok;
}

// 🔷 Keycloak
export function loginWithKeycloak() {
  window.location.href = `${AUTH_BASE}/auth/login`;
}

// 🔷 Yandex
export function loginWithYandex() {
  window.location.href = `${AUTH_BASE}/auth/yandex/login`;
}

export function logout() {
  window.location.href = `${AUTH_BASE}/auth/logout`;
}