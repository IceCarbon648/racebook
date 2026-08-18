import { useLocation, useNavigate } from 'react-router-dom';
import type { Mod } from '../../types';

const ModDetail = () => {
    const location = useLocation();
    const navigate = useNavigate();
    const mod = location.state?.mod as Mod;

    if (!mod) {
        navigate('/mods');
        return null;
    }

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