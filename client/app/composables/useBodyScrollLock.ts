export const useBodyScrollLock = () => {
  const openedModalsCount = useState<number>("modals-count", () => 0);
  const isLocked = computed(() => openedModalsCount.value > 0);

  const lock = () => {
    openedModalsCount.value++;

    if (openedModalsCount.value === 1 && typeof document !== "undefined") {
      const scrollbarWidth = window.innerWidth - document.documentElement.clientWidth;

      if (scrollbarWidth > 0) {
        document.body.style.setProperty("--scrollbar-width", `${scrollbarWidth}px`);
        document.body.style.paddingRight = `${scrollbarWidth}px`;
      }
      document.body.classList.add("scroll-locked");
      document.body.style.overflow = "hidden";
    }
  };

  const unlock = () => {
    openedModalsCount.value--;
    if (openedModalsCount.value < 0) openedModalsCount.value = 0;

    if (openedModalsCount.value === 0 && typeof document !== "undefined") {
      document.body.style.overflow = "";
      document.body.style.paddingRight = "";

      document.body.style.removeProperty("--scrollbar-width");
    }
  };

  return {
    lock,
    unlock,
    isLocked,
  };
};
