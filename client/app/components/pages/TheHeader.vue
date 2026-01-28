<script setup lang="ts">
const props = defineProps<{
  title: string;
  backgroundImage: string;
  buttonText?: string;
  buttonUrl?: string;
}>();

const bgStyle = computed(() => `url('${props.backgroundImage}')`);

const isTop = ref(true);

const handleScroll = () => {
  if (typeof window !== "undefined") {
    isTop.value = window.scrollY === 0;
  }
};

onMounted(() => {
  handleScroll();

  window.addEventListener("scroll", handleScroll);
});

onUnmounted(() => {
  window.removeEventListener("scroll", handleScroll);
});
</script>

<template>
  <div class="navigation" :class="{ transparent: isTop }">

    <nav class="navigation-container">
      <div class="links-container">
        <NuxtLinkLocale to="/">
          <SvgoLogo class="link main" :class="{ transparent: isTop }" />
        </NuxtLinkLocale>
        <div class="other-links">
          <NuxtLinkLocale to="/litters" class="link secondary" :class="{ transparent: isTop }">{{
              $t("nav.kittens")
            }}</NuxtLinkLocale>
          <NuxtLinkLocale to="/cats/females" class="link secondary" :class="{ transparent: isTop }">{{
              $t("nav.femalecats")
            }}</NuxtLinkLocale>
          <NuxtLinkLocale to="/cats/males" class="link secondary" :class="{ transparent: isTop }">{{
              $t("nav.malecats")
            }}</NuxtLinkLocale>
          <NuxtLinkLocale to="/catcolors" class="link secondary" :class="{ transparent: isTop }">{{
              $t("nav.catcolors")
            }}</NuxtLinkLocale>
          <NuxtLinkLocale to="/gallery" class="link secondary" :class="{ transparent: isTop }">{{
              $t("nav.gallery")
            }}</NuxtLinkLocale>
        </div>
      </div>

      <div>
        <SvgoMoon/>
      </div>
    </nav>
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
.navigation {
  z-index: 100;
  background-color: var(--color-background-card);
  backdrop-filter: blur(var(--blur-base));
  -webkit-backdrop-filter: blur(var(--blur-base));
  padding: 0.75rem 0;
  border: 1px solid var(--color-border-card);
  display: flex;
  justify-content: center;
  position: fixed;
  left: 0;
  right: 0;
  top: 0;
  width: 100%;
  margin: 0;
}

.navigation.transparent {
  background-color: transparent;
  border-color: transparent;
  box-shadow: none;
  backdrop-filter: none;
}

.navigation-container {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 2rem;
  width: 100%;
  max-width: 1280px;
}

.links-container {
  display: flex;
  align-items: center;
  flex-direction: row;
  gap: 4.75rem;
}

.other-links {
  display: flex;
  gap: 2.5rem;
}

.link {
  color: var(--color-text-base);
  text-decoration: none;
}

.link.main {
  font-size: 190px;
  height: auto;
  margin-bottom: 0;
  margin-top: 0.5rem;
}

.link.secondary {
  font-size: 1.25rem;
  padding: var(--padding-small) var(--padding-large);
  border-radius: var(--border-radius-main);
  transition:
    backdrop-filter 0.3s,
    background-color 0.3s;
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

.indev {
  position: fixed;
  bottom: 0;
  left: 0;
  margin: 20px;
  color: red;
  z-index: 1000;
  font-weight: bold
}
</style>
