import { BrowserRouter } from 'react-router-dom';
import { AuthProvider } from './contexts';
import { AppRoutes } from './routes';
import { Navbar } from './components';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

const App = () => {
    return (
        <BrowserRouter>
            <AuthProvider>
                <Navbar />
                <AppRoutes />
                <ToastContainer
                    position="top-center"
                    theme="dark"
                    autoClose={4000}
                    newestOnTop
                />
            </AuthProvider>
        </BrowserRouter>
    );
};

export default App;