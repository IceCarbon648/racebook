import type { Mod, MyMod } from '../../../types';
import type { ModCardProps } from './index.types';

const isMod = (mod: Mod | MyMod): mod is Mod => {
    return 'creator' in mod;
};

const isMyMod = (mod: Mod | MyMod): mod is MyMod => {
    return 'uid' in mod;
};

const ModCard = ({ mod, onClick, onEdit, onDelete }: ModCardProps) => {
    return (
        <div
            className="flex flex-col border border-gray-200 rounded-lg overflow-hidden cursor-pointer hover:shadow-md transition-shadow"
            onClick={() => isMod(mod) && onClick?.(mod)}
        >
            <img
                src={isMod(mod) ? mod.previewImageUrl : mod.imageUrl}
                alt={mod.title}
                className="w-full h-44 object-cover"
            />
            <div className="flex flex-col gap-1 p-3">
                <h3 className="text-sm font-semibold text-gray-900">{mod.title}</h3>
                <p className="text-xs text-gray-500">{mod.type}</p>
                {isMod(mod) && (
                    <p className="text-xs text-gray-400">By {mod.creator}</p>
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
    );
};

export default ModCard;