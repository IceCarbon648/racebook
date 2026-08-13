import axiosInstance from './axiosInstance';
import type { AuthResponse, RegisterRequest } from '../../types';

export const register = async (data: RegisterRequest): Promise<boolean> => {
    const formData = new FormData();
    formData.append('email', data.email);
    formData.append('username', data.username);
    formData.append('password', data.password);

    const response = await axiosInstance.post<boolean>('/user/register', formData);
    
    return response.data;
};

export const me = async (): Promise<AuthResponse> => {
    const response = await axiosInstance.get<AuthResponse>('/user/@me');

    return response.data;
}