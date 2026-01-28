<script setup lang="ts">
definePageMeta({
  title: "pages.cats.females.title",
  backgroundImage: "/header.jpg",
});

const { t } = useI18n();

useSeoMeta({
  title: t("seo.titles.femalecats"),
});

const parentCatApi = useParentCatApi();
const { data: parentCatsPage, pending } = await parentCatApi.getByPage(false);
</script>

<template>
  <div v-if="!pending" class="cats-container">
    <ParentCatListItem
      v-for="parentCat in parentCatsPage.items"
      :key="parentCat.id"
      :parent-cat="parentCat"
    />
    <PageSelector
      v-if="parentCatsPage.totalPages > 1"
      :current-page="parentCatsPage.pageNumber"
      :total="parentCatsPage.totalPages"
      url="/femalecats"
    />
  </div>
  <div v-else class="cats-container">
    <ParentCatListItemSkeleton />
    <ParentCatListItemSkeleton />
    <ParentCatListItemSkeleton />
  </div>
</template>

<style scoped>
.cats-container {
  background-color: var(--color-pages-secondary-background);
  display: flex;
  flex-direction: column;
  gap: 2rem;
  width: 100%;
  padding: var(--padding-large) 6.875rem;
}
</style>
