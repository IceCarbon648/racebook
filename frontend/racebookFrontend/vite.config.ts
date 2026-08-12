import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-react';

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [plugin()],
    server: {
        host: true,
        port: 3000,
        proxy: {
            '/api': {
                target: 'http://api:8080',
                changeOrigin: true,
                secure: false
            }
        }
    }
})
