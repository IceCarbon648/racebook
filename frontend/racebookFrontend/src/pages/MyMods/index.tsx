import { useState } from 'react';
import { ModCard, ModModal, Pagination } from '../../components';
import type { MyMod } from '../../types';
import Background from '../../components/molecules/Background/Index';
import { PRIMARY_BUTTON_COLOUR, PRIMARY_BUTTON_HOVER_COLOUR } from '../../constants/theme';
import { useMyMods } from '../../hooks/useMyMods';
import { usePagination } from '../../hooks/usePagination';

const MyMods = () => {
    const { mods, isLoading, isError, uploadMod, editMod, deleteMod } = useMyMods();
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [modalMode, setModalMode] = useState<'upload' | 'edit'>('upload');
    const [selectedMod, setSelectedMod] = useState<MyMod | null>(null);
    const { page, setPage, totalPages, paginated } = usePagination(mods);

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

    const handleModalClose = () => {
        setIsModalOpen(false);
        setSelectedMod(null);
    };

    const handleModalSubmit = async (formData: FormData) => {
        if (modalMode === 'upload') {
            await uploadMod(formData);
        } else if (selectedMod) {
            await editMod(selectedMod.modId, formData);
        }
    };

    const handleDeleteClick = (modId: string) => {
        const confirmed = window.confirm('Are you sure you want to delete this mod?');
        if (!confirmed) return;

        deleteMod(modId);
    };

    if (page > totalPages && totalPages > 0) setPage(totalPages);

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
            <Background reveal={false} />
            <div className="flex items-center justify-between mb-6">
                <h1 className="text-[40px] font-bold text-white">My Mods</h1>
                <button
                    onClick={handleUploadClick}
                    className={`px-4 py-2 bg-[${PRIMARY_BUTTON_COLOUR}] hover:bg-[${PRIMARY_BUTTON_HOVER_COLOUR}] text-sm text-white rounded`}
                >
                    Upload Mod
                </button>
            </div>
            {mods.length === 0 ? (
                <div className="flex items-center justify-center min-h-50">
                    <p className="text-white">You haven't uploaded any mods yet</p>
                </div>
            ) : (
                <>
                    <div className="flex justify-center">
                        <div className="grid grid-cols-[repeat(4,16rem)] justify-around gap-y-16 w-9/10">
                            {paginated.map((mod) => (
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
                    </div>
                    <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
                </>
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