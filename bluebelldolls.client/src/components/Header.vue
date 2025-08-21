<script setup lang="ts">
import BaseButton from '@/components/base/BaseButton.vue'

defineProps<{
  title?: string,
  buttonText?: string,
  buttonVisibility?: boolean,
  to?: string
}>()
</script>

<template>
  <header class="
          flex
          flex-col
          h-[508px]
          bg-[url('@/assets/images/header-bg.png')]
          bg-cover
          bg-center
          w-[100%]
          px-[33px] py-[44px]
          gap-[160px]
          ">
    <div class="space-y-[15px] max-w-[430px]">
      <h2 class="text-[39px] text-white font-bold">{{ title }}</h2>
      <BaseButton v-if="buttonText" variant="header" :to>
        {{ buttonText }}
      </BaseButton>
    </div>
  </header>
</template>

<style>
  .commonButton {
    font-size: 21px;
    font-weight: 500;
    color: white;
  }
</style>
