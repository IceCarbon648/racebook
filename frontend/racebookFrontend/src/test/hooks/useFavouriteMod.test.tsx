import { describe, it, expect, vi, beforeEach } from 'vitest';
import { renderHook, act } from '@testing-library/react';
import { useFavouriteMod } from '../../hooks/useFavouriteMod';
import { addToFavourites, deleteFromFavourites } from '../../services';
import { notify } from '../../utils/notify';
import { queryKeys } from '../../constants/queryKeys';
import { createWrapper } from '../helpers/queryWrapper';

vi.mock('../../services', () => ({
    addToFavourites: vi.fn(),
    deleteFromFavourites: vi.fn(),
}));

vi.mock('../../utils/notify', () => ({
    notify: {
        success: vi.fn(),
        error: vi.fn(),
        apiError: vi.fn(),
    },
}));

const MOD_ID = '22222222-2222-2222-2222-222222222222';

const NOT_YET_FAVOURITED = false;
const ALREADY_FAVOURITED = true;

const toggle = async (isFavourite: boolean) => {
    const { wrapper, queryClient } = createWrapper();
    const { result } = renderHook(() => useFavouriteMod(), { wrapper });

    await act(async () => {
        await result.current.toggleFavourite(MOD_ID, isFavourite);
    });

    return { queryClient };
};

beforeEach(() => {
    vi.clearAllMocks();
    vi.mocked(addToFavourites).mockResolvedValue(undefined as never);
    vi.mocked(deleteFromFavourites).mockResolvedValue(undefined as never);
});

describe('useFavouriteMod', () => {
    it('adds the mod when it is not yet favourited', async () => {
        await toggle(NOT_YET_FAVOURITED);

        expect(addToFavourites).toHaveBeenCalledWith(MOD_ID);
        expect(deleteFromFavourites).not.toHaveBeenCalled();
    });

    it('removes the mod when it is already favourited', async () => {
        await toggle(ALREADY_FAVOURITED);

        expect(deleteFromFavourites).toHaveBeenCalledWith(MOD_ID);
        expect(addToFavourites).not.toHaveBeenCalled();
    });

    it('confirms that the mod was added', async () => {
        await toggle(NOT_YET_FAVOURITED);

        expect(notify.success).toHaveBeenCalledWith('Added to favourites');
    });

    it('confirms that the mod was removed', async () => {
        await toggle(ALREADY_FAVOURITED);

        expect(notify.success).toHaveBeenCalledWith('Removed from favourites');
    });

    it('refreshes both the mods list and the favourites list', async () => {
        const { wrapper, queryClient } = createWrapper();
        const invalidateQueries = vi.spyOn(queryClient, 'invalidateQueries');
        const { result } = renderHook(() => useFavouriteMod(), { wrapper });

        await act(async () => {
            await result.current.toggleFavourite(MOD_ID, NOT_YET_FAVOURITED);
        });

        expect(invalidateQueries).toHaveBeenCalledWith({ queryKey: queryKeys.mods });
        expect(invalidateQueries).toHaveBeenCalledWith({ queryKey: queryKeys.favourites });
    });

    it('reports a failure instead of confirming success', async () => {
        vi.mocked(addToFavourites).mockRejectedValue(new Error('Network error'));

        await toggle(NOT_YET_FAVOURITED);

        expect(notify.apiError).toHaveBeenCalled();
        expect(notify.success).not.toHaveBeenCalled();
    });

    it('does not throw when the request fails', async () => {
        vi.mocked(deleteFromFavourites).mockRejectedValue(new Error('Network error'));

        await expect(toggle(ALREADY_FAVOURITED)).resolves.toBeDefined();
    });

    it('leaves the lists alone when the request fails', async () => {
        vi.mocked(addToFavourites).mockRejectedValue(new Error('Network error'));

        const { wrapper, queryClient } = createWrapper();
        const invalidateQueries = vi.spyOn(queryClient, 'invalidateQueries');
        const { result } = renderHook(() => useFavouriteMod(), { wrapper });

        await act(async () => {
            await result.current.toggleFavourite(MOD_ID, NOT_YET_FAVOURITED);
        });

        expect(invalidateQueries).not.toHaveBeenCalled();
    });
});
