<template>
  <div class="card-expanded">
    <div class="card-photo-container">
      <Skeleton
        class="card-expanded-photo"
        style="width: 16rem; height: 16rem; margin-bottom: 0; border-radius: 1rem"
      />
      <Skeleton width="9rem" />
    </div>

    <CardWrapper :enable-blur="true" :show-border="false" class="card-info-container w-full">
      <div class="card-header">
        <Skeleton width="40%" height="1.5rem" radius="8px" />
        <Skeleton width="20%" height="0.8rem" />
      </div>

      <Skeleton width="90%" height="0.7rem" class="mt-[1rem]" />
      <Skeleton width="60%" height="0.7rem" />

      <div class="buttons-container gap-(--padding-small)">
        <Skeleton width="8rem" height="2.5rem" radius="1rem" />
      </div>
    </CardWrapper>
  </div>
</template>
