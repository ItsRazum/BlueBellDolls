<script setup lang="ts">

import {KittenStatus, PhotosType} from "~~/enums/enums";
import KittenProps from "~/components/KittenProps.vue";

const props = withDefaults(defineProps<{
  kitten: KittenListDto;
  variant?: 'default' | 'compact';
  readOnly: boolean;
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

const mainPhoto = ref<PhotoDto>({
  id: 1,
  url: 'photo.png',
  type: PhotosType.Photos,
  isMain: true
});

const photo = ref<PhotoDto>({
  id: 2,
  url: 'photo.png',
  type: PhotosType.Photos,
  isMain: false
});

const kittenDetail = ref<KittenDetailDto>({
  id: props.kitten.id,
  name: props.kitten.name,
  description: props.kitten.description,
  class: props.kitten.class,
  status: props.kitten.status,
  litterId: props.kitten.id,
  litterLetter: props.kitten.id,
  color: props.kitten.color,
  birthDay: props.kitten.birthDay,
  photos: [mainPhoto.value, photo.value, photo.value, photo.value, photo.value],
});

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
        <RouterLink :to="kittenPageUrl" class="btn">Подробнее</RouterLink>
        <RouterLink :to="kittenPageUrl" class="btn">Забронировать</RouterLink>
      </div>
    </div>
  </CardWrapper>

  <div v-else class="card-expanded">
    <div class="card-photo-container">
      <img class="card-expanded-photo" :src="kitten.mainPhotoUrl" :alt="kitten.name" />
      <button v-if="!props.readOnly" @click="openKittenModal" class="link-btn">Больше фото</button>
    </div>
    <CardWrapper :enable-blur="true" class="card-info-container" style="width: 100%;">
      <div class="card-header">
        <h2>{{ kitten.name }}</h2>
        <span style="font-weight: 500; color: var(--color-text-caption);">{{ kitten.birthDay }}</span>
      </div>
      <div class="card-info-body">
        <CardWrapper class="card-info-props">
          <kitten-props :kitten="props.kitten" />
          <span style="margin-top: 0.875rem;"> {{ kitten.description }}</span>
        </CardWrapper>
      </div>

      <div  v-if="!props.readOnly" class="buttons-container" style="gap: var(--padding-small)">
        <button @click="openKittenModal">Подробнее</button>
        <button @click="openBookingModal">Забронировать</button>
      </div>

    </CardWrapper>
  </div>

  <KittenModal :kitten="kittenDetail" :is-open="isKittenModalOpen" @close="closeKittenModal" />
  <KittenBookingModal :kitten="props.kitten" :is-open="isBookingModalOpen" @close="closeBookingModal" />
</template>

<style scoped>

.card-info-props {
  padding: var(--padding-large);
}

</style>
