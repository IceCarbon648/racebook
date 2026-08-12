import { Routes, Route } from 'react-router-dom';
import ProtectedRoute from './ProtectedRoute';
import Landing from '../pages/Landing';
import Login from '../pages/Login';
import Register from '../pages/Register';
import Mods from '../pages/Mods';
import MyMods from '../pages/MyMods';
import ModDetail from '../pages/ModDetail';
import Favourites from '../pages/Favourites';

const AppRoutes = () => {
    return (
        <Routes>
            <Route path="/" element={<Landing />} />
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />
            <Route path="/mods" element={<Mods />} />
            <Route path="/mods/:modId" element={<ModDetail />} />
            <Route path="/my-mods" element={
                <ProtectedRoute>
                    <MyMods />
                </ProtectedRoute>
            } />
            <Route path="/favourites" element={
                <ProtectedRoute>
                    <Favourites />
                </ProtectedRoute>
            } />
        </Routes>
    );
};

export default AppRoutes;