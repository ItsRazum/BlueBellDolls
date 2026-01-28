<script setup lang="ts">
import { Swiper, SwiperSlide } from "swiper/vue";
import { Navigation, Pagination } from "swiper/modules";
import "swiper/css";
import "swiper/css/pagination";
import "swiper/css/navigation";

const config = useRuntimeConfig();
const apiBaseUrl = config.public.apiBase;

const props = withDefaults(
  defineProps<{
    photos: PhotoDto[];
    aspectRatio?: "4:3" | "1:1";
    controlsPosition?: "outside" | "inside";
  }>(),
  {
    aspectRatio: "4:3",
    controlsPosition: "outside",
  },
);

const modules = [Navigation, Pagination];

const prevButton = ref(null);
const nextButton = ref(null);
</script>

<template>
  <div class="gallery-wrapper" :class="[`mode-${controlsPosition}`]">
    <button ref="prevButton" class="nav-btn prev-btn">
      <svg width="24" height="24" viewBox="0 0 24 24" fill="none">
        <path
          d="M15 19L8 12L15 5"
          :stroke="controlsPosition === 'inside' ? 'white' : 'var(--color-text-base)'"
          stroke-width="2"
          stroke-linecap="round"
          stroke-linejoin="round"
        />
      </svg>
    </button>

    <swiper
      :style="{
        '--swiper-navigation-color': '#ffffff',
        '--swiper-pagination-color': '#ffffff',
        'max-width': props.aspectRatio == '4:3' ? '37.5rem' : '25rem',
      }"
      :space-between="10"
      :pagination="{ clickable: true }"
      :navigation="{
        prevEl: prevButton,
        nextEl: nextButton,
      }"
      :modules="modules"
      class="main-swiper"
    >
      <swiper-slide v-for="photo in photos" :key="photo.id">
        <div class="image-container">
          <img
            :class="props.aspectRatio === '4:3' ? 'image-4x3' : 'image-1x1'"
            :src="apiBaseUrl + photo.url"
            loading="lazy"
            alt="cat photo"
          />
        </div>
      </swiper-slide>
    </swiper>

    <button ref="nextButton" class="nav-btn next-btn">
      <svg width="24" height="24" viewBox="0 0 24 24" fill="none">
        <path
          d="M9 19L16 12L9 5"
          :stroke="controlsPosition === 'inside' ? 'white' : 'var(--color-text-base)'"
          stroke-width="2"
          stroke-linecap="round"
          stroke-linejoin="round"
        />
      </svg>
    </button>
  </div>
</template>

<style scoped>
.gallery-wrapper {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: var(--padding-large);
  position: relative;
  margin: 0 auto;
}

.main-swiper {
  display: block;
  border-radius: var(--border-radius-main);
}

.image-container {
  width: max-content;
  display: flex;
  align-items: center;
}

.image-4x3 {
  width: 37.5rem;
  height: 28.125rem;
  border-radius: var(--border-radius-main);
  object-fit: cover;
  box-shadow: var(--shadow-base);
}

.image-1x1 {
  width: 25rem;
  height: 25rem;
  border-radius: var(--border-radius-main);
  object-fit: cover;
  box-shadow: var(--shadow-base);
}

.nav-btn {
  all: unset;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 2.625rem;
  height: 2.625rem;
  border-radius: 50%;
  opacity: 0;
  transition:
    opacity 0.3s ease,
    transform 0.2s ease,
    background-color 0.2s;
  z-index: 10;
}

.nav-btn:hover {
  transform: scale(1.1);
  opacity: 1;
}

.nav-btn:disabled,
.nav-btn[disabled] {
  opacity: 0 !important;
  cursor: default;
  pointer-events: none;
}

.gallery-wrapper:hover .nav-btn {
  opacity: 1;
}

.mode-outside {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: var(--padding-large);
}

.mode-outside .nav-btn {
  position: relative;
  background: transparent;
}
.mode-outside .nav-btn:hover {
  transform: scale(1.1);
  background-color: rgba(0, 0, 0, 0.05);
}

.mode-inside {
  display: block;
}

.mode-inside .nav-btn {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  background: rgba(0, 0, 0, 0.3);
  backdrop-filter: blur(var(--blur-base));
  -webkit-backdrop-filter: blur(5px);
}

.mode-inside .nav-btn:hover {
  background: rgba(0, 0, 0, 0.6);
  transform: translateY(-50%) scale(1.05);
}

.mode-inside .prev-btn {
  left: 1rem;
}

.mode-inside .next-btn {
  right: 1rem;
}

.nav-btn:disabled {
  opacity: 0.2;
  cursor: default;
}

:deep(.swiper-pagination) {
  position: relative;
  bottom: 0;
  margin-top: 1rem;
}

:deep(.swiper-pagination-bullet) {
  width: 0.625rem;
  height: 0.625rem;
  background-color: var(--color-button-disabled);
  opacity: 0.6;
  margin: 0 5px !important;
  transition: all 0.3s ease;
}

:deep(.swiper-pagination-bullet-active) {
  background-color: var(--color-text-base);
  opacity: 0.7;
  transform: scale(1.1);
}
</style>
