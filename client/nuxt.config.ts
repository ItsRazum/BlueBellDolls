// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2025-07-15',
  devtools: { enabled: true },
  css: ['~/assets/css/main.css'],
  runtimeConfig: {
      public: {
          apiBase: ''
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
