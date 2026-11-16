import { describe, it, expect, vi } from 'vitest';
import { renderHook } from '@testing-library/react';
import { useInvalidateMods } from '../../hooks/myMods/useInvalidateMods';
import { queryKeys } from '../../constants/queryKeys';
import { createWrapper } from '../helpers/queryWrapper';

describe('useInvalidateMods', () => {
    it('invalidates the my mods query', () => {
        const { wrapper, queryClient } = createWrapper();
        const invalidateQueries = vi.spyOn(queryClient, 'invalidateQueries');

        const { result } = renderHook(() => useInvalidateMods(), { wrapper });
        result.current();

        expect(invalidateQueries).toHaveBeenCalledWith({ queryKey: queryKeys.myMods });
    });

    it('invalidates the public mods query', () => {
        const { wrapper, queryClient } = createWrapper();
        const invalidateQueries = vi.spyOn(queryClient, 'invalidateQueries');

        const { result } = renderHook(() => useInvalidateMods(), { wrapper });
        result.current();

        expect(invalidateQueries).toHaveBeenCalledWith({ queryKey: queryKeys.mods });
    });

    it('invalidates nothing else', () => {
        const { wrapper, queryClient } = createWrapper();
        const invalidateQueries = vi.spyOn(queryClient, 'invalidateQueries');

        const { result } = renderHook(() => useInvalidateMods(), { wrapper });
        result.current();

        expect(invalidateQueries).toHaveBeenCalledTimes(2);
    });

    it('invalidates nothing until it is called', () => {
        const { wrapper, queryClient } = createWrapper();
        const invalidateQueries = vi.spyOn(queryClient, 'invalidateQueries');

        renderHook(() => useInvalidateMods(), { wrapper });

        expect(invalidateQueries).not.toHaveBeenCalled();
    });
});
