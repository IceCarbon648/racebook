import reactLogo from '../../../assets/images/react.svg';
import viteLogo from '../../../assets/images/vite.svg';
import { diagonal_lines } from '../../../assets';
import type { ModCardProps } from './index.types';
import { Tilt } from '@gfazioli/react-tilt';

const cardShape = "M 0,0 L 255,0 L 255,215 L 247,223 L 159,223 L 127,255 L 16,255 L 0,239 Z";

const ModCard = ({
    title, type, imageUrl, creator, isFavourite,
    onClick, onEdit, onDelete, onFavourite
}: ModCardProps) => {
    return (
        <div className="relative h-64 w-64 text-left">
            <Tilt threshold={15} hoverScale={1.08}>
                <img src={diagonal_lines} alt="" className="absolute right-5 top-45 w-24 h-6"/>

                <div
                    className="flex flex-col p-5 cursor-pointer"
                    onClick={onClick}
                >
                    <Tilt.Layer depth={1.1}>
                        <img src={imageUrl} alt={title} className="w-full aspect-video object-cover rounded-sm" />
                    </Tilt.Layer>

                    <div className="flex flex-col gap-3">
                        <div>
                            <Tilt.Layer depth={2.2} className="text-3xl text-white">
                                <p>{title}</p>
                            </Tilt.Layer>

                            {creator && (
                                <Tilt.Layer depth={0.55} className="text-sm text-[#d7d7d7]">
                                    <p>@{creator}</p>
                                </Tilt.Layer>
                            )}
                        </div>

                        <div className="flex flex-row items-center justify-between w-11/20">
                            {isFavourite !== undefined && onFavourite && (
                                <button
                                    onClick={(e) => { e.stopPropagation(); onFavourite(); }}
                                >
                                    <img src={isFavourite ? reactLogo : viteLogo} alt="" className="h-7 w-7" />
                                </button>
                            )}

                            <Tilt.Layer depth={0.825} className="flex justify-center items-center w-13/20 h-5 text-center text-white text-[10px] font-bold bg-[#930093] rounded-full">
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

                    <svg className="pointer-events-none absolute inset-0 h-full w-full" viewBox="0 0 256 256" preserveAspectRatio="none">
                        <path d={cardShape} fill="none" stroke="#cc00cc" strokeWidth="2" />
                    </svg>
                </div>
            </Tilt>
        </div>
    );
};

export default ModCard;