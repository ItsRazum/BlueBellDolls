<script setup lang="ts">
definePageMeta({
  layout: "custom",
});

const { t } = useI18n();

const route = useRoute();

const catColorId = computed(() => Number(route.params.id));

const catColorApi = useCatColorApi();

const { data: catColor, status } = await catColorApi.getById(catColorId);

const pending = computed(() => status.value === "pending");

const catColorString = computed(() => humanizeString(catColor.value.identifier));

useSeoMeta({
  title: computed(() => {
    if (pending.value || !catColor.value) return t("seo.titles.loading");

    return t("seo.titles.catcolor", { color: catColorString.value });
  }),
});
</script>

<template>
  <TheHeader
    :title="catColor ? $t('pages.catcolor.title', { color: catColorString }) : $t('common.loading')"
    background-image="/header.jpg"
    :button-text="$t('pages.catcolor.buttonText')"
    button-url="/catcolors"
  />

  <main>
    <div class="content-container">
      <CatColorDetail :cat-color-id="catColorId" />
    </div>
  </main>
</template>

<style scoped>
.content-container {
  display: flex;
  align-items: center;
  flex-direction: column;
  padding: var(--padding-large);
  background-color: var(--color-pages-secondary-background);
  gap: var(--padding-pages);
}
</style>
