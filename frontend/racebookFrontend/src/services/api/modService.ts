import axiosInstance from './axiosInstance';
import type { Mod, MyMod } from '../../types';

export const getAllMods = async (): Promise<Mod[]> => {
    const response = await axiosInstance.get<Mod[]>('/mods');

    return response.data;
};

export const getMyMods = async (): Promise<MyMod[]> => {
    const response = await axiosInstance.get<MyMod[]>('/mods/my-mods');

    return response.data;
};

export const uploadMod = async (formData: FormData): Promise<void> => {
    await axiosInstance.post('/mod/upload', formData);
};

export const editMod = async (modId: string, formData: FormData): Promise<void> => {
    await axiosInstance.put(`/mod/edit/${modId}`, formData);
};

export const deleteMod = async (modId: string): Promise<void> => {
    await axiosInstance.delete(`/mods/${modId}`);
};