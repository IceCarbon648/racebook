import type { Mod, MyMod } from '../../../types';
import type { ModCardProps } from './index.types';
import './index.css';

const isMod = (mod: Mod | MyMod): mod is Mod => {
    return 'creator' in mod;
};

const isMyMod = (mod: Mod | MyMod): mod is MyMod => {
    return 'uid' in mod;
};

const ModCard = ({ mod, onClick, onEdit, onDelete }: ModCardProps) => {
    return (
        <div
            className="mod-card"
            onClick={() => isMod(mod) && onClick?.(mod)}
        >
            <img
                src={isMod(mod) ? mod.previewImageUrl : mod.imageUrl}
                alt={mod.title}
                className="mod-card-image"
            />
            <div className="mod-card-info">
                <h3 className="mod-card-title">{mod.title}</h3>
                <p className="mod-card-type">{mod.type}</p>
                {isMod(mod) && (
                    <p className="mod-card-creator">By {mod.creator}</p>
                )}
            </div>
            {isMyMod(mod) && (
                <div className="mod-card-actions">
                    <button
                        className="mod-card-edit"
                        onClick={(e) => {
                            e.stopPropagation();
                            onEdit?.(mod);
                        }}
                    >
                        Edit
                    </button>
                    <button
                        className="mod-card-delete"
                        onClick={(e) => {
                            e.stopPropagation();
                            onDelete?.(mod.modId);
                        }}
                    >
                        Delete
                    </button>
                </div>
            )}
        </div>
    );
};

export default ModCard;