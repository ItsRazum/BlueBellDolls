<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, resolveComponent, watch } from 'vue';

const props = defineProps<{
  title: string;
  backgroundImage: string;
  buttonText?: string;
  buttonUrl?: string;
}>();

const { locale, setLocale, locales } = useI18n();
const config = useAppConfig();
const colorMode = useColorMode();

const { lock, unlock } = useBodyScrollLock();

const isLangMenuOpen = ref(false);
const langMenuRef = ref<HTMLElement | null>(null);
const isTop = ref(true);
let scrollUnlockTimer: ReturnType<typeof setTimeout> | null = null;

const {
  isOpen: isMobileMenuOpen,
  toggle: toggleMobileMenu,
  close: closeMobileMenu
} = useModal();

const flagIcons: Record<string, any> = {
  ru: resolveComponent('SvgoRu'),
  en: resolveComponent('SvgoUs')
};

const bgStyle = computed(() => `url('${props.backgroundImage}')`);

const toggleLangMenu = () => {
  isLangMenuOpen.value = !isLangMenuOpen.value;
};

const selectLanguage = (code: string) => {
  setLocale(code);
  isLangMenuOpen.value = false;
};

const handleScroll = () => {
  if (typeof window !== "undefined") {
    isTop.value = window.scrollY === 0;
  }
};

const toggleTheme = () => {
  colorMode.preference = colorMode.value === 'dark' ? 'light' : 'dark';
};

const closeMenu = (e: Event) => {
  if (langMenuRef.value && !langMenuRef.value.contains(e.target as Node)) {
    isLangMenuOpen.value = false;
  }
};

watch(isMobileMenuOpen, (isOpen) => {
  if (typeof window === 'undefined') return;

  if (scrollUnlockTimer) {
    clearTimeout(scrollUnlockTimer);
    scrollUnlockTimer = null;
  }

  if (isOpen) {
    lock();
  } else {
    scrollUnlockTimer = setTimeout(() => {
      unlock();
    }, 300);
  }
});

onMounted(() => {
  handleScroll();
  window.addEventListener("scroll", handleScroll);
  window.addEventListener("click", closeMenu);
});

onUnmounted(() => {
  window.removeEventListener("scroll", handleScroll);
  window.removeEventListener("click", closeMenu);
  if (scrollUnlockTimer) clearTimeout(scrollUnlockTimer);
  unlock();
});
</script>

<template>
  <Teleport to="body">
    <Transition name="fade">
      <div
          v-if="isMobileMenuOpen"
          class="menu-backdrop"
          @click="closeMobileMenu"
      ></div>
    </Transition>
  </Teleport>

  <div
      class="navigation"
      :class="{
      transparent: isTop && !isMobileMenuOpen,
      expanded: isMobileMenuOpen
    }"
  >

    <nav class="navigation-container">
      <div class="logo-wrapper">
        <NuxtLinkLocale to="/">
          <SvgoLogo class="link main" :class="{ transparent: isTop && !isMobileMenuOpen }" />
        </NuxtLinkLocale>
      </div>

      <div class="links-container desktop-only">
        <div class="other-links">
          <NuxtLinkLocale to="/litters" class="link secondary" :class="{ transparent: isTop }">
            {{ $t("nav.kittens") }}
          </NuxtLinkLocale>
          <NuxtLinkLocale to="/cats/females" class="link secondary" :class="{ transparent: isTop }">
            {{ $t("nav.femalecats") }}
          </NuxtLinkLocale>
          <NuxtLinkLocale to="/cats/males" class="link secondary" :class="{ transparent: isTop }">
            {{ $t("nav.malecats") }}
          </NuxtLinkLocale>
          <NuxtLinkLocale to="/catcolors" class="link secondary" :class="{ transparent: isTop }">
            {{ $t("nav.catcolors") }}
          </NuxtLinkLocale>
          <NuxtLinkLocale to="/gallery" class="link secondary" :class="{ transparent: isTop }">
            {{ $t("nav.gallery") }}
          </NuxtLinkLocale>
        </div>
      </div>

      <div class="right-actions">
        <div class="control-buttons" :class="{ transparent: isTop && !isMobileMenuOpen }">
          <button @click="toggleTheme" class="toggle-button" :class="{ transparent: isTop && !isMobileMenuOpen }">
            <ClientOnly>
              <template #default>
                <SvgoSun v-if="colorMode.value === 'dark'" class="icon-svg" />
                <SvgoMoon v-else class="icon-svg" />
              </template>
              <template #fallback>
                <div class="icon-svg" />
              </template>
            </ClientOnly>
          </button>

          <div class="lang-container" ref="langMenuRef">
            <button
                @click.stop="toggleLangMenu"
                class="toggle-button"
                :class="{ transparent: isTop && !isMobileMenuOpen, active: isLangMenuOpen }"
            >
              <SvgoLang class="icon-svg" />
            </button>

            <Transition name="fade-slide">
              <div v-if="isLangMenuOpen" class="lang-dropdown">
                <div class="lang-list">
                  <button
                      v-for="lang in locales"
                      :key="lang.code"
                      class="lang-item"
                      :class="{ active: locale === lang.code }"
                      @click="selectLanguage(lang.code)"
                  >
                    <component :is="flagIcons[lang.code]" class="icon-flag" />
                    <span class="lang-code">{{ lang.name }}</span>
                  </button>
                </div>
              </div>
            </Transition>
          </div>
        </div>

        <button
            class="burger-button mobile-only"
            :class="{ 'active': isMobileMenuOpen, 'transparent': isTop && !isMobileMenuOpen }"
            @click="toggleMobileMenu"
        >
          <span></span>
          <span></span>
          <span></span>
        </button>
      </div>
    </nav>

    <div class="mobile-dropdown" :class="{ open: isMobileMenuOpen }">
      <div class="mobile-dropdown-inner">
        <div class="mobile-dropdown-container">
          <div class="mobile-links-list">
            <NuxtLinkLocale to="/litters" class="link secondary" @click="closeMobileMenu">
              {{ $t("nav.kittens") }}
            </NuxtLinkLocale>
            <NuxtLinkLocale to="/cats/females" class="link secondary" @click="closeMobileMenu">
              {{ $t("nav.femalecats") }}
            </NuxtLinkLocale>
            <NuxtLinkLocale to="/cats/males" class="link secondary" @click="closeMobileMenu">
              {{ $t("nav.malecats") }}
            </NuxtLinkLocale>
            <NuxtLinkLocale to="/catcolors" class="link secondary" @click="closeMobileMenu">
              {{ $t("nav.catcolors") }}
            </NuxtLinkLocale>
            <NuxtLinkLocale to="/gallery" class="link secondary" @click="closeMobileMenu">
              {{ $t("nav.gallery") }}
            </NuxtLinkLocale>
          </div>
          <div class="mobile-contacts-block">
            <h2 class="mobile-column-title">{{ $t('common.contacts.title') }}</h2>
            <a :href='`tel:` + config.contacts.phoneLink' class="contact-item">
              <span>{{config.contacts.phoneDisplay}}</span>
            </a>
            <a :href='`mailto:` + config.contacts.email' class="contact-item">
              <span>{{config.contacts.email}}</span>
            </a>
            <div class="social-row">
              <a :href='config.contacts.telegram' class="social-btn">Telegram</a>
              <a :href='config.contacts.whatsapp' class="social-btn">WhatsApp</a>
            </div>
            <p class="address-text">
              {{$t('common.contacts.address')}}
            </p>
          </div>
        </div>
      </div>
    </div>

  </div>

  <div class="image-container">
    <div class="title-container">
      <h1 class="title">{{ $t(title) }}</h1>
      <NuxtLinkLocale class="button-link" v-if="buttonUrl" :to="buttonUrl">{{
          $t(buttonText ?? "")
        }}</NuxtLinkLocale>
    </div>
  </div>

  <span class="indev">Сайт находится в разработке<br>некоторые элементы могут претерпеть изменения в итоговой версии.</span>
</template>

<style scoped>

.menu-backdrop {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  background-color: rgba(0, 0, 0, 0.5);
  backdrop-filter: blur(4px);
  z-index: 99;
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

.navigation {
  z-index: 100;
  background-color: var(--color-background-card);
  backdrop-filter: blur(var(--blur-base));
  -webkit-backdrop-filter: blur(var(--blur-base));
  padding: 0.75rem 0;
  border-bottom: 1px solid var(--color-border-card);

  display: flex;
  flex-direction: column;
  justify-content: center;
  position: fixed;
  left: 0;
  right: 0;
  top: 0;
  width: 100%;
  transition: background-color 0.3s ease, border-color 0.3s ease;
}

.navigation.transparent {
  background-color: transparent;
  border-color: transparent;
  box-shadow: none;
  backdrop-filter: none;
  -webkit-backdrop-filter: none;
}

.navigation-container {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 2rem;
  width: 100%;
  max-width: 1280px;
  margin: 0 auto;
  box-sizing: border-box;
}

.right-actions {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.links-container {
  display: flex;
  align-items: center;
  flex-direction: row;
}

.other-links {
  display: flex;
  gap: 2.5rem;
  margin: 0 4.75rem;
}

.link {
  color: var(--color-text-base);
  text-decoration: none;
  transition: color 0.3s;
}

.link.main {
  width: 190px;
  height: 45px;
  margin: 0.5rem 0 0 0;
}

.link.secondary {
  font-size: 1.25rem;
  padding: var(--padding-small) var(--padding-large);
  border-radius: var(--border-radius-main);
  transition: background-color 0.3s, color 0.3s, backdrop-filter 0.3s;
}

.link.transparent {
  color: white;
}

.secondary:hover {
  background-color: var(--color-hover);
}

.secondary.transparent:hover {
  background-color: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(var(--blur-base));
}

.control-buttons {
  display: flex;
  align-items: center;
  gap: 4px;
  padding: 4px;
  border-radius: 9999px;

  background-color: rgba(255, 255, 255, 0.1);
  border: 1px solid var(--color-border-card, transparent);
  transition: background-color 0.3s, border-color 0.3s;
}

.control-buttons.transparent {
  background-color: rgba(255, 255, 255, 0.1);
  border-color: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(var(--blur-base));
  -webkit-backdrop-filter: blur(var(--blur-base));
}

.toggle-button {
  all: unset;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;

  width: 40px;
  height: 40px;
  border-radius: 50%;

  color: var(--color-text-base);
  transition: background-color 0.2s ease, color 0.2s ease;
}

.toggle-button:hover, .toggle-button.active {
  background-color: rgba(0, 0, 0, 0.1);
}

.toggle-button.transparent {
  color: white;
}

.toggle-button.transparent:hover, .toggle-button.transparent.active {
  background-color: rgba(255, 255, 255, 0.25);
}

.icon-svg {
  width: 24px;
  height: 24px;
  margin: 0;
}

.burger-button {
  display: none;
  flex-direction: column;
  justify-content: space-between;
  width: 30px;
  height: 20px;
  background: transparent;
  border: none;
  cursor: pointer;
  padding: 0;
  z-index: 201;
  box-shadow: none;
}

.burger-button span {
  width: 100%;
  height: 2px;
  background-color: var(--color-text-base);
  border-radius: 2px;
  transition: all 0.3s ease;
}

.burger-button.transparent span {
  background-color: white;
}

.burger-button.active span:nth-child(1) {
  transform: translateY(9px) rotate(45deg);
}
.burger-button.active span:nth-child(2) {
  opacity: 0;
}
.burger-button.active span:nth-child(3) {
  transform: translateY(-9px) rotate(-45deg);
}

.lang-container {
  position: relative;
}

.lang-dropdown {
  position: absolute;
  top: calc(100% + 0.5rem);
  right: 0;
  min-width: 120px;
  z-index: 200;
}

.lang-list {
  display: flex;
  flex-direction: column;
  padding: var(--padding-small);
  gap: 0.25rem;

  background-color: var(--color-background-lang-selector);
  border: 1px solid var(--color-border-card);
  border-radius: var(--border-radius-main);
  box-shadow: var(--shadow-base);
  backdrop-filter: blur(var(--blur-base));
}

.lang-item {
  all: unset;
  display: flex;
  align-items: center;
  padding: 0.5rem 1rem;
  border-radius: 0.5rem;
  cursor: pointer;
  transition: background-color 0.2s;
  color: var(--color-text-base);
  font-weight: 500;
}

.lang-item:hover {
  background-color: var(--color-hover);
}

.lang-item.active {
  background-color: var(--color-background-lang-item);
  font-weight: 700;
}

.icon-flag {
  width: 24px;
  height: 24px;
  object-fit: cover;
  margin-right: 0.5rem;
}

.mobile-dropdown {
  display: grid;
  grid-template-rows: 0fr;
  transition: grid-template-rows 0.3s ease-out;
}

.mobile-dropdown-container {
  display: flex;
  justify-content: space-evenly;
  padding: 1.5rem 1rem;
  gap: 2rem;
}

.mobile-dropdown.open {
  grid-template-rows: 1fr;
  border-top: 1px solid var(--color-border-card);
  margin-top: 0.5rem;
}

.mobile-dropdown-inner {
  overflow: hidden;
  padding: 0;
}

.mobile-links-list {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
}

.mobile-contacts-block {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.mobile-column-title {
  font-size: 1rem;
  font-weight: 700;
  margin: 0;
  text-transform: uppercase;
}

.image-container {
  display: flex;
  align-items: center;
  background-image: v-bind(bgStyle);
  background-size: cover;
  width: 100%;
  border-inline: var(--border-global);
  height: 31.25rem;
  max-width: 1280px;
  padding: 0;
  margin: 0 auto;
}

.title-container {
  display: flex;
  align-items: flex-start;
  flex-direction: column;
  gap: var(--padding-medium);
  padding: var(--padding-extra-large);
}

.title {
  color: white;
  max-width: 31.25rem;
  margin: 0;
}

.button-link {
  all: unset;
  background-color: white;
  padding: 0.625rem 1.25rem;
  border-radius: 1.5rem;
  font-family: var(--font-family-base);
  transition: background-color 0.3s ease;
  font-size: 1.125rem;
  color: black;
  cursor: pointer;
}

.button-link:hover {
  background-color: rgb(235, 235, 245);
}

.contact-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  text-decoration: none;
  color: var(--color-text-base);
  font-size: 1rem;
  font-weight: 500;
  transition: opacity 0.2s;
}

.contact-item:hover {
  opacity: 0.8;
}

.social-row {
  display: flex;
  gap: 0.5rem;
}

.social-btn {
  padding: 0.4rem 0.8rem;
  border-radius: 6px;
  background-color: rgba(255,255,255, 0.1);
  color: var(--color-text-base);
  font-size: 0.9rem;
  text-decoration: none;
  border: 1px solid var(--color-border-card);
}

.address-text {
  font-size: 0.9rem;
  opacity: 0.7;
  line-height: 1.4;
  margin: 0;
}

.indev {
  position: fixed;
  bottom: 0;
  left: 0;
  margin: 20px;
  color: red;
  z-index: 1000;
  font-weight: bold;
}

.fade-slide-enter-active,
.fade-slide-leave-active {
  transition: all 0.2s ease;
}

.fade-slide-enter-from,
.fade-slide-leave-to {
  opacity: 0;
  transform: translateY(-10px);
}

@media (max-width: 1192px) {
  .links-container {
    flex: 1;
    width: auto;
  }
  .other-links {
    margin: 0 1rem;
    justify-content: space-evenly;
    gap: unset;
    width: 100%;
  }
}

@media (max-width: 934px) {
  .desktop-only {
    display: none !important;
  }

  .burger-button.mobile-only {
    display: flex;
  }

  .link.main {
    height: auto;
  }
}

@media (min-width: 935px) {
  .mobile-dropdown {
    display: none !important;
  }
  .burger-button.mobile-only {
    display: none !important;
  }
}

@media (max-width: 534px) {
  .link.main {
    width: 150px;
    height: auto;
  }

  .mobile-dropdown-container {
    flex-direction: column;
    align-items: center;
    text-align: center;
  }

  .social-row {
    justify-content: center;
  }
}

</style>