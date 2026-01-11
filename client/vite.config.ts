import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react-swc';

// https://vitejs.dev/config/
export default defineConfig({
  build: {
    outDir: '../server/Services/Restore/Restore.API/wwwroot',
  },
  server: {
    port: 3000,
    host: true, // Enable to listen on all addresses (needed for Docker)
    strictPort: true,
    watch: {
      usePolling: true, // Enable polling for Docker volumes
    },
  },
  plugins: [react()],
});
