import { useState, useEffect } from 'react';
import { uploadMod, editMod, deleteMod, getMyMods } from '../../services';
import { ModCard, ModModal } from '../../components';
import type { MyMod } from '../../types';
import './index.css';

const MyMods = () => {
    const [mods, setMods] = useState<MyMod[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [modalMode, setModalMode] = useState<'upload' | 'edit'>('upload');
    const [selectedMod, setSelectedMod] = useState<MyMod | null>(null);

    useEffect(() => {
        fetchMods();
    }, []);

    const fetchMods = async () => {
        try {
            const data = await getMyMods();
            setMods(data);
        } catch {
            setError('Failed to load mods');
        } finally {
            setIsLoading(false);
        }
    };

    const handleUploadClick = () => {
        setSelectedMod(null);
        setModalMode('upload');
        setIsModalOpen(true);
    };

    const handleEditClick = (mod: MyMod) => {
        setSelectedMod(mod);
        setModalMode('edit');
        setIsModalOpen(true);
    };

    const handleDeleteClick = async (modId: string) => {
        const confirmed = window.confirm('Are you sure you want to delete this mod?');
        if (!confirmed) return;

        try {
            await deleteMod(modId);
            setMods((prev) => prev.filter((m) => m.modId !== modId));
        } catch {
            setError('Failed to delete mod');
        }
    };

    const handleModalClose = () => {
        setIsModalOpen(false);
        setSelectedMod(null);
    };

    const handleModalSubmit = async (formData: FormData) => {
        if (modalMode === 'upload') {
            await uploadMod(formData);
            await fetchMods();
        } else if (selectedMod) {
            await editMod(selectedMod.modId, formData);
            await fetchMods();
        }
    };

    if (isLoading) return <div className="my-mods-status">Loading mods...</div>;
    if (error) return <div className="my-mods-status">{error}</div>;

    return (
        <div className="my-mods">
            <div className="my-mods-header">
                <h1>My Mods</h1>
                <button className="my-mods-upload" onClick={handleUploadClick}>
                    Upload Mod
                </button>
            </div>
            {mods.length === 0 ? (
                <div className="my-mods-empty">
                    You haven't uploaded any mods yet
                </div>
            ) : (
                <div className="my-mods-grid">
                    {mods.map((mod) => (
                        <ModCard
                            key={mod.modId}
                            mod={mod}
                            onEdit={handleEditClick}
                            onDelete={handleDeleteClick}
                        />
                    ))}
                </div>
            )}
            <ModModal
                isOpen={isModalOpen}
                mode={modalMode}
                mod={selectedMod ?? undefined}
                onClose={handleModalClose}
                onSubmit={handleModalSubmit}
            />
        </div>
    );
};

export default MyMods;