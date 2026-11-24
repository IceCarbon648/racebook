import { useCallback, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { login as loginService, logout as logoutService, me as meService } from '../../services';
import { AuthContext } from './context';
import type { User } from '../../types';

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
    const navigate = useNavigate();
    const queryClient = useQueryClient();

    const { data: user = null, isLoading } = useQuery<User | null>({
        queryKey: ['me'],
        queryFn: async () => {
            const response = await meService();
            return {
                uid: response.uid,
                username: response.username
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

    const login = useCallback(
        async (email: string, password: string): Promise<void> => {
            await loginMutation.mutateAsync({ email, password });
        },
        [loginMutation.mutateAsync],
    );

    const logout = useCallback(async (): Promise<void> => {
        await logoutMutation.mutateAsync();
    }, [logoutMutation.mutateAsync]);

    const value = useMemo(
        () => ({
            user,
            isAuthenticated,
            isLoading,
            isLoggingIn: loginMutation.isPending,
            login,
            logout,
        }),
        [user, isAuthenticated, isLoading, loginMutation.isPending, login, logout],
    );

    return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
};