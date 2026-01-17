<script setup lang="ts">

import Skeleton from "~/components/Skeleton.vue";

const props = withDefaults(defineProps<{
  variant?: 'default' | 'compact';
  isReadOnly?: boolean;
}>(), {
  variant: 'default',
  isReadOnly: false,
});

</script>

<template>
  <CardWrapper v-if="variant === 'compact'" class="card-compact">
    <div class="card-compact-content">
      <Skeleton width="17.125rem" height="17.125rem" radius="1rem" />
      <Skeleton width="80%" height="1.5rem" radius="0.5rem" />
      <Skeleton width="60%"/>
      <CardWrapper style="padding: var(--padding-small)">
        <Skeleton width="95%" />
        <Skeleton width="85%"/>
        <Skeleton width="60%"/>
      </CardWrapper>
      <div class="buttons" style="justify-content: space-between;">
        <Skeleton width="100%" height="2.2rem" radius="1rem" />
      </div>
    </div>
  </CardWrapper>

  <div v-else class="card-expanded">
    <div class="card-photo-container">
      <Skeleton
          class="card-expanded-photo"
          style="width: 16rem; height: 16rem; margin-bottom: 0; border-radius: 1rem"
      />
      <Skeleton width="9rem" />
    </div>

    <CardWrapper :enable-blur="true" :show-border="false" class="card-info-container" style="width: 100%;">

      <div class="card-header">
        <Skeleton width="40%" height="1.5rem" radius="8px" />
        <Skeleton width="20%" height="0.8rem" />
      </div>

      <div class="card-info-body">
        <CardWrapper class="card-info-props" :show-border="false">
          <div style="display: flex; flex-direction: column; gap: 0.5rem;">
            <Skeleton width="20%" height="1.1rem" />
            <Skeleton width="30%" height="1.1rem" />
          </div>

          <Skeleton width="90%" height="0.7rem" style="margin-top: 1rem;" />
          <Skeleton width="60%" height="0.7rem" />
        </CardWrapper>
      </div>

      <div class="buttons-container" style="gap: var(--padding-small)">
        <Skeleton width="8rem" height="2.5rem" radius="1rem" />
      </div>

    </CardWrapper>
  </div>

</template>

<style scoped>

.buttons {
  display: flex;
  flex-direction: row;
  gap: var(--padding-small);
}

.card-info-container {
  width: 100%;
}

.card-info-props {
  padding: var(--padding-large);
}

</style>