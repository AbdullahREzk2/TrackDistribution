import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { api } from '../api/client';

const STATUSES = ['Draft', 'Submitted', 'Distributed'];

export default function TrackList() {
  const [tracks, setTracks] = useState([]);
  const [statusFilter, setStatusFilter] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    setLoading(true);
    setError(null);
    api
      .getTracks(statusFilter ? { status: statusFilter } : {})
      .then(setTracks)
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, [statusFilter]);

  return (
    <div className="page">
      <div className="page-header">
        <h1>Tracks</h1>
        <label className="filter">
          Status:
          <select value={statusFilter} onChange={(e) => setStatusFilter(e.target.value)}>
            <option value="">All</option>
            {STATUSES.map((s) => (
              <option key={s} value={s}>
                {s}
              </option>
            ))}
          </select>
        </label>
      </div>

      {loading && <p>Loading tracks…</p>}
      {error && <p className="error">{error}</p>}

      {!loading && !error && tracks.length === 0 && <p className="muted">No tracks found.</p>}

      {!loading && !error && tracks.length > 0 && (
        <table className="table">
          <thead>
            <tr>
              <th>Title</th>
              <th>Artist</th>
              <th>Genre</th>
              <th>Status</th>
              <th>Release Date</th>
            </tr>
          </thead>
          <tbody>
            {tracks.map((t) => (
              <tr key={t.id}>
                <td>
                  <Link to={`/tracks/${t.id}`}>{t.title}</Link>
                </td>
                <td>{t.artistName}</td>
                <td>{t.genre}</td>
                <td>
                  <span className={`badge badge-${t.status.toLowerCase()}`}>{t.status}</span>
                </td>
                <td>{new Date(t.releaseDate).toLocaleDateString()}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}