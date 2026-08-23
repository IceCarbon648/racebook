import reactLogo from '../../../assets/images/react.svg';
import viteLogo from '../../../assets/images/vite.svg';
import type { Mod, MyMod } from '../../../types';
import type { ModCardProps } from './index.types';
import { useEffect, useRef } from 'react';
import VanillaTilt from 'vanilla-tilt';

const cardShape = "M 0,0 L 255,0 L 255,215 L 247,223 L 159,223 L 127,255 L 16,255 L 0,239 Z";

const isMyMod = (mod: Mod | MyMod): mod is MyMod => {
    return 'uid' in mod;
};

const ModCard = ({ mod, onClick, onEdit, onDelete, onFavourite }: ModCardProps) => {
    const tiltRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        if (tiltRef.current) {
            VanillaTilt.init(tiltRef.current, {
                max: 15,
                speed: 400,
                scale: 1.05
            });
        }

        return () => {
            tiltRef.current?.vanillaTilt?.destroy();
        };
    }, []);
    return (
        <div ref={tiltRef} className="relative" style={{ width: 256, height: 256 }}>
            <div className="relative" style={{ width: 256, height: 256 }}>
                <div className="absolute inset-0 flex flex-col cursor-pointer"
                    onClick={() => !isMyMod(mod) && onClick?.(mod)}>
                    <img
                        src={!isMyMod(mod) ? mod.previewImageUrl : mod.imageUrl}
                        alt={mod.title}
                        className="w-full h-44 object-cover"
                    />
                    <div className="flex justify-start gap-1 p-3">
                        <h3 className="text-sm font-semibold">{mod.title}</h3>
                        <p className="text-xs">{mod.type}</p>
                        {!isMyMod(mod) && (
                            <p className="text-xs">By {mod.creator}</p>
                        )}
                    </div>
                    {isMyMod(mod) && (
                        <div className="flex gap-2 p-3 border-t border-gray-200">
                            <button
                                onClick={(e) => {
                                    e.stopPropagation();
                                    onEdit?.(mod);
                                }}
                                className="flex-1 py-1.5 text-xs font-medium border border-gray-900 rounded hover:bg-gray-50"
                            >
                                Edit
                            </button>
                            <button
                                onClick={(e) => {
                                    e.stopPropagation();
                                    onDelete?.(mod.modId);
                                }}
                                className="flex-1 py-1.5 text-xs font-medium border border-red-200 rounded text-red-500 hover:bg-red-50"
                            >
                                Delete
                            </button>
                        </div>
                    )}
                </div>

                {!isMyMod(mod) && mod.isFavourite !== null && (
                    <button
                        className="absolute bottom-0 right-0 flex items-center gap-1 font-medium text-gray-500 hover:text-gray-900 transition-colors"
                        onClick={(e) => {
                            e.stopPropagation();
                            onFavourite?.(mod.modId, mod.isFavourite!);
                        }}
                    >
                        <img
                            src={mod.isFavourite ? reactLogo : viteLogo}
                            alt=""
                            className="w-4 h-4"
                        />
                        {mod.isFavourite ? 'Unfavourite' : 'Favourite'}
                    </button>
                )}

                <svg
                    className="absolute inset-0 pointer-events-none"
                    width="256"
                    height="256"
                    viewBox="0 0 256 256"
                >
                    <path
                        d={cardShape}
                        fill="none"
                        stroke="#8e0093"
                        strokeWidth="2"
                    />
                </svg>
            </div>
        </div>
    );
};

export default ModCard;