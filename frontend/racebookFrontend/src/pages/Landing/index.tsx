import { Link } from 'react-router-dom';
import { useAuth } from '../../contexts';
import { racebookLogo } from '../../assets/images';

const Landing = () => {
    const { isAuthenticated } = useAuth();

    return (
        <div className="flex flex-col items-center justify-center min-h-[calc(100vh-4rem)] gap-6 text-center px-4">
            <img src={racebookLogo} alt="Racebook logo" className="h-32 w-auto" />
            <h1 className="text-5xl font-bold text-gray-900">Welcome to Racebook</h1>
            <p className="text-lg text-gray-500 max-w-md">
                Your go-to destination for Blur mods
            </p>
            <div className="flex gap-4 mt-2">
                <Link
                    to="/mods"
                    className="px-6 py-2.5 text-sm font-medium border border-gray-900 rounded hover:bg-gray-50"
                >
                    Browse Mods
                </Link>
                {!isAuthenticated && (
                    <Link
                        to="/register"
                        className="px-6 py-2.5 text-sm font-medium border border-gray-200 rounded hover:bg-gray-50"
                    >
                        Get Started
                    </Link>
                )}
            </div>
        </div>
    );
};

export default Landing;