<script setup lang="ts">

defineProps<{
  photoUrl: string,
  name: string,
  description: string,
  redirectUrl: string,
}>();

</script>

<template>
  <CardWrapper class="adv">
    <div class="adv-info-container">
      <img class="adv-photo" :src="photoUrl" :alt="name" />
      <CardWrapper class="adv-props-container">
        <h2>{{ name }}</h2>
        <span class="description">{{ description.slice(0, 150) }}</span>
      </CardWrapper>
      <div>
        <RouterLink :to="redirectUrl" class="btn" style="text-decoration: none">Перейти</RouterLink>
      </div>
    </div>
  </CardWrapper>
</template>

<style scoped>
.adv {
  padding: var(--padding-large);
  max-width: 294px;
  text-align: left;
  border-radius: var(--border-radius-main);
  overflow: hidden;
}

.adv-photo {
  width: 100%;
  height: 216px;
  object-fit: cover;
  border-radius: var(--border-radius-main);
  box-shadow: var(--shadow-base);
}

.adv-info-container {
  display: flex;
  flex-direction: column;
  gap: var(--padding-small);
  flex-grow: 1;
  height: 100%;
  line-height: 1;
}

.adv-props-container {
  display: flex;
  flex-direction: column;
  padding: var(--padding-small);
  gap: var(--padding-small);
}

span {
  font-weight: 400;
  line-height: 1.2;
}

</style>