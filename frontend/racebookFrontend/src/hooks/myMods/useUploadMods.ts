import { useMutation } from '@tanstack/react-query';
import { uploadMod as uploadModService } from '../../services';
import { useInvalidateMods } from './useInvalidateMods';

export const useUploadMod = () => {
    const invalidate = useInvalidateMods();

    const mutation = useMutation({
        mutationFn: uploadModService,
        onSuccess: invalidate,
    });

    return {
        uploadMod: (formData: FormData) => mutation.mutateAsync(formData),
        isUploading: mutation.isPending,
    };
};