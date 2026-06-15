import { defineConfig } from 'vite'
import react, { reactCompilerPreset } from '@vitejs/plugin-react'
import babel from '@rolldown/plugin-babel'
import path from 'path'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    react(),
    babel({ presets: [reactCompilerPreset()] })
  ],
    resolve: {
      alias: {
      
      '@': path.resolve(__dirname, './src'), 
    },
  },
  build: {
    //Outputs the built React app directly into your .NET backend's wwwroot folder
    //Change 'API' below if your backend project folder is named something else (e.g., 'MyAnimeApp.Api')
    outDir: path.resolve(__dirname, '../../API/wwwroot'), 
    emptyOutDir: true, // Safely clears the folder before building
  },
})
