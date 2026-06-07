import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/api': {
        // HTTP avoids Node rejecting the dev HTTPS certificate (dotnet dev-certs).
        // launchSettings "http" profile: http://localhost:5022
        target: 'http://localhost:5022',
        changeOrigin: true,
        secure: false,
      },
    },
  },
})
