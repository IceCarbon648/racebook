import reactLogo from '../../../assets/images/react.svg';
import viteLogo from '../../../assets/images/vite.svg';
import type { ModCardProps } from './index.types';
import { Tilt } from '@gfazioli/react-tilt';

const cardShape = "M 0,0 L 255,0 L 255,215 L 247,223 L 159,223 L 127,255 L 16,255 L 0,239 Z";

const ModCard = ({
    title, type, imageUrl, creator, isFavourite,
    onClick, onEdit, onDelete, onFavourite
}: ModCardProps) => {
    return (
        <div className="relative h-64 w-64">
            <Tilt threshold={22.5} hoverScale={1.08}>
                <div
                    className="flex flex-col p-4 cursor-pointer"
                    onClick={onClick}
                >
                    <Tilt.Layer depth={1.1}>
                        <img src={imageUrl} alt={title} className="w-full aspect-video object-cover" />
                    </Tilt.Layer>

                    <div>
                        <Tilt.Layer className="self-start" depth={2.2}>
                            <h2>{title}</h2>
                        </Tilt.Layer>

                        {creator && (
                            <Tilt.Layer className="self-end mt-auto" depth={0.55}>
                                <p>By {creator}</p>
                            </Tilt.Layer>
                        )}
                    </div>

                    <div>
                        <Tilt.Layer className="self-end" depth={0.825}>
                            <p>{type}</p>
                        </Tilt.Layer>

                        {isFavourite !== undefined && onFavourite && (
                            <button
                                onClick={(e) => { e.stopPropagation(); onFavourite(); }}
                                className="absolute bottom-0 right-0 flex items-center gap-1.5 text-xs font-medium text-gray-400 transition-colors hover:text-gray-100"
                            >
                                <img src={isFavourite ? reactLogo : viteLogo} alt="" className="h-3.5 w-3.5" />
                                {isFavourite ? 'Unfavourite' : 'Favourite'}
                            </button>
                        )}
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
                        <path d={cardShape} fill="none" stroke="#8e0093" strokeWidth="2" />
                    </svg>
                </div>
            </Tilt>
        </div>
    );
};

export default ModCard;