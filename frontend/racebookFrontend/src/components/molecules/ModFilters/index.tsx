import { CATEGORIES } from '../../../constants/categories';
import { PRIMARY_BUTTON_COLOUR, PRIMARY_BUTTON_HOVER_COLOUR } from '../../../constants/theme';
import Dropdown from '../Dropdown';
import type { ModFiltersProps } from './index.types';

const ORDER_LABELS = {
    newest: 'Newest first',
    oldest: 'Oldest first',
} as const;

const baseClass =
    'text-sm text-gray-300 border border-gray-400 rounded bg-black/50 hover:bg-white/10';

const ModFilters = ({
    search, category, order,
    onSearchChange, onCategoryChange, onOrderChange, onReset,
}: ModFiltersProps) => {
    return (
        <div className="flex justify-center gap-3 mb-6">
            <input
                type="text"
                value={search}
                onChange={(e) => onSearchChange(e.target.value)}
                placeholder="Search by title or creator..."
                className={`px-3 py-2 focus:outline-none w-1/3 focus:border-white ${baseClass}`}
            />

            <Dropdown
                value={category}
                options={CATEGORIES}
                label="Filter by category"
                onChange={onCategoryChange}
                className="w-32"
            />
            <Dropdown
                value={ORDER_LABELS[order]}
                options={[ORDER_LABELS.newest, ORDER_LABELS.oldest]}
                label="Sort order"
                onChange={(v) => onOrderChange(v === ORDER_LABELS.newest ? 'newest' : 'oldest')}
                className="w-32"
            />

            <button
                onClick={onReset}
                className={`px-4 py-2 text-sm font-medium text-white rounded bg-[${PRIMARY_BUTTON_COLOUR}] hover:bg-[${PRIMARY_BUTTON_HOVER_COLOUR}] cursor-pointer`}
            >
                Reset
            </button>
        </div>
    );
};

export default ModFilters;
