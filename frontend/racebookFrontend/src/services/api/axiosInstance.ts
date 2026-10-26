import axios from 'axios';
import { queryClient } from '../../queryClient';

const axiosInstance = axios.create({
    baseURL: import.meta.env.VITE_API_URL ?? '/api',
    withCredentials: true
});

axiosInstance.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error?.response?.status === 401) {
            queryClient.setQueryData(['me'], null);
        }
        return Promise.reject(error);
    }
);

export default axiosInstance;