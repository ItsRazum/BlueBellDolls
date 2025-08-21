<script setup lang="ts">
  import ContentFrame from '@/components/base/ContentFrame.vue'
  import BaseButton from '@/components/base/BaseButton.vue'

  defineProps<{
    imagePath?: string
    header?: string
    text?: string
  }>()
</script>

<template>
  <ContentFrame class="flex flex-col space-y-[10px] max-w-[376px]">
    <img class="rounded-2xl w-[100%] h-[224px] object-cover shadow-xl" :src="imagePath" alt="img" />
    <ContentFrame class="w-[100%] space-y-[10px]" compact="true">
      <h1 class="text-[21px] font-bold">{{ header }}</h1>
      <h1 class="text-[14.5px]">{{ text }}</h1>
    </ContentFrame>
    <BaseButton class="text-[15px] w-[100%]">
      Перейти
    </BaseButton>
  </ContentFrame>
</template>
