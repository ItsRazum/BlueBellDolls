<script setup lang="ts">
import KittenDetail from "~/components/entities/detail/KittenDetail.vue";

definePageMeta({
  layout: "custom",
});

const { t } = useI18n();

const route = useRoute();
const kittenId = Number(route.params.id);

const kittenApi = useKittenApi();

const { data: kitten, pending } = await kittenApi.getById(kittenId);

useSeoMeta({
  title: computed(() => {
    if (pending.value || !kitten.value) return t("seo.titles.loading");

    return t("seo.titles.kitten", { name: kitten.value.name });
  }),
});
</script>

<template>
  <TheHeader
    :title="kitten?.name != null ? $t('pages.kitten.title', { name: kitten.name }) : $t('common.loading')"
    background-image="/header.jpg"
    :button-text="$t('pages.kitten.buttonText')"
    button-url="/litters"
  />
  <main>
    <div class="content-container">
      <KittenDetail :kittenId="kittenId" />
    </div>
  </main>
</template>

<style scoped>
.content-container {
  display: flex;
  justify-content: center;
  padding: var(--padding-large);
  background-color: var(--color-pages-secondary-background);
}
</style>
