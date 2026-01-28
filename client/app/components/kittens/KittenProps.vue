<script setup lang="ts">
const props = defineProps<{
  kitten: Kitten;
}>();

import { KittenClass, KittenStatus } from "~~/enums/enums";

const { t } = useI18n();

const statusColor = computed(() => {
  switch (props.kitten.status) {
    case KittenStatus.Available:
      return "var(--color-status-available)";
    case KittenStatus.Reserved:
    case KittenStatus.UnderObservation:
      return "var(--color-status-reserved)";
    case KittenStatus.Sold:
      return "var(--color-status-unavailable)";
    default:
      return "";
  }
});

const genderPrefix = computed(() => (props.kitten.isMale ? "male" : "female"));
const genderColor = computed(() => `var(--color-gender-${genderPrefix.value})`);
const genderText = computed(() => t(`components.common.cats.${genderPrefix.value}Default`));

const status = computed(() =>
  t(`components.common.kittens.statuses.${genderPrefix.value}${KittenStatus[props.kitten.status]}`),
);

const classDescription = computed(() =>
  t(`components.common.kittens.tips.classes.${KittenClass[props.kitten.class]}`),
);

const statusDescription = computed(() =>
  t(`components.common.kittens.tips.statuses.${KittenStatus[props.kitten.status]}`),
);

const colorDisplayName = computed(() => humanizeString(props.kitten.catColor.identifier));

const colorModal = useModal();
</script>

<template>
  <div class="card-info-props">
    <div class="card-property">
      <span>Пол: </span>
      <span :style="{ color: genderColor }">{{ genderText }}</span>
    </div>
    <div class="card-property" v-if="kitten.catColor">
      <span>{{ $t("components.common.cats.color") }}: </span>
      <span v-if="kitten.catColor?.id === 0" class="link-btn">{{ colorDisplayName }}</span>
      <button
        v-else
        class="link-btn"
        :class="{ underline: kitten.catColor?.id != 0 }"
        @click="colorModal.open"
      >
        {{ colorDisplayName }}
      </button>
    </div>
    <div class="card-property tooltip-wrapper">
      <span>{{ $t("components.common.kittens.class") }}: </span>
      <span style="color: var(--color-kitten-class)">{{ KittenClass[kitten.class] }}</span>
      <span class="tooltip">{{ classDescription }}</span>
    </div>
    <div class="card-property tooltip-wrapper">
      <span>{{ $t("components.common.kittens.status") }}: </span>
      <span :style="{ color: statusColor }">{{ status }}</span>
      <span class="tooltip">{{ statusDescription }}</span>
    </div>
  </div>

  <CatColorModal
    v-if="kitten.catColor"
    :is-open="colorModal.isOpen.value"
    @close="colorModal.close"
    :cat-color-id="kitten.catColor.id"
  />
</template>

<style scoped>
.link-btn {
  all: unset;

  color: var(--color-link);
  font-weight: 525;

  font-family: var(--font-family-base);
  font-size: 1.125rem;
  transition: color 0.2s;
}

.link-btn.underline {
  text-decoration: underline;
  cursor: pointer;
}

.link-btn.underline:hover {
  color: var(--color-link-hover);
}
</style>
