<script setup lang="ts">
import KittenProps from "~/components/kittens/KittenProps.vue";
const props = defineProps<{
  kittenId: number;
}>();

const { locale } = useI18n();

const bookingModal = useModal();

const kittenApi = useKittenApi();

const { data: kitten, pending } = await kittenApi.getById(props.kittenId);
</script>

<template>
  <div v-if="pending" class="content-container">
    <div
      style="display: flex; flex-direction: column; align-items: center; gap: var(--padding-large)"
    >
      <Skeleton theme="dark" width="25rem" height="25rem" radius="var(--border-radius-main)" />
      <Skeleton theme="dark" style="width: 9rem" />
    </div>

    <CardWrapper class="card-info-container">
      <div class="card-header">
        <Skeleton width="16rem" height="2rem" radius="0.825rem" />
        <Skeleton width="6rem" height="1rem" radius="0.825rem" />
      </div>
      <div class="card-info-body">
        <CardWrapper class="card-info-props">
          <Skeleton width="35%" height="1rem" radius="0.825rem" />
          <Skeleton width="50%" height="1rem" radius="0.825rem" />
          <Skeleton width="25%" height="1rem" radius="0.825rem" />
          <Skeleton width="40%" height="1rem" radius="0.825rem" />
          <div
            v-if="locale == 'ru'"
            style="display: flex; flex-direction: column; margin-top: 0.875rem; gap: 0.3rem"
          >
            <Skeleton width="90%" height="0.825rem" radius="0.825rem" />
            <Skeleton width="95%" height="0.825rem" radius="0.825rem" />
            <Skeleton width="70%" height="0.825rem" radius="0.825rem" />
          </div>
        </CardWrapper>
      </div>
      <div class="buttons-container" style="gap: var(--padding-small)">
        <Skeleton width="9rem" height="2.5rem" radius="1rem" />
      </div>
    </CardWrapper>
  </div>

  <div v-else class="content-container">
    <PhotoGallery :photos="kitten.photos" aspectRatio="1:1" controls-position="inside" />
    <CardWrapper class="card-info-container">
      <div class="card-header">
        <h2>{{ kitten.name }}</h2>
        <span style="font-weight: 500; color: var(--color-text-caption)">{{
          kitten.birthDay
        }}</span>
      </div>
      <div class="card-info-body">
        <CardWrapper class="card-info-props">
          <kitten-props :kitten="kitten" />
          <span v-if="locale == 'ru'" style="margin-top: 0.875rem"> {{ kitten.description }}</span>
        </CardWrapper>
      </div>
      <div class="buttons-container" style="gap: var(--padding-small)">
        <button @click="bookingModal.open">{{ $t("components.common.kittens.book") }}</button>
      </div>
    </CardWrapper>
  </div>

  <KittenBookingModal
    :kitten="kitten"
    :is-open="bookingModal.isOpen.value"
    @close="bookingModal.close"
  />
</template>

<style scoped>
.card-info-container {
  min-width: 25rem;
  margin: 0;
  height: min-content;
}

.card-info-props {
  padding: var(--padding-large);
  max-width: 20rem;
}

.content-container {
  display: flex;
  gap: var(--padding-large);
}
</style>
