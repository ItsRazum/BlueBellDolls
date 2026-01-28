<script setup lang="ts">
const props = defineProps({
  enableBlur: {
    type: Boolean,
    default: false,
  },
  showBorder: {
    type: Boolean,
    default: true,
  },
});
</script>

<template>
  <div
    :class="['card-wrapper', { 'card-wrapper--blurred': enableBlur }]"
    :style="`borderWidth: ${showBorder ? 1 : 0}px;`"
  >
    <slot></slot>
  </div>
</template>

<style scoped>
.card-wrapper {
  background-color: var(--color-background-card);
  border: solid var(--color-border-card);
  border-radius: var(--border-radius-main);
  box-shadow: var(--shadow-base);
}

.card-wrapper--blurred {
  backdrop-filter: blur(var(--blur-base));
  -webkit-backdrop-filter: blur(var(--blur-base));
}
</style>
