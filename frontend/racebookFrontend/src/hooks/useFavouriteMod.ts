import { useMutation, useQueryClient } from '@tanstack/react-query';
import { addToFavourites, deleteFromFavourites } from '../services';
import { queryKeys } from '../constants/queryKeys';
import { notify } from '../utils/notify';


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

    const toggleFavourite = async (modId: string, isFavourite: boolean) => {
        try {
            await mutation.mutateAsync({ modId, isFavourite });

            notify.success(isFavourite ? 'Removed from favourites' : 'Added to favourites');
        } catch (err) {
            notify.apiError(err, 'Could not update your favourites, please try again');
        }
    };

    return { toggleFavourite, isTogglingFavourite: mutation.isPending };
};