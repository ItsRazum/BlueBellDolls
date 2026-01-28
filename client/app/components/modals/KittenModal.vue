<script setup lang="ts">
import KittenDetail from "~/components/entities/detail/KittenDetail.vue";

defineProps<{
  kittenId: number;
  isOpen: boolean;
}>();

const emit = defineEmits(["close"]);

const close = () => {
  emit("close");
};
</script>

<template>
  <BaseModal :is-open="isOpen" @close="close" :copy-url="`/kittens/${kittenId}`">
    <KittenDetail :kittenId="kittenId" />
  </BaseModal>
</template>
