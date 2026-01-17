<script setup lang="ts">

const props = defineProps<{
  catColor: CatColorDto;
  isOpen: boolean;
}>();

const emit = defineEmits(['close']);

const close = () => {
  emit('close');
};

const loaded = ref(false);

</script>

<template>
<BaseModal
    :isOpen="props.isOpen"
    @close="close">
  <div v-if="loaded" class="main-container">
    <PhotoGallery :photos="props.catColor.photos" controls-position="outside" />
    <CardWrapper class="info-wrapper">
      <div class="info-container">
        <h2>Окрас: {{props.catColor.identifier}}</h2>
        <span>{{props.catColor.description}}</span>
      </div>
    </CardWrapper>
  </div>
  <div v-else class="main-container">
    <Skeleton theme="dark" style="
      width: 37.5rem;
      height: 28.125rem;
      border-radius: var(--border-radius-main);
      margin: 0 3.875rem;" />
    <Skeleton theme="dark" style="width: 9rem;" />
    <CardWrapper class="info-wrapper" style="width: 100%">
      <div class="info-container">
        <Skeleton width="16rem" height="2rem" radius="0.825rem" />
        <div style="display: flex; flex-direction: column; gap: var(--padding-small);">
          <Skeleton width="100%" height="0.5rem" radius="0.825rem" />
          <Skeleton width="80%" height="0.5rem" radius="0.825rem" />
          <Skeleton width="90%" height="0.5rem" radius="0.825rem" />
        </div>
      </div>
    </CardWrapper>
  </div>
</BaseModal>
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