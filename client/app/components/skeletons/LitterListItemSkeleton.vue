<script setup lang="ts">

import Skeleton from "~/components/Skeleton.vue";

</script>

<template>
  <div style="display: flex; width: 100%;">
    <CardWrapper class="litter-card">
      <div class="card-expanded">
        <div class="card-photo-container">
          <Skeleton
              class="card-expanded-photo"
              style="width: 18.5rem; height: 18.5rem; margin-bottom: 0; border-radius: 1rem"
          />
          <Skeleton width="9rem" />
        </div>

        <CardWrapper :enable-blur="true" :show-border="false" class="card-info-container">

          <div class="card-header">
            <Skeleton width="40%" height="2rem" radius="8px" />
            <Skeleton width="20%" height="1.5rem" />
          </div>

          <div class="card-info-body">
            <CardWrapper class="card-info-props" :show-border="false">
              <div style="display: flex; flex-direction: column; gap: 0.5rem;">
                <Skeleton width="20%" height="1.2rem" />
                <Skeleton width="30%" height="1.2rem" />
                <Skeleton width="15%" height="1.2rem" />
                <Skeleton width="20%" height="1.2rem" />
              </div>

              <Skeleton width="90%" height="0.875rem" style="margin-top: 1rem;" />
              <Skeleton width="60%" height="0.875rem" />
            </CardWrapper>
          </div>

        </CardWrapper>
      </div>
      <div class="separator"/>
      <div class="kittens-grid">
        <KittenListItemSkeleton />
        <KittenListItemSkeleton />
      </div>
    </CardWrapper>
  </div>


</template>

<style scoped>

.litter-card {
  width: 100%;
  padding: var(--padding-large);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: var(--padding-extra-large);
}

.card-info-container {
  width: 100%;
}

.card-info-props {
  padding: var(--padding-large);
}

.kittens-grid {
  display: flex;
  flex-direction: column;
  gap: var(--padding-extra-large);
  margin-top: 20px;
  width: 100%;
}

</style>