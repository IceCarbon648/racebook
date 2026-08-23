import type { Mod, MyMod } from '../../../types';

export interface ModCardProps {
    mod: Mod | MyMod;
    onClick?: (mod: Mod) => void;
    onEdit?: (mod: MyMod) => void;
    onDelete?: (modId: string) => void;
    onFavourite?: (modId: string, isFavourite: boolean) => void;
}