import { useEffect } from 'react';
import fluidCursor from '../../../hooks/useFluidCursor';

interface FluidRevealProps {
    revealSrc: string;
    baseSrc: string;
    depthSrc: string;
    parallax: number;
}

const FluidReveal = ({ revealSrc, baseSrc, depthSrc,  parallax = 0.03 }: FluidRevealProps) => {
    useEffect(() => {
        fluidCursor(baseSrc, revealSrc, depthSrc, parallax);
    }, [baseSrc, revealSrc]);

    return (
        <div className="pointer-events-none fixed inset-0" style={{ zIndex: -2 }}>
            <canvas id="fluid" className="h-screen w-screen" />
        </div>
    );
};

export default FluidReveal;