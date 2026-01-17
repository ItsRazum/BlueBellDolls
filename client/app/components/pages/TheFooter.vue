<script setup lang="ts">

const form = reactive({
  name: '',
  phone: ''
});

const isDeveloperModalOpen = ref(false);
const openDeveloperModal = () => {
  isDeveloperModalOpen.value = true;
}

const closeDeveloperModal = () => {
  isDeveloperModalOpen.value = false;
}

</script>

<template>
  <div class="container">
    <div class="contacts-container">
      <div class="form-container">
        <span class="mb-[0.875rem] text-2xl font-medium">Остались вопросы? Мы готовы вам перезвонить.</span>
        <input v-model="form.name" placeholder="Имя" />
        <input v-model="form.phone" placeholder="Номер телефона" v-maska="'+7(###) ###-##-##'"/>
        <button style="width: max-content">Отправить</button>
        <p style="color: var(--color-text-context)">
          Нажимая кнопку <b>«Отправить»</b>,<br>Вы соглашаетесь с нашей <RouterLink to="/privacy">Политикой конфиденциальности</RouterLink>
        </p>
      </div>
      <div class="separator vertical h-[15rem] m-[2rem]"/>
      <div class="form-container">
        <span class="mb-[0.875rem] text-2xl font-medium">Или свяжитесь с нами самостоятельно.</span>
        <p class="contacts text-xl">
          <b>
            Номер телефона:<br>
            <a href="tel:+79166468510">+7(916)646-85-10</a> (Елена)
            <br>
            <br>
            Эл. почта:<br>
            <a href="mailto:ragdoll-bluebelldolls@mail.ru">ragdoll-bluebelldolls@mail.ru</a>
          </b>
        </p>
      </div>
    </div>
    <div class="footer-container">
      <div class="links-container">
        <div class="block-container">
          <NuxtLink to="/" class="link primary">BlueBellDolls</NuxtLink>
          <NuxtLink to="/litters" class="link secondary">Котята</NuxtLink>
          <NuxtLink to="/parentcats?isMale=false" class="link secondary">Кошки</NuxtLink>
          <NuxtLink to="/parentcats?isMale=true" class="link secondary">Коты</NuxtLink>
          <NuxtLink to="/gallery" class="link secondary">Галерея</NuxtLink>
        </div>
        <div class="block-container">
          <NuxtLink to="/about" class="link primary">О нас</NuxtLink>
          <NuxtLink to="/history" class="link secondary">История</NuxtLink>
          <NuxtLink to="/achievements" class="link secondary">Достижения</NuxtLink>
          <NuxtLink to="/route" class="link secondary">Как добраться</NuxtLink>
        </div>
        <div class="block-container">
          <NuxtLink to="/about-ragdoll" class="link primary">Порода Рэгдолл</NuxtLink>
          <NuxtLink to="/ragdoll-history" class="link secondary">История</NuxtLink>
          <NuxtLink to="/ragdoll-stats" class="link secondary">Характеристика</NuxtLink>
        </div>
        <div class="block-container">
          <span class="link primary">Приватность</span>
          <NuxtLink to="/privacy" class="link secondary">Политика конфиденциальности</NuxtLink>
          <NuxtLink to="/cookies" class="link secondary">Файлы Cookie</NuxtLink>
        </div>
        <div class="block-container">
          <span class="link primary">Соц. сети</span>
          <a class="link secondary">Telegram</a>
          <a class="link secondary">Instagram</a>
          <a class="link secondary">Facebook</a>
          <a class="link secondary">VK</a>
        </div>
      </div>
      <div class="author-container">
        <p class="dev">Разработка сайта: <button @click="openDeveloperModal" class="link-btn dev-link">Demid Krisov</button></p>
      </div>
    </div>
  </div>


  <DeveloperModal @close="closeDeveloperModal" :is-open="isDeveloperModalOpen" />
</template>

<style scoped>

.container {
  display: flex;
  flex-direction: column;
  border-inline: var(--border-global);
  width: 100%;
  max-width: 1280px;
}

.contacts-container {
  display: flex;
  background-color: var(--color-pages-primary-background);
}

.footer-container {
  display: flex;
  flex-direction: column;
  padding: 1.25rem 5.5rem;
  box-sizing: border-box;
  gap: 5rem;
  background-color: var(--color-dark-blue);
}

.form-container {
  display: flex;
  flex-direction: column;
  padding: var(--padding-large);
  gap: var(--padding-small);
}

a {
  cursor: pointer;
}

span {
  cursor: unset;
}

a:hover {
  text-decoration: underline;
}

.links-container {
  display: flex;
  width: 100%;
  justify-content: space-between;
}

.block-container {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.link {
  text-decoration: none;
}

.link.primary {
  color: white;
  font-size: 1.5rem;
  font-weight: bold;
}

.link.secondary {
  color: rgba(255, 255, 255, 0.7);
  font-size: 1rem;
}

.author-container {
  width: 100%;
  display: flex;
  justify-content: flex-end;
}

.dev {
  color: rgba(255, 255, 255, 0.7);
}

.dev-link {
  color: #899DD5;
}

</style>