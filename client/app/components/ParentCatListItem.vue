<script setup lang="ts">

const props = withDefaults(defineProps<{
  parentCat: ParentCatListDto;
  variant?: 'default' | 'compact';
}>(), {
  variant: 'default',
});

const config = useRuntimeConfig();
const apiBaseUrl = config.public.apiBase;

const catPageUrl = computed(() => `/parentcats/${props.parentCat.id}`);
const colorUrl = computed(() => `/colors/${props.parentCat.color.replace(' ', '').toLowerCase()}`);
const genderVariants = computed(() => props.variant == "default" ? ['Мужской', 'Женский'] : ['Мальчик', 'Девочка']);
const genderText = computed(() => props.parentCat.isMale ? genderVariants.value[0] : genderVariants.value[1]);
const genderColor = computed(() => props.parentCat.isMale ? 'var(--color-gender-male)' : 'var(--color-gender-female)');

function getImageUrl(imagePath: string | null): string {
  return `${apiBaseUrl}/${imagePath}`;
}
</script>

<template>
  <CardWrapper v-if="variant === 'compact'" class="adv">
    <div class="adv-info-container">
      <img class="adv-photo" :src="parentCat.mainPhotoUrl" :alt="parentCat.name" />
        <h2>{{ parentCat.name }}</h2>
        <div class="subtitle">
          <span style="font-weight: 525">{{ parentCat.birthDay }}</span>
          <span> • </span>
          <span :style="{ color: genderColor }">{{ genderText }}</span>
        </div>
        <CardWrapper v-if="parentCat.description" style="padding: var(--padding-small)">
          <span class="description">{{ parentCat.description.slice(0, 70) }}</span>
        </CardWrapper>
        <div>
          <RouterLink :to="catPageUrl" class="btn" style="text-decoration: none">Подробнее</RouterLink>
        </div>
    </div>
  </CardWrapper>

  <div v-else class="cat-card-default">
    <div class="photoCard">
      <img class="cat-photo-default" :src="parentCat.mainPhotoUrl" :alt="parentCat.name" />
      <RouterLink :to="catPageUrl" class="link">Больше фото</RouterLink>
    </div>
    <CardWrapper :enable-blur="true" class="cat-info-container">
      <div class="cat-header">
        <h2 style="margin: 0">{{ parentCat.name }}</h2>
        <span style="font-weight: 505; color: var(--color-text-caption);">{{ parentCat.birthDay }}</span>
      </div>
      <div class="cat-info-body">
        <CardWrapper class="cat-info-unit">
          <div class="cat-property">
            <span>Пол: </span>
            <span :style="{ color: genderColor }">{{ genderText }}</span>
          </div>
          <div class="cat-property">
            <span>Окрас: </span>
            <RouterLink :to="colorUrl" class="link">{{ parentCat.color }}</RouterLink>
          </div>
          <span> {{ parentCat.description }}</span>
        </CardWrapper>
      </div>

      <div class="buttons-container">
        <RouterLink :to="catPageUrl" class="btn" style="text-decoration: none">Подробнее</RouterLink>
      </div>

    </CardWrapper>
  </div>
</template>

<style scoped>

/* Common */

h2 {
  font-weight: bold;
}

.link {
  color: var(--color-link);
  font-weight: 525;
  text-decoration: underline;
  padding: 0;
}

.link:hover {
  background-color: transparent;
}

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
  height: 252px;
  width: 252px;
  object-fit: cover;
  flex-grow: 0;
  border-radius: var(--border-radius-main);
  box-shadow: var(--shadow-base);
}

.buttons-container {
  align-self: flex-start;
  margin-top: auto;
}

</style>
