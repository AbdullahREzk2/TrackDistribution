import { BrowserRouter, Routes, Route, Link, useNavigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import TrackList from './pages/TrackList';
import TrackDetail from './pages/TrackDetail';
import Login from './pages/Login';

function Header() {
  const { isAuthenticated, logout } = useAuth();
  const navigate = useNavigate();

  return (
    <header className="app-header">
      <Link to="/" className="brand">
        Track Distribution
      </Link>
      <nav>
        {isAuthenticated ? (
          <button
            className="link-button"
            onClick={() => {
              logout();
              navigate('/');
            }}
          >
            Sign out
          </button>
        ) : (
          <Link to="/login">Sign in</Link>
        )}
      </nav>
    </header>
  );
}

export default function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Header />
        <main>
          <Routes>
            <Route path="/" element={<TrackList />} />
            <Route path="/tracks/:id" element={<TrackDetail />} />
            <Route path="/login" element={<Login />} />
          </Routes>
        </main>
      </BrowserRouter>
    </AuthProvider>
  );
}