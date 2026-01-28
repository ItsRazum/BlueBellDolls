<script setup lang="ts">
const props = withDefaults(
  defineProps<{
    parentCat: ParentCatListDto;
    variant?: "default" | "compact";
  }>(),
  {
    variant: "default",
  },
);

const { locale } = useI18n();

const config = useRuntimeConfig();
const apiBaseUrl = config.public.apiBase;

const genderVariants = computed(() =>
  props.variant == "default" ? ["Мужской", "Женский"] : ["Мальчик", "Девочка"],
);
const genderText = computed(() =>
  props.parentCat.isMale ? genderVariants.value[0] : genderVariants.value[1],
);
const genderColor = computed(() =>
  props.parentCat.isMale ? "var(--color-gender-male)" : "var(--color-gender-female)",
);

const parentCatModal = useModal();

const catColorModal = useModal();
</script>

<template>
  <CardWrapper v-if="variant === 'compact'" class="card-compact">
    <div class="card-compact-content">
      <img
        class="card-compact-photo"
        :src="apiBaseUrl + parentCat.mainPhotoUrl"
        :alt="parentCat.name"
      />
      <h2>{{ parentCat.name }}</h2>
      <div class="subtitle">
        <span class="font-medium">{{ parentCat.birthDay }}</span>
        <span> • </span>
        <span :style="{ color: genderColor }">{{ genderText }}</span>
      </div>
      <CardWrapper :show-border="false" v-if="parentCat.description" class="p-(--padding-small)">
        <span class="description">{{ parentCat.description.slice(0, 70) }}</span>
      </CardWrapper>
      <div>
        <button @click="parentCatModal.open">Подробнее</button>
      </div>
    </div>
  </CardWrapper>

  <div v-else class="card-expanded">
    <div class="card-photo-container">
      <img
        class="card-expanded-photo"
        :src="apiBaseUrl + parentCat.mainPhotoUrl"
        :alt="parentCat.name"
      />
      <button class="link-btn" @click="parentCatModal.open">
        {{ $t("components.common.morePhotos") }}
      </button>
    </div>
    <CardWrapper :enable-blur="true" :show-border="false" class="card-info-container w-full">
      <div class="card-header">
        <h2 class="m-0">{{ parentCat.name }}</h2>
        <span class="font-medium text-(--color-text-caption)">{{ parentCat.birthDay }}</span>
      </div>
      <div class="card-info-body">
        <CardWrapper class="card-info-props p-(--padding-large)" :show-border="false">
          <div class="card-property">
            <span>{{ $t("components.common.cats.gender") }}: </span>
            <span :style="{ color: genderColor }">{{ genderText }}</span>
          </div>
          <div class="card-property">
            <span>{{ $t("components.common.cats.color") }}: </span>
            <button class="link-btn" @click="catColorModal.open">{{ parentCat.catColor }}</button>
          </div>
          <span v-if="locale == 'ru'" class="mt-[1rem]"> {{ parentCat.description }}</span>
        </CardWrapper>
      </div>

      <div class="buttons-container">
        <button @click="parentCatModal.open">Подробнее</button>
      </div>

      <ParentCatModal
        :is-open="parentCatModal.isOpen.value"
        @close="parentCatModal.close"
        :parent-cat-id="parentCat.id"
      />
      <CatColorModal
        v-if="parentCat.catColor"
        :is-open="catColorModal.isOpen.value"
        @close="catColorModal.close"
        :cat-color-id="parentCat.catColor.id"
      />
    </CardWrapper>
  </div>
</template>
