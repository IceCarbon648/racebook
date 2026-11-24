import { useState } from 'react';
import type { SubmitEvent } from 'react';
import FormPanel from '../../molecules/FormPanel';
import Dropdown from '../../molecules/Dropdown';
import { FilePicker } from '../../atoms';
import type { ModModalProps } from './index.types';
import { PRIMARY_BUTTON_COLOUR, PRIMARY_BUTTON_HOVER_COLOUR } from '../../../constants/theme';
import { MAX_MOD_TITLE_LENGTH, MAX_MOD_DESCRIPTION_LENGTH } from '../../../constants/inputLimits';
import { useCategories } from '../../../hooks/api/useCategories';
import { getFileNameFromUrl } from '../../../helpers/getFileNameFromUrl';
import { modUploadSchema, modEditSchema, validate } from '../../../validation';
import { notify } from '../../../utils/notify';
import { getResponseMessage } from '../../../utils/getResponseMessage';

const validateModForm = (
    formData: FormData,
    type: string,
    isEditing: boolean,
    categories: string[],
) =>
    validate(isEditing ? modEditSchema(categories) : modUploadSchema(categories), {
        title: formData.get('title'),
        type,
        description: formData.get('description'),
        modFile: formData.get('modFile'),
        previewImage: formData.get('previewImage'),
    });

const MODE_COPY = {
    upload: { heading: 'Upload Mod', submit: 'UPLOAD' },
    edit: { heading: 'Edit Mod', submit: 'SAVE CHANGES' },
} as const;

const ModModal = ({ mode, mod, onClose, onSubmit }: ModModalProps) => {
    const {
        data: categories = [],
        isLoading: isLoadingCategories,
        isError: isCategoriesError,
    } = useCategories();

    const isEditing = mode === 'edit';
    const existingMod = isEditing ? mod : undefined;
    const copy = MODE_COPY[mode];

    const [type, setType] = useState(existingMod?.type ?? '');
    const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});
    const [error, setError] = useState<string | null>(null);
    const [isSaving, setIsSaving] = useState(false);

    const handleSubmit = async (event: SubmitEvent<HTMLFormElement>) => {
        event.preventDefault();
        setError(null);

        const formData = new FormData(event.currentTarget);
        formData.set('type', type);

        const errors = await validateModForm(formData, type, isEditing, categories);

        setFieldErrors(errors ?? {});
        if (errors) return;

        setIsSaving(true);

        try {
            await onSubmit(formData);
            notify.success(isEditing ? 'Changes saved' : 'Mod uploaded');
            onClose();
        } catch (err) {
            setError(getResponseMessage(err));
        } finally {
            setIsSaving(false);
        }
    };

    const inputClass = "px-3 py-2 bg-black/50 text-sm text-gray-300 border border-gray-400 rounded focus:outline-none focus:border-gray-200";
    const errorClass = "text-xs text-left text-red-400";

    return (
        <div
            className="fixed inset-0 bg-black/50 flex items-center justify-center z-50"
            onClick={onClose}
        >
            <div onClick={(e) => e.stopPropagation()}>
                <FormPanel className="min-h-full w-full flex flex-col justify-start gap-2">
                    <form onSubmit={handleSubmit} className="flex flex-col gap-2">
                        <div className="flex justify-between pt-4">
                            <div className="flex w-1/4" />
                            <h2 className="flex w-1/2 justify-center text-2xl font-bold text-white">
                                {copy.heading}
                            </h2>
                            <button
                                type="button"
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
                                    name="title"
                                    type="text"
                                    defaultValue={existingMod?.title}
                                    maxLength={MAX_MOD_TITLE_LENGTH}
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
                                    name="description"
                                    defaultValue={existingMod?.description}
                                    maxLength={MAX_MOD_DESCRIPTION_LENGTH}
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
                                        existingFileName={getFileNameFromUrl(existingMod?.modFileUrl)}
                                        optional={isEditing}
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
                                        existingFileName={getFileNameFromUrl(existingMod?.imageUrl)}
                                        optional={isEditing}
                                    />
                                    {fieldErrors.previewImage && (
                                        <p className={errorClass}>{fieldErrors.previewImage}</p>
                                    )}
                                </div>
                            </div>
                        </div>

                        <div className="flex justify-center gap-4 pt-10">
                            <button
                                type="button"
                                onClick={onClose}
                                className="w-1/2 px-4 py-2 text-sm font-medium border border-gray-400 text-gray-300 rounded hover:bg-white/10 cursor-pointer"
                            >
                                CANCEL
                            </button>
                            <button
                                type="submit"
                                disabled={isSaving || isLoadingCategories || isCategoriesError}
                                className={`w-1/2 px-4 py-2 bg-[${PRIMARY_BUTTON_COLOUR}] hover:bg-[${PRIMARY_BUTTON_HOVER_COLOUR}] text-sm text-white rounded disabled:opacity-50 disabled:cursor-not-allowed cursor-pointer`}
                            >
                                {isSaving ? 'SAVING...' : copy.submit}
                            </button>
                        </div>
                    </form>
                </FormPanel>
            </div>
        </div>
    );
};

export default ModModal;