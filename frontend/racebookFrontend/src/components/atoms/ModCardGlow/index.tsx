import { useTiltContext } from "@gfazioli/react-tilt";
import { NEON } from "../../../constants/theme";
import { MOD_CARD } from "../../../constants/customDivs";

const ModCardGlow = () => {
    const { isHovering } = useTiltContext();

    const blur = isHovering ? NEON.glowBlurSelected : NEON.glowBlur;
    const border = isHovering ? NEON.borderSelected : NEON.borderColour;

    return (
        <svg
            className="pointer-events-none absolute -inset-6 h-[calc(100%+3rem)] w-[calc(100%+3rem)] -z-10"
            viewBox="-24 -24 304 304"
            preserveAspectRatio="none"
        >
            <defs>
                <filter id="cardGlow" x="-50%" y="-50%" width="200%" height="200%">
                    <feGaussianBlur stdDeviation={blur} />
                </filter>
            </defs>

            <path
                d={MOD_CARD}
                fill="none"
                stroke={NEON.glowColour}
                strokeWidth={blur}
                filter="url(#cardGlow)"
            />

            <path
                d={MOD_CARD}
                fill={NEON.fill}
                fillOpacity={NEON.fillOpacity}
                stroke={border}
                strokeWidth={NEON.borderWidth}
                strokeLinejoin="round"
            />
        </svg>
    );
};

export default ModCardGlow;