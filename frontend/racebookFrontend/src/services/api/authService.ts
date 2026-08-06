import axiosInstance from './axiosInstance';
import type { LoginRequest, AuthResponse } from '../../types';

export const login = async (data: LoginRequest): Promise<AuthResponse> => {
    const formData = new FormData();
    
    formData.append('email', data.email);
    formData.append('password', data.password);

    const response = await axiosInstance.post<AuthResponse>('/auth/login', formData);

    return response.data;
};

export const logout = async (): Promise<void> => {
    await axiosInstance.post('/auth/logout');
};