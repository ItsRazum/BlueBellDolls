<script setup lang="ts">
import { watch, onUnmounted } from 'vue';

const props = defineProps<{
  isOpen: boolean;
}>();

const emit = defineEmits(['close']);
const year = new Date().getFullYear();

const lockScroll = () => {
  if (typeof document !== 'undefined') {
    document.body.style.overflow = 'hidden';
  }
};

const unlockScroll = () => {
  if (typeof document !== 'undefined') {
    document.body.style.overflow = '';
  }
};

const isMouseDownOnBackdrop = ref(false);

const handleMouseDown = (event: MouseEvent) => {
  if (event.target === event.currentTarget) {
    isMouseDownOnBackdrop.value = true;
  } else {
    isMouseDownOnBackdrop.value = false;
  }
};

const handleMouseUp = (event: MouseEvent) => {
  if (!isMouseDownOnBackdrop.value) return;

  if (event.target === event.currentTarget) {
    emit('close');
  }

  isMouseDownOnBackdrop.value = false;
};

watch(() => props.isOpen, (newValue) => {
  if (newValue) {
    lockScroll();
  } else {
    unlockScroll();
  }
});

onUnmounted(() => {
  unlockScroll();
});

const close = () => {
  emit('close');
};
</script>

<template>
  <Teleport to="body">
    <Transition name="modal-fade"
                class="modal-backdrop"
                @mousedown="handleMouseDown"
                @mouseup="handleMouseUp">
      <div v-if="isOpen" class="modal-backdrop">
        <CardWrapper :enable-blur="true" class="modal-container">
          <div class="modal-toolbar">
            <button class="close-btn" @click="close">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M18 6L6 18" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
                <path d="M6 6L18 18" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
              </svg>
            </button>
          </div>
          <slot />
          <span class="copyright">©{{ year }} BlueBellDolls Cattery.</span>
        </CardWrapper>
      </div>
    </Transition>
</Teleport>
</template>

<style scoped>

.modal-backdrop {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
  padding: var(--padding-large);
}

.modal-container {
  display: flex;
  flex-direction: column;
  justify-content: center;
  padding: var(--padding-large);
  gap: var(--padding-large);
}

.modal-toolbar {
  display: flex;
  justify-content: right;
}

.close-btn {
  all: unset;
  background: transparent;
  border: none;
  cursor: pointer;
  display: flex;
  justify-content: center;
  align-items: center;
  color: var(--color-text-base);
}

.copyright {
  color: var(--color-text-caption);
  font-weight: 500;
  text-align: center;
}

.modal-fade-enter-from,
.modal-fade-leave-to {
  opacity: 0;
}

.modal-fade-enter-from .modal-container,
.modal-fade-leave-to .modal-container {
  transform: translateY(20px) scale(0.95);
  opacity: 0;
}


.modal-fade-enter-active,
.modal-fade-leave-active {
  transition: opacity 0.3s ease;
}

.modal-fade-enter-active .modal-container,
.modal-fade-leave-active .modal-container {
  transition: all 0.3s ease-out;
}

</style>