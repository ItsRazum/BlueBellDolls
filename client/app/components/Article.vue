<script setup lang="ts">

defineProps<{
  photoUrl: string,
  title: string,
  description: string,
  redirectUrl: string,
}>();

</script>

<template>
  <CardWrapper class="card-compact">
    <div class="adv-info-container">
      <img class="card-compact-photo" :src="photoUrl" :alt="title" />
      <CardWrapper class="adv-props-container" :enable-blur="true">
        <h2>{{ title }}</h2>
        <span class="description">{{ description.slice(0, 150) }}</span>
      </CardWrapper>
      <div>
        <NuxtLink :to="redirectUrl" class="btn">Перейти</NuxtLink>
      </div>
    </div>
  </CardWrapper>
</template>

<style scoped>

.card-compact-photo {
  width: 100%;
  height: 13.5rem;
}

.adv-info-container {
  display: flex;
  flex-direction: column;
  box-sizing: border-box;
  height: auto;
  gap: var(--padding-small);
  flex-grow: 1;
  line-height: 1;
}

.adv-props-container {
  display: flex;
  flex-direction: column;
  margin: 0 var(--padding-large);
  margin-top: -40px;
  padding: var(--padding-small);
  gap: var(--padding-small);
}

.description {
  font-weight: 400;
  line-height: 1.2;
}

.btn {
  margin: var(--padding-large);
  margin-top: 0;
}

</style>