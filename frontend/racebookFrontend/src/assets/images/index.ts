interface Background {
    original: string;
    tinted: string;
}

import racebookLogo from './react.svg';
import favourite from './vite.svg';
import bgOneOriginal from './backgrounds/bg1/original.png';
import bgOneTinted from './backgrounds/bg1/tinted.png';

export const backgrounds: Background[] = [
    {
        original: bgOneOriginal,
        tinted: bgOneTinted
    }
];

export { racebookLogo, favourite };