<script setup lang="ts">

const props = withDefaults(defineProps<{
  theme?: 'light' | 'dark',
  width?: string,
  height?: string,
  radius?: string
}>(), {
  theme: 'light',
  width: '100%',
  height: '1em',
  radius: '8px',
});

</script>

<template>

  <div :class="`skeleton-base-${props.theme}`"
       :style="{ width: props.width, height: props.height, borderRadius: props.radius }">
  </div>

</template>

<style scoped>
.skeleton-base-light {
  background-color: var(--color-skeleton-light-base);
  background-image: linear-gradient(
      90deg,
      var(--color-skeleton-light-base) 0px,
      var(--color-skeleton-light-higlight) 50px,
      var(--color-skeleton-light-base) 100px
  );
  background-size: 1000px;
  animation: shimmer 2s linear infinite;
}

.skeleton-base-dark {
  background-color: var(--color-skeleton-dark-base);
  background-image: linear-gradient(
      90deg,
      var(--color-skeleton-dark-base) 0px,
      var(--color-skeleton-dark-higlight) 50px,
      var(--color-skeleton-dark-base) 100px
  );
  background-size: 1000px;
  animation: shimmer 2s linear infinite;
}

@keyframes shimmer {
  0% { background-position: -100px; }
  40%, 100% { background-position: 800px; }
}
</style>