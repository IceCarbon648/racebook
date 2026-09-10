import { Dropdown } from '../../atoms';
import { CATEGORIES } from '../../../constants/categories';
import type { ModFiltersProps } from './index.types';

const ORDER_LABELS = {
    newest: 'Newest first',
    oldest: 'Oldest first',
} as const;

const ModFilters = ({
    search, category, order,
    onSearchChange, onCategoryChange, onOrderChange, onReset,
}: ModFiltersProps) => {
    return (
        <div className="flex items-center gap-3 mb-6">
            <input
                type="text"
                value={search}
                onChange={(e) => onSearchChange(e.target.value)}
                placeholder="Search by title or creator..."
                className="flex-1 px-3 py-2 text-sm border border-gray-200 rounded focus:outline-none focus:border-gray-400"
            />
            <Dropdown
                value={category}
                options={CATEGORIES}
                onChange={onCategoryChange}
            />
            <Dropdown
                value={ORDER_LABELS[order]}
                options={[ORDER_LABELS.newest, ORDER_LABELS.oldest]}
                onChange={(v) => onOrderChange(v === ORDER_LABELS.newest ? 'newest' : 'oldest')}
            />
            <button
                onClick={onReset}
                className="px-4 py-2 text-sm font-medium border border-gray-200 rounded hover:bg-gray-50"
            >
                Reset
            </button>
        </div>
    );
};

export default ModFilters;