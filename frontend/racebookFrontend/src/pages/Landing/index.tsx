import { Link } from 'react-router-dom';
import { useAuth } from '../../contexts';
import Background from '../../components/molecules/Background/Index';

const Landing = () => {
    const { isAuthenticated } = useAuth();

    return (

        <div className="relative flex flex-col items-center justify-center min-h-[calc(100vh-4rem)] gap-6 text-center px-4">
            <Background reveal={true} />
            <h1 className="text-5xl font-bold text-white drop-shadow-[0_0_4px_rgba(0,0,0,1)]">Welcome to Racebook</h1>
            <p className="text-lg text-gray-300 max-w-md drop-shadow-[0_0_4px_rgba(0,0,0,1)]">
                Your go-to destination for Blur mods
            </p>
            <div className="flex gap-4 mt-2">
                <Link
                    to="/mods"
                    className={`px-6 py-2.5 text-sm text-white font-medium rounded bg-[#930093] hover:bg-[#600060]`}
                >
                    Browse Mods
                </Link>
                {!isAuthenticated && (
                    <Link
                        to="/register"
                        className="px-6 py-2.5 text-sm text-gray-300 font-medium border border-gray-400 rounded hover:bg-white/10 bg-black/50 cursor-pointer"
                    >
                        Get Started
                    </Link>
                )}
            </div>
        </div>
    );
};

export default Landing;