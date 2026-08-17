import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { getFavourites } from '../../services';
import { ModCard } from '../../components';
import type { Mod } from '../../types';

const Favourites = () => {
    const navigate = useNavigate();
    const [favourites, setFavourites] = useState<Mod[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchFavourites = async () => {
            try {
                const data = await getFavourites();
                setFavourites(data);
            } catch {
                setError('Failed to load favourites');
            } finally {
                setIsLoading(false);
            }
        };

        fetchFavourites();
    }, []);

    const handleModClick = (mod: Mod) => {
        navigate(`/mods/${mod.modId}`, { state: { mod } });
    };

    if (isLoading) return (
        <div className="flex items-center justify-center min-h-[calc(100vh-4rem)]">
            <p className="text-gray-500">Loading favourites...</p>
        </div>
    );

    if (error) return (
        <div className="flex items-center justify-center min-h-[calc(100vh-4rem)]">
            <p className="text-red-500">{error}</p>
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
                        mod={mod}
                        onClick={handleModClick}
                    />
                ))}
            </div>
        </div>
    );
};

export default Favourites;