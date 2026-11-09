interface FilePickerProps {
    className?: string,
    id: string;
    label: string;
    accept: string;
    file: File | null;
    existingFileName: string | null;
    optional?: boolean;
    onChange: (file: File | null) => void;
}

const FilePicker = ({ className, id, label, accept, existingFileName, file, onChange }: FilePickerProps) => {
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
                    type="file"
                    accept={accept}
                    onChange={(e) => onChange(e.target.files?.[0] ?? null)}
                    className="sr-only"
                />
            </label>

            <p className="truncate text-xs text-gray-400" title={displayName}>
                {displayName}
            </p>
        </div>
    );
};

export default FilePicker;