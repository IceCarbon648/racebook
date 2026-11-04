import { useState, useEffect } from 'react';
import FormPanel from '../../molecules/FormPanel';
import Dropdown from '../../molecules/Dropdown';
import { FilePicker } from '../../atoms';
import type { ModModalProps } from './index.types';
import { PRIMARY_BUTTON_COLOUR, PRIMARY_BUTTON_HOVER_COLOUR } from '../../../constants/theme';
import { MAX_MOD_TITLE_LENGTH, MAX_MOD_DESCRIPTION_LENGTH } from '../../../constants/inputLimits';
import { useCategories } from '../../../hooks/useCategories';
import { getFileNameFromUrl } from '../../../helpers/getFileNameFromUrl';
import { modUploadSchema, modEditSchema, validate } from '../../../validation';
import { getResponseMessage } from '../../../utils/getResponseMessage';
import { toast } from 'react-toastify';

const ModModal = ({ isOpen, mode, mod, onClose, onSubmit }: ModModalProps) => {
    const [title, setTitle] = useState('');
const [type, setType] = useState('');
const [description, setDescription] = useState('');
const [modFile, setModFile] = useState<File | null>(null);
const [previewImage, setPreviewImage] = useState<File | null>(null);
const [isLoading, setIsLoading] = useState(false);
const [error, setError] = useState<string | null>(null);
const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});

const {
    categories,
    isLoading: isLoadingCategories,
    isError: isCategoriesError,
} = useCategories();

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
    setFieldErrors({});
}, [isOpen, mode, mod]);

const handleSubmit = async () => {
    setError(null);

    const schema = mode === 'upload' ? modUploadSchema(categories) : modEditSchema(categories);

    const errors = await validate(schema, { title, type, description, modFile, previewImage });

    setFieldErrors(errors ?? {});

    if (errors) return;

    const formData = new FormData();
    if (title) formData.append('title', title);
    if (type) formData.append('type', type);
    if (description) formData.append('description', description);
    if (modFile) formData.append('modFile', modFile);
    if (previewImage) formData.append('previewImage', previewImage);

    setIsLoading(true);
    try {
        await onSubmit(formData);
        toast.success(mode === 'upload' ? 'Mod uploaded' : 'Changes saved');
        onClose();
    } catch (err) {
        setError(getResponseMessage(err));
    } finally {
        setIsLoading(false);
    }
};

    if (!isOpen) return null;

    const inputClass = "px-3 py-2 bg-black/50 text-sm text-gray-300 border border-gray-400 rounded focus:outline-none focus:border-gray-200";
    const errorClass = "text-xs text-left text-red-400";

    return (
        <div
            className="fixed inset-0 bg-black/50 flex items-center justify-center z-50"
            onClick={onClose}
        >
            <div onClick={(e) => e.stopPropagation()}>
                <FormPanel className="min-h-full w-full flex flex-col justify-start gap-2">
                    <div className="flex justify-between pt-4">
                        <div className="flex w-1/4 "></div>
                        <h2 className="flex w-1/2 justify-center text-2xl font-bold text-white">
                            {mode === 'upload' ? 'Upload Mod' : 'Edit Mod'}
                        </h2>
                        <button
                            onClick={onClose}
                            className="flex w-1/4 justify-end items-center text-gray-400 hover:text-white text-xl cursor-pointer"
                            aria-label="Close"
                        >
                            ✕
                        </button>
                    </div>

                    <div className="flex flex-col gap-4">
                        {error && <p className="text-sm text-red-400">{error}</p>}

                        {isCategoriesError && (
                            <p className="text-sm text-red-400">
                                Could not load categories, please refresh and try again
                            </p>
                        )}

                        <div className="flex flex-col gap-1.5">
                            <label htmlFor="title" className="flex justify-start font-medium text-white">
                                Title
                            </label>
                            <input
                                id="title"
                                type="text"
                                value={title}
                                maxLength={MAX_MOD_TITLE_LENGTH}
                                onChange={(e) => setTitle(e.target.value)}
                                placeholder="Enter mod title"
                                aria-invalid={!!fieldErrors.title}
                                className={inputClass}
                            />
                            {fieldErrors.title && <p className={errorClass}>{fieldErrors.title}</p>}
                        </div>

                        <div className="flex flex-col gap-1.5">
                            <span className="flex justify-start font-medium text-white">
                                Category
                            </span>
                            <Dropdown
                                value={type}
                                options={categories}
                                label={isLoadingCategories ? 'Loading categories...' : 'Select a category'}
                                onChange={setType}
                                className="w-full"
                            />
                            {fieldErrors.type && <p className={errorClass}>{fieldErrors.type}</p>}
                        </div>

                        <div className="flex flex-col gap-1.5">
                            <label htmlFor="description" className="flex justify-start font-medium text-white">
                                Description
                            </label>
                            <textarea
                                id="description"
                                value={description}
                                maxLength={MAX_MOD_DESCRIPTION_LENGTH}
                                onChange={(e) => setDescription(e.target.value)}
                                placeholder="Enter mod description"
                                rows={3}
                                aria-invalid={!!fieldErrors.description}
                                className={`${inputClass} resize-none`}
                            />
                            {fieldErrors.description && (
                                <p className={errorClass}>{fieldErrors.description}</p>
                            )}
                        </div>

                        <div className="flex justify-between gap-4">
                            <div className="flex flex-col gap-1.5 w-1/2">
                                <FilePicker
                                    className="w-full"
                                    id="modFile"
                                    label="Mod File"
                                    accept=".tpf"
                                    file={modFile}
                                    existingFileName={mode === 'edit' && mod ? getFileNameFromUrl(mod.modFileUrl) : null}
                                    optional={mode === 'edit'}
                                    onChange={setModFile}
                                />
                                {fieldErrors.modFile && (
                                    <p className={errorClass}>{fieldErrors.modFile}</p>
                                )}
                            </div>
                            <div className="flex flex-col gap-1.5 w-1/2">
                                <FilePicker
                                    className="w-full"
                                    id="previewImage"
                                    label="Preview Image"
                                    accept=".png,.jpg,.jpeg"
                                    file={previewImage}
                                    existingFileName={mode === 'edit' && mod ? getFileNameFromUrl(mod.imageUrl) : null}
                                    optional={mode === 'edit'}
                                    onChange={setPreviewImage}
                                />
                                {fieldErrors.previewImage && (
                                    <p className={errorClass}>{fieldErrors.previewImage}</p>
                                )}
                            </div>
                        </div>
                    </div>

                    <div className="flex justify-center gap-4 pt-10">
                        <button
                            onClick={onClose}
                            className="w-1/2 px-4 py-2 text-sm font-medium border border-gray-400 text-gray-300 rounded hover:bg-white/10 cursor-pointer"
                        >
                            CANCEL
                        </button>
                        <button
                            onClick={handleSubmit}
                            disabled={isLoading || isLoadingCategories || isCategoriesError}
                            className={`w-1/2 px-4 py-2 bg-[${PRIMARY_BUTTON_COLOUR}] hover:bg-[${PRIMARY_BUTTON_HOVER_COLOUR}] text-sm text-white rounded disabled:opacity-50 disabled:cursor-not-allowed cursor-pointer`}
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