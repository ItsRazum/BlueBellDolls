<script setup lang="ts">
definePageMeta({
  title: "pages.litters.title",
  backgroundImage: "/header.jpg",
  alias: ["/kittens"],
});

const { t } = useI18n();

useSeoMeta({
  title: computed(() => t("seo.titles.litters")),
});

const litterApi = useLitterApi();

const { data: littersPage, pending } = await litterApi.getByPage();
</script>

<template>
  <div class="first-message-container">
    <message-box image-url="/photo.png" :text="$t('pages.litters.welcomeMessage')" />
  </div>
  <div class="articles-container">
    <Article
      :title="$t('pages.litters.articles.kittenBookingRules.title')"
      photo-url="photo.png"
      :description="$t('pages.litters.articles.kittenBookingRules.description')"
      redirect-url="/about-ragdoll"
    />
    <Article
      :title="$t('pages.litters.articles.preparingForTheKittenMove.title')"
      photo-url="photo.png"
      :description="$t('pages.litters.articles.preparingForTheKittenMove.description')"
      redirect-url="/litters"
    />
    <Article
      :title="$t('pages.litters.articles.faq.title')"
      photo-url="photo.png"
      :description="$t('pages.litters.articles.faq.description')"
      redirect-url="/faq"
    />
  </div>
  <div class="litters-container" v-if="!pending">
    <LitterListItem v-for="lit in littersPage.items" :key="lit.id" :litter="lit" />
    <PageSelector
      v-if="littersPage.totalPages > 1"
      url="/litters"
      :current-page="littersPage.pageNumber"
      :total="littersPage.totalPages"
    />
  </div>
  <div class="litters-container" v-else>
    <LitterListItemSkeleton />
    <LitterListItemSkeleton />
  </div>
  <div class="last-message-container">
    <MessageBox image-url="photo.png" :text="$t('pages.litters.visitInvitationMessage')" />
  </div>
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
