export interface ModCardProps {
    title: string;
    type: string;
    imageUrl: string;
    creator?: string;
    isFavourite?: boolean;
    onClick?: () => void;
    onEdit?: () => void;
    onDelete?: () => void;
    onFavourite?: () => void;
}