import { useQuery } from '@tanstack/react-query';
import { getFavourites } from '../../services';
import { queryKeys } from '../../constants/queryKeys';

export const useFavourites = () => {
    const favourites = useQuery({
        queryKey: queryKeys.favourites,
        queryFn: getFavourites,
    });

    return favourites;
};