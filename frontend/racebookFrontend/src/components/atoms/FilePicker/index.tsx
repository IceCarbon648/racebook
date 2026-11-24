import { useState } from 'react';

interface FilePickerProps {
    className?: string;
    /** Doubles as the form field name. */
    id: string;
    label: string;
    accept: string;
    existingFileName: string | null;
    optional?: boolean;
}

const FilePicker = ({ className, id, label, accept, existingFileName }: FilePickerProps) => {
    const [file, setFile] = useState<File | null>(null);

    const displayName = file?.name ?? existingFileName ?? 'No file chosen';

    return (
        <div className={`flex flex-col gap-1.5 ${className}`}>
            <span className="flex justify-start font-medium text-white">
                {label}
            </span>

            <label
                htmlFor={id}
                className="px-3 py-2 text-xs font-medium text-center text-gray-300 bg-black/50 border border-gray-400 rounded cursor-pointer hover:bg-black/70 focus-within:border-gray-200"
            >
                CHOOSE FILE
                <input
                    id={id}
                    name={id}
                    type="file"
                    accept={accept}
                    onChange={(e) => setFile(e.target.files?.[0] ?? null)}
                    className="sr-only"
                />
            </label>

            <p
                className={`truncate text-xs ${file ? 'text-gray-200' : 'text-gray-400'}`}
                title={displayName}
            >
                {displayName}
            </p>
        </div>
    );
};

export default FilePicker;