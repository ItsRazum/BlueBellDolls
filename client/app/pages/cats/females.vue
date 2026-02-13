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
  <div v-if="!pending" class="page-block secondary">
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
  <div v-else class="page-block secondary">
    <ParentCatListItemSkeleton />
    <ParentCatListItemSkeleton />
    <ParentCatListItemSkeleton />
  </div>
</template>

<style scoped>

@media (max-width: 874px) {
  .cats-container {
    padding: var(--padding-small);
  }
}

</style>
