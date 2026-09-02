import { useEffect } from 'react';
import fluidCursor from '../../../hooks/useFluidCursor';

interface FluidRevealProps {
    revealSrc: string;
    baseSrc: string;
}

const FluidReveal = ({ revealSrc, baseSrc }: FluidRevealProps) => {
    useEffect(() => {
        fluidCursor(baseSrc, revealSrc);
    }, [baseSrc, revealSrc]);

    return (
        <div className="pointer-events-none fixed inset-0" style={{ zIndex: -2 }}>
            <canvas id="fluid" className="h-screen w-screen" />
        </div>
    );
};

export default FluidReveal;