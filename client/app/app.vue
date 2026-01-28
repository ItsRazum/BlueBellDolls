<template>
  <NuxtLayout>
    <NuxtPage />
  </NuxtLayout>
</template>

<script setup lang="ts">

const { t } = useI18n();

useHead({
  titleTemplate: (title) => {
    return title
    ? `${title} | BlueBellDolls`
    : t('seo.titles.main');
  }
})

</script>
