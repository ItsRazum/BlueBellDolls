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
          <span class="font-medium">{{ parentCat.birthDay }}</span>
          <span> • </span>
          <span :style="{ color: genderColor }">{{ genderText }}</span>
        </div>
        <CardWrapper :show-border="false" v-if="parentCat.description" class="p-(--padding-small)">
          <span class="description">{{ parentCat.description.slice(0, 70) }}</span>
        </CardWrapper>
        <div>
          <NuxtLink :to="catPageUrl" class="btn" style="text-decoration: none">Подробнее</NuxtLink>
        </div>
    </div>
  </CardWrapper>

  <div v-else class="card-expanded">
    <div class="card-photo-container">
      <img class="card-expanded-photo" :src="parentCat.mainPhotoUrl" :alt="parentCat.name" />
      <NuxtLink :to="catPageUrl" class="link">Больше фото</NuxtLink>
    </div>
    <CardWrapper :enable-blur="true" :show-border="false" class="card-info-container w-full">
      <div class="card-header">
        <h2 class="m-0">{{ parentCat.name }}</h2>
        <span class="font-medium text-(--color-text-caption)">{{ parentCat.birthDay }}</span>
      </div>
      <div class="card-info-body">
        <CardWrapper class="card-info-props p-(--padding-large)" :show-border="false">
          <div class="card-property">
            <span>Пол: </span>
            <span :style="{ color: genderColor }">{{ genderText }}</span>
          </div>
          <div class="card-property">
            <span>Окрас: </span>
            <NuxtLink :to="colorUrl" class="link">{{ parentCat.color }}</NuxtLink>
          </div>
          <span class="mt-[1rem]"> {{ parentCat.description }}</span>
        </CardWrapper>
      </div>

      <div class="buttons-container">
        <NuxtLink :to="catPageUrl" class="btn no-underline">Подробнее</NuxtLink>
      </div>

    </CardWrapper>
  </div>
</template>
