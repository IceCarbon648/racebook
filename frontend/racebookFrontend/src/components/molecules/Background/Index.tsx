import { useState } from 'react';
import { backgrounds } from '../../../assets/images';
import { FluidReveal } from '../../atoms';
import type { BackgroundProps } from './index.types';

const Background = ({ reveal = false }: BackgroundProps) => {
    const [set] = useState(() =>
        backgrounds[Math.floor(Math.random() * backgrounds.length)]
    );

    if (reveal) {
        return (
            <FluidReveal
                revealSrc={set.original}
                baseSrc={set.tinted}
                depthSrc={set.depthMap}
                parallax={0.03}
            />
        );
    }

    return (
        <img
            src={set.tinted}
            alt=""
            className="pointer-events-none fixed inset-0 -z-20 h-full w-full object-cover"
        />
    );
};

export default Background;