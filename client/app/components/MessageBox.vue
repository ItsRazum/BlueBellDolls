<script setup lang="ts">

defineProps<{
  imageUrl: string;
  text: string;
}>();

</script>

<template>
  <div class="flex">
    <img :src="imageUrl" alt="image" class="image-block" />
    <CardWrapper class="message-block" :enable-blur="true">
      <span class="text-[1.5rem] font-medium">{{text}}</span>
    </CardWrapper>
  </div>
</template>

<style scoped>

.image-block {
  width: 23.75rem;
  height: 15.625rem;
  border-radius: var(--border-radius-main);
  object-fit: cover;
}

.message-block {
  padding: var(--padding-large);
  margin: 1.875rem 0 1.875rem -6.875rem;
}

</style>