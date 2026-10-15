import { useMutation, useQueryClient } from '@tanstack/react-query';
import { addToFavourites, deleteFromFavourites } from '../services';
import { queryKeys } from '../constants/queryKeys';

export const useFavouriteMod = () => {
    const queryClient = useQueryClient();

    const mutation = useMutation({
        mutationFn: ({ modId, isFavourite }: { modId: string; isFavourite: boolean }) =>
            isFavourite ? deleteFromFavourites(modId) : addToFavourites(modId),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: queryKeys.mods });
            queryClient.invalidateQueries({ queryKey: queryKeys.favourites });
        },
    });

    const toggleFavourite = (modId: string, isFavourite: boolean) =>
        mutation.mutate({ modId, isFavourite });

    return { toggleFavourite, isTogglingFavourite: mutation.isPending };
};