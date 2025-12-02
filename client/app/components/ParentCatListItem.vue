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
  <CardWrapper v-if="variant === 'compact'" class="card-compact">
    <div class="card-compact-content">
      <img class="card-compact-photo" :src="parentCat.mainPhotoUrl" :alt="parentCat.name" />
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

  <div v-else class="card-expanded">
    <div class="card-photo-container">
      <img class="card-expanded-photo" :src="parentCat.mainPhotoUrl" :alt="parentCat.name" />
      <RouterLink :to="catPageUrl" class="link">Больше фото</RouterLink>
    </div>
    <CardWrapper :enable-blur="true" class="card-info-container">
      <div class="card-header">
        <h2 style="margin: 0">{{ parentCat.name }}</h2>
        <span style="font-weight: 505; color: var(--color-text-caption);">{{ parentCat.birthDay }}</span>
      </div>
      <div class="card-info-body">
        <CardWrapper class="card-info-props">
          <div class="card-property">
            <span>Пол: </span>
            <span :style="{ color: genderColor }">{{ genderText }}</span>
          </div>
          <div class="card-property">
            <span>Окрас: </span>
            <RouterLink :to="colorUrl" class="link">{{ parentCat.color }}</RouterLink>
          </div>
          <span style="margin-top: 14px;"> {{ parentCat.description }}</span>
        </CardWrapper>
      </div>

      <div class="buttons-container">
        <RouterLink :to="catPageUrl" class="btn" style="text-decoration: none">Подробнее</RouterLink>
      </div>

    </CardWrapper>
  </div>
</template>

<style scoped>

.card-info-props {
  padding: var(--padding-large);
}

</style>
