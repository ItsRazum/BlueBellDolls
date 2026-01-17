<script setup lang="ts">

import KittenProps from "~/components/KittenProps.vue";

const props = withDefaults(defineProps<{
  isOpen: boolean;
  photos: PhotoDto[];
  skeleton: boolean;
}>(), {
  skeleton: false,
});

const emit = defineEmits(['close']);

const close = () => {
  emit('close');
};

</script>

<template>
  <BaseModal :isOpen="isOpen" @close="close">
    <div v-if="!skeleton" class="container">
      <PhotoGallery :photos="props.photos"
                    aspectRatio="1:1"
                    controls-position="inside" />
      <slot/>
    </div>
    <div v-else class="container">
      <div style="display: flex; flex-direction: column; align-items: center; gap: var(--padding-large);">
        <Skeleton theme="dark" width="25rem" height="25rem" radius="var(--border-radius-main)" />
        <Skeleton theme="dark" style="width: 9rem;" />
      </div>

      <slot/>
    </div>
  </BaseModal>
</template>

<style scoped>

.container {
  display: flex;
  gap: var(--padding-large);
}

</style>