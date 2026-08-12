import { useState, useEffect } from 'react';
import type { ModModalProps } from './index.types';
import './index.css';

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
        <div className="modal-overlay" onClick={onClose}>
            <div className="modal" onClick={(e) => e.stopPropagation()}>
                <div className="modal-header">
                    <h2>{mode === 'upload' ? 'Upload Mod' : 'Edit Mod'}</h2>
                    <button className="modal-close" onClick={onClose}>✕</button>
                </div>
                <div className="modal-body">
                    {error && <p className="modal-error">{error}</p>}
                    <div className="modal-field">
                        <label htmlFor="title">Title</label>
                        <input
                            id="title"
                            type="text"
                            value={title}
                            onChange={(e) => setTitle(e.target.value)}
                            placeholder="Enter mod title"
                        />
                    </div>
                    <div className="modal-field">
                        <label htmlFor="type">Category</label>
                        <input
                            id="type"
                            type="text"
                            value={type}
                            onChange={(e) => setType(e.target.value)}
                            placeholder="Enter mod category"
                        />
                    </div>
                    <div className="modal-field">
                        <label htmlFor="description">Description</label>
                        <textarea
                            id="description"
                            value={description}
                            onChange={(e) => setDescription(e.target.value)}
                            placeholder="Enter mod description"
                            rows={4}
                        />
                    </div>
                    <div className="modal-field">
                        <label htmlFor="modFile">
                            Mod File {mode === 'edit' && <span className="modal-optional">(optional)</span>}
                        </label>
                        <input
                            id="modFile"
                            type="file"
                            accept=".tpf"
                            onChange={(e) => setModFile(e.target.files?.[0] ?? null)}
                        />
                    </div>
                    <div className="modal-field">
                        <label htmlFor="previewImage">
                            Preview Image {mode === 'edit' && <span className="modal-optional">(optional)</span>}
                        </label>
                        <input
                            id="previewImage"
                            type="file"
                            accept=".png,.jpg,.jpeg"
                            onChange={(e) => setPreviewImage(e.target.files?.[0] ?? null)}
                        />
                    </div>
                </div>
                <div className="modal-footer">
                    <button className="modal-cancel" onClick={onClose}>
                        Cancel
                    </button>
                    <button
                        className="modal-submit"
                        onClick={handleSubmit}
                        disabled={isLoading}
                    >
                        {isLoading ? 'Saving...' : mode === 'upload' ? 'Upload' : 'Save Changes'}
                    </button>
                </div>
            </div>
        </div>
    );
};

export default ModModal;