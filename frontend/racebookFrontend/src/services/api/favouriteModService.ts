import axiosInstance from './axiosInstance';
import type { Mod } from '../../types';

export const getFavourites = async (): Promise<Mod[]> => {
    const response = await axiosInstance.get('/favourite-mod');

    return response.data;
};

export const addToFavourites = async (modId: string): Promise<void> => {
    await axiosInstance.post('/favourite-mod', JSON.stringify(modId), {
        headers: { 'Content-Type': 'application/json' }
    });
};

export const deleteFromFavourites = async (modId: string): Promise<void> => {
    await axiosInstance.delete('/favourite-mod', {
        data: JSON.stringify(modId),
        headers: { 'Content-Type': 'application/json' }
    });
};