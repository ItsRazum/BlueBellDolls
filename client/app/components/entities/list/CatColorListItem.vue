<script setup lang="ts">
defineProps<{
  catColor: CatColorListDto;
}>();

const { locale } = useI18n();

const config = useRuntimeConfig();
const apiBaseUrl = config.public.apiBase;

const catColorModal = useModal();
</script>

<template>
  <div class="card-expanded">
    <div class="card-photo-container">
      <img
        class="card-expanded-photo"
        :src="apiBaseUrl + catColor.mainPhotoUrl"
        :alt="catColor.identifier"
      />
      <button @click="catColorModal.open" class="link-btn">
        {{ $t("components.common.morePhotos") }}
      </button>
    </div>
    <CardWrapper
      :enable-blur="true"
      :show-border="false"
      style="gap: var(--padding-medium)"
      class="card-info-container w-full"
    >
      <div class="card-header">
        <h2>{{ $t("components.common.catcolors.title", { color: humanizeString(catColor.identifier) }) }}</h2>
      </div>
      <div v-if="locale == 'ru'" class="card-info-body">
        <span class="mt-[0.875rem]"> {{ catColor.description }}</span>
      </div>
      <div class="buttons-container gap-(--padding-small)">
        <button @click="catColorModal.open">{{ $t("components.common.more") }}</button>
      </div>
    </CardWrapper>
  </div>

  <CatColorModal
    :cat-color-id="catColor.id"
    :is-open="catColorModal.isOpen.value"
    @close="catColorModal.close"
  />
</template>
