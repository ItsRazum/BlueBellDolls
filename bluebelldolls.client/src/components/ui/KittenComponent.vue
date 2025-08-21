<script setup lang="ts">
  import type { Kitten } from '@/models/Kitten.ts'
  import ContentFrame from '@/components/base/ContentFrame.vue'
  import { computed } from 'vue'

  const props = defineProps<{
    kitten: Kitten
  }>()

  const kittenImage = computed(() => {
    const firstPhoto = [...props.kitten.Photos.values()[0]] ?? ''
    return `data:image/*;base64,${firstPhoto}`
  })
</script>

<template>
  <ContentFrame class="max-w-[316px] space-y-[10px] max-h-[508px]">
    <img :src="kittenImage" :alt="props.kitten.name" class="rounded-[10px] object-cover w-[274px] h-[274px] shadow-2xl" />
    <div class="flex flex-col space-y-[8px]">
      <h1 class="text-[24px] font-semibold">{{ props.kitten.name }}</h1>
    </div>

  </ContentFrame>
</template>
