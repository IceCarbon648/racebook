import { useState, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getAllMods, addToFavourites, deleteFromFavourites } from '../../services';
import type { Mod } from '../../types';
import { ModCard, Dropdown } from '../../components';

const CATEGORIES = ['ALL', 'VEHICLE', 'ENVIRONMENT', 'UI', 'PACK', 'MISC'];
const PAGE_SIZE = 16;

const Mods = () => {
    const navigate = useNavigate();
    const queryClient = useQueryClient();
    const [search, setSearch] = useState('');
    const [category, setCategory] = useState('ALL');
    const [order, setOrder] = useState<'newest' | 'oldest'>('newest');
    const [page, setPage] = useState(1);

    const { data: mods = [], isLoading, isError } = useQuery({
        queryKey: ['mods'],
        queryFn: getAllMods,
    });

    const favouriteMutation = useMutation({
        mutationFn: ({ modId, isFavourite }: { modId: string; isFavourite: boolean }) =>
            isFavourite ? deleteFromFavourites(modId) : addToFavourites(modId),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['mods'] });
            queryClient.invalidateQueries({ queryKey: ['favourites'] });
        },
    });

    const handleReset = () => {
        setSearch('');
        setCategory('ALL');
        setOrder('newest');
        setPage(1);
    };

    const filtered = useMemo(() => {
        let result = [...mods];

        if (search.trim()) {
            const keyword = search.toLowerCase();
            result = result.filter(
                (m) =>
                    m.title.toLowerCase().includes(keyword) ||
                    m.creator.toLowerCase().includes(keyword)
            );
        }

        if (category !== 'ALL') {
            result = result.filter((m) => m.type === category);
        }

        result.sort((a, b) => {
            const dateA = new Date(a.uploadDate).getTime();
            const dateB = new Date(b.uploadDate).getTime();
            return order === 'newest' ? dateB - dateA : dateA - dateB;
        });

        return result;
    }, [mods, search, category, order]);

    const totalPages = Math.ceil(filtered.length / PAGE_SIZE);
    const paginated = filtered.slice((page - 1) * PAGE_SIZE, page * PAGE_SIZE);

    const handleModClick = (mod: Mod) => {
        navigate(`/mods/${mod.modId}`, { state: { mod } });
    };

    if (isLoading) return (
        <div className="flex items-center justify-center min-h-[calc(100vh-4rem)]">
            <p className="text-gray-500">Loading mods...</p>
        </div>
    );

    if (isError) return (
        <div className="flex items-center justify-center min-h-[calc(100vh-4rem)]">
            <p className="text-red-500">Failed to load mods</p>
        </div>
    );

    return (
        <div className="px-6 py-8">
            <h1 className="text-2xl font-bold text-gray-900 mb-6">Mods</h1>

            <div className="flex items-center gap-3 mb-6">
                <input
                    type="text"
                    value={search}
                    onChange={(e) => { setSearch(e.target.value); setPage(1); }}
                    placeholder="Search by title or creator..."
                    className="flex-1 px-3 py-2 text-sm border border-gray-200 rounded focus:outline-none focus:border-gray-400"
                />
                <Dropdown
                    value={category}
                    options={CATEGORIES}
                    onChange={(v) => { setCategory(v); setPage(1); }}
                />
                <Dropdown
                    value={order === 'newest' ? 'Newest first' : 'Oldest first'}
                    options={['Newest first', 'Oldest first']}
                    onChange={(v) => { setOrder(v === 'Newest first' ? 'newest' : 'oldest'); setPage(1); }}
                />
                <button
                    onClick={handleReset}
                    className="px-4 py-2 text-sm font-medium border border-gray-200 rounded hover:bg-gray-50"
                >
                    Reset
                </button>
            </div>

            {paginated.length === 0 ? (
                <div className="flex items-center justify-center min-h-50">
                    <p className="text-gray-500">No mods found</p>
                </div>
            ) : (
                <div className="grid grid-cols-4 gap-6">
                    {paginated.map((mod) => (
                        <ModCard
                            key={mod.modId}
                            title={mod.title}
                            type={mod.type}
                            imageUrl={mod.previewImageUrl}
                            creator={mod.creator}
                            isFavourite={mod.isFavourite ?? undefined}
                            onClick={() => handleModClick(mod)}
                            onFavourite={() => favouriteMutation.mutate({ modId: mod.modId, isFavourite: mod.isFavourite! })}
                        />
                    ))}
                </div>
            )}

            {totalPages > 1 && (
                <div className="flex items-center justify-center gap-2 mt-8">
                    <button
                        onClick={() => setPage((p) => p - 1)}
                        disabled={page === 1}
                        className="px-3 py-1.5 text-sm border border-gray-200 rounded hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
                    >
                        Previous
                    </button>
                    {Array.from({ length: totalPages }, (_, i) => i + 1).map((p) => (
                        <button
                            key={p}
                            onClick={() => setPage(p)}
                            className={`px-3 py-1.5 text-sm border rounded ${page === p
                                ? 'border-gray-900 bg-gray-900 text-white'
                                : 'border-gray-200 hover:bg-gray-50'
                                }`}
                        >
                            {p}
                        </button>
                    ))}
                    <button
                        onClick={() => setPage((p) => p + 1)}
                        disabled={page === totalPages}
                        className="px-3 py-1.5 text-sm border border-gray-200 rounded hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
                    >
                        Next
                    </button>
                </div>
            )}
        </div>
    );
};

export default Mods;