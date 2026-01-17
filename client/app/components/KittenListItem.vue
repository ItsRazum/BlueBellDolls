<script setup lang="ts">

import {KittenStatus, PhotosType} from "~~/enums/enums";
import KittenProps from "~/components/KittenProps.vue";

const props = withDefaults(defineProps<{
  kitten: KittenListDto;
  variant?: 'default' | 'compact';
  readOnly?: boolean;
}>(), {
  variant: 'default',
  readOnly: false,
});

const config = useRuntimeConfig();
const apiBaseUrl = config.public.apiBase;

const isBookingModalOpen = ref(false);

const openBookingModal = () => {
  isBookingModalOpen.value = true;
}

const closeBookingModal = () => {
  isBookingModalOpen.value = false;
}

const isKittenModalOpen = ref(false);
const openKittenModal = () => {
  isKittenModalOpen.value = true;
}

const closeKittenModal = () => {
  isKittenModalOpen.value = false;
}

const litterPageUrl = computed(() => `/litters/${props.kitten.litterId}`)
const kittenPageUrl = computed(() => `/kittens/${props.kitten.id}`);

const genderVariants = computed(() => props.variant == "default" ? ['Мужской', 'Женский'] : ['Мальчик', 'Девочка']);

const genderText = computed(() => props.kitten.isMale ? genderVariants.value[0] : genderVariants.value[1]);

const genderColor = computed(() => props.kitten.isMale ? 'var(--color-gender-male)' : 'var(--color-gender-female)');

</script>

<template>
  <CardWrapper v-if="variant === 'compact'" class="card-compact">
    <div class="card-compact-content">
      <img class="card-compact-photo" :src="apiBaseUrl + kitten.mainPhotoUrl" :alt="kitten.name" />
      <h2>{{ kitten.name }}</h2>
      <div class="subtitle">
        <span class="font-medium">{{ kitten.birthDay }}</span>
        <span> • </span>
        <span :style="{ color: genderColor }">{{ genderText }}</span>
        <span> • </span>
        <NuxtLink :to="litterPageUrl" class="link">Помёт «{{ kitten.litterLetter }}»</NuxtLink>
      </div>
      <CardWrapper :show-border="false" class="p-(--padding-small)" v-if="kitten.description">
        <span class="description">{{ kitten.description.slice(0, 70) }}</span>
      </CardWrapper>
      <div class="buttons-container justify-between">
        <button class="w-full" @click="openKittenModal">Подробнее</button>
        <button class="w-full" @click="openBookingModal" v-if="kitten.status === KittenStatus.Available">Забронировать</button>
      </div>
    </div>
  </CardWrapper>

  <div v-else class="card-expanded">
    <div class="card-photo-container">
      <img class="card-expanded-photo" :src="apiBaseUrl + kitten.mainPhotoUrl" :alt="kitten.name" />
      <button v-if="!props.readOnly" @click="openKittenModal" class="link-btn">Больше фото</button>
    </div>
    <CardWrapper :enable-blur="true" :show-border="false" class="card-info-container w-full">
      <div class="card-header">
        <h2>{{ kitten.name }}</h2>
        <span class="font-medium color-(--color-text-caption)">{{ kitten.birthDay }}</span>
      </div>
      <div class="card-info-body">
        <CardWrapper :show-border="false" class="card-info-props">
          <kitten-props :kitten="props.kitten" />
          <span class="mt-[0.875rem]"> {{ kitten.description }}</span>
        </CardWrapper>
      </div>

      <div  v-if="!props.readOnly" class="buttons-container" style="gap: var(--padding-small)">
        <button @click="openKittenModal">Подробнее</button>
        <button @click="openBookingModal" v-if="kitten.status === KittenStatus.Available">Забронировать</button>
      </div>

    </CardWrapper>
  </div>

  <KittenModal :kitten-id="kitten.id" :is-open="isKittenModalOpen" @close="closeKittenModal" />
  <KittenBookingModal :kitten="props.kitten" :is-open="isBookingModalOpen" @close="closeBookingModal" />
</template>

<style scoped>

.card-info-props {
  padding: var(--padding-large);
}

</style>
