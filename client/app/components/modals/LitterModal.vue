<script setup lang="ts">
import LitterDetail from "~/components/entities/detail/LitterDetail.vue";

defineProps<{
  isOpen: boolean;
  litterId: number;
}>();

const emit = defineEmits(["close"]);

const close = () => {
  emit("close");
};
</script>

<template>
  <BaseModal :is-open="isOpen" @close="close" :copy-url="`/litters/${litterId}`">
    <LitterDetail :litter-id="litterId" />
  </BaseModal>
</template>
