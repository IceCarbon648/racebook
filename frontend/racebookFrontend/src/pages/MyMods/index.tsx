import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { uploadMod, editMod, deleteMod, getMyMods } from '../../services';
import { ModCard, ModModal } from '../../components';
import type { MyMod } from '../../types';

const MyMods = () => {
    const queryClient = useQueryClient();
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [modalMode, setModalMode] = useState<'upload' | 'edit'>('upload');
    const [selectedMod, setSelectedMod] = useState<MyMod | null>(null);

    const { data: mods = [], isLoading, isError } = useQuery({
        queryKey: ['myMods'],
        queryFn: getMyMods,
    });

    const invalidate = () => {
        queryClient.invalidateQueries({ queryKey: ['myMods'] });
        queryClient.invalidateQueries({ queryKey: ['mods'] });
    };

    const uploadMutation = useMutation({
        mutationFn: uploadMod,
        onSuccess: invalidate,
    });

    const editMutation = useMutation({
        mutationFn: ({ modId, formData }: { modId: string; formData: FormData }) =>
            editMod(modId, formData),
        onSuccess: invalidate,
    });

    const deleteMutation = useMutation({
        mutationFn: deleteMod,
        onSuccess: invalidate,
    });

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

    const handleDeleteClick = (modId: string) => {
        const confirmed = window.confirm('Are you sure you want to delete this mod?');
        if (!confirmed) return;

        deleteMutation.mutate(modId);
    };

    const handleModalClose = () => {
        setIsModalOpen(false);
        setSelectedMod(null);
    };

    const handleModalSubmit = async (formData: FormData) => {
        if (modalMode === 'upload') {
            await uploadMutation.mutateAsync(formData);
        } else if (selectedMod) {
            await editMutation.mutateAsync({ modId: selectedMod.modId, formData });
        }
    };

    if (isLoading) return (
        <div className="flex items-center justify-center min-h-[calc(100vh-4rem)]">
            <p className="text-gray-500">Loading mods...</p>
        </div>
    );

    if (isError) return (
        <div className="flex items-center justify-center min-h-[calc(100vh-4rem)]">
            <p className="text-red-500">Failed to load mods</p>
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
                <div className="flex items-center justify-center min-h-50">
                    <p className="text-gray-500">You haven't uploaded any mods yet</p>
                </div>
            ) : (
                <div className="grid grid-cols-[repeat(auto-fill,minmax(220px,1fr))] gap-6">
                    {mods.map((mod) => (
                        <ModCard
                            key={mod.modId}
                            title={mod.title}
                            type={mod.type}
                            imageUrl={mod.imageUrl}
                            onEdit={() => handleEditClick(mod)}
                            onDelete={() => handleDeleteClick(mod.modId)}
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