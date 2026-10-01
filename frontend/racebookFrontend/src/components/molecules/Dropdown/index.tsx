import { useState, useRef, useEffect } from 'react';
import SlotMachineText from '../../atoms/SlotMachineText';

interface DropdownProps {
    value: string;
    options: string[];
    label?: string;
    className?: string;
    onChange: (value: string) => void;
}

const Dropdown = ({ value, options, label, className = '', onChange }: DropdownProps) => {
    const [open, setOpen] = useState(false);
    const ref = useRef<HTMLDivElement>(null);

    useEffect(() => {
        if (!open) return;

        const onPointerDown = (e: PointerEvent) => {
            if (!ref.current?.contains(e.target as Node)) setOpen(false);
        };
        const onKeyDown = (e: KeyboardEvent) => {
            if (e.key === 'Escape') setOpen(false);
        };

        document.addEventListener('pointerdown', onPointerDown);
        document.addEventListener('keydown', onKeyDown);

        return () => {
            document.removeEventListener('pointerdown', onPointerDown);
            document.removeEventListener('keydown', onKeyDown);
        };
    }, [open]);

    return (
        <div ref={ref} className={`relative ${className}`}>
            <button
                type="button"
                onClick={() => setOpen((o) => !o)}
                aria-haspopup="listbox"
                aria-expanded={open}
                aria-label={label}
                className="flex w-full items-center justify-between gap-3 px-3 py-2 text-sm text-gray-300 bg-black/50 border border-gray-400 rounded cursor-pointer hover:border-gray-200"
            >
                {value}
                <span className={`text-xs transition-transform ${open ? 'rotate-180' : ''}`}>▾</span>
            </button>

            {open && (
                <ul
                    role="listbox"
                    className="absolute left-0 top-full z-50 mt-1 w-full overflow-hidden bg-black/50 border border-gray-400 rounded backdrop-blur-sm"
                >
                    {options.map((option) => (
                        <li key={option} role="option" aria-selected={option === value}>
                            <button
                                type="button"
                                onClick={() => { onChange(option); setOpen(false); }}
                                className="group/roll w-full px-3 py-2 text-left text-sm text-gray-300 cursor-pointer hover:bg-white/10"
                            >
                                <SlotMachineText text={option} />
                            </button>
                        </li>
                    ))}
                </ul>
            )}
        </div>
    );
};

export default Dropdown;
