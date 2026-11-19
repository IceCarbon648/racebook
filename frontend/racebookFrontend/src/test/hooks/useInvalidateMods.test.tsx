import { describe, it, expect, vi } from 'vitest';
import { renderHook } from '@testing-library/react';
import { useInvalidateMods } from '../../hooks/myMods/useInvalidateMods';
import { queryKeys } from '../../constants/queryKeys';
import { createWrapper } from '../helpers/queryWrapper';

describe('useInvalidateMods', () => {
    it('invalidates both mod lists when called, and nothing before', () => {
        const { wrapper, queryClient } = createWrapper();
        const invalidateQueries = vi.spyOn(queryClient, 'invalidateQueries');

        const { result } = renderHook(() => useInvalidateMods(), { wrapper });

        expect(invalidateQueries).not.toHaveBeenCalled();

        result.current();

        expect(invalidateQueries).toHaveBeenCalledWith({ queryKey: queryKeys.myMods });
        expect(invalidateQueries).toHaveBeenCalledWith({ queryKey: queryKeys.mods });
        expect(invalidateQueries).toHaveBeenCalledTimes(2);
    });
});