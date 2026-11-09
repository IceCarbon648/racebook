import { useQuery } from '@tanstack/react-query';
import { getCategories } from '../services/api/categoryService';
import { queryKeys } from '../constants/queryKeys';

export const useCategories = () => {
    const { data: categories = [], isLoading, isError } = useQuery({
        queryKey: queryKeys.categories,
        queryFn: getCategories,
        staleTime: Infinity,
    });

    return { categories, isLoading, isError };
};