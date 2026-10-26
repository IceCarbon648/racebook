import { BrowserRouter } from 'react-router-dom';
import { AuthProvider } from './contexts';
import { AppRoutes } from './routes';
import { Navbar } from './components';

const App = () => {
    return (
        <BrowserRouter>
            <AuthProvider>
                <Navbar />
                <AppRoutes />
            </AuthProvider>
        </BrowserRouter>
    );
};

export default App;