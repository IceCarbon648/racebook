import type { MyMod } from '../../../types';

export interface ModModalProps {
    mode: 'upload' | 'edit';
    mod?: MyMod;
    onClose: () => void;
    onSubmit: (formData: FormData) => Promise<void>;
}