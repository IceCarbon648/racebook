import { useState } from 'react';
import { PAGE_SIZE } from '../constants/pagination';

export const usePagination = <T,>(items: T[], pageSize: number = PAGE_SIZE) => {
    const [page, setPage] = useState(1);

    const totalPages = Math.ceil(items.length / pageSize);
    const safePage = Math.min(page, Math.max(totalPages, 1));

    const paginated = items.slice((safePage - 1) * pageSize, safePage * pageSize);

    return { page: safePage, setPage, totalPages, paginated };
};