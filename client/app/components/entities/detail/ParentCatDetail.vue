<script setup lang="ts">
const props = defineProps<{
  targetId: number;
}>();

import { PhotosType } from "~~/enums/enums";

const { locale, t } = useI18n();

const activePhotosTab = ref(PhotosType.Photos);
const parentCatApi = useParentCatApi();

const { data: parentCat, pending } = await parentCatApi.getById(props.targetId);
const tabLabels = computed<Record<PhotosType, string>>(() => ({
  [PhotosType.Photos]: t("components.detail.parentcats.photosLabels.Photos"),
  [PhotosType.Titles]: t("components.detail.parentcats.photosLabels.Titles"),
  [PhotosType.GenTests]: t("components.detail.parentcats.photosLabels.GenTests"),
}));
const filteredPhotos = computed(() => {
  if (!parentCat.value || !parentCat.value.photos) return [];

  return parentCat.value.photos.filter((photo) => photo.type === activePhotosTab.value);
});

const genderPrefix = computed(() => (parentCat.value.isMale ? "male" : "female"));
const genderColor = computed(() => `var(--color-gender-${genderPrefix.value})`);
const genderText = computed(() => t(`components.common.cats.${genderPrefix.value}Default`));
</script>

<template>
  <div v-if="!pending" class="content-container">
    <CardWrapper class="gallery-column">
      <div class="tabs-header">
        <button
          v-for="(label, typeKey) in tabLabels"
          :key="typeKey"
          @click="activePhotosTab = Number(typeKey)"
          class="tab-btn"
          :class="{ active: activePhotosTab === Number(typeKey) }"
        >
          {{ label }}
        </button>
      </div>
      <div class="gallery-wrapper">
        <PhotoGallery
          :key="activePhotosTab"
          :photos="filteredPhotos"
          aspectRatio="1:1"
          controls-position="inside"
        />
      </div>
    </CardWrapper>
    <CardWrapper class="card-info-container">
      <div class="card-header">
        <h2>{{ parentCat.name }}</h2>
        <span style="font-weight: 500; color: var(--color-text-caption)">{{
          parentCat.birthDay
        }}</span>
      </div>
      <div class="card-info-body">
        <CardWrapper class="card-info-props">
          <div class="card-property">
            <span>{{ $t("components.common.cats.gender") }}: </span>
            <span :style="{ color: genderColor }">{{ genderText }}</span>
          </div>
          <div class="card-property">
            <span>{{ $t("components.common.cats.color") }}: </span>
            <NuxtLink class="link">{{ parentCat.color }}</NuxtLink>
          </div>
          <span v-if="locale == 'ru'" style="margin-top: 0.875rem">
            {{ parentCat.description }}</span
          >
        </CardWrapper>
      </div>
    </CardWrapper>
  </div>
  <div v-else class="content-container">
    <div
      style="display: flex; flex-direction: column; align-items: center; gap: var(--padding-large)"
    >
      <CardWrapper class="gallery-column">
        <div class="tabs-header">
          <Skeleton
            theme="dark"
            width="8rem"
            height="2.625rem"
            radius="var(--border-radius-main)"
          />
          <Skeleton
            theme="dark"
            width="8rem"
            height="2.625rem"
            radius="var(--border-radius-main)"
          />
          <Skeleton
            theme="dark"
            width="8rem"
            height="2.625rem"
            radius="var(--border-radius-main)"
          />
        </div>
        <Skeleton theme="dark" width="25rem" height="25rem" radius="var(--border-radius-main)" />
        <Skeleton theme="dark" style="width: 9rem" />
      </CardWrapper>
    </div>
    <CardWrapper class="card-info-container">
      <div class="card-header">
        <Skeleton width="16rem" height="2rem" radius="0.825rem" />
        <Skeleton width="6rem" height="1rem" radius="0.825rem" />
      </div>
      <div class="card-info-body">
        <CardWrapper class="card-info-props">
          <Skeleton width="35%" height="1rem" radius="0.825rem" />
          <Skeleton width="50%" height="1rem" radius="0.825rem" />

          <div
            v-if="locale == 'ru'"
            style="display: flex; flex-direction: column; margin-top: 0.875rem; gap: 0.3rem"
          >
            <Skeleton width="90%" height="0.825rem" radius="0.825rem" />
            <Skeleton width="20rem" height="0.825rem" radius="0.825rem" />
            <Skeleton width="70%" height="0.825rem" radius="0.825rem" />
          </div>
        </CardWrapper>
      </div>
    </CardWrapper>
  </div>
</template>

<style scoped>
.content-container {
  display: flex;
  gap: var(--padding-large);
}

.tabs-header {
  width: 100%;
  display: flex;
  gap: 0.5rem;
  justify-content: center;
}

.tab-btn {
  flex: 1;
  background: transparent;
  border: 1px solid var(--color-border-card);
  padding: var(--padding-small) var(--padding-medium);
  line-height: 1.1;
  cursor: pointer;
  font-weight: 500;
  font-size: 1.125rem;
  color: var(--color-text-caption);
  border-radius: var(--border-radius-main);
  transition: all 0.2s ease;
}

.gallery-wrapper {
  position: relative;
  width: 100%;
}

.gallery-column {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: var(--padding-small);
  padding: var(--padding-small);
}

.card-info-container {
  margin: 0;
  height: min-content;
  flex: 1;
  min-width: 25rem;
}

.card-info-props {
  padding: var(--padding-large);
}

.content-container {
  display: flex;
  gap: var(--padding-large);
}

.tab-btn.active {
  cursor: default;
  pointer-events: none;
  color: var(--color-context-blue);
  background-color: var(--color-background-blue);
}
</style>
