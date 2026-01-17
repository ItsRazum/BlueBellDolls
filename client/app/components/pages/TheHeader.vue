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
  if (typeof window !== 'undefined') {
    isTop.value = window.scrollY === 0;
  }
};

onMounted(() => {
  handleScroll();

  window.addEventListener('scroll', handleScroll);
});

onUnmounted(() => {
  window.removeEventListener('scroll', handleScroll);
});

</script>

<template>
  <div class="navigation" :class="{'transparent': isTop}">
    <div class="links-container">
      <NuxtLink to="/" class="link main" :class="{'transparent': isTop}">BlueBellDolls</NuxtLink>
      <div class="other-links">
        <NuxtLink to="/litters?page=1" class="link secondary" :class="{'transparent': isTop}">Помёты</NuxtLink>
        <NuxtLink to="/parentcats?isMale=false&page=1" class="link secondary" :class="{'transparent': isTop}">Кошки</NuxtLink>
        <NuxtLink to="/parentcats?isMale=true&page=1" class="link secondary" :class="{'transparent': isTop}">Коты</NuxtLink>
        <NuxtLink to="/catcolors?page=1" class="link secondary" :class="{'transparent': isTop}">Окрасы</NuxtLink>
        <NuxtLink to="/gallery" class="link secondary" :class="{'transparent': isTop}">Галерея</NuxtLink>
      </div>
    </div>
  </div>
  <div class="image-container">
    <div class="title-container">
      <h1 class="title">{{title}}</h1>
      <NuxtLink class="button-link" v-if="buttonUrl" :to="buttonUrl">{{buttonText}}</NuxtLink>
    </div>
  </div>
</template>

<style scoped>

.navigation {
  z-index: 100;
  background-color: var(--color-background-card);
  backdrop-filter: blur(var(--blur-base));
  -webkit-backdrop-filter: blur(var(--blur-base));
  padding: 0.750rem 0;
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

.links-container {
  display: flex;
  align-items: center;
  gap: 4.75rem;
  width: 100%;
  max-width: 1280px;
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
  font-size: 1.75rem;
  font-weight: bold;
  padding-left: 2rem;
}

.link.secondary {
  font-size: 1.25rem;
  padding: var(--padding-small)  var(--padding-large);
  border-radius: var(--border-radius-main);
  transition: backdrop-filter 0.3s, background-color 0.3s;
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

</style>