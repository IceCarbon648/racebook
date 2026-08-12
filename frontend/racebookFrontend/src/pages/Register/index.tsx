import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { register } from '../../services';
import './index.css';

const Register = () => {
    const navigate = useNavigate();
    const [email, setEmail] = useState('');
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState<string | null>(null);
    const [isLoading, setIsLoading] = useState(false);

    const handleSubmit = async () => {
        setError(null);
        setIsLoading(true);

        try {
            const success = await register({ email, username, password });

            if (success) {
                navigate('/login');
            } else {
                setError('Email already in use');
            }
        } catch {
            setError('Something went wrong, please try again');
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="register">
            <div className="register-card">
                <h1>Register</h1>
                {error && <p className="register-error">{error}</p>}
                <div className="register-form">
                    <div className="register-field">
                        <label htmlFor="email">Email</label>
                        <input
                            id="email"
                            type="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            placeholder="Enter your email"
                        />
                    </div>
                    <div className="register-field">
                        <label htmlFor="username">Username</label>
                        <input
                            id="username"
                            type="text"
                            value={username}
                            onChange={(e) => setUsername(e.target.value)}
                            placeholder="Enter your username"
                        />
                    </div>
                    <div className="register-field">
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
                        className="register-btn"
                        onClick={handleSubmit}
                        disabled={isLoading}
                    >
                        {isLoading ? 'Registering...' : 'Register'}
                    </button>
                </div>
                <p className="register-login">
                    Already have an account? <Link to="/login">Login</Link>
                </p>
            </div>
        </div>
    );
};

export default Register;