// https://nuxt.com/docs/api/configuration/nuxt-config
import tailwindcss from "@tailwindcss/vite";

export default defineNuxtConfig({
  compatibilityDate: '2025-07-15',
  devtools: { enabled: true },
  css: ['~/assets/css/main.css'],
  vite: {
      plugins: [
          tailwindcss()
      ]
  },
  runtimeConfig: {
      public: {
          apiBase: 'localhost:5070/api'
      }
  },
  imports: {
      dirs: ['types/*.ts', 'types/*.d.ts']
  },
    components: [
        {
            path: '~/components',
            pathPrefix: false,
        },
    ],
});
