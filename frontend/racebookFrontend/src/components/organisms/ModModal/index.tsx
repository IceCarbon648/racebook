import { useState, useEffect } from 'react';
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
        } catch {
            setError('Something went wrong, please try again');
        } finally {
            setIsLoading(false);
        }
    };

    if (!isOpen) return null;

    return (
        <div
            className="fixed inset-0 bg-black/50 flex items-center justify-center z-50"
            onClick={onClose}
        >
            <div
                className="bg-white rounded-lg w-full max-w-md max-h-[90vh] overflow-y-auto flex flex-col"
                onClick={(e) => e.stopPropagation()}
            >
                <div className="flex items-center justify-between px-6 py-4 border-b border-gray-200">
                    <h2 className="text-lg font-semibold text-gray-900">
                        {mode === 'upload' ? 'Upload Mod' : 'Edit Mod'}
                    </h2>
                    <button
                        onClick={onClose}
                        className="text-gray-400 hover:text-gray-600 text-xl"
                    >
                        ✕
                    </button>
                </div>
                <div className="flex flex-col gap-4 px-6 py-4">
                    {error && <p className="text-sm text-red-500">{error}</p>}
                    <div className="flex flex-col gap-1.5">
                        <label htmlFor="title" className="text-sm font-medium text-gray-700">
                            Title
                        </label>
                        <input
                            id="title"
                            type="text"
                            value={title}
                            onChange={(e) => setTitle(e.target.value)}
                            placeholder="Enter mod title"
                            className="px-3 py-2 text-sm border border-gray-200 rounded focus:outline-none focus:border-gray-400"
                        />
                    </div>
                    <div className="flex flex-col gap-1.5">
                        <label htmlFor="type" className="text-sm font-medium text-gray-700">
                            Category
                        </label>
                        <select
                            id="type"
                            value={type}
                            onChange={(e) => setType(e.target.value)}
                            className="px-3 py-2 text-sm border border-gray-200 rounded focus:outline-none focus:border-gray-400"
                        >
                            <option value="">Select a category</option>
                            <option value="Vehicle">Vehicle</option>
                            <option value="Environment">Environment</option>
                            <option value="UI">UI</option>
                            <option value="Pack">Pack</option>
                            <option value="Misc">Misc</option>
                        </select>
                    </div>
                    <div className="flex flex-col gap-1.5">
                        <label htmlFor="description" className="text-sm font-medium text-gray-700">
                            Description
                        </label>
                        <textarea
                            id="description"
                            value={description}
                            onChange={(e) => setDescription(e.target.value)}
                            placeholder="Enter mod description"
                            rows={4}
                            className="px-3 py-2 text-sm border border-gray-200 rounded focus:outline-none focus:border-gray-400 resize-none"
                        />
                    </div>
                    <div className="flex flex-col gap-1.5">
                        <label htmlFor="modFile" className="text-sm font-medium text-gray-700">
                            Mod File{' '}
                            {mode === 'edit' && (
                                <span className="text-xs text-gray-400 font-normal">(optional)</span>
                            )}
                        </label>
                        <input
                            id="modFile"
                            type="file"
                            accept=".tpf"
                            onChange={(e) => setModFile(e.target.files?.[0] ?? null)}
                            className="text-sm text-gray-500 file:mr-3 file:py-1.5 file:px-3 file:border file:border-gray-200 file:rounded file:text-xs file:font-medium file:bg-white hover:file:bg-gray-50"
                        />
                    </div>
                    <div className="flex flex-col gap-1.5">
                        <label htmlFor="previewImage" className="text-sm font-medium text-gray-700">
                            Preview Image{' '}
                            {mode === 'edit' && (
                                <span className="text-xs text-gray-400 font-normal">(optional)</span>
                            )}
                        </label>
                        <input
                            id="previewImage"
                            type="file"
                            accept=".png,.jpg,.jpeg"
                            onChange={(e) => setPreviewImage(e.target.files?.[0] ?? null)}
                            className="text-sm text-gray-500 file:mr-3 file:py-1.5 file:px-3 file:border file:border-gray-200 file:rounded file:text-xs file:font-medium file:bg-white hover:file:bg-gray-50"
                        />
                    </div>
                </div>
                <div className="flex justify-end gap-3 px-6 py-4 border-t border-gray-200">
                    <button
                        onClick={onClose}
                        className="px-4 py-2 text-sm font-medium border border-gray-200 rounded hover:bg-gray-50"
                    >
                        Cancel
                    </button>
                    <button
                        onClick={handleSubmit}
                        disabled={isLoading}
                        className="px-4 py-2 text-sm font-medium border border-gray-900 rounded hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
                    >
                        {isLoading ? 'Saving...' : mode === 'upload' ? 'Upload' : 'Save Changes'}
                    </button>
                </div>
            </div>
        </div>
    );
};

export default ModModal;