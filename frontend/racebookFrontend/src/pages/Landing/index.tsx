import { Link } from 'react-router-dom';
import { useAuth } from '../../contexts';
import './index.css';

const Landing = () => {
    const { isAuthenticated } = useAuth();

    return (
        <div className="landing">
            <div className="landing-content">
                <img src="/src/assets/images/logo.svg" alt="Racebook logo" className="landing-logo" />
                <h1>Welcome to Racebook</h1>
                <p>Your go-to destination for Blur mods</p>
                <div className="landing-actions">
                    <Link to="/mods" className="landing-btn-primary">
                        Browse Mods
                    </Link>
                    {!isAuthenticated && (
                        <Link to="/register" className="landing-btn-secondary">
                            Get Started
                        </Link>
                    )}
                </div>
            </div>
        </div>
    );
};

export default Landing;