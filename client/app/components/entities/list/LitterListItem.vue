<script setup lang="ts">
import { KittenStatus } from "~~/enums/enums";

const props = defineProps<{
  litter: LitterDetailDto;
}>();

const { locale } = useI18n();

const config = useRuntimeConfig();
const apiBaseUrl = config.public.apiBase;

const freeKittensCount = computed(() => {
  return props.litter.kittens.filter((k) => k.status === KittenStatus.Available).length;
});

const litterModal = useModal();

const motherModal = useModal();

const fatherModal = useModal();
</script>

<template>
  <CardWrapper class="litter-card">
    <div class="card-expanded">
      <Avatar :expanded="true" :photoUrl="litter.photos[0].url" :alt="litter.name" :read-only="false" @click="litterModal.open" />
      <CardWrapper :enable-blur="true" :show-border="false" class="card-info-container">
        <div class="card-header">
          <h2 class="text-2xl md:text-4xl font-bold">
            {{ $t("components.common.litters.title", { letter: litter.letter }) }}
          </h2>
          <span class="font-medium text-lg md:text-2xl text-(--color-text-caption)">{{
              litter.birthDay
            }}</span>
        </div>
        <CardWrapper :show-border="false" class="card-info-body">
          <div class="card-info-props">
            <div class="card-property">
              <span>{{ $t("components.common.litters.father") }}: </span>
              <button class="link-btn" @click="fatherModal.open">
                {{ litter.fatherCat.name }}
              </button>
            </div>
            <div class="card-property">
              <span>{{ $t("components.common.litters.mother") }}: </span>
              <button class="link-btn" @click="motherModal.open">
                {{ litter.motherCat.name }}
              </button>
            </div>

            <div class="card-property">
              <span style="color: var(--color-context-blue)">{{
                  litter.kittens.length + ` `
                }}</span>
              <span>{{ $t("components.common.litters.totalKittens", litter.kittens.length) }}</span>
            </div>

            <div class="card-property">
              <span style="color: var(--color-context-blue)">{{ freeKittensCount + ` ` }}</span>
              <span>{{ $t("components.common.litters.availableKittens", freeKittensCount) }}</span>
            </div>
            <span v-if="locale == 'ru'" class="description-text">
              {{ litter.description }}</span
            >
          </div>
        </CardWrapper>
      </CardWrapper>
    </div>
    <div class="separator horizontal" />
    <div class="kittens-grid">
      <KittenListItem v-for="kitten in litter.kittens" :key="kitten.id" :kitten="kitten" />
    </div>
  </CardWrapper>

  <LitterModal
      :is-open="litterModal.isOpen.value"
      :litterId="props.litter.id"
      @close="litterModal.close"
  />
  <ParentCatModal
      :is-open="motherModal.isOpen.value"
      :parent-cat-id="litter.motherCatId"
      @close="motherModal.close"
  />
  <ParentCatModal
      :is-open="fatherModal.isOpen.value"
      :parent-cat-id="litter.fatherCatId"
      @close="fatherModal.close"
  />
</template>

<style scoped>
span {
  font-size: 1.125rem;
}

.description-text {
  margin-top: var(--padding-small);
  display: block;
}

.litter-card {
  width: 100%;
  box-sizing: border-box;
  padding: var(--padding-large);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: var(--padding-extra-large);
}

.card-info-container {
  width: 100%;
  gap: var(--padding-large);
}

.card-info-body {
  padding: var(--padding-large);
}

.card-property span,
.card-property button,
.card-property a {
  font-size: 1.25rem;
}

.kittens-grid {
  display: flex;
  flex-direction: column;
  gap: var(--padding-extra-large);
  margin-top: 20px;
  width: 100%;
}

@media (max-width: 720px) {
  .litter-card {
    padding: 1rem;
    gap: 1.5rem;
  }

  span {
    font-size: 0.95rem;
  }

  .card-property span,
  .card-property button,
  .card-property a {
    font-size: 1rem;
  }

  .card-info-body {
    padding: 1rem;
  }

  .kittens-grid {
    gap: 1.5rem;
    margin-top: 10px;
  }
}
</style>
