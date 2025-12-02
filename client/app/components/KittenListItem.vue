<script setup lang="ts">

import {KittenStatus} from "~~/enums/enums";

const props = withDefaults(defineProps<{
  kitten: KittenListDto;
  variant?: 'default' | 'compact';
}>(), {
  variant: 'default',
});

const config = useRuntimeConfig();
const apiBaseUrl = config.public.apiBase;

const litterPageUrl = computed(() => `/litters/${props.kitten.litterId}`)
const kittenPageUrl = computed(() => `/kittens/${props.kitten.id}`);

const genderVariants = computed(() => props.variant == "default" ? ['Мужской', 'Женский'] : ['Мальчик', 'Девочка']);

const genderText = computed(() => props.kitten.isMale ? genderVariants.value[0] : genderVariants.value[1]);

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

function getImageUrl(imagePath: string | null): string {
  return `${apiBaseUrl}/${imagePath}`;
}


</script>

<template>
  <CardWrapper v-if="variant === 'compact'" class="card-compact">
    <div class="card-compact-content">
      <img class="card-compact-photo" :src="kitten.mainPhotoUrl" :alt="kitten.name" />
      <h2>{{ kitten.name }}</h2>
      <div class="subtitle">
        <span style="font-weight: 525">{{ kitten.birthDay }}</span>
        <span> • </span>
        <span :style="{ color: genderColor }">{{ genderText }}</span>
        <span> • </span>
        <RouterLink :to="litterPageUrl" class="link">Помёт «{{ kitten.litterLetter }}»</RouterLink>
      </div>
      <CardWrapper v-if="kitten.description" style="padding: var(--padding-small)">
        <span class="description">{{ kitten.description.slice(0, 70) }}</span>
      </CardWrapper>
      <div class="buttons-container" style="justify-content: space-between;">
        <RouterLink :to="kittenPageUrl" class="btn" style="text-decoration: none">Подробнее</RouterLink>
        <RouterLink :to="kittenPageUrl" class="btn" style="text-decoration: none; padding: 10px 15px">Забронировать</RouterLink>
      </div>
    </div>
  </CardWrapper>

  <div v-else class="card-expanded">
    <div class="card-photo-container">
      <img class="card-expanded-photo" :src="kitten.mainPhotoUrl" :alt="kitten.name" />
      <RouterLink :to="kittenPageUrl" class="link">Больше фото</RouterLink>
    </div>
    <CardWrapper :enable-blur="true" class="card-info-container">
      <div class="card-header">
        <h2 style="margin: 0">{{ kitten.name }}</h2>
        <span style="font-weight: 500; color: var(--color-text-caption);">{{ kitten.birthDay }}</span>
      </div>
      <div class="card-info-body">
        <CardWrapper class="card-info-props">
          <div class="card-property">
            <span>Пол: </span>
            <span :style="{ color: genderColor }">{{ genderText }}</span>
          </div>
          <div class="card-property">
            <span>Окрас: </span>
            <button class="link-btn" @click="openColorModal">{{ kitten.color }}</button>
          </div>
          <div class="card-property">
            <span>Класс: </span>
            <span style="color: var(--color-kitten-class)">{{ kitten.class }}</span>
          </div>
          <div class="card-property">
            <span>Статус: </span>
            <span :style="{ color: statusColor }">{{ status }}</span>
          </div>
          <span style="margin-top: 14px;"> {{ kitten.description }}</span>
        </CardWrapper>
      </div>

      <div class="buttons-container" style="gap: var(--padding-small)">
        <RouterLink :to="kittenPageUrl" class="btn" style="text-decoration: none">Подробнее</RouterLink>
        <RouterLink :to="kittenPageUrl" class="btn" style="text-decoration: none">Забронировать</RouterLink>
      </div>

    </CardWrapper>
  </div>

  <BaseModal
    :is-open="isColorModalOpen"
    @close="closeColorModal">
    <div>
      <span>{{kitten.color}}</span>
    </div>

  </BaseModal>
</template>

<style scoped>

.card-info-props {
  padding: var(--padding-large);
}

.link-btn {
  all: unset;

  color: var(--color-link);
  text-decoration: underline;
  font-weight: 525;

  cursor: pointer;
  font-family: var(--font-family-base);
  font-size: 18px;
  transition: color 0.2s;
}

.link-btn:hover {
  color: var(--color-link-hover);
  background-color: transparent;
}

</style>
