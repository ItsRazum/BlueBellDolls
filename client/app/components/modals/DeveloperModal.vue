<script setup lang="ts">
const props = defineProps<{
  isOpen: boolean;
}>();

const emit = defineEmits(["close"]);

const close = () => {
  emit("close");
};
</script>

<template>
  <BaseModal :isOpen="isOpen" @close="close">
    <div class="modal-content">
      <div class="profile-section">
        <div class="avatar-placeholder">DK</div>
        <div class="profile-info">
          <h2 class="name">Крисов Демид</h2>
          <span class="role">Fullstack Developer & UI/UX</span>

          <div class="stack-badges">
            <span class="badge">Nuxt 3</span>
            <span class="badge">Vue</span>
            <span class="badge">.NET</span>
          </div>
        </div>
      </div>

      <div class="bio-text">
        <p>
          Сайт <b>BlueBellDolls</b> - пример моего подхода к разработке. Весь проект реализован мной
          в одиночку: от создания дизайн-макетов до архитектуры сервера и сборки всей системы.
        </p>
        <p class="sub-text">
          Создаю современные веб-сервисы, которые помогают бизнесу расти. Хотите похожий сайт или
          сложную систему? Пишите, обсудим детали :)
        </p>
      </div>

      <div class="actions-container">
        <a href="https://t.me/NtRazum" target="_blank" class="action-btn telegram">
          <span>Написать в Telegram</span>
          <svg
            width="20"
            height="20"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
          >
            <line x1="22" y1="2" x2="11" y2="13"></line>
            <polygon points="22 2 15 22 11 13 2 9 22 2"></polygon>
          </svg>
        </a>

        <a href="mailto:krisov.demid@gmail.com" class="action-btn email">
          <span>krisov.demid@gmail.com</span>
          <svg
            width="20"
            height="20"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
          >
            <path
              d="M4 4h16c1.1 0 2 .9 2 2v12c0 1.1-.9 2-2 2H4c-1.1 0-2-.9-2-2V6c0-1.1.9-2 2-2z"
            ></path>
            <polyline points="22,6 12,13 2,6"></polyline>
          </svg>
        </a>
      </div>
    </div>
  </BaseModal>
</template>

<style scoped>
.modal-content {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
  max-width: 32rem;
}

.profile-section {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.avatar-placeholder {
  width: 4rem;
  height: 4rem;
  background: linear-gradient(135deg, #60a5fa, #a78bfa);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  font-size: 1.5rem;
  color: white;
  flex-shrink: 0;
  font-family: var(--font-family-base);
}

.profile-info {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.name {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 700;
  color: var(--color-text-base);
}

.role {
  font-size: 0.9rem;
  color: var(--color-text-context);
}

.stack-badges {
  display: flex;
  gap: 0.5rem;
  margin-top: 0.25rem;
}

.badge {
  font-size: 0.75rem;
  padding: 0.25rem 0.5rem;
  border-radius: 6px;
  background-color: rgba(255, 255, 255, 0.1);
  color: var(--color-text-base);
  font-weight: 500;
}

.bio-text {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.bio-text p {
  margin: 0;
  font-size: 1.125rem;
  line-height: 1.5;
  color: var(--color-text-base);
}

.bio-text strong {
  color: var(--color-text-light);
}

.sub-text {
  font-size: 1rem !important;
  color: var(--color-text-context) !important;
}

.actions-container {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.action-btn {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 1.25rem;
  border-radius: var(--border-radius-main);
  text-decoration: none;
  font-weight: 600;
  transition:
    transform 0.2s,
    opacity 0.2s,
    background-color 0.2s;
}

.action-btn:active {
  transform: scale(0.98);
}

.telegram {
  background-color: #229ed9;
  color: white;
  border: none;
}

.telegram:hover {
  background-color: #1e88e5;
}

.email {
  background-color: var(--color-background-card);
  border: 1px solid var(--color-border-card);
  color: var(--color-text-base);
}

.email:hover {
  background-color: var(--color-hover);
}
</style>
