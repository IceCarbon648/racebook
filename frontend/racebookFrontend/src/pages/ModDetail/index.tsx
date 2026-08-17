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
        <div className="flex gap-10 px-6 py-8 max-w-5xl mx-auto">
            <div className="flex-shrink-0 w-80">
                <img
                    src={mod.previewImageUrl}
                    alt={mod.title}
                    className="w-full rounded-lg object-cover"
                />
            </div>
            <div className="flex flex-col gap-6 flex-1">
                <div className="flex items-center justify-between">
                    <h1 className="text-2xl font-bold text-gray-900">{mod.title}</h1>
                    <button
                        onClick={handleDownload}
                        className="px-6 py-2 text-sm font-medium border border-gray-900 rounded hover:bg-gray-50"
                    >
                        Download
                    </button>
                </div>
                <div className="flex flex-col gap-3">
                    <div className="flex gap-4 text-sm">
                        <span className="font-medium text-gray-900 w-24">Creator</span>
                        <span className="text-gray-500">{mod.creator}</span>
                    </div>
                    <div className="flex gap-4 text-sm">
                        <span className="font-medium text-gray-900 w-24">Category</span>
                        <span className="text-gray-500">{mod.type}</span>
                    </div>
                    <div className="flex gap-4 text-sm">
                        <span className="font-medium text-gray-900 w-24">Uploaded</span>
                        <span className="text-gray-500">{new Date(mod.uploadDate).toLocaleDateString()}</span>
                    </div>
                    <div className="flex gap-4 text-sm">
                        <span className="font-medium text-gray-900 w-24">Last edited</span>
                        <span className="text-gray-500">{new Date(mod.editDate).toLocaleDateString()}</span>
                    </div>
                </div>
                <div className="flex flex-col gap-2">
                    <h2 className="text-lg font-semibold text-gray-900">Description</h2>
                    <p className="text-sm text-gray-500 leading-relaxed">{mod.description}</p>
                </div>
            </div>
        </div>
    );
};

export default ModDetail;