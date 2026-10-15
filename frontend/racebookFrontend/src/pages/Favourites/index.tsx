import { useNavigate } from 'react-router-dom';
import { ModCard } from '../../components';
import type { Mod } from '../../types';
import Background from '../../components/molecules/Background/Index';
import { Pagination } from '../../components/molecules';
import { useFavouriteMod } from '../../hooks/useFavouriteMod';
import { useFavourites } from '../../hooks/useFavourites';
import { usePagination } from '../../hooks/usePagination';

const Favourites = () => {
    const navigate = useNavigate();

    const { favourites, isLoading, isError } = useFavourites();
    const { toggleFavourite } = useFavouriteMod();
    const { page, setPage, totalPages, paginated } = usePagination(favourites);

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
            <p className="text-white">You have no favourites yet</p>
        </div>
    );

    return (
        <div className="flex flex-col gap-8 px-6 py-8">
            <Background reveal={false} />
            <h1 className="text-[40px] font-bold text-white">Favourites</h1>

            <div className="flex justify-center">
                <div className="grid grid-cols-[repeat(4,16rem)] justify-around gap-y-16 w-9/10">
                    {paginated.map((mod) => (
                        <ModCard
                            key={mod.modId}
                            title={mod.title}
                            type={mod.type}
                            imageUrl={mod.previewImageUrl}
                            creator={mod.creator}
                            isFavourite={true}
                            onClick={() => handleModClick(mod)}
                            onFavourite={() => toggleFavourite(mod.modId, true)}
                        />
                    ))}
                </div>
            </div>

            <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
        </div>
    );
};

export default Favourites;
