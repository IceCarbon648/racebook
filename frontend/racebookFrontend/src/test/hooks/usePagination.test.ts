import { describe, it, expect } from 'vitest';
import { renderHook, act } from '@testing-library/react';
import { usePagination } from '../../hooks/usePagination';
import { PAGE_SIZE } from '../../constants/pagination';

const TEST_PAGE_SIZE = 4;
const ITEMS_SPANNING_THREE_PAGES = 10;
const ITEMS_FITTING_ONE_PAGE = 3;

const FIRST_PAGE = 1;
const SECOND_PAGE = 2;
const LAST_PAGE = 3;

const makeItems = (count: number) =>
    Array.from({ length: count }, (_, index) => `item-${index + 1}`);

describe('usePagination', () => {
    it('starts on the first page', () => {
        const { result } = renderHook(() =>
            usePagination(makeItems(ITEMS_SPANNING_THREE_PAGES), TEST_PAGE_SIZE));

        expect(result.current.page).toBe(FIRST_PAGE);
    });

    it('returns every item when they fit on a single page', () => {
        const items = makeItems(ITEMS_FITTING_ONE_PAGE);

        const { result } = renderHook(() => usePagination(items, TEST_PAGE_SIZE));

        expect(result.current.totalPages).toBe(1);
        expect(result.current.paginated).toEqual(items);
    });

    it('splits items across pages using the given page size', () => {
        const { result } = renderHook(() =>
            usePagination(makeItems(ITEMS_SPANNING_THREE_PAGES), TEST_PAGE_SIZE));

        expect(result.current.totalPages).toBe(LAST_PAGE);
        expect(result.current.paginated).toEqual(['item-1', 'item-2', 'item-3', 'item-4']);
    });

    it('returns the next slice when the page changes', () => {
        const { result } = renderHook(() =>
            usePagination(makeItems(ITEMS_SPANNING_THREE_PAGES), TEST_PAGE_SIZE));

        act(() => result.current.setPage(SECOND_PAGE));

        expect(result.current.page).toBe(SECOND_PAGE);
        expect(result.current.paginated).toEqual(['item-5', 'item-6', 'item-7', 'item-8']);
    });

    it('returns a short final page', () => {
        const { result } = renderHook(() =>
            usePagination(makeItems(ITEMS_SPANNING_THREE_PAGES), TEST_PAGE_SIZE));

        act(() => result.current.setPage(LAST_PAGE));

        expect(result.current.paginated).toEqual(['item-9', 'item-10']);
    });

    it('clamps to the first page when the item count shrinks', () => {
        const { result, rerender } = renderHook(
            ({ items }: { items: string[] }) => usePagination(items, TEST_PAGE_SIZE),
            { initialProps: { items: makeItems(ITEMS_SPANNING_THREE_PAGES) } }
        );

        act(() => result.current.setPage(LAST_PAGE));
        expect(result.current.page).toBe(LAST_PAGE);

        rerender({ items: makeItems(ITEMS_FITTING_ONE_PAGE) });

        expect(result.current.page).toBe(FIRST_PAGE);
        expect(result.current.paginated).toEqual(['item-1', 'item-2', 'item-3']);
    });

    it('never reports a page below the first when there are no items', () => {
        const { result } = renderHook(() => usePagination([], TEST_PAGE_SIZE));

        expect(result.current.page).toBe(FIRST_PAGE);
        expect(result.current.totalPages).toBe(0);
        expect(result.current.paginated).toEqual([]);
    });

    it('falls back to the configured page size when none is given', () => {
        const { result } = renderHook(() => usePagination(makeItems(PAGE_SIZE + 1)));

        expect(result.current.totalPages).toBe(2);
        expect(result.current.paginated).toHaveLength(PAGE_SIZE);
    });
});