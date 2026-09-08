import type { User } from '../../types';

export interface AuthContextType {
    user: User | null;
    isAuthenticated: boolean;
    isLoading: boolean;
    isLoggingIn: boolean;
    login: (email: string, password: string) => Promise<void>;
    logout: () => Promise<void>;
}