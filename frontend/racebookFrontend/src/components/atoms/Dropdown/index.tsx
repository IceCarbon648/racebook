import { useState, useRef, useEffect } from 'react';
import { SlotMachineText } from '../';
import { NEON } from '../../../constants/theme';

interface DropdownProps {
    value: string;
    options: string[];
    onChange: (value: string) => void;
}

const TRIGGER_H = 40;
const OPTION_H = 36;
const CHAMFER = 16;

const buildShape = (w: number, h: number) =>
    `M 0,0 L ${w},0 L ${w},${h - CHAMFER} L ${w - CHAMFER},${h} L ${CHAMFER},${h} L 0,${h - CHAMFER} Z`;

const Dropdown = ({ value, options, onChange }: DropdownProps) => {
    const [open, setOpen] = useState(false);
    const [width, setWidth] = useState(0);
    const ref = useRef<HTMLDivElement>(null);

    useEffect(() => {
        const el = ref.current;
        if (!el) return;

        const observer = new ResizeObserver(([entry]) => {
            setWidth(entry.contentRect.width);
        });
        observer.observe(el);

        return () => observer.disconnect();
    }, []);

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

    const height = open ? TRIGGER_H + options.length * OPTION_H : TRIGGER_H;
    const shape = buildShape(width, height);

    return (
        <div ref={ref} className="relative" style={{ height: TRIGGER_H }}>
            <svg
                className="pointer-events-none absolute -inset-6 -z-10"
                width={width + 48}
                height={height + 48}
                viewBox={`-24 -24 ${width + 48} ${height + 48}`}
            >
                <defs>
                    <filter id="dropdownGlow" x="-50%" y="-50%" width="200%" height="200%">
                        <feGaussianBlur stdDeviation={NEON.glowBlur} />
                    </filter>
                </defs>

                <path d={shape} fill="none" stroke={NEON.glowColour} strokeWidth={NEON.glowBlur} filter="url(#dropdownGlow)" />
                <path
                    d={shape}
                    fill={NEON.fill}
                    fillOpacity={NEON.fillOpacity}
                    stroke={NEON.borderColour}
                    strokeWidth={NEON.borderWidth}
                    strokeLinejoin="round"
                />
            </svg>

            <button
                type="button"
                onClick={() => setOpen((o) => !o)}
                aria-haspopup="listbox"
                aria-expanded={open}
                className="flex w-full items-center justify-between gap-3 px-4 text-sm text-white"
                style={{ height: TRIGGER_H }}
            >
                <SlotMachineText text={value} />
                <span className={`transition-transform ${open ? 'rotate-180' : ''}`}>▾</span>
            </button>

            {open && (
                <ul role="listbox" className="absolute left-0 w-full" style={{ top: TRIGGER_H }}>
                    {options.map((option) => (
                        <li key={option} role="option" aria-selected={option === value}>
                            <button
                                type="button"
                                onClick={() => {
                                    onChange(option);
                                    setOpen(false);
                                }}
                                className="flex w-full items-center px-4 text-left text-sm text-white transition-colors hover:bg-white/10"
                                style={{ height: OPTION_H }}
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