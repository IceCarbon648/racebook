import { useQuery } from '@tanstack/react-query';
import { getFavourites } from '../services';
import { queryKeys } from '../constants/queryKeys';

export const useFavourites = () => {
    const { data: favourites = [], isLoading, isError } = useQuery({
        queryKey: queryKeys.favourites,
        queryFn: getFavourites,
    });

    return { favourites, isLoading, isError };
};