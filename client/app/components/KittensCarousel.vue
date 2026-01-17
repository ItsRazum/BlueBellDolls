<script setup lang="ts">
import { ref } from 'vue';
import { Carousel, Slide } from 'vue3-carousel';
import 'vue3-carousel/dist/carousel.css';

defineProps<{
  kittens: KittenListDto[]
}>();

const carousel = ref(null);

</script>

<template>
  <div class="carousel-wrapper">

    <button @click="carousel?.prev()" class="nav-btn prev-btn">
      <svg width="24" height="24" viewBox="0 0 24 24" fill="none">
        <path d="M15 19L8 12L15 5" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
      </svg>
    </button>

    <div class="overflow-hidden rounded-(--border-radius-main)">
      <Carousel
          ref="carousel"
          :breakpoints="{
        400: {
          itemsToShow: 1,
          snapAlign: 'start',
        },
        700: {
          itemsToShow: 2,
          snapAlign: 'start',
        },
        1100: {
          itemsToShow: 3,
          snapAlign: 'start',
        },
     }"
          :wrap-around="true"
          :transition="300"
          :autoplay="5000"
      >
        <Slide v-for="kitten in kittens" :key="kitten.id">
          <div class="w-auto">
            <KittenListItem :kitten="kitten" variant="compact" />
          </div>
        </Slide>
      </Carousel>
    </div>

    <button @click="carousel?.next()" class="nav-btn next-btn">
      <svg width="24" height="24" viewBox="0 0 24 24" fill="none">
        <path d="M9 19L16 12L9 5" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
      </svg>
    </button>

  </div>
</template>

<style scoped>

.carousel-wrapper {
  display: flex;
  align-items: center;
  gap: var(--padding-extra-large);
  width: 100%;
  max-width: 1150px;
  margin: 0 auto;
}

.nav-btn {
  all: unset;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  width: 2.5rem;
  height: 2.5rem;
  border-radius: 50%;
  color: var(--color-text-base);
  transition: all 0.2s ease;
}

.nav-btn:hover {
  background-color: rgba(255,255,255,0.3);
  transform: scale(1.1);
}

</style>