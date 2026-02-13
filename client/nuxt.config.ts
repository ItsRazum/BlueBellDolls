import tailwindcss from "@tailwindcss/vite";

export default defineNuxtConfig({
  app: {
      head: {
          bodyAttrs: {
              class: 'preload'
          }
      }
  },
  compatibilityDate: "2025-07-15",
  devtools: { enabled: true },
  css: ["~/assets/css/main.css"],
  modules: ["nuxt-svgo", "@nuxtjs/i18n", "@nuxtjs/color-mode"],
  svgo: {
    autoImportPath: "./assets/",
      global: false,
      defaultImport: 'component',
      component: {
        rendersSvg: true,
        wrapper: false,
      }
  },
  vite: {
    plugins: [tailwindcss()],
  },
  runtimeConfig: {
    public: {
      apiBase: "http://localhost:5297",
    },
  },
  imports: {
    dirs: ["types/*.ts", "types/*.d.ts"],
  },
  components: [
    {
      path: "~/components",
      pathPrefix: false,
    },
  ],
  i18n: {
    langDir: "locales",
    locales: [
      { code: "ru", iso: "ru-RU", file: "ru.json", name: "Русский" },
      { code: "en", iso: "en-US", file: "en.json", name: "English" },
    ],
    defaultLocale: "ru",
    strategy: "prefix_except_default",

    detectBrowserLanguage: {
      useCookie: true,
      cookieKey: "i18n_redirected",
      redirectOn: "root",
    },
  },
  colorMode: {
    classSuffix: '',
    preference: 'system',
    fallback: 'light',

    dataValue: 'theme'
  }
});