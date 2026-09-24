import { card_accent } from '../../../assets';
import { NEON } from '../../../constants/theme';
import type { ModCardProps } from './index.types';
import { Tilt, useTiltContext } from '@gfazioli/react-tilt';
import { MOD_CARD } from '../../../constants/customDivs';
import { useEffect, useState } from 'react';

const favouriteIconShape = "M20.8 4.6a5.5 5.5 0 0 0-7.8 0L12 5.7l-1-1.1a5.5 5.5 0 0 0-7.8 7.8l1.1 1L12 21l7.7-7.7 1.1-1a5.5 5.5 0 0 0 0-7.7z";
const editIconShape = "M43.12,380.17l7.39,7.38l275.52-275.83l74.1,74.19L126.57,459.77l-101.1,27.04l27.73-103.94L43.12,380.17l7.39,7.38L43.12,380.17l-10.09-2.69L0.64,498.87c-0.96,3.6,0.07,7.44,2.71,10.08c2.64,2.64,6.48,3.67,10.08,2.7l121.25-32.42c1.76-0.47,3.4-1.42,4.69-2.71l282.91-283.23c4.08-4.08,4.08-10.68,0-14.76l-88.86-88.96c-1.97-1.97-4.6-3.06-7.39-3.06c-2.79,0-5.42,1.09-7.39,3.06L35.73,372.79c-1.29,1.29-2.23,2.92-2.7,4.69L43.12,380.17z M364.82,58.1l-7.39,7.38l81.49,81.58l-21.49,21.51l-74.1-74.19l28.87-28.91L364.82,58.1l-7.39,7.38L364.82,58.1l-7.39-7.38L321.19,87c-4.08,4.08-4.08,10.68,0,14.76l88.86,88.96c1.97,1.97,4.6,3.06,7.39,3.06c2.79,0,5.42-1.09,7.39-3.06l36.24-36.29c4.08-4.08,4.08-10.68,0-14.76l-88.86-88.96c-1.97-1.97-4.6-3.06-7.39-3.06c-2.79,0-5.42,1.09-7.39,3.06L364.82,58.1z M398.48,24.4l7.39,7.38c7.28-7.29,16.73-10.88,26.28-10.89c9.54,0.01,18.98,3.61,26.26,10.89l21.54,21.57c7.27,7.28,10.86,16.75,10.87,26.31c-0.01,9.58-3.62,19.05-10.87,26.32l-26.27,26.3l-74.1-74.19l26.29-26.32L398.48,24.4l-7.39-7.38l-33.66,33.7c-4.08,4.08-4.08,10.68,0,14.76l88.86,88.96c1.97,1.97,4.6,3.06,7.39,3.06c2.79,0,5.42-1.09,7.39-3.06l33.66-33.7c11.3-11.31,16.99-26.24,16.98-41.08c0.01-14.82-5.67-29.75-16.98-41.07l-21.55-21.57C461.89,5.7,446.97-0.01,432.15,0c-14.82-0.01-29.75,5.69-41.06,17.02L398.48,24.4z";
const deleteIconShape = "M36 26v10.997c0 1.659-1.337 3.003-3.009 3.003h-9.981c-1.662 0-3.009-1.342-3.009-3.003v-10.997h16zm-2 0v10.998c0 .554-.456 1.002-1.002 1.002h-9.995c-.554 0-1.002-.456-1.002-1.002v-10.998h12zm-9-5c0-.552.451-1 .991-1h4.018c.547 0 .991.444.991 1 0 .552-.451 1-.991 1h-4.018c-.547 0-.991-.444-.991-1zm0 6.997c0-.551.444-.997 1-.997.552 0 1 .453 1 .997v6.006c0 .551-.444.997-1 .997-.552 0-1-.453-1-.997v-6.006zm4 0c0-.551.444-.997 1-.997.552 0 1 .453 1 .997v6.006c0 .551-.444.997-1 .997-.552 0-1-.453-1-.997v-6.006zm-6-5.997h-4.008c-.536 0-.992.448-.992 1 0 .556.444 1 .992 1h18.016c.536 0 .992-.448.992-1 0-.556-.444-1-.992-1h-4.008v-1c0-1.653-1.343-3-3-3h-3.999c-1.652 0-3 1.343-3 3v1z";

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

                <div className="flex h-64 flex-col pl-5 pt-5 pr-5 pb-3 cursor-pointer" onClick={onClick}>
                    <Tilt.Layer depth={-0.5}>
                        <img src={imageUrl} alt={title} className="w-full aspect-video object-cover rounded-sm" />
                    </Tilt.Layer>

                    <div className="flex flex-col gap-4">
                        <div className="">
                            <Tilt.Layer depth={-0.5} className="text-2xl text-white whitespace-nowrap">
                                <p>{title}</p>
                            </Tilt.Layer>

                                <Tilt.Layer depth={-0.5} className="text-sm text-[#d7d7d7]">
                                    <p>{creator ? `@${creator}` : '\u00A0'}</p>
                                </Tilt.Layer>
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
                        <div>
                            <button
                                onClick={(e) => { e.stopPropagation(); onEdit(); }}
                                aria-label="Edit mod"
                                className="absolute right-18 top-58 text-[#00BBFA] transition-[filter] duration-200 hover:drop-shadow-[0_0_6px_rgba(0,187,250,1)]"
                            >
                                <svg viewBox="0 0 512 512" className="h-5.5 w-5.5 drop-shadow-[0_0_3px_rgba(0,187,250,1)]" fill="currentColor" stroke="currentColor" strokeWidth="16">
                                    <path d={editIconShape} />
                                </svg>
                            </button>
                            <button
                                onClick={(e) => { e.stopPropagation(); onDelete(); }}
                                aria-label="Delete mod"
                                className="absolute right-3 top-58 text-[#D10005] transition-[filter] duration-200 hover:drop-shadow-[0_0_6px_rgba(209,0,5,1)]"
                            >
                                <svg viewBox="18 19 20 22" className="h-5.5 w-5.5 drop-shadow-[0_0_3px_rgba(209,0,5,1)]" fill="currentColor">
                                    <path fillRule="evenodd" d={deleteIconShape} />
                                </svg>
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