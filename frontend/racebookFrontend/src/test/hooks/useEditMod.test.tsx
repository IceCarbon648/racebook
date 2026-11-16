import { describe, it, expect, vi, beforeEach } from 'vitest';
import { renderHook, act } from '@testing-library/react';
import { useEditMod } from '../../hooks/myMods/useEditMod';
import { editMod as editModService } from '../../services';
import { queryKeys } from '../../constants/queryKeys';
import { createWrapper } from '../helpers/queryWrapper';

vi.mock('../../services', () => ({
    editMod: vi.fn(),
}));

const MOD_ID = '22222222-2222-2222-2222-222222222222';

const makeFormData = () => {
    const formData = new FormData();
    formData.append('title', 'Neon Skyline');
    return formData;
};

beforeEach(() => {
    vi.clearAllMocks();
    vi.mocked(editModService).mockResolvedValue(undefined as never);
});

describe('useEditMod', () => {
    it('passes the mod id and the form data to the service', async () => {
        const { wrapper } = createWrapper();
        const { result } = renderHook(() => useEditMod(), { wrapper });
        const formData = makeFormData();

        await act(async () => {
            await result.current.editMod(MOD_ID, formData);
        });

        expect(editModService).toHaveBeenCalledWith(MOD_ID, formData);
    });

    it('refreshes both the my mods list and the public mods list', async () => {
        const { wrapper, queryClient } = createWrapper();
        const invalidateQueries = vi.spyOn(queryClient, 'invalidateQueries');
        const { result } = renderHook(() => useEditMod(), { wrapper });

        await act(async () => {
            await result.current.editMod(MOD_ID, makeFormData());
        });

        expect(invalidateQueries).toHaveBeenCalledWith({ queryKey: queryKeys.myMods });
        expect(invalidateQueries).toHaveBeenCalledWith({ queryKey: queryKeys.mods });
    });

    it('rejects so the caller can show the error itself', async () => {
        vi.mocked(editModService).mockRejectedValue(new Error('Upload service unavailable'));

        const { wrapper } = createWrapper();
        const { result } = renderHook(() => useEditMod(), { wrapper });

        await act(async () => {
            await expect(result.current.editMod(MOD_ID, makeFormData()))
                .rejects.toThrow('Upload service unavailable');
        });
    });

    it('does not refresh the lists when the edit fails', async () => {
        vi.mocked(editModService).mockRejectedValue(new Error('Upload service unavailable'));

        const { wrapper, queryClient } = createWrapper();
        const invalidateQueries = vi.spyOn(queryClient, 'invalidateQueries');
        const { result } = renderHook(() => useEditMod(), { wrapper });

        await act(async () => {
            await result.current.editMod(MOD_ID, makeFormData()).catch(() => undefined);
        });

        expect(invalidateQueries).not.toHaveBeenCalled();
    });

    it('is not pending before an edit starts', () => {
        const { wrapper } = createWrapper();
        const { result } = renderHook(() => useEditMod(), { wrapper });

        expect(result.current.isEditing).toBe(false);
    });
});