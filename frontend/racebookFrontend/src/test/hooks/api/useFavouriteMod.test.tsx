import { describe, it, expect, vi, beforeEach } from 'vitest';
import { renderHook, act } from '@testing-library/react';
import { useFavouriteMod } from '../../../hooks/api/useFavouriteMod';
import { addToFavourites, deleteFromFavourites } from '../../../services';
import { queryKeys } from '../../../constants/queryKeys';
import { createWrapper } from '../../helpers/queryWrapper';

vi.mock('../../../services', () => ({
    addToFavourites: vi.fn(),
    deleteFromFavourites: vi.fn(),
}));

const MOD_ID = '22222222-2222-2222-2222-222222222222';

const NOT_YET_FAVOURITED = false;
const ALREADY_FAVOURITED = true;

const toggle = async (isFavourite: boolean) => {
    const { wrapper, queryClient } = createWrapper();
    const invalidateQueries = vi.spyOn(queryClient, 'invalidateQueries');
    const { result } = renderHook(() => useFavouriteMod(), { wrapper });

    await act(async () => {
        await result.current.toggleFavourite(MOD_ID, isFavourite);
    });

    return { invalidateQueries };
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

    it('removes the mod and confirms it when it is already favourited', async () => {
        await toggle(ALREADY_FAVOURITED);

        expect(deleteFromFavourites).toHaveBeenCalledWith(MOD_ID);
        expect(addToFavourites).not.toHaveBeenCalled();
    });

    it('refreshes both the mods list and the favourites list', async () => {
        const { invalidateQueries } = await toggle(NOT_YET_FAVOURITED);

        expect(invalidateQueries).toHaveBeenCalledWith({ queryKey: queryKeys.mods });
        expect(invalidateQueries).toHaveBeenCalledWith({ queryKey: queryKeys.favourites });
    });

    it('rejects so the caller can report the failure', async () => {
        vi.mocked(addToFavourites).mockRejectedValue(new Error('Network error'));

        const { wrapper } = createWrapper();
        const { result } = renderHook(() => useFavouriteMod(), { wrapper });

        await act(async () => {
            await expect(result.current.toggleFavourite(MOD_ID, NOT_YET_FAVOURITED))
                .rejects.toThrow('Network error');
        });
    });
});