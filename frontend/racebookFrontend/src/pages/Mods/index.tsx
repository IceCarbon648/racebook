import { useState, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getAllMods, addToFavourites, deleteFromFavourites } from '../../services';
import type { Mod } from '../../types';
import { ModCard } from '../../components';
import { ModFilters, Pagination } from '../../components/molecules';
import { PAGE_SIZE } from '../../constants/pagination';
import Background from '../../components/molecules/Background/Index';

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

    if (page > totalPages && totalPages > 0) setPage(totalPages);

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
            <Background reveal={false} />
            <h1 className="text-2xl font-bold text-gray-900 mb-6">Mods</h1>

            <ModFilters
                search={search}
                category={category}
                order={order}
                onSearchChange={(v) => { setSearch(v); setPage(1); }}
                onCategoryChange={(v) => { setCategory(v); setPage(1); }}
                onOrderChange={(v) => { setOrder(v); setPage(1); }}
                onReset={handleReset}
            />

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

            <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
        </div>
    );
};

export default Mods;