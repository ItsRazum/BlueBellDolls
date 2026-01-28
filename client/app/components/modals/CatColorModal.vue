<script setup lang="ts">
import CatColorDetail from "~/components/entities/detail/CatColorDetail.vue";

defineProps<{
  catColorId: number;
  isOpen: boolean;
}>();

const emit = defineEmits(["close"]);

const close = () => {
  emit("close");
};
</script>

<template>
  <BaseModal :isOpen="isOpen" @close="close" :copy-url="`/catcolors/${catColorId}`">
    <CatColorDetail :cat-color-id="catColorId" />
  </BaseModal>
</template>
