import { card_accent } from '../../../assets';
import { NEON } from '../../../constants/theme';
import type { ModCardProps } from './index.types';
import { Tilt } from '@gfazioli/react-tilt';
import { MOD_CARD } from '../../../constants/customDivs';
import ModCardGlow from '../../atoms/ModCardGlow/index'
import { useState } from 'react';
import { favouriteIconShape, deleteIconShape, editIconShape } from '../../../constants/customButtons';

const ModCard = ({
    title, type, imageUrl, creator, isFavourite,
    onClick, onEdit, onDelete, onFavourite
}: ModCardProps) => {
    const [hovered, setHovered] = useState(false);
    return (
        <div
            className="relative h-64 w-64 text-left"
            data-test="mod-card"
            onMouseEnter={() => setHovered(true)}
            onMouseLeave={() => setHovered(false)}
        >

            <div
                className="pointer-events-none absolute inset-0 backdrop-blur-[3px] transition-transform duration-300"
                style={{
                    clipPath: `path('${MOD_CARD}')`,
                    WebkitClipPath: `path('${MOD_CARD}')`,
                    transform: hovered ? 'scale(1.08)' : 'scale(1)',
                }}
            />

            <Tilt threshold={15} hoverScale={1.08}>
                <Tilt.Layer depth={0}>
                    <img src={card_accent} alt="" className="absolute right-5 top-42 w-24 h-6 opacity-7" />
                </Tilt.Layer>

                <div
                    className="flex h-64 flex-col pl-5 pt-5 pr-5 pb-3 cursor-pointer focus-visible:outline-2 focus-visible:outline-white"
                    role="button"
                    tabIndex={0}
                    aria-label={`View ${title}`}
                    onClick={onClick}
                    onKeyDown={(event) => {
                        if (event.key === 'Enter' || event.key === ' ') {
                            event.preventDefault();
                            onClick?.();
                        }
                    }}
                >
                    <Tilt.Layer depth={-0.5}>
                        <img src={imageUrl} alt={title} className="w-full aspect-video object-cover rounded-sm" />
                    </Tilt.Layer>

                    <div className="flex flex-col gap-4">
                        <div className="">
                            <Tilt.Layer depth={-0.15} className="text-2xl text-white whitespace-nowrap">
                                <p className="truncate" title={title}>{title}</p>
                            </Tilt.Layer>

                            <Tilt.Layer depth={-0.15} className="text-sm text-[#d7d7d7]">
                                <p>{creator ? `@${creator}` : '\u00A0'}</p>
                            </Tilt.Layer>
                        </div>

                        <div className="flex flex-row items-center justify-between w-14/25">
                            {isFavourite !== undefined && onFavourite && (
                                <Tilt.Layer depth={0.2} className="flex">
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
                                depth={0.2}
                                className="flex justify-center items-center w-13/20 h-5 text-center text-white text-[10px] font-bold rounded-full"
                                style={{ backgroundColor: NEON.fill }}
                            >
                                <p>{type}</p>
                            </Tilt.Layer>
                        </div>
                    </div>

                    {onEdit && onDelete && (
                        <div>
                            <button
                                onClick={(e) => { e.stopPropagation(); onEdit(); }}
                                aria-label="Edit mod"
                                className="absolute right-18 top-58 cursor-pointer text-[#00BBFA] transition-[filter] duration-200 hover:drop-shadow-[0_0_6px_rgba(0,187,250,1)]"
                            >
                                <svg viewBox="0 0 512 512" className="h-5.5 w-5.5 drop-shadow-[0_0_3px_rgba(0,187,250,1)]" fill="currentColor" stroke="currentColor" strokeWidth="16">
                                    <path d={editIconShape} />
                                </svg>
                            </button>
                            <button
                                onClick={(e) => { e.stopPropagation(); onDelete(); }}
                                aria-label="Delete mod"
                                className="absolute right-5 top-58 cursor-pointer text-[#D10005] transition-[filter] duration-200 hover:drop-shadow-[0_0_6px_rgba(209,0,5,1)]"
                            >
                                <svg viewBox="18 19 20 22" className="h-5.5 w-5.5 drop-shadow-[0_0_3px_rgba(209,0,5,1)]" fill="currentColor">
                                    <path fillRule="evenodd" d={deleteIconShape} />
                                </svg>
                            </button>
                        </div>
                    )}
                    <ModCardGlow />
                </div>
            </Tilt>
        </div>
    );
};

export default ModCard;