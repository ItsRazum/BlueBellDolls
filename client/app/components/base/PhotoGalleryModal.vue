<script setup lang="ts">

import KittenProps from "~/components/KittenProps.vue";

const props = defineProps<{
  isOpen: boolean;
  photos: PhotoDto[]
}>();

const emit = defineEmits(['close']);

const close = () => {
  emit('close');
};

</script>

<template>
  <BaseModal :isOpen="isOpen" @close="close">
    <div class="container">
      <PhotoGallery :photos="props.photos"
                    aspectRatio="1:1"
                    controls-position="inside" />
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