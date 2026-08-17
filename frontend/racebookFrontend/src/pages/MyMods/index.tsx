import { useState, useEffect } from 'react';
import { uploadMod, editMod, deleteMod, getMyMods } from '../../services';
import { ModCard, ModModal } from '../../components';
import type { MyMod } from '../../types';

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
            <div className="flex items-center justify-between mb-6">
                <h1 className="text-2xl font-bold text-gray-900">My Mods</h1>
                <button
                    onClick={handleUploadClick}
                    className="px-4 py-2 text-sm font-medium border border-gray-900 rounded hover:bg-gray-50"
                >
                    Upload Mod
                </button>
            </div>
            {mods.length === 0 ? (
                <div className="flex items-center justify-center min-h-[200px]">
                    <p className="text-gray-500">You haven't uploaded any mods yet</p>
                </div>
            ) : (
                <div className="grid grid-cols-[repeat(auto-fill,minmax(220px,1fr))] gap-6">
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