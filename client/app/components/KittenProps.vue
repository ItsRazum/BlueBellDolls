<script setup lang="ts">

const props = defineProps<{
  kitten: Kitten;
}>()

import {KittenStatus} from "~~/enums/enums";

const genderText = computed(() => props.kitten.isMale ? 'Мужской' : 'Женский');

const statusVariants = computed(() => {
  switch (props.kitten.status) {
    case KittenStatus.Available:
      return ['Доступен', 'Доступна'];
    case KittenStatus.Reserved:
      return ['Зарезервирован', 'Зарезервирована'];
    case KittenStatus.UnderObservation:
      return ['Под наблюдением', 'Под наблюдением'];
    case KittenStatus.Sold:
      return ['В новом доме', 'В новом доме'];
    default:
      return ['', ''];
  }
});

const status = computed(() => props.kitten.isMale ? statusVariants.value[0] : statusVariants.value[1]);
const statusColor = computed(() => {
  switch (props.kitten.status) {
    case KittenStatus.Available:
      return 'var(--color-status-available)';
    case KittenStatus.Reserved:
    case KittenStatus.UnderObservation:
      return 'var(--color-status-reserved)';
    case KittenStatus.Sold:
      return 'var(--color-status-unavailable)';
    default:
      return '';
  }
})

const genderColor = computed(() => props.kitten.isMale ? 'var(--color-gender-male)' : 'var(--color-gender-female)');

const isColorModalOpen = ref(false);
const openColorModal = () => {
  isColorModalOpen.value = true;
}

const closeColorModal = () => {
  isColorModalOpen.value = false;
}

</script>

<template>
  <div class="card-info-props">
    <div class="card-property">
      <span>Пол: </span>
      <span :style="{ color: genderColor }">{{ genderText }}</span>
    </div>
    <div class="card-property">
      <span>Окрас: </span>
      <button class="link-btn" @click="openColorModal">{{ kitten.color.identifier }}</button>
    </div>
    <div class="card-property">
      <span>Класс: </span>
      <span style="color: var(--color-kitten-class)">{{ kitten.class }}</span>
    </div>
    <div class="card-property">
      <span>Статус: </span>
      <span :style="{ color: statusColor }">{{ status }}</span>
    </div>
  </div>

  <CatColorModal
      :is-open="isColorModalOpen"
      @close="closeColorModal"
      :cat-color="props.kitten.color"/>

</template>

<style scoped>

.link-btn {
  all: unset;

  color: var(--color-link);
  text-decoration: underline;
  font-weight: 525;

  cursor: pointer;
  font-family: var(--font-family-base);
  font-size: 1.125rem;
  transition: color 0.2s;
}

.link-btn:hover {
  color: var(--color-link-hover);
  background-color: transparent;
}

</style>