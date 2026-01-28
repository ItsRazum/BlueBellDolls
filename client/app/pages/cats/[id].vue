<script setup lang="ts">
import ParentCatDetail from "~/components/entities/detail/ParentCatDetail.vue";

definePageMeta({
  layout: "custom",
});

const route = useRoute();

const { t } = useI18n();

const parentCatId = computed(() => Number(route.params.id));

const parentCatApi = useParentCatApi();

const { data: parentCat, pending } = await parentCatApi.getById(parentCatId.value);

const genderPrefix = computed(() => parentCat.value.isMale ? "male" : "female");

useSeoMeta({
  title: computed(() => {
    if (!pending || !parentCat.value) {
      return t("seo.titles.loading");
    }
    return t("seo.titles.cat", { name: parentCat.value.name });
  }),
});
</script>

<template>
  <TheHeader
    :title="parentCat?.name ? $t('pages.cat.title', { name: parentCat.name }) : $t('common.loading')"
    background-image="/header.jpg"
    :button-text="$t(`pages.cat.${genderPrefix}ButtonText`)"
    :button-url="`/cats/${parentCat.isMale ? `males` : `females`}`"
  />
  <main>
    <div class="content-container">
      <ParentCatDetail :target-id="parentCatId" />
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
