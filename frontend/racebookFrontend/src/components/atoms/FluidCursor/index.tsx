import { useEffect, useRef } from 'react';
import fluidCursor from '../../../hooks/useFluidCursor';

interface FluidCursorProps {
    revealImageSrc: string;
    backgroundImageSrc: string;
}

const FluidCursor = ({ revealImageSrc, backgroundImageSrc }: FluidCursorProps) => {
    const revealRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        fluidCursor();

        const applyMask = () => {
            const canvas = document.getElementById('fluid') as HTMLCanvasElement;
            if (!canvas || !revealRef.current) return;
            revealRef.current.style.maskImage = `url(${canvas.toDataURL()})`;
            requestAnimationFrame(applyMask);
        };

        requestAnimationFrame(applyMask);
    }, []);

    return (
        <>
            <div className="fixed inset-0 z-0 pointer-events-none opacity-0">
                <canvas id="fluid" className="w-screen h-screen" />
            </div>

            <div
                className="fixed inset-0 -z-20 bg-cover bg-center"
                style={{ backgroundImage: `url(${backgroundImageSrc})` }}
            />

            <div
                ref={revealRef}
                className="fixed inset-0 -z-10 bg-cover bg-center"
                style={{
                    backgroundImage: `url(${revealImageSrc})`,
                    maskSize: '100% 100%',
                    WebkitMaskSize: '100% 100%',
                }}
            />
        </>
    );
};

export default FluidCursor;