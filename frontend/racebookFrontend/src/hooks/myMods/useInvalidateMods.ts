import { useQueryClient } from '@tanstack/react-query';
import { queryKeys } from '../../constants/queryKeys';

export const useInvalidateMods = () => {
    const queryClient = useQueryClient();

    return () => {
        queryClient.invalidateQueries({ queryKey: queryKeys.myMods });
        queryClient.invalidateQueries({ queryKey: queryKeys.mods });
    };
};