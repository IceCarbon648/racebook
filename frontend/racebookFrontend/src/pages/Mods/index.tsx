import { useState, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import type { Mod } from '../../types';
import { ModCard } from '../../components';
import { ModFilters, Pagination } from '../../components/molecules';
import Background from '../../components/molecules/Background/Index';
import { useMods } from '../../hooks/useMods';
import { useFavouriteMod } from '../../hooks/useFavouriteMod';
import { usePagination } from '../../hooks/usePagination';

const Mods = () => {
    const navigate = useNavigate();
    const [search, setSearch] = useState('');
    const [category, setCategory] = useState('ALL TYPES');
    const [order, setOrder] = useState<'newest' | 'oldest'>('newest');

    const { mods, isLoading, isError } = useMods();
    const { toggleFavourite } = useFavouriteMod();

    const handleReset = () => {
        setSearch('');
        setCategory('ALL TYPES');
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

        if (category !== 'ALL TYPES') {
            result = result.filter((m) => m.type === category);
        }

        result.sort((a, b) => {
            const dateA = new Date(a.uploadDate).getTime();
            const dateB = new Date(b.uploadDate).getTime();
            return order === 'newest' ? dateB - dateA : dateA - dateB;
        });

        return result;
    }, [mods, search, category, order]);

    const { page, setPage, totalPages, paginated } = usePagination(filtered);

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
        <div className="flex flex-col gap-8 px-6 py-8">
            <Background reveal={false} />
            <h1 className="text-[40px] font-bold text-white">Mods</h1>

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
                <div className="flex justify-center">
                    <div className="grid grid-cols-[repeat(4,16rem)] justify-around gap-y-16 w-9/10">
                        {paginated.map((mod) => (
                            <ModCard
                                key={mod.modId}
                                title={mod.title}
                                type={mod.type}
                                imageUrl={mod.previewImageUrl}
                                creator={mod.creator}
                                isFavourite={mod.isFavourite ?? undefined}
                                onClick={() => handleModClick(mod)}
                                onFavourite={() => toggleFavourite(mod.modId, mod.isFavourite!)}
                            />
                        ))}
                    </div>
                </div>
            )}

            <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
        </div>
    );
};

export default Mods;