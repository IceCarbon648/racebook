import { createContext, useContext } from 'react';
import { useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { login as loginService, logout as logoutService, me as meService } from '../../services';
import type { AuthContextType } from './index.types';
import type { User } from '../../types';

const AuthContext = createContext<AuthContextType | null>(null);

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
    const navigate = useNavigate();
    const queryClient = useQueryClient();

    const { data: user = null, isLoading } = useQuery<User | null>({
        queryKey: ['me'],
        queryFn: async () => {
            const response = await meService();
            return {
                uid: response.uid,
                username: response.username,
                amaxUsername: response.amaxUsername ?? undefined,
            };
        },
        retry: false,
        staleTime: Infinity,
    });

    const isAuthenticated = user !== null;

    const loginMutation = useMutation({
        mutationFn: loginService,
        onSuccess: async () => {
            await queryClient.invalidateQueries();
            navigate('/mods');
        },
    });

    const logoutMutation = useMutation({
        mutationFn: logoutService,
        onSuccess: () => {
            queryClient.clear();
            navigate('/login');
        },
    });

    const login = async (email: string, password: string): Promise<void> => {
        await loginMutation.mutateAsync({ email, password });
    };

    const logout = async (): Promise<void> => {
        await logoutMutation.mutateAsync();
    };

    return (
        <AuthContext.Provider
            value={{
                user,
                isAuthenticated,
                isLoading,
                isLoggingIn: loginMutation.isPending,
                login,
                logout,
            }}
        >
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (!context) throw new Error('useAuth must be used within an AuthProvider');
    return context;
};