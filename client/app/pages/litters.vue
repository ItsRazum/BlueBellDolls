<script setup lang="ts">

definePageMeta({
  title: "Лучшие котята Рэгдолл для вашей семьи.",
  backgroundImage: "/header.jpg"
});

const litterApi = useLitterApi();

const { data: littersPage, pending } = await litterApi.getByPage();

</script>

<template>
  <main>
    <div class="first-message-container">
      <message-box image-url="/photo.png" text="Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua." />
    </div>
    <div class="articles-container">
      <Article title="Условия брони котёнка"
               photo-url="photo.png"
               description="Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
               redirect-url="/about-ragdoll"/>
      <Article title="Подготовка к переезду"
               photo-url="photo.png"
               description="Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
               redirect-url="/litters"/>
      <Article title="Ответы на частые вопросы"
               photo-url="photo.png"
               description="Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
               redirect-url="/faq"/>
    </div>
    <div class="litters-container" v-if="!pending">
      <LitterListItem v-for="lit in littersPage.items" :key="lit.id" :litter="lit" />
      <PageSelector v-if="littersPage.totalPages > 1" url="/litters" :current-page="littersPage.pageNumber" :total="littersPage.totalPages" />
    </div>
    <div class="litters-container" v-else>
      <LitterListItemSkeleton/>
      <LitterListItemSkeleton/>
    </div>
    <div class="last-message-container">
      <MessageBox image-url="photo.png" text="Приезжайте к нам в гости! Наш питомник открыт для посещения, вы можете приехать, познакомиться с котятами вживую, сделать фотографии и лично пообщаться с нами. Договориться о посещении можно по телефону или в удобном мессенджере."/>
    </div>
  </main>
</template>

<style scoped>

.litters-container {
  display: flex;
  flex-direction: column;
  gap: 3.75rem;
  background-color: var(--color-pages-primary-background);
  padding: var(--padding-large) 6.25rem;
  align-items: center;
}

.first-message-container {
  background-color: var(--color-pages-primary-background);
  padding: var(--padding-large);
}

.articles-container {
  display: flex;
  justify-content: center;
  background-color: var(--color-pages-secondary-background);
  gap: var(--padding-extra-large);
  padding: var(--padding-pages);
}

.last-message-container {
  background-color: var(--color-pages-secondary-background);
  padding: var(--padding-large) 10rem;
}

</style>