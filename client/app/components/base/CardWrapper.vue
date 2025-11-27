<script setup lang="ts">
  defineProps({
    enableBlur: {
      type: Boolean,
      default: false
    }
  });
</script>

<template>
  <div :class="[
      'card-wrapper',
      { 'card-wrapper--blurred': enableBlur }
    ]">
    <slot></slot>
  </div>
</template>

<style scoped>
  .card-wrapper {
    background-color: var(--color-background-card);
    border: 1px solid var(--color-border-card);
    border-radius: var(--border-radius-main);
    box-shadow: var(--shadow-base);
  }

  .card-wrapper--blurred {
    backdrop-filter: blur(var(--blur-base));
    -webkit-backdrop-filter: blur(var(--blur-base));
  }

</style>
