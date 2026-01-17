<script setup lang="ts">

definePageMeta({
  title: "Питомник Рэгдолл №1 в России.",
  backgroundImage: "/header.jpg",
  buttonText: "Котята на продажу",
  buttonUrl: "/litters"
})

const kittenApi = useKittenApi();

const { data: kittens, pending } = await kittenApi.getAvailableKittens();

</script>

<template>
  <main>
    <div class="articles-container">
      <Article title="О породе рэгдолл"
               photo-url="photo.png"
               description="Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
               redirect-url="/about-ragdoll"/>
      <Article title="Свободные котята"
               photo-url="photo.png"
               description="Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
               redirect-url="/litters"/>
      <Article title="О нас"
               photo-url="photo.png"
               description="Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
               redirect-url="/about"/>
    </div>
    <div class="first-message-container">
      <MessageBox imageUrl="/photo.png" text="Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua." />
    </div>
    <div v-if="pending" class="kittens-container">
      <h2 class="text-3xl">Котята на продажу</h2>
      <div class="carousel-wrapper">
        <KittenListItemSkeleton variant="compact"/>
        <KittenListItemSkeleton variant="compact"/>
        <KittenListItemSkeleton variant="compact"/>
      </div>
    </div>
    <div v-else-if="kittens.length === 0" class="kittens-container">
      <div class="no-kittens-container">
        <span class="font-bold text-3xl text-white">Скоро здесь появятся новые котята</span>
        <span class="text-2xl text-white text-center">Готовимся к пополнению в питомнике. Следите за обновлениями, чтобы не пропустить свое счастье.</span>
      </div>
    </div>
    <div v-else-if="kittens.length < 4" class="kittens-container">
      <h2 class="text-3xl">Котята на продажу</h2>
      <div class="carousel-wrapper">
        <KittenListItem variant="compact" v-for="kitten in kittens" :key="kitten.id" :kitten="kitten" />
      </div>
      <RouterLink to="litters" class="text-[1.25rem]">Показать больше</RouterLink>
    </div>
    <div v-else class="kittens-container">
      <h2 class="text-3xl">Котята на продажу</h2>
      <KittensCarousel :kittens="kittens" />
      <RouterLink to="litters" class="text-[1.25rem]">Показать больше</RouterLink>
    </div>
    <div class="cats-container">
      <span class="kittens-title">У нашего питомника только лучшие кошки-производители. Убедитесь сами</span>
      <div class="cats-articles-container">
        <Article title="Наши кошки"
                 photo-url="photo.png"
                 description="Узнайте больше о наших прекрасных дамах"
                 redirect-url="/litters"/>
        <Article title="Наши коты"
                 photo-url="photo.png"
                 description="Познакомьтесь с нашими очаровательными мальчиками"
                 redirect-url="/about"/>
      </div>
    </div>
  </main>
</template>

<style scoped>

.articles-container {
  display: flex;
  justify-content: center;
  background-color: var(--color-pages-primary-background);
  gap: var(--padding-extra-large);
  padding: var(--padding-pages);
}

.first-message-container {
  display: flex;
  background-color: var(--color-pages-secondary-background);
  padding: var(--padding-large) 5.375rem;
}

.kittens-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  background-color: var(--color-pages-primary-background);
  padding: var(--padding-large) var(--padding-extra-large);
  gap: var(--padding-extra-large);
}

.no-kittens-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: space-between;
  width: 48.75rem;
  height: 19rem;
  padding: var(--padding-large);
  border-radius: var(--border-radius-main);
  background-image: url("/photo.png");
  background-size: cover;
}

.carousel-wrapper {
  display: flex;
  align-items: center;
  gap: var(--padding-extra-large);
  justify-content: center;
  width: 100%;
  max-width: 1150px;
  margin: 0 auto;
}

.cats-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  background-color: var(--color-pages-secondary-background);
  gap: var(--padding-extra-large);
  padding: var(--padding-large);
}

.cats-articles-container {
  display: flex;
  justify-content: center;
  gap: var(--padding-large);
}

.kittens-title {
  font-weight: 500;
  font-size: 1.875rem;
  max-width: 55rem;
  text-align: center;
}

</style>
