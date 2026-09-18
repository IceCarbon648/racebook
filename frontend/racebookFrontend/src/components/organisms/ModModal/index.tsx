import { useState, useEffect } from 'react';
import FormPanel from '../../molecules/FormPanel';
import { CATEGORIES } from '../../../constants/categories';
import type { ModModalProps } from './index.types';

const ModModal = ({ isOpen, mode, mod, onClose, onSubmit }: ModModalProps) => {
    const [title, setTitle] = useState('');
    const [type, setType] = useState('');
    const [description, setDescription] = useState('');
    const [modFile, setModFile] = useState<File | null>(null);
    const [previewImage, setPreviewImage] = useState<File | null>(null);
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        if (mode === 'edit' && mod) {
            setTitle(mod.title);
            setType(mod.type);
            setDescription(mod.description);
        } else {
            setTitle('');
            setType('');
            setDescription('');
            setModFile(null);
            setPreviewImage(null);
        }
        setError(null);
    }, [isOpen, mode, mod]);

    const handleSubmit = async () => {
        setError(null);

        if (mode === 'upload') {
            if (!title || !type || !description || !modFile || !previewImage) {
                setError('All fields are required');
                return;
            }
        }

        const formData = new FormData();
        if (title) formData.append('title', title);
        if (type) formData.append('type', type);
        if (description) formData.append('description', description);
        if (modFile) formData.append('modFile', modFile);
        if (previewImage) formData.append('previewImage', previewImage);

        setIsLoading(true);
        try {
            await onSubmit(formData);
            onClose();
        } catch (err: any) {
            const data = err?.response?.data;
            const fieldErrors = data?.errors
                ? Object.values(data.errors).flat().join(' ')
                : null;

            setError(
                fieldErrors
                ?? data?.message
                ?? data?.detail
                ?? 'Something went wrong, please try again'
            );
        } finally {
            setIsLoading(false);
        }
    };

    if (!isOpen) return null;

    const inputClass = "px-3 py-2 bg-black/50 text-sm text-gray-300 border border-gray-400 rounded focus:outline-none focus:border-gray-200";

    return (
        <div
            className="fixed inset-0 bg-black/60 flex items-center justify-center z-50"
            onClick={onClose}
        >
            <div onClick={(e) => e.stopPropagation()}>
                <FormPanel>
                    <div className="flex items-center justify-between">
                        <h2 className="text-2xl font-bold text-white">
                            {mode === 'upload' ? 'Upload Mod' : 'Edit Mod'}
                        </h2>
                        <button
                            onClick={onClose}
                            className="text-gray-400 hover:text-white text-xl cursor-pointer"
                            aria-label="Close"
                        >
                            ✕
                        </button>
                    </div>

                    <div className="flex flex-col gap-4">
                        {error && <p className="text-sm text-red-400">{error}</p>}

                        <div className="flex flex-col gap-1.5">
                            <label htmlFor="title" className="flex justify-start font-medium text-white">
                                Title
                            </label>
                            <input
                                id="title"
                                type="text"
                                value={title}
                                onChange={(e) => setTitle(e.target.value)}
                                placeholder="Enter mod title"
                                className={inputClass}
                            />
                        </div>

                        <div className="flex flex-col gap-1.5">
                            <label htmlFor="type" className="flex justify-start font-medium text-white">
                                Category
                            </label>
                            <select
                                id="type"
                                value={type}
                                onChange={(e) => setType(e.target.value)}
                                className={inputClass}
                            >
                                <option value="">Select a category</option>
                                {CATEGORIES.filter((c) => c !== 'ALL').map((c) => (
                                    <option key={c} value={c}>{c}</option>
                                ))}
                            </select>
                        </div>

                        <div className="flex flex-col gap-1.5">
                            <label htmlFor="description" className="flex justify-start font-medium text-white">
                                Description
                            </label>
                            <textarea
                                id="description"
                                value={description}
                                onChange={(e) => setDescription(e.target.value)}
                                placeholder="Enter mod description"
                                rows={3}
                                className={`${inputClass} resize-none`}
                            />
                        </div>

                        <div className="flex flex-col gap-1.5">
                            <label htmlFor="modFile" className="flex justify-start font-medium text-white">
                                Mod File{' '}
                                {mode === 'edit' && (
                                    <span className="text-xs text-gray-400 font-normal ml-1">(optional)</span>
                                )}
                            </label>
                            <input
                                id="modFile"
                                type="file"
                                accept=".tpf"
                                onChange={(e) => setModFile(e.target.files?.[0] ?? null)}
                                className="text-sm text-gray-400 file:mr-3 file:py-1.5 file:px-3 file:border file:border-gray-400 file:rounded file:text-xs file:font-medium file:bg-black/50 file:text-gray-300 hover:file:bg-black/70"
                            />
                        </div>

                        <div className="flex flex-col gap-1.5">
                            <label htmlFor="previewImage" className="flex justify-start font-medium text-white">
                                Preview Image{' '}
                                {mode === 'edit' && (
                                    <span className="text-xs text-gray-400 font-normal ml-1">(optional)</span>
                                )}
                            </label>
                            <input
                                id="previewImage"
                                type="file"
                                accept=".png,.jpg,.jpeg"
                                onChange={(e) => setPreviewImage(e.target.files?.[0] ?? null)}
                                className="text-sm text-gray-400 file:mr-3 file:py-1.5 file:px-3 file:border file:border-gray-400 file:rounded file:text-xs file:font-medium file:bg-black/50 file:text-gray-300 hover:file:bg-black/70"
                            />
                        </div>
                    </div>

                    <div className="flex justify-end gap-3">
                        <button
                            onClick={onClose}
                            className="px-4 py-2 text-sm font-medium border border-gray-400 text-gray-300 rounded hover:bg-white/10 cursor-pointer"
                        >
                            CANCEL
                        </button>
                        <button
                            onClick={handleSubmit}
                            disabled={isLoading}
                            className="px-4 py-2 bg-[#930093] hover:bg-[#600060] text-sm text-white rounded disabled:opacity-50 disabled:cursor-not-allowed cursor-pointer"
                        >
                            {isLoading ? 'SAVING...' : mode === 'upload' ? 'UPLOAD' : 'SAVE CHANGES'}
                        </button>
                    </div>
                </FormPanel>
            </div>
        </div>
    );
};

export default ModModal;