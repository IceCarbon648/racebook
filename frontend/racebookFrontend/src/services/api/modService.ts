import axiosInstance from './axiosInstance';
import type { Mod, MyMod, ModUploadRequest, ModEditRequest } from '../../types';

export const getAllMods = async (): Promise<Mod[]> => {
    const response = await axiosInstance.get<Mod[]>('/mods');

    return response.data;
};

export const getMyMods = async (): Promise<MyMod[]> => {
    const response = await axiosInstance.get<MyMod[]>('/mods/my-mods');

    return response.data;
};

export const uploadMod = async (data: ModUploadRequest): Promise<void> => {
    const formData = new FormData();
    formData.append('title', data.title);
    formData.append('type', data.type);
    formData.append('description', data.description);
    formData.append('modFile', data.modFile);
    formData.append('previewImage', data.previewImage);

    await axiosInstance.post('/mods', formData);
};

export const editMod = async (modId: string, data: ModEditRequest): Promise<void> => {
    const formData = new FormData();

    if (data.title) formData.append('title', data.title);
    if (data.type) formData.append('type', data.type);
    if (data.description) formData.append('description', data.description);
    if (data.modFile) formData.append('modFile', data.modFile);
    if (data.previewImage) formData.append('previewImage', data.previewImage);

    await axiosInstance.put(`/mods/${modId}`, formData);
};

export const deleteMod = async (modId: string): Promise<void> => {
    await axiosInstance.delete(`/mods/${modId}`);
};