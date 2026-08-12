import { Link } from 'react-router-dom';
import { useAuth } from '../../../contexts';
import './index.css';

const Navbar = () => {
    const { user, isAuthenticated, logout } = useAuth();

    return (
        <nav className="navbar">
            <div className="navbar-left">
                <Link to="/">
                    <img src="/src/assets/images/logo.svg" alt="Racebook logo" className="navbar-logo" />
                </Link>
                <Link to="/mods">Mods</Link>
                {isAuthenticated && (
                    <>
                        <Link to="/my-mods">My Mods</Link>
                        <Link to="/favourites">Favourites</Link>
                    </>
                )}
            </div>
            <div className="navbar-right">
                {isAuthenticated ? (
                    <div className="navbar-profile">
                        <span className="navbar-username">{user?.username}</span>
                        <div className="navbar-dropdown">
                            <button onClick={logout}>Logout</button>
                        </div>
                    </div>
                ) : (
                    <>
                        <Link to="/register">Register</Link>
                        <Link to="/login">Login</Link>
                    </>
                )}
            </div>
        </nav>
    );
};

export default Navbar;