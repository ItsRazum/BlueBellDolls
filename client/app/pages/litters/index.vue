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
  <div class="page-block primary">
    <message-box image-url="/photo.png" :text="$t('pages.litters.welcomeMessage')" />
  </div>
  <div class="page-block secondary flex-row">
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
  <div class="page-block primary" v-if="!pending">
    <LitterListItem v-for="lit in littersPage.items" :key="lit.id" :litter="lit" />
    <PageSelector
      v-if="littersPage.totalPages > 1"
      url="/litters"
      :current-page="littersPage.pageNumber"
      :total="littersPage.totalPages"
    />
  </div>
  <div class="page-block primary" v-else>
    <LitterListItemSkeleton />
    <LitterListItemSkeleton />
  </div>
  <div class="last-message-container page-block secondary">
    <MessageBox image-url="photo.png" :text="$t('pages.litters.visitInvitationMessage')" />
  </div>
</template>

<style scoped>

.last-message-container {
  padding: var(--padding-large) 10rem;
}

</style>
