<script setup lang="ts">
defineProps<{
  photoUrl: string;
  readOnly?: boolean;
  expanded?: boolean;
  alt: string;
}>()

const config = useRuntimeConfig();
const apiBase = import.meta.env.DEV ? config.public.apiBase : '';

defineEmits(["click"]);

</script>

<template>
  <div class="card-photo-container">
    <img class="card-expanded-photo" :class="{ large: expanded }" :src="apiBase + photoUrl" :alt="alt" />
    <button v-if="!readOnly" @click="$emit('click')" class="link-btn desktop-only">
      {{ $t("components.common.morePhotos") }}
    </button>
  </div>
</template>

<style scoped>
@media (max-width: 604px) {
  .link-btn {
    font-size: 0.9rem;
  }
}
</style>