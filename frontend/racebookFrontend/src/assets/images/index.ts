interface Background {
    original: string;
    tinted: string;
    depthMap: string;
}

import racebookLogo from './react.svg';
import favourite from './vite.svg';
import card_accent from './card_accent.png';

import bgOneOriginal from './backgrounds/bg1/original.png';
import bgOneTinted from './backgrounds/bg1/tinted.png';
import bgOneDepthMap from './backgrounds/bg1/depthMap.png';

import bgTwoOriginal from './backgrounds/bg2/original.png';
import bgTwoTinted from './backgrounds/bg2/tinted.png';
import bgTwoDepthMap from './backgrounds/bg2/depthMap.png';

export const backgrounds: Background[] = [
    {
        original: bgOneOriginal,
        tinted: bgOneTinted,
        depthMap: bgOneDepthMap
    },
    {
        original: bgTwoOriginal,
        tinted: bgTwoTinted,
        depthMap: bgTwoDepthMap
    }
];

export { racebookLogo, favourite, card_accent };