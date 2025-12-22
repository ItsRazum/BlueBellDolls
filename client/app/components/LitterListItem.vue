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

const freeKittensCount = computed(() => {
  return props.litter.kittens.filter(k => k.status === KittenStatus.Available).length;
});

const availableKittensString = freeKittensCount.value == 1 ? ` свободен` : ` свободных`;

const isLitterModalOpen = ref(false);
const openLitterModal = () => {
  isLitterModalOpen.value = true;
}

const closeLitterModal = () => {
  isLitterModalOpen.value = false;
}


</script>

<template>
  <CardWrapper class="litter-card">
    <div class="card-expanded">
      <div class="card-photo-container">
        <img class="card-expanded-photo" :src="litter.photos[0].url" :alt="litterDisplayName" />
        <button class="link-btn" @click="openLitterModal">Больше фото</button>
      </div>
      <CardWrapper :enable-blur="true" class="card-info-container">
        <div class="card-header">
          <h2 style="font-size: 36px">{{ litterDisplayName }}</h2>
          <span style="font-weight: 505; color: var(--color-text-caption); font-size: 24px">{{ litter.birthDay }}</span>
        </div>
        <CardWrapper class="card-info-body">
          <div class="card-info-props">
            <div class="card-property">
              <span>Папа: </span>
              <RouterLink>{{ litter.fatherCat.name }}</RouterLink>
            </div>
            <div class="card-property">
              <span>Мама: </span>
              <RouterLink>{{ litter.motherCat.name }}</RouterLink>
            </div>
            <div class="card-property">
              <span style="color: var(--color-context-blue);">{{ litter.kittens.length }}</span>
              <span>{{ kittensString }} всего</span>
            </div>

            <div class="card-property">
              <span style="color: var(--color-context-blue);">{{ litter.kittens.length }}</span>
              <span>{{ availableKittensString }}</span>
            </div>
          </div>
          <span> {{ litter.description }}</span>
        </CardWrapper>
      </CardWrapper>
    </div>
    <div class="separator"/>
    <div class="kittens-grid">
      <KittenListItem
          v-for="kitten in litter.kittens"
          :key="kitten.id"
          :kitten="kitten" />
    </div>
  </CardWrapper>
  <LitterModal :is-open="isLitterModalOpen" :litter="props.litter" @close="closeLitterModal" />
</template>

<style scoped>

span {
  font-size: 1.125rem;
}

.litter-card {
  padding: var(--padding-large);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: var(--padding-extra-large);
}

.card-info-container {
  gap: var(--padding-large);
}

.card-info-body {
  padding: var(--padding-large);
}

.card-property span,
.card-property a {
  font-size: 1.25rem;
}

.card-expanded-photo {
  height: 295px;
  width: 295px;
}

.kittens-grid {
  display: flex;
  flex-direction: column;
  gap: var(--padding-extra-large);
  margin-top: 20px;
  width: 100%;
}

</style>