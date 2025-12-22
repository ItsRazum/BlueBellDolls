<script setup lang="ts">

const props = defineProps<{
  catColor: CatColorDto;
  isOpen: boolean;
}>();

const emit = defineEmits(['close']);

const close = () => {
  emit('close');
};

</script>

<template>
<BaseModal
    :isOpen="props.isOpen"
    @close="close">
  <div class="main-container">
    <PhotoGallery :photos="props.catColor.photos" controls-position="outside" />
    <CardWrapper class="info-wrapper">
      <div class="info-container">
        <h2>Окрас: {{props.catColor.identifier}}</h2>
        <span>{{props.catColor.description}}</span>
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