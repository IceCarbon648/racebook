import axiosInstance from './axiosInstance';
import type { LoginRequest, AuthResponse } from '../../types';

export const login = async (data: LoginRequest): Promise<void> => {
    const formData = new FormData();
    
    formData.append('email', data.email);
    formData.append('password', data.password);

    await axiosInstance.post<AuthResponse>('/auth/login', formData);
};

export const logout = async (): Promise<void> => {
    await axiosInstance.post('/auth/logout');
};