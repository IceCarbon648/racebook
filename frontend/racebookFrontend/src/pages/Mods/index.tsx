import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { getAllMods } from '../../services';
import { ModCard } from '../../components';
import type { Mod } from '../../types';
import './index.css';

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

    if (isLoading) return <div className="mods-status">Loading mods...</div>;
    if (error) return <div className="mods-status">{error}</div>;
    if (mods.length === 0) return <div className="mods-status">No mods found</div>;

    return (
        <div className="mods">
            <h1>Mods</h1>
            <div className="mods-grid">
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