import { useEffect, useState } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { api } from '../api/client';
import { useAuth } from '../context/AuthContext';

const STATUSES = ['Draft', 'Submitted', 'Distributed'];

export default function TrackDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();

  const [track, setTrack] = useState(null);
  const [dsps, setDsps] = useState([]);
  const [selectedDspIds, setSelectedDspIds] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [actionError, setActionError] = useState(null);
  const [actionLoading, setActionLoading] = useState(false);

  const loadTrack = () => {
    setLoading(true);
    setError(null);
    api
      .getTrack(id)
      .then(setTrack)
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    loadTrack();
    api.getDsps().then(setDsps).catch(() => {});
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [id]);

  const toggleDsp = (dspId) => {
    setSelectedDspIds((prev) =>
      prev.includes(dspId) ? prev.filter((d) => d !== dspId) : [...prev, dspId]
    );
  };

  const handleDistribute = async () => {
    if (!isAuthenticated) return navigate('/login', { state: { from: `/tracks/${id}` } });
    if (selectedDspIds.length === 0) return;
    setActionError(null);
    setActionLoading(true);
    try {
      const updated = await api.distributeTrack(id, selectedDspIds);
      setTrack(updated);
      setSelectedDspIds([]);
    } catch (err) {
      setActionError(err.message);
    } finally {
      setActionLoading(false);
    }
  };

  const handleStatusChange = async (newStatus) => {
    if (!isAuthenticated) return navigate('/login', { state: { from: `/tracks/${id}` } });
    setActionError(null);
    setActionLoading(true);
    try {
      const updated = await api.updateTrackStatus(id, newStatus);
      setTrack(updated);
    } catch (err) {
      setActionError(err.message);
    } finally {
      setActionLoading(false);
    }
  };

  if (loading) return <div className="page">Loading…</div>;
  if (error) return <div className="page error">{error}</div>;
  if (!track) return null;

  const distributedDspIds = new Set(track.distributions.map((d) => d.dspId));
  const availableDsps = dsps.filter((d) => !distributedDspIds.has(d.id));

  return (
    <div className="page">
      <Link to="/">&larr; Back to tracks</Link>
      <h1>{track.title}</h1>
      <div className="detail-grid">
        <div>
          <strong>Artist</strong>
          <div>{track.artistName}</div>
        </div>
        <div>
          <strong>Genre</strong>
          <div>{track.genre}</div>
        </div>
        <div>
          <strong>ISRC</strong>
          <div>{track.isrc}</div>
        </div>
        <div>
          <strong>Release Date</strong>
          <div>{new Date(track.releaseDate).toLocaleDateString()}</div>
        </div>
        <div>
          <strong>Status</strong>
          <div>
            <span className={`badge badge-${track.status.toLowerCase()}`}>{track.status}</span>
          </div>
        </div>
      </div>

      <section>
        <h2>Status</h2>
        <div className="status-actions">
          {STATUSES.map((s) => (
            <button
              key={s}
              disabled={actionLoading || s === track.status}
              onClick={() => handleStatusChange(s)}
              className={s === track.status ? 'active' : ''}
            >
              {s}
            </button>
          ))}
        </div>
      </section>

      <section>
        <h2>DSP Distribution</h2>
        {track.distributions.length === 0 && <p className="muted">Not yet distributed.</p>}
        {track.distributions.length > 0 && (
          <table className="table">
            <thead>
              <tr>
                <th>DSP</th>
                <th>Status</th>
                <th>Submitted At</th>
              </tr>
            </thead>
            <tbody>
              {track.distributions.map((d) => (
                <tr key={d.id}>
                  <td>{d.dspName}</td>
                  <td>
                    <span className={`badge badge-${d.status.toLowerCase()}`}>{d.status}</span>
                  </td>
                  <td>{new Date(d.submittedAt).toLocaleString()}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}

        {availableDsps.length > 0 && (
          <div className="distribute-box">
            <h3>Submit to more DSPs</h3>
            <div className="dsp-checkboxes">
              {availableDsps.map((d) => (
                <label key={d.id}>
                  <input
                    type="checkbox"
                    checked={selectedDspIds.includes(d.id)}
                    onChange={() => toggleDsp(d.id)}
                  />
                  {d.name}
                </label>
              ))}
            </div>
            <button disabled={actionLoading || selectedDspIds.length === 0} onClick={handleDistribute}>
              {actionLoading ? 'Submitting…' : 'Distribute'}
            </button>
          </div>
        )}

        {actionError && <p className="error">{actionError}</p>}
        {!isAuthenticated && <p className="muted small">Sign in to distribute or change status.</p>}
      </section>
    </div>
  );
}