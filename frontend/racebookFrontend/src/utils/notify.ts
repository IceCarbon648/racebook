import { toast } from 'react-toastify';
import { getResponseMessage } from './getResponseMessage';

const show = (type: 'success' | 'error', message: string) => {
    toast.dismiss();
    toast.clearWaitingQueue();
    toast[type](message);
};

export const notify = {
    success: (message: string) => show('success', message),
    error: (message: string) => show('error', message),
    apiError: (error: unknown, fallback?: string) => show('error', getResponseMessage(error, fallback)),
};