<script setup lang="ts">

import KittenProps from "~/components/KittenProps.vue";
import PhotoGalleryModal from "~/components/base/PhotoGalleryModal.vue";

const props = defineProps<{
  isOpen: boolean;
  kitten: KittenDetailDto;
}>();

const emit = defineEmits(['close']);

const close = () => {
  emit('close');
};

const isBookingModalOpen = ref(false);

const openBookingModal = () => {
  isBookingModalOpen.value = true;
}

const closeBookingModal = () => {
  isBookingModalOpen.value = false;
}

</script>

<template>
  <PhotoGalleryModal :is-open="isOpen" :photos="props.kitten.photos" @close="close">
    <CardWrapper class="card-info-container">
      <div class="card-header">
        <h2>{{ kitten.name }}</h2>
        <span style="font-weight: 500; color: var(--color-text-caption);">{{ kitten.birthDay }}</span>
      </div>
      <div class="card-info-body">
        <CardWrapper class="card-info-props">
          <kitten-props :kitten="props.kitten" />
          <span style="margin-top: 0.875rem;"> {{ kitten.description }}</span>
        </CardWrapper>
      </div>
      <div class="buttons-container" style="gap: var(--padding-small)">
        <button @click="openBookingModal">Забронировать</button>
      </div>
    </CardWrapper>
  </PhotoGalleryModal>
  <KittenBookingModal :kitten="props.kitten" :is-open="isBookingModalOpen" @close="closeBookingModal" />
</template>

<style scoped>

.card-info-container {
  margin: 0;
  height: min-content;
}

.card-info-props {
  padding: var(--padding-large);
  max-width: 20rem;
}

</style>