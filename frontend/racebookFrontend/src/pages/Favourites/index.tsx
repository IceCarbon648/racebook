import { useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { getFavourites } from '../../services';
import { ModCard } from '../../components';
import type { Mod } from '../../types';

const Favourites = () => {
    const navigate = useNavigate();

    const { data: favourites = [], isLoading, isError } = useQuery({
        queryKey: ['favourites'],
        queryFn: getFavourites,
    });

    const handleModClick = (mod: Mod) => {
        navigate(`/mods/${mod.modId}`, { state: { mod } });
    };

    if (isLoading) return (
        <div className="flex items-center justify-center min-h-[calc(100vh-4rem)]">
            <p className="text-gray-500">Loading favourites...</p>
        </div>
    );

    if (isError) return (
        <div className="flex items-center justify-center min-h-[calc(100vh-4rem)]">
            <p className="text-red-500">Failed to load favourites</p>
        </div>
    );

    if (favourites.length === 0) return (
        <div className="flex items-center justify-center min-h-[calc(100vh-4rem)]">
            <p className="text-gray-500">You have no favourites yet</p>
        </div>
    );

    return (
        <div className="px-6 py-8">
            <h1 className="text-2xl font-bold text-gray-900 mb-6">Favourites</h1>
            <div className="grid grid-cols-[repeat(auto-fill,minmax(220px,1fr))] gap-6">
                {favourites.map((mod) => (
                    <ModCard
                        key={mod.modId}
                        title={mod.title}
                        type={mod.type}
                        imageUrl={mod.previewImageUrl}
                        creator={mod.creator}
                        onClick={() => handleModClick(mod)}
                    />
                ))}
            </div>
        </div>
    );
};

export default Favourites;