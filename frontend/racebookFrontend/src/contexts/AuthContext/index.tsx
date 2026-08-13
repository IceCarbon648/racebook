import { createContext, useState, useContext } from 'react';
import { useNavigate } from 'react-router-dom';
import { login as loginService, logout as logoutService, me as meService } from '../../services';
import type { AuthContextType } from './index.types';
import type { User } from '../../types';

const AuthContext = createContext<AuthContextType | null>(null);

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
    const [user, setUser] = useState<User | null>(null);
    const navigate = useNavigate();

    const isAuthenticated = user !== null;

    const login = async (email: string, password: string): Promise<void> => {
        await loginService({ email, password });

        const response = await meService();

        const user: User = {
            uid: response.uid,
            username: response.username,
            amaxUsername: response.amaxUsername ?? undefined
        };
        setUser(user);
        navigate('/mods');
    };

    const logout = async (): Promise<void> => {
        await logoutService();
        setUser(null);
        navigate('/login');
    };

    return (
        <AuthContext.Provider value={{ user, isAuthenticated, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (!context) throw new Error('useAuth must be used within an AuthProvider');
    return context;
};