<script setup lang="ts">
const props = defineProps<{
  catColorId: number;
}>();

const { locale } = useI18n();

const catColorApi = useCatColorApi();

const { data: catColor, pending } = await catColorApi.getById(props.catColorId);
</script>

<template>
  <div v-if="!pending" class="main-container">
    <PhotoGallery :photos="catColor.photos" controls-position="outside" />
    <CardWrapper class="info-wrapper">
      <div class="info-container">
        <h2>{{ $t('components.common.catcolors.title', { color: humanizeString(catColor.identifier) }) }}</h2>
        <span v-if="locale == 'ru'">{{ catColor.description }}</span>
      </div>
    </CardWrapper>
  </div>
  <div v-else class="main-container">
    <Skeleton
      theme="dark"
      style="
        width: 37.5rem;
        height: 28.125rem;
        border-radius: var(--border-radius-main);
        margin: 0 3.875rem;
      "
    />
    <Skeleton theme="dark" style="width: 9rem" />
    <CardWrapper class="info-wrapper" style="width: 100%">
      <div class="info-container">
        <Skeleton width="16rem" height="2rem" radius="0.825rem" />
        <div
          v-if="locale == 'ru'"
          style="display: flex; flex-direction: column; gap: var(--padding-small)"
        >
          <Skeleton width="100%" height="0.5rem" radius="0.825rem" />
          <Skeleton width="80%" height="0.5rem" radius="0.825rem" />
          <Skeleton width="90%" height="0.5rem" radius="0.825rem" />
        </div>
      </div>
    </CardWrapper>
  </div>
</template>

<style scoped>
.main-container {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  margin: var(--padding-large);
  gap: var(--padding-large);
}

.info-wrapper {
  max-width: 37.5rem;
}

.info-container {
  display: flex;
  flex-direction: column;
  margin: var(--padding-large);
  gap: var(--padding-large);
}
</style>
