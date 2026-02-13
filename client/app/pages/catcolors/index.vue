<script setup lang="ts">
import CatColorListItemSkeleton from "~/components/skeletons/CatColorListItemSkeleton.vue";

definePageMeta({
  title: "pages.catcolors.title",
  backgroundImage: "/header.jpg",
});

const { t } = useI18n();

useSeoMeta({
  title: t("seo.titles.catcolors"),
});

const catColorApi = useCatColorApi();
const { data: colorsPage, pending } = await catColorApi.getByPage();
</script>

<template>
  <div v-if="!pending" class="page-block secondary">
    <CatColorListItem
      v-for="catColor in colorsPage.items"
      :key="catColor.id"
      :cat-color="catColor"
    />
    <PageSelector
      v-if="colorsPage.totalPages > 1"
      :current-page="colorsPage.pageNumber"
      :total="colorsPage.totalPages"
      url="/femalecats"
    />
  </div>
  <div v-else class="page-block secondary">
    <CatColorListItemSkeleton />
    <CatColorListItemSkeleton />
    <CatColorListItemSkeleton />
  </div>
</template>
