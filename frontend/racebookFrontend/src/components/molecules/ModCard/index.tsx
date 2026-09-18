import { card_accent } from '../../../assets';
import { NEON } from '../../../constants/theme';
import type { ModCardProps } from './index.types';
import { Tilt, useTiltContext } from '@gfazioli/react-tilt';
import { MOD_CARD } from '../../../constants/customDivs';
import { useEffect, useState } from 'react';

const favouriteIconShape = "M20.8 4.6a5.5 5.5 0 0 0-7.8 0L12 5.7l-1-1.1a5.5 5.5 0 0 0-7.8 7.8l1.1 1L12 21l7.7-7.7 1.1-1a5.5 5.5 0 0 0 0-7.7z";

const CardGlow = () => {
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

const HoverBridge = ({ onChange }: { onChange: (v: boolean) => void }) => {
    const { isHovering } = useTiltContext();

    useEffect(() => {
        onChange(isHovering);
    }, [isHovering, onChange]);

    return null;
};

const ModCard = ({
    title, type, imageUrl, creator, isFavourite,
    onClick, onEdit, onDelete, onFavourite
}: ModCardProps) => {
    const [hovered, setHovered] = useState(false);
    return (
        <div className="relative h-64 w-64 text-left">

            <div
                className="pointer-events-none absolute inset-0 backdrop-blur-[3px] transition-transform duration-300"
                style={{
                    clipPath: `path('${MOD_CARD}')`,
                    WebkitClipPath: `path('${MOD_CARD}')`,
                    transform: hovered ? 'scale(1.08)' : 'scale(1)',
                }}
            />

            <Tilt threshold={15} hoverScale={1.08}>
                <HoverBridge onChange={setHovered} />
                <Tilt.Layer depth={0}>
                    <img src={card_accent} alt="" className="absolute right-5 top-42 w-24 h-6 opacity-7" />
                </Tilt.Layer>

                <div className="flex flex-col pl-5 pt-5 pr-5 pb-3 cursor-pointer" onClick={onClick}>
                    <Tilt.Layer depth={-0.5}>
                        <img src={imageUrl} alt={title} className="w-full aspect-video object-cover rounded-sm" />
                    </Tilt.Layer>

                    <div className="flex flex-col gap-4">
                        <div className="">
                            <Tilt.Layer depth={-0.5} className="text-2xl text-white whitespace-nowrap">
                                <p>{title}</p>
                            </Tilt.Layer>

                            {creator && (
                                <Tilt.Layer depth={-0.5} className="text-sm text-[#d7d7d7]">
                                    <p>@{creator}</p>
                                </Tilt.Layer>
                            )}
                        </div>

                        <div className="flex flex-row items-center justify-between w-1/2">

                            
                                {isFavourite !== undefined && onFavourite && (
                                    <Tilt.Layer depth={0} className="flex">
                                    <button
                                        onClick={(e) => { e.stopPropagation(); onFavourite(); }}
                                        aria-label={isFavourite ? 'Remove from favourites' : 'Add to favourites'}
                                        className="text-white transition-[filter] duration-200 hover:drop-shadow-[0_0_6px_rgba(255,255,255,0.9)]"
                                    >
                                        <svg
                                            viewBox="0 0 24 24"
                                            className="h-7 w-7"
                                            fill={isFavourite ? 'currentColor' : 'none'}
                                            stroke="currentColor"
                                            strokeWidth="2"
                                            strokeLinecap="round"
                                            strokeLinejoin="round"
                                        >
                                            <path d={favouriteIconShape} />
                                        </svg>
                                    </button>
                                    </Tilt.Layer>
                                )}
                            

                            <Tilt.Layer
                                depth={-0.2}
                                className="flex justify-center items-center w-13/20 h-5 text-center text-white text-[10px] font-bold rounded-full"
                                style={{ backgroundColor: NEON.fill }}
                            >
                                <p>{type}</p>
                            </Tilt.Layer>
                        </div>
                    </div>

                    {onEdit && onDelete && (
                        <div className="flex gap-2">
                            <button onClick={(e) => { e.stopPropagation(); onEdit(); }} className="flex-1 rounded border border-gray-500 py-1 text-xs font-medium hover:bg-white/10">
                                Edit
                            </button>
                            <button onClick={(e) => { e.stopPropagation(); onDelete(); }} className="flex-1 rounded border border-red-400 py-1 text-xs font-medium text-red-400 hover:bg-red-500/10">
                                Delete
                            </button>
                        </div>
                    )}
                    
                    <CardGlow />
                </div>
            </Tilt>
        </div>
    );
};

export default ModCard;