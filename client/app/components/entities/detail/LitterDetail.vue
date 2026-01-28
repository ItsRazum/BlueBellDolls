<script setup lang="ts">
import { KittenStatus } from "~~/enums/enums";

const props = defineProps<{
  litterId: number;
}>();

const { locale } = useI18n();

const litterApi = useLitterApi();

const { data: litter, pending } = await litterApi.getById(props.litterId);

const freeKittensCount = computed(() => {
  if (!litter.value.kittens) return 0;
  return litter.value.kittens.filter((k) => k.status === KittenStatus.Available).length;
});

const motherModal = useModal();

const fatherModal = useModal();
</script>

<template>
  <div v-if="!pending" class="content-container">
    <PhotoGallery :photos="litter.photos" aspectRatio="1:1" controls-position="inside" />
    <CardWrapper class="card-info-container">
      <div class="card-header">
        <h2>{{ $t("components.common.litters.title") }} «{{ litter.letter }}»</h2>
        <span>{{ litter.birthDay }}</span>
      </div>
      <CardWrapper class="card-info-body">
        <div class="card-info-props">
          <div class="card-property">
            <span>{{ $t("components.common.litters.father") }}: </span>
            <button class="link-btn" @click="fatherModal.open">{{ litter.fatherCat.name }}</button>
          </div>
          <div class="card-property">
            <span>{{ $t("components.common.litters.mother") }}: </span>
            <button class="link-btn" @click="motherModal.open">{{ litter.motherCat.name }}</button>
          </div>

          <div class="card-property">
            <span style="color: var(--color-context-blue)">{{ litter.kittens.length + ` ` }}</span>
            <span>{{ $t("components.common.litters.totalKittens", litter.kittens.length) }}</span>
          </div>

          <div class="card-property">
            <span style="color: var(--color-context-blue)">{{ freeKittensCount + ` ` }}</span>
            <span>{{ $t("components.common.litters.availableKittens", freeKittensCount) }}</span>
          </div>
          <span v-if="locale == 'ru'" style="margin-top: var(--padding-small)">
            {{ litter.description }}</span
          >
        </div>
      </CardWrapper>
    </CardWrapper>

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
  </div>
  <div v-else class="content-container">
    <CardWrapper class="card-info-container">
      <div
        style="
          display: flex;
          flex-direction: column;
          align-items: center;
          gap: var(--padding-large);
        "
      >
        <Skeleton theme="dark" width="25rem" height="25rem" radius="var(--border-radius-main)" />
        <Skeleton theme="dark" style="width: 9rem" />
      </div>
      <div class="card-header">
        <Skeleton width="16rem" height="2rem" radius="0.825rem" />
        <Skeleton width="6rem" height="1rem" radius="0.825rem" />
      </div>
      <div class="card-info-body">
        <CardWrapper class="card-info-props">
          <Skeleton width="35%" height="1.25rem" radius="0.825rem" />
          <Skeleton width="50%" height="1.25rem" radius="0.825rem" />
          <Skeleton width="25%" height="1.25rem" radius="0.825rem" />
          <Skeleton width="40%" height="1.25rem" radius="0.825rem" />

          <div
            v-if="locale == 'ru'"
            style="display: flex; flex-direction: column; margin-top: 0.875rem; gap: 0.3rem"
          >
            <Skeleton width="90%" height="0.825rem" radius="0.825rem" />
            <Skeleton width="20rem" height="0.825rem" radius="0.825rem" />
            <Skeleton width="70%" height="0.825rem" radius="0.825rem" />
          </div>
        </CardWrapper>
      </div>
    </CardWrapper>
  </div>
</template>

<style scoped>
.content-container {
  display: flex;
  gap: var(--padding-large);
}

.card-info-container {
  min-width: 25rem;
  margin: 0;
  height: min-content;
}

.card-info-props {
  padding: var(--padding-large);
}

.card-property span,
.card-property a {
  font-size: 1.25rem;
}
</style>
