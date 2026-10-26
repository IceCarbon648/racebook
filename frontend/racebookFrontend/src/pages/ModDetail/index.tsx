import { useParams, useNavigate } from 'react-router-dom';
import { NEON, PRIMARY_BUTTON_COLOUR, PRIMARY_BUTTON_HOVER_COLOUR } from '../../constants/theme';
import Background from '../../components/molecules/Background/Index';
import { useMods } from '../../hooks/useMods';

const ModDetail = () => {
    const { modId } = useParams<{ modId: string }>();
    const navigate = useNavigate();

    const { mods, isLoading, isError } = useMods();
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
        <div className="flex justify-center gap-12 px-10 py-10 min-h-[calc(100vh-4rem)]">
            <Background reveal={false} />

            <div className="flex flex-col items-center justify-center gap-6 w-1/2">
                <div className="flex flex-col gap-2">
                    <h1 className="text-5xl font-bold text-white">{mod.title}</h1>

                    <div className="flex justify-center gap-8">
                        <p className="text-gray-100">@{mod.creator}</p>

                        <span
                            className="inline-flex self-start px-4 py-1 text-[10px] font-bold tracking-wide text-white rounded-full"
                            style={{ backgroundColor: NEON.fill }}
                        >
                            {mod.type}
                        </span>
                    </div>
                </div>
                <div className="flex gap-6 text-sm text-gray-200">
                    <p>Uploaded {new Date(mod.uploadDate).toLocaleDateString()}</p>
                    <p>Last edited {new Date(mod.editDate).toLocaleDateString()}</p>
                </div>
                <p className="text-sm text-gray-300">
                    {mod.description}
                </p>
                <button
                    onClick={handleDownload}
                    className={`px-4 py-2 bg-[${PRIMARY_BUTTON_COLOUR}] hover:bg-[${PRIMARY_BUTTON_HOVER_COLOUR}] text-sm text-white rounded`}
                >
                    Download
                </button>
            </div>

            <div className="flex items-center w-1/2">
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