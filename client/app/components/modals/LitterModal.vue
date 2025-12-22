<script setup lang="ts">

import PhotoGalleryModal from "~/components/base/PhotoGalleryModal.vue";
import {KittenStatus} from "~~/enums/enums";

const props = defineProps<{
  isOpen: boolean;
  litter: LitterDetailDto;
}>();

const emit = defineEmits(['close']);

const close = () => {
  emit('close');
};

const litterDisplayName = computed(() => `Помёт «${props.litter.letter}»`);

const kittensString = computed(() => {
  switch (props.litter.kittens.length) {
    case 1:
      return ` котёнок`;
    case 2:
    case 3:
    case 4:
      return ` котёнка`;
    default:
      return ` котят`;
  }
});

const freeKittensCount = computed(() => {
  return props.litter.kittens.filter(k => k.status === KittenStatus.Available).length;
});

const availableKittensString = freeKittensCount.value == 1 ? ` свободен` : ` свободных`;

</script>

<template>
  <PhotoGalleryModal :is-open="isOpen" :photos="props.litter.photos" @close="close">
    <CardWrapper class="card-info-container">
      <div class="card-header">
        <h2>{{ litterDisplayName }}</h2>
        <span>{{ litter.birthDay }}</span>
      </div>
      <CardWrapper class="card-info-body">
        <div class="card-info-props">
          <div class="card-property">
            <span>Папа: </span>
            <RouterLink>{{ litter.fatherCat.name }}</RouterLink>
          </div>

          <div class="card-property">
            <span>Мама: </span>
            <RouterLink>{{ litter.motherCat.name }}</RouterLink>
          </div>

          <div class="card-property">
            <span style="color: var(--color-context-blue);">{{ litter.kittens.length }}</span>
            <span>{{ kittensString }} всего</span>
          </div>

          <div class="card-property">
            <span style="color: var(--color-context-blue);">{{ litter.kittens.length }}</span>
            <span>{{ availableKittensString }}</span>
          </div>
          <span style="margin-top: var(--padding-small)"> {{ litter.description }}</span>
        </div>
      </CardWrapper>
    </CardWrapper>
  </PhotoGalleryModal>
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

.card-property span,
.card-property a {
  font-size: 1.25rem;
}

</style>