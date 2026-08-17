import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { getAllMods } from '../../services';
import { ModCard } from '../../components';
import type { Mod } from '../../types';

const Mods = () => {
    const navigate = useNavigate();
    const [mods, setMods] = useState<Mod[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

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

    const handleModClick = (mod: Mod) => {
        navigate(`/mods/${mod.modId}`, { state: { mod } });
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

    if (mods.length === 0) return (
        <div className="flex items-center justify-center min-h-[calc(100vh-4rem)]">
            <p className="text-gray-500">No mods found</p>
        </div>
    );

    return (
        <div className="px-6 py-8">
            <h1 className="text-2xl font-bold text-gray-900 mb-6">Mods</h1>
            <div className="grid grid-cols-[repeat(auto-fill,minmax(220px,1fr))] gap-6">
                {mods.map((mod) => (
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

export default Mods;