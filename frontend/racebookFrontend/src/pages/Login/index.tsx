import { useState } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../contexts';
import './index.css';

const Login = () => {
    const { login } = useAuth();
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState<string | null>(null);
    const [isLoading, setIsLoading] = useState(false);

    const handleSubmit = async () => {
        setError(null);
        setIsLoading(true);

        try {
            await login(email, password);
        } catch {
            setError('Invalid email or password');
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="login">
            <div className="login-card">
                <h1>Login</h1>
                {error && <p className="login-error">{error}</p>}
                <div className="login-form">
                    <div className="login-field">
                        <label htmlFor="email">Email</label>
                        <input
                            id="email"
                            type="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            placeholder="Enter your email"
                        />
                    </div>
                    <div className="login-field">
                        <label htmlFor="password">Password</label>
                        <input
                            id="password"
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            placeholder="Enter your password"
                        />
                    </div>
                    <button
                        className="login-btn"
                        onClick={handleSubmit}
                        disabled={isLoading}
                    >
                        {isLoading ? 'Logging in...' : 'Login'}
                    </button>
                </div>
                <p className="login-register">
                    Don't have an account? <Link to="/register">Register</Link>
                </p>
            </div>
        </div>
    );
};

export default Login;