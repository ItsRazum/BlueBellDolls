<script setup lang="ts">

import {KittenClass, KittenStatus} from "~~/enums/enums";

const props = defineProps<{
  litter: LitterDetailDto;
}>();

const config = useRuntimeConfig();
const apiBaseUrl = config.public.apiBase;
const litterDisplayName = computed(() => `Помёт «${props.litter.letter}»`);

const catLink = 'api/parentcats/'
const fatherCatLink = computed(() => catLink + props.litter.fatherCatId)
const motherCatLink = computed(() => catLink + props.litter.motherCatId)

const freeKittensCount = computed(() => {
  let count = 0;
  for (let i = 0; i < 5; i++) {
    if (props.litter.kittens[i].status == KittenStatus.Available) {
      count++;
    }
  }
});

const kittensString = computed(() => {
  switch (props.litter.kittens.length) {
    case 1:
      return ` котёнок`;
    case 2:
    case 3:
    case 4:
      return ` котёнка`;
    default:
      return ` котят`;
  }
});

const availableKittensString = computed(() => props.litter.kittens.length === 1 ? ` свободен` : ` свободных`);
</script>

<template>
<CardWrapper class="litter-card">
  <div class="litter-info-card">
    <div class="photoCard">
      <img class="litter-photo-default" :src="litter.photos[0].url" :alt="litterDisplayName" />
      <RouterLink class="link">Больше фото</RouterLink>
    </div>
    <CardWrapper :enable-blur="true" class="litter-info-container">
      <div class="litter-header">
        <h2 style="font-size: 36px">{{ litterDisplayName }}</h2>
        <span style="font-weight: 505; color: var(--color-text-caption); font-size: 24px">{{ litter.birthDay }}</span>
      </div>
        <CardWrapper class="litter-info-body">
          <div class="litter-info-unit">
            <div class="litter-property">
              <span>Папа: </span>
              <RouterLink>{{ litter.fatherCat.name }}</RouterLink>
            </div>
            <div class="litter-property">
              <span>Мама: </span>
              <RouterLink>{{ litter.motherCat.name }}</RouterLink>
            </div>
            <div class="litter-property">
              <span style="color: var(--color-context-blue);">{{ litter.kittens.length }}</span>
              <span>{{ kittensString }} всего</span>
            </div>

            <div class="litter-property">
              <span style="color: var(--color-context-blue);">{{ litter.kittens.length }}</span>
              <span>{{ availableKittensString }}</span>
            </div>
          </div>
          <span> {{ litter.description }}</span>
        </CardWrapper>
    </CardWrapper>
  </div>
  <div class="litter-separator"/>
  <div class="kittens-grid">
    <KittenListItem
      v-for="kitten in litter.kittens"
      :key="kitten.id"
      :kitten="kitten"
      variant="default"/>
  </div>
</CardWrapper>
</template>

<style scoped>

span {
  font-size: 18px;
}

a,
.link {
  color: var(--color-link);
  font-weight: 525;
  text-decoration: underline;
  padding: 0;
}

.litter-card {
  padding: var(--padding-large);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: var(--padding-extra-large);
}

.litter-info-container {
  margin-left: -20px;
  height: min-content;
  width: 100%;
  display: flex;
  flex-direction: column;
  gap: var(--padding-large);
  padding: var(--padding-large);
}

.litter-header {
  display: flex;
  flex-direction: column;
  justify-content: flex-start;
  align-items: flex-start;
  gap: var(--padding-small);
}

.litter-info-body {
  display: flex;
  flex-direction: column;
  gap: var(--padding-large);
  font-weight: 600;
  padding: var(--padding-large);
}

.litter-info-unit {
  height: min-content;
  display: flex;
  flex-direction: column;
  gap: var(--padding-small);
}

.litter-property {
  width: max-content;
}

.litter-property span,
.litter-property a {
  font-size: 21px;
}

.photoCard {
  flex-grow: 0;
  display: flex;
  flex-direction: column;
  justify-content: flex-start;
  align-items: center;
  flex-shrink: 0;
  gap: 10px;
  padding: 0;
}

.litter-photo-default {
  height: 295px;
  width: 295px;
  object-fit: cover;
  flex-grow: 0;
  border-radius: var(--border-radius-main);
  box-shadow: var(--shadow-base);
}

.litter-info-card {
  width: 1000px;
  display: flex;
  line-height: 1;
}

.litter-separator {
  width: 100%;
  max-width: 700px;
  height: 1px;

  background-color: var(--color-text-context);
}

.kittens-grid {
  display: flex;
  flex-direction: column;
  gap: var(--padding-extra-large);
  margin-top: 20px;
  width: 100%;
}

</style>