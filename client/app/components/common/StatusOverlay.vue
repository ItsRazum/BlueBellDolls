<script setup lang="ts">

const props = defineProps<{
  message: string | null;
  isSuccess: boolean;
  successTitle?: string;
  errorTitle?: string;
}>();

const emit = defineEmits(['close']);

const { t } = useI18n();

const title = computed(() => {
  return props.isSuccess
      ? props.successTitle || t('common.thankYou')
      : props.errorTitle || t('common.error');
});
</script>

<template>
  <transition name="fade">
    <div
        v-if="message"
        class="status-overlay"
        :class="{ success: isSuccess, error: !isSuccess }"
        @click="emit('close')"
    >
      <div class="toolbar">
        <button class="close-btn" v-if="!isSuccess" @click.stop="emit('close')">
          <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M18 6L6 18" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
            <path d="M6 6L18 18" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
        </button>
      </div>

      <div class="overlay-content">
        <div class="icon-circle">
          <span v-if="isSuccess">✔</span>
          <span v-else>✖</span>
        </div>

        <h3>{{ title }}</h3>
        <p>{{ message }}</p>
      </div>
    </div>
  </transition>
</template>

<style scoped>
.status-overlay {
  position: absolute;
  inset: 0;
  z-index: 10;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  backdrop-filter: blur(4px);
  padding: 2rem;
  text-align: center;
}

.status-overlay.success {
  border: 2px solid var(--color-success-overlay-stroke);
  background-color: var(--color-success-overlay-background);
}

.status-overlay.error {
  border: 2px solid var(--color-error-overlay-stroke);
  background-color: var(--color-error-overlay-background);
}

.toolbar {
  position: absolute;
  top: 1rem;
  right: 1rem;
}

.close-btn {
  background: none;
  border: none;
  cursor: pointer;
  color: var(--color-text-base);
  padding: 4px;
}

.overlay-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
}

.icon-circle {
  font-size: 2rem;
  width: 3rem;
  height: 3rem;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
}

.success .icon-circle {
  background: #7cdf9e;
}
.error .icon-circle {
  background: #ef5858;
}

.fade-enter-active, .fade-leave-active { transition: opacity 0.3s ease; }
.fade-enter-from, .fade-leave-to { opacity: 0; }
</style>