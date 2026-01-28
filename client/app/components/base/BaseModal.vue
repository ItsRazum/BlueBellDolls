<script setup lang="ts">
import { watch, ref } from "vue";

const props = defineProps<{
  isOpen: boolean;
  copyUrl?: string;
}>();

const emit = defineEmits(["close"]);
const year = new Date().getFullYear();

const { lock, unlock } = useBodyScrollLock();

const isMouseDownOnBackdrop = ref(false);

const isCopied = ref(false);

const copyLink = async () => {
  try {
    await navigator.clipboard.writeText(window.location.origin + props.copyUrl);

    isCopied.value = true;

    setTimeout(() => {
      isCopied.value = false;
    }, 2000);
  } catch (err) {
    console.error("Could not copy link!", err);
  }
};

const handleMouseDown = (event: MouseEvent) => {
  isMouseDownOnBackdrop.value = event.target === event.currentTarget;
};

const handleMouseUp = (event: MouseEvent) => {
  if (!isMouseDownOnBackdrop.value) return;

  if (event.target === event.currentTarget) {
    emit("close");
  }

  isMouseDownOnBackdrop.value = false;
};

watch(
  () => props.isOpen,
  (newValue) => {
    if (newValue) {
      lock();
    }
  },
);

const close = () => {
  emit("close");
};
</script>

<template>
  <Teleport to="body">
    <Transition
      name="modal-fade"
      class="modal-backdrop"
      @after-leave="unlock"
      @mousedown="handleMouseDown"
      @mouseup="handleMouseUp"
    >
      <div v-if="isOpen" class="modal-backdrop">
        <div class="copied" :class="{ show: isCopied }">
          <div class="copied-inner">
            <SvgoSuccess class="copied-icon" />
            <span class="text-white">{{ $t("components.modals.linkCopied") }}</span>
          </div>
        </div>

        <CardWrapper :enable-blur="true" class="modal-container">
          <div class="modal-toolbar">
            <div class="tooltip-wrapper" v-if="copyUrl != null">
              <button class="close-btn" @click="copyLink">
                <SvgoShare />
              </button>
              <span class="tooltip">{{ $t("components.modals.copyLink") }}</span>
            </div>
            <div class="tooltip-wrapper">
              <button class="close-btn" @click="close">
                <SvgoClose />
              </button>
              <span class="tooltip">{{ $t("components.modals.close") }}</span>
            </div>
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
  align-items: center;
  gap: var(--padding-small);
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

.close-btn:disabled {
  opacity: 0.6;
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

.copied {
  position: fixed;
  top: 2rem;
  left: 50%;
  transform: translateX(-50%) translateY(-20px);

  border-radius: var(--border-radius-main);
  background-color: var(--color-tooltip-background);
  color: #fff;
  padding: var(--padding-large);

  opacity: 0;
  pointer-events: none;
  z-index: 1001;

  transition: all 0.3s cubic-bezier(0.16, 1, 0.3, 1);
}

.copied.show {
  opacity: 1;
  transform: translateX(-50%) translateY(0);
}

.copied-inner {
  display: flex;
  gap: var(--padding-small);
  align-items: center;
}

.copied-icon {
  color: var(--color-status-available);
  width: 28px;
  height: 28px;
  margin: 0
}
</style>
