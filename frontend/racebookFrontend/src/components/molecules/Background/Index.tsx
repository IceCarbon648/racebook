import { useEffect, useState } from 'react';
import { backgrounds } from '../../../assets/images';
import FluidReveal from '../../atoms/FluidReveal';

interface BackgroundProps {
    reveal?: boolean;
    interval?: number;
}

const Background = ({ reveal = false, interval = 15000 }: BackgroundProps) => {
    const [index, setIndex] = useState(() =>
        Math.floor(Math.random() * backgrounds.length)
    );

    useEffect(() => {
        if (backgrounds.length < 2) return;

        const id = setInterval(() => {
            setIndex((i) => (i + 1) % backgrounds.length);
        }, interval);

        return () => clearInterval(id);
    }, [interval]);

    const set = backgrounds[index];

    if (reveal) {
        return <FluidReveal revealSrc={set.original} baseSrc={set.tinted} depthSrc={set.depthMap} parallax={0.03} />;
    }

    return (
        <div className="pointer-events-none fixed inset-0" style={{ zIndex: -2 }}>
            {backgrounds.map((bg, i) => (
                <div
                    key={i}
                    className="absolute inset-0 bg-cover bg-center transition-opacity duration-1000"
                    style={{
                        backgroundImage: `url(${bg.tinted})`,
                        opacity: i === index ? 1 : 0,
                    }}
                />
            ))}
        </div>
    );
};

export default Background;