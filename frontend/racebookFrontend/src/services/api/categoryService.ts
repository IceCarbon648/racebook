import axiosInstance from '../../lib/axiosInstance';

export const getCategories = async (): Promise<string[]> => {
    const response = await axiosInstance.get<string[]>('/category');
    return response.data;
};