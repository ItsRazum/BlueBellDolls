<script setup lang="ts">
  import ContentFrame from '@/components/base/ContentFrame.vue'

  defineProps<{
    imagePath?: string
    text?: string
  }>()
</script>

<template>
  <div class="p-[21px] flex justify-start items-center">
    <img :src="imagePath" alt="img" class="w-[380px] h-[250px] rounded-2xl object-cover" />
    <ContentFrame class="-ml-[110px] w-[100%] max-w-[838px] p-[24px] shadow" blur="true">
      <h4 class="text-[24px] font-semibold leading-snug">
        {{ text }}
      </h4>
    </ContentFrame>
  </div>
</template>
