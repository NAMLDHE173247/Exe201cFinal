import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      "/api": {
        target: "http://localhost:5082", // backend API
        changeOrigin: true,
      },
      "/images": {
        target: "http://localhost:5082", // static files backend
        changeOrigin: true,
      },
    },
  },
})
