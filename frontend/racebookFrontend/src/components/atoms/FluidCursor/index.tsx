import { useEffect, useRef } from 'react';
import fluidCursor from '../../../hooks/useFluidCursor';

interface FluidCursorProps {
    revealImageSrc: string;
    backgroundImageSrc: string;
}

const FluidCursor = ({ revealImageSrc, backgroundImageSrc }: FluidCursorProps) => {
    const outputCanvasRef = useRef<HTMLCanvasElement>(null);

    useEffect(() => {
        fluidCursor();

        const fluidCanvas = document.getElementById('fluid') as HTMLCanvasElement;
        const outputCanvas = outputCanvasRef.current;
        if (!fluidCanvas || !outputCanvas) return;

        const ctx = outputCanvas.getContext('2d');
        if (!ctx) return;

        const bgImage = new Image();
        const revealImage = new Image();
        bgImage.src = backgroundImageSrc;
        revealImage.src = revealImageSrc;

        const resize = () => {
            outputCanvas.width = window.innerWidth;
            outputCanvas.height = window.innerHeight;
        };

        resize();
        window.addEventListener('resize', resize);

        let animationId: number;

        const draw = () => {
            if (!bgImage.complete || !revealImage.complete) {
                animationId = requestAnimationFrame(draw);
                return;
            }

            const w = outputCanvas.width;
            const h = outputCanvas.height;

            ctx.clearRect(0, 0, w, h);
            ctx.drawImage(bgImage, 0, 0, w, h);

            ctx.save();
            ctx.globalCompositeOperation = 'source-over';
            
            const offscreen = new OffscreenCanvas(w, h);
            const offCtx = offscreen.getContext('2d')!;
            
            offCtx.drawImage(revealImage, 0, 0, w, h);
            offCtx.globalCompositeOperation = 'destination-in';
            offCtx.drawImage(fluidCanvas, 0, 0, w, h);
            
            ctx.drawImage(offscreen, 0, 0);
            ctx.restore();

            animationId = requestAnimationFrame(draw);
        };

        draw();

        return () => {
            window.removeEventListener('resize', resize);
            cancelAnimationFrame(animationId);
        };
    }, [revealImageSrc, backgroundImageSrc]);

    return (
        <>
            <div className="fixed inset-0 pointer-events-none opacity-0" style={{ zIndex: -1 }}>
                <canvas id="fluid" className="w-screen h-screen" />
            </div>

            <canvas
                ref={outputCanvasRef}
                className="fixed inset-0 w-screen h-screen pointer-events-none"
                style={{ zIndex: -2 }}
            />
        </>
    );
};

export default FluidCursor;