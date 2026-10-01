import { useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { getFavourites } from '../../services';
import { ModCard } from '../../components';
import type { Mod } from '../../types';
import Background from '../../components/molecules/Background/Index';

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
        <div className="flex flex-col gap-8 px-6 py-8">
            <Background reveal={false}/>
            <h1 className="text-[40px] font-bold text-white">Favourites</h1>

            <div className="flex justify-center">
                <div className="grid grid-cols-[repeat(4,16rem)] justify-around gap-y-16 w-9/10">
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
        </div>
    );
};

export default Favourites;