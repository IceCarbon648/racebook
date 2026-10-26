export interface ModFiltersProps {
    search: string;
    category: string;
    order: 'newest' | 'oldest';
    onSearchChange: (value: string) => void;
    onCategoryChange: (value: string) => void;
    onOrderChange: (value: 'newest' | 'oldest') => void;
    onReset: () => void;
}