<script setup lang="ts">
const route = useRoute();

const headerTitle = computed(() => route.meta.title as string);
const headerImage = computed(() => route.meta.backgroundImage as string);
const headerBtnText = computed(() => route.meta.buttonText as string);
const headerBtnUrl = computed(() => route.meta.buttonUrl as string);
</script>

<template>
  <TheHeader
    :title="headerTitle"
    :background-image="headerImage"
    :button-text="headerBtnText"
    :button-url="headerBtnUrl"
  />

  <main>
    <slot />
  </main>

  <TheFooter />
</template>

<style scoped></style>
