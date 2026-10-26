import { useQuery } from '@tanstack/react-query';
import { getAllMods } from '../services';
import { queryKeys } from '../constants/queryKeys';

export const useMods = () => {
    const { data: mods = [], isLoading, isError } = useQuery({
        queryKey: queryKeys.mods,
        queryFn: getAllMods,
    });

    return { mods, isLoading, isError };
};