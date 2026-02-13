<script setup lang="ts">
defineProps<{
  imageUrl: string;
  text: string;
}>();
</script>

<template>
  <div class="message-container">
    <img :src="imageUrl" alt="image" class="image-block" />
    <CardWrapper class="message-block" :enable-blur="true">
      <span class="font-medium">{{ text }}</span>
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

.message-container {
  display: flex;
  flex-direction: row;
}

@media (max-width: 700px) {
  .message-container {
    flex-direction: column;
    align-items: center;
  }

  .message-block {
    margin: -2.875rem 0 0 0;
  }
}

</style>
