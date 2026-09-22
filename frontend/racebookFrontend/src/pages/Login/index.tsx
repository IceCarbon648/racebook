import { useState } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../contexts';
import Background from '../../components/molecules/Background/Index';
import FormPanel from '../../components/molecules/FormPanel';

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
        <div className="flex items-center justify-center min-h-[calc(100vh-4rem)]">
            <Background reveal={true}/>
            {/*<div className="relative flex flex-col gap-6 w-full h-150 max-w-sm p-10">*/}
            <FormPanel className="min-h-full w-full flex flex-col justify-start gap-12">
                <h1 className="flex justify-center text-[46px] font-bold text-white pt-4">Login</h1>

                <div className="flex flex-col gap-18">
                    <div className="flex flex-col gap-8">
                        <div className="flex flex-col gap-1.5">
                        <label htmlFor="email" className="flex justify-start font-medium text-white">
                            Email
                        </label>
                        <input
                            id="email"
                            type="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            placeholder="Enter your email"
                            className="px-3 py-2 bg-black/50 text-sm text-gray-300 border border-gray-400 rounded focus:outline-none focus:border-gray-200"
                        />
                    </div>
                    <div className="flex flex-col gap-1.5">
                        <label htmlFor="password" className="flex justify-start font-medium text-white">
                            Password
                        </label>
                        <input
                            id="password"
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            placeholder="Enter your password"
                            className="px-3 py-2 bg-black/50 text-sm text-gray-300 border border-gray-400 rounded focus:outline-none focus:border-gray-200"
                        />
                    </div>
                    {error && (
                        <p className="text-sm text-red-500">{error}</p>
                    )}
                    </div>
                    <div>
                        <button
                            onClick={handleSubmit}
                            disabled={isLoading}
                            className="justify-center px-4 py-2 w-1/2 bg-[#930093] hover:bg-[#600060] text-sm text-white rounded disabled:opacity-50 disabled:cursor-not-allowed"
                        >
                            {isLoading ? 'LOGGING IN...' : 'LOGIN'}
                        </button>
                    </div>
                </div>
                <div className="flex h-full">
                    <p className="flex items-end justify-center w-full gap-4 text-sm text-center text-gray-400">
                    Don't have an account?{' '}
                    <Link to="/register" className="font-medium text-blue-400 underline cursor-pointer">
                        Register
                    </Link>
                </p>
                </div>
            </FormPanel>
            {/*</div>*/}
        </div>
    );
};

export default Login;