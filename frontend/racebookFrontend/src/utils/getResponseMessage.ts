import axios from 'axios';

export const getResponseMessage = (
    error: unknown,
    fallback = 'Something went wrong, please try again'
): string => {
    if (axios.isAxiosError(error)) {
        const data = error.response?.data as any;

        const fieldErrors = data?.errors
            ? Object.values(data.errors).flat().join(' ')
            : null;

        return fieldErrors ?? data?.message ?? data?.detail ?? fallback;
    }

    return fallback;
};

export const getResponseStatus = (error: unknown): number | undefined =>
    axios.isAxiosError(error) ? error.response?.status : undefined;