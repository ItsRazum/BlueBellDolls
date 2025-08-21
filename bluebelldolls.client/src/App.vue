<script setup lang="ts">
import Header from '@/components/Header.vue'
import { useRoute } from 'vue-router';

defineProps<{
  title?: string,
  buttonText?: string,
  buttonVisibility?: boolean,
  to?: string
}>()

const route = useRoute();
</script>

<template>

  <div class="container mx-auto max-w-[1280px] font-sans">
    <nav class="z-50 fixed top-0 ">
      <div class="ml-[44px] my-[5px]">
        <div class="flex justify-start gap-[75px] items-center">
          <div>
            <h2 class="text-[27px] font-bold text-white">
              <RouterLink to="/">BlueBellDolls</RouterLink>
            </h2>
          </div>
          <div class="flex justify-start gap-[60px] items-center">
            <div>
              <RouterLink to="/litters"    class="commonButton">Помёты</RouterLink>
            </div>
            <div>
              <RouterLink to="/femalecats" class="commonButton">Кошки</RouterLink>
            </div>
            <div>
              <RouterLink to="/malecats"   class="commonButton">Коты</RouterLink>
            </div>
            <div>
              <RouterLink to="/colors"     class="commonButton">Окрасы</RouterLink>
            </div>
            <div>
              <RouterLink to="/gallery"    class="commonButton">Галерея</RouterLink>
            </div>
          </div>
        </div>
      </div>
    </nav>
    <div>
      <Header :title="route.meta.title"
              to="/litters"
              :buttonText="route.meta.buttonText"
              :buttonVisibility="buttonVisibility" />
    </div>
    <main class="flex justify-center w-[100%]">
      <RouterView />
    </main>
  </div>
</template>
