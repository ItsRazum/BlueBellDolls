<script setup lang="ts">
const props = defineProps<{
  currentPage: number;
  total: number;
  url: string;
}>();

const pages = computed(() => {
  const result = [];
  for (let page = 1; page <= props.total; page++) {
    result.push(page);
  }
  return result;
});

const prevButton = computed(() => {
  const isDisabled = props.currentPage <= 1;
  return {
    tag: isDisabled ? "button" : "NuxtLink",
    props: isDisabled
      ? { disabled: true, class: "nav-btn arrow-btn" }
      : { to: getLink(props.currentPage - 1), class: "nav-btn arrow-btn" },
  };
});

const nextButton = computed(() => {
  const isDisabled = props.currentPage >= props.total;
  return {
    tag: isDisabled ? "button" : "NuxtLink",
    props: isDisabled
      ? { disabled: true, class: "nav-btn arrow-btn" }
      : { to: getLink(props.currentPage + 1), class: "nav-btn arrow-btn" },
  };
});

const getLink = (page: number) => `${props.url}?page=${page}`;
</script>

<template>
  <CardWrapper class="controls-container">
    <component :is="prevButton.tag" v-bind="prevButton.props">
      <svg width="24" height="24" viewBox="0 0 24 24" fill="none">
        <path
          d="M15 19L8 12L15 5"
          stroke="currentColor"
          stroke-width="2"
          stroke-linecap="round"
          stroke-linejoin="round"
        />
      </svg>
    </component>
    <div v-for="page in pages" :key="page" class="pages-container">
      <span class="nav-btn page-number current" v-if="currentPage == page">{{ page }}</span>
      <NuxtLinkLocale class="nav-btn page-number" v-else :to="getLink(page)">{{
        page
      }}</NuxtLinkLocale>
    </div>
    <component :is="nextButton.tag" v-bind="nextButton.props">
      <svg width="24" height="24" viewBox="0 0 24 24" fill="none">
        <path
          d="M9 19L16 12L9 5"
          stroke="currentColor"
          stroke-width="2"
          stroke-linecap="round"
          stroke-linejoin="round"
        />
      </svg>
    </component>
  </CardWrapper>
</template>

<style scoped>
.nav-btn {
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0.8rem 0.725rem;
  border-radius: calc(var(--border-radius-main) - 0.25rem);
  transition:
    opacity 0.3s ease,
    background-color 0.2s;
  font-size: 1.25rem;
  font-family: var(--font-family-base);
  font-weight: 600;
  text-decoration: none;
  line-height: 1;
}

.nav-btn:hover {
  background-color: var(--color-hover);
}

.nav-btn:disabled,
.nav-btn[disabled] {
  background-color: transparent;
  box-shadow: none;
  opacity: 0.5 !important;
  cursor: default;
  pointer-events: none;
}

.controls-container {
  display: flex;
  width: min-content;
  gap: 4px;
  padding: 0.25rem;
}

.pages-container {
  display: flex;
  gap: 4px;
}

.nav-btn.page-number {
  padding: 0.8rem 0.725rem;
  font-size: 1.25rem;
  color: var(--color-text-context);
}

.nav-btn.arrow-btn {
  color: var(--color-text-base);
  padding: 0.6rem 0.225rem;
}

.nav-btn:not(:disabled):not(.current):hover {
  background-color: var(--color-hover);
}

.nav-btn.current {
  cursor: default;
  pointer-events: none;
  color: var(--color-context-blue);
  background-color: var(--color-background-blue);
}

.nav-btn:disabled {
  opacity: 0.5;
  cursor: default;
  pointer-events: none;
}
</style>
