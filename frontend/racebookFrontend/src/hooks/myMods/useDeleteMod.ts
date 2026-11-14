import { useMutation } from '@tanstack/react-query';
import { deleteMod as deleteModService } from '../../services';
import { useInvalidateMods } from './useInvalidateMods';

export const useDeleteMod = () => {
    const mutation = useMutation({
        mutationFn: deleteModService,
        onSuccess: useInvalidateMods(),
    });

    return {
        deleteMod: (modId: string) => mutation.mutateAsync(modId),
        isDeleting: mutation.isPending,
    };
};