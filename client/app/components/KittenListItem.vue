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
const colorUrl = computed(() => `/colors/${props.kitten.color.replace(' ', '').toLowerCase()}`);

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

function getImageUrl(imagePath: string | null): string {
  return `${apiBaseUrl}/${imagePath}`;
}
</script>

<template>
  <CardWrapper v-if="variant === 'compact'" class="adv">
    <div class="adv-info-container">
      <img class="adv-photo" :src="kitten.mainPhotoUrl" :alt="kitten.name" />
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

  <div v-else class="cat-card-default">
    <div class="photoCard">
      <img class="cat-photo-default" :src="kitten.mainPhotoUrl" :alt="kitten.name" />
      <RouterLink :to="kittenPageUrl" class="link">Больше фото</RouterLink>
    </div>
    <CardWrapper :enable-blur="true" class="cat-info-container">
      <div class="cat-header">
        <h2 style="margin: 0">{{ kitten.name }}</h2>
        <span style="font-weight: 500; color: var(--color-text-caption);">{{ kitten.birthDay }}</span>
      </div>
      <div class="cat-info-body">
        <CardWrapper class="cat-info-unit">
          <div class="cat-property">
            <span>Пол: </span>
            <span :style="{ color: genderColor }">{{ genderText }}</span>
          </div>
          <div class="cat-property">
            <span>Окрас: </span>
            <RouterLink :to="colorUrl" class="link">{{ kitten.color }}</RouterLink>
          </div>
          <div class="cat-property">
            <span>Класс: </span>
            <span style="color: var(--color-kitten-class)">{{ kitten.class }}</span>
          </div>
          <div class="cat-property">
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
</template>

<style scoped>

/* Compact */
.adv {
  max-width: 294px;
  text-align: left;
  border-radius: var(--border-radius-main);
  overflow: hidden;
}

.adv-info-container {
  margin: var(--padding-medium);
  display: flex;
  flex-direction: column;
  gap: var(--padding-small);
  flex-grow: 1;
  height: 100%;
  line-height: 1;
}

.adv-photo {
  width: 100%;
  height: 274px;
  object-fit: cover;
  border-radius: var(--border-radius-main);
  box-shadow: var(--shadow-base);
}

.subtitle {
  font-size: 0.9rem;
  font-weight: bold;
  color: var(--color-text-caption);
  display: flex;
  align-items: center;
  gap: 4px;
}

.description {
  font-size: 0.9rem;
  font-weight: 500;
}

/* default */
.cat-card-default {
  width: 1000px;
  display: flex;
  line-height: 1;
}

.cat-info-container {
  margin-left: -20px;
  height: min-content;
  width: 100%;
  display: flex;
  flex-direction: column;
  gap: var(--padding-small);
  padding: var(--padding-large);
}

.cat-header {
  display: flex;
  flex-direction: column;
  justify-content: flex-start;
  align-items: flex-start;
  gap: var(--padding-small);
}

.cat-info-body {
  display: flex;
  flex-direction: row;
  gap: var(--padding-large);
  font-weight: 600;
}

.cat-info-unit {
  height: min-content;
  display: flex;
  flex-direction: column;
  gap: var(--padding-small);
  padding: var(--padding-large);
}

.cat-property {
  width: max-content;
}

.cat-property span,
.cat-property .link {
  font-size: 18px;
}

.photoCard {
  flex-grow: 0;
  display: flex;
  flex-direction: column;
  justify-content: flex-start;
  align-items: center;
  flex-shrink: 0;
  gap: 10px;
  padding: 0;
}

.cat-photo-default {
  height: 275px;
  width: 275px;
  object-fit: cover;
  flex-grow: 0;
  border-radius: var(--border-radius-main);
  box-shadow: var(--shadow-base);
}

.buttons-container {
  display: flex;
}

</style>
