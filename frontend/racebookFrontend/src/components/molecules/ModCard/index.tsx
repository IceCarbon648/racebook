import { useState } from 'react';
import { card_accent } from '../../../assets';
import { NEON } from '../../../constants/theme';
import type { ModCardProps } from './index.types';
import { Tilt } from '@gfazioli/react-tilt';

const cardShape = "M 0,0 L 255,0 L 255,215 L 247,223 L 159,223 L 127,255 L 16,255 L 0,239 Z";
const favouriteIconShape = "M20.8 4.6a5.5 5.5 0 0 0-7.8 0L12 5.7l-1-1.1a5.5 5.5 0 0 0-7.8 7.8l1.1 1L12 21l7.7-7.7 1.1-1a5.5 5.5 0 0 0 0-7.7z";

const ModCard = ({
    title, type, imageUrl, creator, isFavourite,
    onClick, onEdit, onDelete, onFavourite
}: ModCardProps) => {
    const [hovered, setHovered] = useState(false);

    const blur = hovered ? NEON.glowBlurSelected : NEON.glowBlur;
    const border = hovered ? NEON.borderSelected : NEON.borderColour;

    return (
        <div
            className="relative h-64 w-64 text-left"
            onMouseEnter={() => setHovered(true)}
            onMouseLeave={() => setHovered(false)}
        >
            <Tilt threshold={15} hoverScale={1.08}>
                <Tilt.Layer depth={-0.75}>
                    <img src={card_accent} alt="" className="absolute right-5 top-45 w-24 h-6 opacity-15" />
                </Tilt.Layer>

                <div className="flex flex-col p-5 cursor-pointer" onClick={onClick}>
                    <Tilt.Layer depth={-1}>
                        <img src={imageUrl} alt={title} className="w-full aspect-video object-cover rounded-sm" />
                    </Tilt.Layer>

                    <div className="flex flex-col gap-3">
                        <div>
                            <Tilt.Layer depth={0.5} className="text-3xl text-white">
                                <p>{title}</p>
                            </Tilt.Layer>

                            {creator && (
                                <Tilt.Layer depth={0.75} className="text-sm text-[#d7d7d7]">
                                    <p>@{creator}</p>
                                </Tilt.Layer>
                            )}
                        </div>

                        <div className="flex flex-row items-center justify-between w-11/20">
                            <Tilt.Layer depth={-0.5}>
                                {isFavourite !== undefined && onFavourite && (
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
                                )}
                            </Tilt.Layer>

                            <Tilt.Layer
                                depth={1}
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
                            d={cardShape}
                            fill="none"
                            stroke={NEON.glowColour}
                            strokeWidth={blur}
                            filter="url(#cardGlow)"
                        />

                        <path
                            d={cardShape}
                            fill={NEON.fill}
                            fillOpacity={NEON.fillOpacity}
                            stroke={border}
                            strokeWidth={NEON.borderWidth}
                            strokeLinejoin="round"
                        />
                    </svg>
                </div>
            </Tilt>
        </div>
    );
};

export default ModCard;