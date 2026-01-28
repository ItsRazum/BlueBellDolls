<script setup lang="ts">
definePageMeta({
  layout: "custom",
});

const { t } = useI18n();
const route = useRoute();
const litterId = computed(() => Number(route.params.id));

const litterApi = useLitterApi();

const { data: litter, status } = await litterApi.getById(litterId);

const pending = computed(() => status.value === "pending");

useSeoMeta({
  title: computed(() => {
    if (pending.value || !litter.value) return t("seo.titles.loading");

    return t("seo.titles.litter", { letter: litter.value.letter });
  }),
});
</script>

<template>
  <TheHeader
    :title="litter ? $t('pages.litter.title', { letter: litter.letter }) : $t('common.loading')"
    background-image="/header.jpg"
    :button-text="$t('pages.litter.buttonText')"
    button-url="/litters"
  />

  <main>
    <div v-if="litter && !pending" class="content-container">
      <LitterListItem :litter="litter" />
      <NuxtLinkLocale to="/litters" class="text-xl">{{ $t('pages.litter.buttonText') }}</NuxtLinkLocale>
    </div>

    <div v-else class="content-container">
      <LitterListItemSkeleton />
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
