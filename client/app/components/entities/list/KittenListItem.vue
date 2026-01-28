<script setup lang="ts">
import { KittenStatus } from "~~/enums/enums";
import KittenProps from "~/components/kittens/KittenProps.vue";

const props = withDefaults(
  defineProps<{
    kitten: KittenListDto;
    variant?: "default" | "compact";
    readOnly?: boolean;
  }>(),
  {
    variant: "default",
    readOnly: false,
  },
);

const { locale, t } = useI18n();

const config = useRuntimeConfig();
const apiBaseUrl = config.public.apiBase;

const photoUrl = getKittenPhotoUrl(props.kitten);

const bookingModal = useModal();
const kittenModal = useModal();

const litterPageUrl = computed(() => `/litters/${props.kitten.litterId}`);
const genderPrefix = computed(() => (props.kitten.isMale ? "male" : "female"));
const genderText = computed(() =>
  t(`components.common.cats.${genderPrefix.value}${capitalizeFirst(props.variant)}`),
);
const genderColor = computed(() => `var(--color-gender-${genderPrefix.value})`);
</script>

<template>
  <CardWrapper v-if="variant === 'compact'" class="card-compact">
    <div class="card-compact-content">
      <img class="card-compact-photo" :src="apiBaseUrl + photoUrl" :alt="kitten.name" />
      <h2>{{ kitten.name }}</h2>
      <div class="subtitle">
        <span class="font-medium">{{ kitten.birthDay }}</span>
        <span> • </span>
        <span :style="{ color: genderColor }">{{ genderText }}</span>
        <span> • </span>
        <NuxtLinkLocale :to="litterPageUrl" class="link">{{
          $t("components.common.litters.title", { letter: kitten.litterLetter })
        }}</NuxtLinkLocale>
      </div>
      <CardWrapper :show-border="false" class="p-(--padding-small)" v-if="kitten.description">
        <span class="description">{{ kitten.description.slice(0, 70) }}</span>
      </CardWrapper>
      <div class="buttons-container w-full justify-between">
        <button class="w-full" @click="kittenModal.open">{{ $t("components.common.more") }}</button>
        <button
          class="w-full"
          @click="bookingModal.open"
          v-if="kitten.status === KittenStatus.Available"
        >
          {{ $t("components.common.kittens.book") }}
        </button>
      </div>
    </div>
  </CardWrapper>

  <div v-else class="card-expanded">
    <div class="card-photo-container">
      <img class="card-expanded-photo" :src="apiBaseUrl + photoUrl" :alt="kitten.name" />
      <button v-if="!props.readOnly" @click="kittenModal.open" class="link-btn">
        {{ $t("components.common.morePhotos") }}
      </button>
    </div>
    <CardWrapper :enable-blur="true" :show-border="false" class="card-info-container w-full">
      <div class="card-header">
        <h2>{{ kitten.name }}</h2>
        <span class="font-medium color-(--color-text-caption)">{{ kitten.birthDay }}</span>
      </div>
      <div class="card-info-body">
        <CardWrapper :show-border="false" class="card-info-props">
          <kitten-props :kitten="props.kitten" />
          <span v-if="locale == 'ru'" class="mt-[0.875rem]"> {{ kitten.description }}</span>
        </CardWrapper>
      </div>

      <div v-if="!props.readOnly" class="buttons-container" style="gap: var(--padding-small)">
        <button @click="kittenModal.open">{{ $t("components.common.more") }}</button>
        <button @click="bookingModal.open" v-if="kitten.status === KittenStatus.Available">
          {{ $t("components.common.kittens.book") }}
        </button>
      </div>
    </CardWrapper>
  </div>

  <KittenModal
    :kitten-id="kitten.id"
    :is-open="kittenModal.isOpen.value"
    @close="kittenModal.close"
  />
  <KittenBookingModal
    :kitten="props.kitten"
    :is-open="bookingModal.isOpen.value"
    @close="bookingModal.close"
  />
</template>

<style scoped>
.card-info-props {
  padding: var(--padding-large);
}
</style>
