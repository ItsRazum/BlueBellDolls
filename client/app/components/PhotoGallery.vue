<script setup lang="ts">

import { Swiper, SwiperSlide } from 'swiper/vue';
import { Navigation, Thumbs, Pagination } from "swiper/modules";
import 'swiper/css';
import 'swiper/css/pagination';
import 'swiper/css/navigation';

const props = defineProps<{
  photos: PhotoDto[]
}>();

const modules = [Navigation, Thumbs, Pagination];

</script>

<template>
  <div class="gallery">
    <swiper
    :style="{
      '--swiper-navigation-color': '#ffffff',
      '--swiper-pagination-color': '#ffffff',
    }"
    :space-between="10"
    :navigation="true"
    :modules="modules"
    class="main-swiper">
      <swiper-slide v-for="photo in photos" :key="photo.id">
        <div class="">
          <img class="image" :src="photo.url" loading="lazy" alt="{{photo.type}}" />
        </div>

      </swiper-slide>
    </swiper>
  </div>
</template>

<style scoped>

.main-swiper {
  width: 100%;
  height: 600px;
  border-radius: var(--border-radius-main);
  overflow: hidden;
  box-shadow: var(--shadow-base);
}

.image-container {
  display: flex;
  align-items: center;
}

.image {
  width: 100%;
  height: 600px;
  border-radius: var(--border-radius-main);
  object-fit: cover;
}

</style>