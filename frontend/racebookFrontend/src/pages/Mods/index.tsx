import { useState, useEffect, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import { getAllMods } from '../../services';
import { ModCard } from '../../components';
import type { Mod } from '../../types';

const CATEGORIES = ['All', 'Vehicle', 'Environment', 'UI', 'Pack', 'Misc'];
const PAGE_SIZE = 16;

const Mods = () => {
    const navigate = useNavigate();
    const [mods, setMods] = useState<Mod[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [search, setSearch] = useState('');
    const [category, setCategory] = useState('All');
    const [order, setOrder] = useState<'newest' | 'oldest'>('newest');
    const [page, setPage] = useState(1);

    useEffect(() => {
        const fetchMods = async () => {
            try {
                const data = await getAllMods();
                setMods(data);
            } catch {
                setError('Failed to load mods');
            } finally {
                setIsLoading(false);
            }
        };

        fetchMods();
    }, []);

    const handleReset = () => {
        setSearch('');
        setCategory('All');
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

        if (category !== 'All') {
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

    const handleFilterChange = (setter: (v: any) => void) => (e: React.ChangeEvent<HTMLSelectElement | HTMLInputElement>) => {
        setter(e.target.value);
        setPage(1);
    };

    if (isLoading) return (
        <div className="flex items-center justify-center min-h-[calc(100vh-4rem)]">
            <p className="text-gray-500">Loading mods...</p>
        </div>
    );

    if (error) return (
        <div className="flex items-center justify-center min-h-[calc(100vh-4rem)]">
            <p className="text-red-500">{error}</p>
        </div>
    );

    return (
        <div className="px-6 py-8">
            <h1 className="text-2xl font-bold text-gray-900 mb-6">Mods</h1>

            {/* Controls */}
            <div className="flex items-center gap-3 mb-6">
                <input
                    type="text"
                    value={search}
                    onChange={handleFilterChange(setSearch)}
                    placeholder="Search by title or creator..."
                    className="flex-1 px-3 py-2 text-sm border border-gray-200 rounded focus:outline-none focus:border-gray-400"
                />
                <select
                    value={category}
                    onChange={handleFilterChange(setCategory)}
                    className="px-3 py-2 text-sm border border-gray-200 rounded focus:outline-none focus:border-gray-400"
                >
                    {CATEGORIES.map((c) => (
                        <option key={c} value={c}>{c}</option>
                    ))}
                </select>
                <select
                    value={order}
                    onChange={handleFilterChange(setOrder)}
                    className="px-3 py-2 text-sm border border-gray-200 rounded focus:outline-none focus:border-gray-400"
                >
                    <option value="newest">Newest first</option>
                    <option value="oldest">Oldest first</option>
                </select>
                <button
                    onClick={handleReset}
                    className="px-4 py-2 text-sm font-medium border border-gray-200 rounded hover:bg-gray-50"
                >
                    Reset
                </button>
            </div>

            {/* Grid */}
            {paginated.length === 0 ? (
                <div className="flex items-center justify-center min-h-[200px]">
                    <p className="text-gray-500">No mods found</p>
                </div>
            ) : (
                <div className="grid grid-cols-4 gap-6">
                    {paginated.map((mod) => (
                        <ModCard
                            key={mod.modId}
                            mod={mod}
                            onClick={handleModClick}
                        />
                    ))}
                </div>
            )}

            {/* Pagination */}
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
                            className={`px-3 py-1.5 text-sm border rounded ${
                                page === p
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