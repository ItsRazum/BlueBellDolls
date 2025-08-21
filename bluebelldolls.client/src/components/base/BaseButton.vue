<script setup lang="ts">
import { computed } from 'vue'

defineProps({
  to: {
    type: [String, Object],
    default: undefined
  },
  variant: {
    type: String,
    default: 'common',
    validator: (value) => ['header', 'common'].includes(value)
  },
})

const variantStyles = {
  header: 'bg-white text-black text-[18px] rounded-full',
  common: 'bg-[#2CA1FF] text-white text-[13px] font-semibold rounded-2xl shadow-2xl hover:shadow-xl transition'
}
</script>

<template>
  <component
    :is="to ? 'RouterLink' : 'button'"
    :to
    class="inline-flex items-center justify-center px-8 py-2  "
    :class="[
      variantStyles[variant]
    ]">
      <slot />
  </component>
</template>
