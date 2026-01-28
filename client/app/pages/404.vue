<script setup lang="ts">
definePageMeta({
  title: "pages.errors.notFound.title",
  backgroundImage: "/header.jpg",
  buttonText: "pages.errors.notFound.buttonText",
  buttonUrl: "/",
});
</script>

<template>
  <div class="content-container">
    <h2 class="text-3xl">Кажется, вы заблудились...</h2>
    <p class="text-2xl text-center">Может, вы имели ввиду эти страницы?</p>
  </div>
  <div class="articles-container">
    <Article
      title="Главная страница"
      photo-url="photo.png"
      description="Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
      redirect-url="/about-ragdoll"
    />
    <Article
      title="Свободные котята"
      photo-url="photo.png"
      description="Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
      redirect-url="/litters"
    />
    <Article
      title="О породе рэгдолл"
      photo-url="photo.png"
      description="Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
      redirect-url="/about"
    />
  </div>
</template>

<style scoped>
.content-container {
  display: flex;
  align-items: center;
  flex-direction: column;
  gap: var(--padding-large);
  background-color: var(--color-pages-secondary-background);
  padding: var(--padding-pages);
  padding-bottom: var(--padding-small);
}

.articles-container {
  display: flex;
  justify-content: center;
  background-color: var(--color-pages-primary-background);
  gap: var(--padding-extra-large);
  padding: var(--padding-pages);
}

.btn {
  margin: var(--padding-large);
  margin-top: 0;
}
</style>
