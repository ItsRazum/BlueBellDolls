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

const loaded = ref(false);

</script>

<template>
  <PhotoGalleryModal :skeleton="!loaded" :is-open="isOpen" :photos="props.litter.photos" @close="close">
    <CardWrapper v-if="loaded" class="card-info-container">
      <div class="card-header">
        <h2>{{ litterDisplayName }}</h2>
        <span>{{ litter.birthDay }}</span>
      </div>
      <CardWrapper class="card-info-body">
        <div class="card-info-props">
          <div class="card-property">
            <span>Папа: </span>
            <NuxtLink>{{ litter.fatherCat.name }}</NuxtLink>
          </div>

          <div class="card-property">
            <span>Мама: </span>
            <NuxtLink>{{ litter.motherCat.name }}</NuxtLink>
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
    <CardWrapper v-else class="card-info-container">
      <div class="card-header">
        <Skeleton width="16rem" height="2rem" radius="0.825rem" />
        <Skeleton width="6rem" height="1rem" radius="0.825rem" />
      </div>
      <div class="card-info-body">
        <CardWrapper class="card-info-props">
          <Skeleton width="35%" height="1.25rem" radius="0.825rem" />
          <Skeleton width="50%" height="1.25rem" radius="0.825rem" />
          <Skeleton width="25%" height="1.25rem" radius="0.825rem" />
          <Skeleton width="40%" height="1.25rem" radius="0.825rem" />

          <div style="display: flex; flex-direction: column; margin-top: 0.875rem; gap: 0.3rem">
            <Skeleton width="90%" height="0.825rem" radius="0.825rem" />
            <Skeleton width="20rem" height="0.825rem" radius="0.825rem" />
            <Skeleton width="70%" height="0.825rem" radius="0.825rem" />
          </div>
        </CardWrapper>
      </div>
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