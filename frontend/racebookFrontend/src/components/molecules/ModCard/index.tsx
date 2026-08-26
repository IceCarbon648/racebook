import reactLogo from '../../../assets/images/react.svg';
import viteLogo from '../../../assets/images/vite.svg';
import type { Mod, MyMod } from '../../../types';
import type { ModCardProps } from './index.types';
import { Tilt } from '@gfazioli/react-tilt';

const cardShape = "M 0,0 L 255,0 L 255,215 L 247,223 L 159,223 L 127,255 L 16,255 L 0,239 Z";

const isMyMod = (mod: Mod | MyMod): mod is MyMod => {
    return 'uid' in mod;
};

const ModCard = ({ mod, onClick, onEdit, onDelete, onFavourite }: ModCardProps) => {
    return (
        <div className="relative h-64 w-64">
            <Tilt className="h-64 w-64" threshold={22.5} hoverScale={1.08}>
                <div
                    className="relative h-64 w-64 cursor-pointer"
                    onClick={() => !isMyMod(mod) && onClick?.(mod)}
                >
                    <Tilt.Layer depth={2.2}>
                        <h2 className="absolute top-1.5 left-3 truncate max-w-58">
                            {mod.title}
                        </h2>
                    </Tilt.Layer>

                    <Tilt.Layer depth={1.1}>
                        <img
                            src={!isMyMod(mod) ? mod.previewImageUrl : mod.imageUrl}
                            alt={mod.title}
                            className="absolute left-1/2 top-10 w-[90%] -translate-x-1/2 aspect-video object-cover"
                        />
                    </Tilt.Layer>

                    {!isMyMod(mod) && (
                        <Tilt.Layer depth={0.55}>
                            <p className="absolute right-4 top-45">
                                By {mod.creator}
                            </p>
                        </Tilt.Layer>
                    )}

                    <Tilt.Layer depth={0.825}>
                        <p className="absolute left-16 top-58">
                            {mod.type}
                        </p>
                    </Tilt.Layer>

                    {isMyMod(mod) && (
                        <div className="absolute top-49 left-3 flex w-58 gap-2">
                            <button
                                onClick={(e) => {
                                    e.stopPropagation();
                                    onEdit?.(mod);
                                }}
                                className="flex-1 rounded border border-gray-500 py-1 text-xs font-medium hover:bg-white/10"
                            >
                                Edit
                            </button>
                            <button
                                onClick={(e) => {
                                    e.stopPropagation();
                                    onDelete?.(mod.modId);
                                }}
                                className="flex-1 rounded border border-red-400 py-1 text-xs font-medium text-red-400 hover:bg-red-500/10"
                            >
                                Delete
                            </button>
                        </div>
                    )}

                    <svg
                        className="pointer-events-none absolute inset-0 h-full w-full"
                        viewBox="0 0 256 256"
                        preserveAspectRatio="none"
                    >
                        <path d={cardShape} fill="none" stroke="#8e0093" strokeWidth="2" />
                    </svg>
                </div>
            </Tilt>

            {!isMyMod(mod) && mod.isFavourite !== null && (
                <button
                    onClick={(e) => {
                        e.stopPropagation();
                        onFavourite?.(mod.modId, mod.isFavourite!);
                    }}
                    className="absolute bottom-0 right-0 flex items-center gap-1.5 text-xs font-medium text-gray-400 transition-colors hover:text-gray-100"
                >
                    <img src={mod.isFavourite ? reactLogo : viteLogo} alt="" className="h-3.5 w-3.5" />
                    {mod.isFavourite ? 'Unfavourite' : 'Favourite'}
                </button>
            )}
        </div>
    );
};

export default ModCard;