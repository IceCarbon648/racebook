import { useLocation, useNavigate } from 'react-router-dom';
import type { Mod } from '../../types';
import './index.css';

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
        <div className="mod-detail">
            <div className="mod-detail-image-container">
                <img
                    src={mod.previewImageUrl}
                    alt={mod.title}
                    className="mod-detail-image"
                />
            </div>
            <div className="mod-detail-content">
                <div className="mod-detail-header">
                    <h1>{mod.title}</h1>
                    <button className="mod-detail-download" onClick={handleDownload}>
                        Download
                    </button>
                </div>
                <div className="mod-detail-meta">
                    <p><span>Creator</span>{mod.creator}</p>
                    <p><span>Category</span>{mod.type}</p>
                    <p><span>Uploaded</span>{new Date(mod.uploadDate).toLocaleDateString()}</p>
                    <p><span>Last edited</span>{new Date(mod.editDate).toLocaleDateString()}</p>
                </div>
                <div className="mod-detail-description">
                    <h2>Description</h2>
                    <p>{mod.description}</p>
                </div>
            </div>
        </div>
    );
};

export default ModDetail;