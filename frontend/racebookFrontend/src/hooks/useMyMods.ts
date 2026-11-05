import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
    getMyMods,
    uploadMod as uploadModService,
    editMod as editModService,
    deleteMod as deleteModService,
} from '../services';
import { queryKeys } from '../constants/queryKeys';

export const useMyMods = () => {
    const queryClient = useQueryClient();

    const { data: mods = [], isLoading, isError } = useQuery({
        queryKey: queryKeys.myMods,
        queryFn: getMyMods,
    });

    const invalidate = () => {
        queryClient.invalidateQueries({ queryKey: queryKeys.myMods });
        queryClient.invalidateQueries({ queryKey: queryKeys.mods });
    };

    const uploadMutation = useMutation({
        mutationFn: uploadModService,
        onSuccess: invalidate,
    });

    const editMutation = useMutation({
        mutationFn: ({ modId, formData }: { modId: string; formData: FormData }) =>
            editModService(modId, formData),
        onSuccess: invalidate,
    });

    const deleteMutation = useMutation({
        mutationFn: deleteModService,
        onSuccess: invalidate,
    });

    const uploadMod = (formData: FormData) => uploadMutation.mutateAsync(formData);

    const editMod = (modId: string, formData: FormData) =>
        editMutation.mutateAsync({ modId, formData });

    const deleteMod = (modId: string) => deleteMutation.mutate(modId);

    return {
        mods,
        isLoading,
        isError,
        uploadMod,
        editMod,
        deleteMod
    };
};