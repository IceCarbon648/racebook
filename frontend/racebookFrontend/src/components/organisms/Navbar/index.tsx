import { Link } from 'react-router-dom';
import { useAuth } from '../../../contexts';
import { racebookLogo } from '../../../assets/images';
import SlotMachineText from '../../atoms/SlotMachineText';

const Navbar = () => {
    const { user, isAuthenticated, logout } = useAuth();

    return (
        <nav className="flex items-center justify-between px-6 h-16 border-b border-gray-200">
            <div className="flex items-center gap-8">
                <Link to="/">
                    <img src={racebookLogo} alt="Racebook logo" className="h-8 w-auto" />
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
            <div className="flex items-center gap-4">
                {isAuthenticated ? (
                    <div className="relative group">
                        <span className="text-sm font-medium cursor-pointer">
                            {user?.username}
                        </span>
                        <div className="absolute right-0 top-full mt-1 w-36 bg-white border border-gray-200 rounded shadow-md hidden group-hover:block z-50">
                            <button
                                onClick={logout}
                                className="w-full text-left px-4 py-2 text-sm hover:bg-gray-50"
                            >
                                Logout
                            </button>
                        </div>
                    </div>
                ) : (
                    <>
                        <Link to="/register" className="text-sm font-medium hover:text-gray-600">
                            Register
                        </Link>
                        <Link to="/login" className="text-sm font-medium px-4 py-2 border border-gray-900 rounded hover:bg-gray-50">
                            Login
                        </Link>
                    </>
                )}
            </div>
        </nav>
    );
};

export default Navbar;