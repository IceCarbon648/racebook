import { Link } from 'react-router-dom';
import { useAuth } from '../../../contexts';
import { racebookLogo } from '../../../assets/images';
import SlotMachineText from '../../atoms/SlotMachineText';

const Navbar = () => {
    const { user, isAuthenticated, logout } = useAuth();

    return (
        <nav className="flex flex-row items-center justify-around px-6 h-16  border border-amber-400">
            <div className="flex flex-row border border-amber-400">
                <Link to="/">
                    <img src={racebookLogo} alt="Racebook logo" className="h-8 w-auto" />
                </Link>
            </div>
            <div className="flex gap-8 border border-amber-400">
                <Link to="/" className="text-sm font-medium hover:text-gray-600">
                    Home
                </Link>
                <Link to="/mods" className="text-sm font-medium hover:text-gray-600">
                    Mods
                </Link>
                {isAuthenticated && (
                    <>
                        <Link to="/my-mods" className="text-sm font-medium hover:text-gray-600">
                            My Mods
                        </Link>
                        <Link to="/favourites" className="text-sm font-medium hover:text-gray-600">
                            <SlotMachineText text="Favourites" />
                        </Link>
                    </>
                )}
            </div>
            <div className="flex gap-4  border border-amber-400">
                {isAuthenticated ? (
                    <div className="relative group">
                        <span className="text-sm font-medium cursor-pointer">
                            {user?.username}
                        </span>
                        <div className="absolute right-0 top-full pt-2 w-36 hidden group-hover:block z-50">
                            <div className="bg-white border border-gray-200 rounded shadow-md">
                                <button
                                    onClick={logout}
                                    className="w-full text-left px-4 py-2 text-sm hover:bg-gray-50"
                                >
                                    Logout
                                </button>
                            </div>
                        </div>
                    </div>
                ) : (
                    <>
                        <Link to="/register" className="text-sm font-medium hover:text-gray-600">
                            Register
                        </Link>
                        <Link to="/login" className="text-sm font-medium hover:text-gray-600">
                            Login
                        </Link>
                    </>
                )}
            </div>
        </nav>
    );
};

export default Navbar;