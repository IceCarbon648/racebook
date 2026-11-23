import { useQuery } from '@tanstack/react-query';
import { getAllMods } from '../../services';
import { queryKeys } from '../../constants/queryKeys';

export const useMods = () => {
    const mods = useQuery({
        queryKey: queryKeys.mods,
        queryFn: getAllMods
    });

    return mods;
};