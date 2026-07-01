const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5080/api';

async function request(path, { method = 'GET', body, auth = false } = {}) {
  const headers = { 'Content-Type': 'application/json' };

  if (auth) {
    const token = localStorage.getItem('jwt_token');
    if (token) headers['Authorization'] = `Bearer ${token}`;
  }

  const res = await fetch(`${API_BASE_URL}${path}`, {
    method,
    headers,
    body: body ? JSON.stringify(body) : undefined,
  });

  if (!res.ok) {
    let errorPayload;
    try {
      errorPayload = await res.json();
    } catch {
      errorPayload = { title: res.statusText };
    }
    const error = new Error(errorPayload.title || 'Request failed');
    error.status = res.status;
    error.details = errorPayload.errors;
    throw error;
  }

  if (res.status === 204) return null;
  return res.json();
}

export const api = {
  login: (username, password) =>
    request('/auth/token', { method: 'POST', body: { username, password } }),

  getArtists: () => request('/artists'),
  createArtist: (data) => request('/artists', { method: 'POST', body: data, auth: true }),

  getTracks: (filters = {}) => {
    const params = new URLSearchParams();
    if (filters.artistId) params.append('artistId', filters.artistId);
    if (filters.genre) params.append('genre', filters.genre);
    if (filters.status) params.append('status', filters.status);
    const qs = params.toString();
    return request(`/tracks${qs ? `?${qs}` : ''}`);
  },
  getTrack: (id) => request(`/tracks/${id}`),
  createTrack: (data) => request('/tracks', { method: 'POST', body: data, auth: true }),
  distributeTrack: (id, dspIds) =>
    request(`/tracks/${id}/distribute`, { method: 'POST', body: { dspIds }, auth: true }),
  updateTrackStatus: (id, status) =>
    request(`/tracks/${id}/status`, { method: 'PATCH', body: { status }, auth: true }),

  getDsps: () => request('/dsps'),
};