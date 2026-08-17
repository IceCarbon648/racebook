import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { register } from '../../services';

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
        <div className="flex items-center justify-center min-h-[calc(100vh-4rem)] px-4">
            <div className="flex flex-col gap-6 w-full max-w-sm p-8 border border-gray-200 rounded-lg">
                <h1 className="text-2xl font-bold text-gray-900">Register</h1>
                {error && (
                    <p className="text-sm text-red-500">{error}</p>
                )}
                <div className="flex flex-col gap-4">
                    <div className="flex flex-col gap-1.5">
                        <label htmlFor="email" className="text-sm font-medium text-gray-700">
                            Email
                        </label>
                        <input
                            id="email"
                            type="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            placeholder="Enter your email"
                            className="px-3 py-2 text-sm border border-gray-200 rounded focus:outline-none focus:border-gray-400"
                        />
                    </div>
                    <div className="flex flex-col gap-1.5">
                        <label htmlFor="username" className="text-sm font-medium text-gray-700">
                            Username
                        </label>
                        <input
                            id="username"
                            type="text"
                            value={username}
                            onChange={(e) => setUsername(e.target.value)}
                            placeholder="Enter your username"
                            className="px-3 py-2 text-sm border border-gray-200 rounded focus:outline-none focus:border-gray-400"
                        />
                    </div>
                    <div className="flex flex-col gap-1.5">
                        <label htmlFor="password" className="text-sm font-medium text-gray-700">
                            Password
                        </label>
                        <input
                            id="password"
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            placeholder="Enter your password"
                            className="px-3 py-2 text-sm border border-gray-200 rounded focus:outline-none focus:border-gray-400"
                        />
                    </div>
                    <button
                        onClick={handleSubmit}
                        disabled={isLoading}
                        className="mt-2 px-4 py-2 text-sm font-medium border border-gray-900 rounded hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
                    >
                        {isLoading ? 'Registering...' : 'Register'}
                    </button>
                </div>
                <p className="text-sm text-center text-gray-500">
                    Already have an account?{' '}
                    <Link to="/login" className="font-medium text-gray-900 hover:underline">
                        Login
                    </Link>
                </p>
            </div>
        </div>
    );
};

export default Register;