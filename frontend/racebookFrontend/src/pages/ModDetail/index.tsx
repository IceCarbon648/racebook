import { useParams, useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { getAllMods } from '../../services';

const ModDetail = () => {
    const { modId } = useParams<{ modId: string }>();
    const navigate = useNavigate();

    const { data: mods = [], isLoading, isError } = useQuery({
        queryKey: ['mods'],
        queryFn: getAllMods,
    });

    const mod = mods.find((m) => m.modId === modId);

    if (isLoading) return (
        <div className="flex items-center justify-center min-h-[calc(100vh-4rem)]">
            <p className="text-gray-500">Loading mod...</p>
        </div>
    );

    if (isError || !mod) return (
        <div className="flex flex-col items-center justify-center gap-4 min-h-[calc(100vh-4rem)]">
            <p className="text-gray-500">Mod not found</p>
            <button
                onClick={() => navigate('/mods')}
                className="px-4 py-2 text-sm font-medium border border-gray-900 rounded hover:bg-gray-50"
            >
                Back to mods
            </button>
        </div>
    );

    const handleDownload = () => {
        window.open(mod.modFileUrl, '_blank');
    };

    return (
        <div className="flex gap-12 px-10 py-10 min-h-[calc(100vh-4rem)]">
            <div className="flex flex-col items-start gap-6 w-[45%]">
                <div className="flex flex-col gap-2">
                    <h1 className="text-5xl font-bold text-gray-900">{mod.title}</h1>
                    <span className="inline-flex self-start px-3 py-1 text-xs font-medium border border-gray-300 rounded-full">
                        {mod.type}
                    </span>
                </div>
                <div className="flex flex-col items-start gap-1.5 text-sm text-gray-500">
                    <p>By {mod.creator}</p>
                    <p>Uploaded {new Date(mod.uploadDate).toLocaleDateString()}</p>
                    <p>Last edited {new Date(mod.editDate).toLocaleDateString()}</p>
                </div>
                <p className="text-sm text-gray-600 leading-relaxed">
                    {mod.description}
                </p>
                <button
                    onClick={handleDownload}
                    className="w-full py-3 text-sm font-medium border border-gray-900 rounded-lg hover:bg-gray-50 mt-auto"
                >
                    Download
                </button>
            </div>

            <div className="flex-1">
                <img
                    src={mod.previewImageUrl}
                    alt={mod.title}
                    className="aspect-video h-[75%] object-cover rounded-xl"
                />
            </div>
        </div>
    );
};

export default ModDetail;