<script setup lang="ts">

  defineProps({
    blur: {
      type: Boolean
    },
    compact: {
      type: Boolean
    },
  })
</script>

<template>
  <div class=" inline-block rounded-2xl bg-white/70 border-1 border-black/20 shadow-lg" :class="blur ? 'backdrop-blur-md' : undefined, compact ? 'p-[10px]' : 'p-[20px]'">
    <slot />
  </div>
</template>
