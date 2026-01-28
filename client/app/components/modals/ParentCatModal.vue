<script setup lang="ts">
import ParentCatDetail from "~/components/entities/detail/ParentCatDetail.vue";

defineProps<{
  parentCatId: number;
  isOpen: boolean;
}>();

const emit = defineEmits(["close"]);

const close = () => {
  emit("close");
};
</script>

<template>
  <BaseModal :is-open="isOpen" @close="close" :copy-url="`/cats/${parentCatId}`">
    <ParentCatDetail :target-id="parentCatId" />
  </BaseModal>
</template>
