interface FilePickerProps {
    className?: string,
    id: string;
    label: string;
    accept: string;
    file: File | null;
    optional?: boolean;
    onChange: (file: File | null) => void;
}

const FilePicker = ({ className, id, label, accept, file, optional, onChange }: FilePickerProps) => {
    return (
        <div className={`flex flex-col gap-1.5 ${className}`}>
            <span className="flex justify-start font-medium text-white">
                {label}
                {optional && (
                    <span className="text-xs text-gray-400 font-normal ml-1 self-center">(optional)</span>
                )}
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

            <p className="truncate text-xs text-gray-400" title={file?.name}>
                {file?.name ?? 'No file chosen'}
            </p>
        </div>
    );
};

export default FilePicker;