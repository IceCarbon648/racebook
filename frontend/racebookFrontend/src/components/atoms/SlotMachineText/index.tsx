interface RollingTextProps {
    text: string;
    stagger?: number;
}

const SlotMachineText = ({ text, stagger = 20 }: RollingTextProps) => {
    return (
        <span className="group inline-flex leading-none" aria-label={text}>
            {text.split('').map((char, i) => (
                <span key={i} className="inline-block overflow-hidden" aria-hidden="true">
                    <span
                        className="relative block transition-transform duration-300 ease-out group-hover:-translate-y-full"
                        style={{ transitionDelay: `${i * stagger}ms` }}
                    >
                        <span className="block">{char === ' ' ? '\u00A0' : char}</span>
                        <span className="absolute left-0 top-full block">
                            {char === ' ' ? '\u00A0' : char}
                        </span>
                    </span>
                </span>
            ))}
        </span>
    );
};

export default SlotMachineText;