import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { getFavourites } from '../../services';
import { ModCard } from '../../components';
import type { Mod } from '../../types';
import './index.css';

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

    if (isLoading) return <div className="favourites-status">Loading favourites...</div>;
    if (error) return <div className="favourites-status">{error}</div>;
    if (favourites.length === 0) return <div className="favourites-status">You have no favourites yet</div>;

    return (
        <div className="favourites">
            <h1>Favourites</h1>
            <div className="favourites-grid">
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