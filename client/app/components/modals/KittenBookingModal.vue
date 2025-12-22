<script setup lang="ts">

import { vMaska } from "maska/vue";

const props = defineProps<{
  kitten: KittenListDto;
  isOpen: boolean;
}>();

const form = reactive({
  name: '',
  phone: ''
});

const isValid = computed(() => {
  return form.name.length > 2 && form.phone.length > 10;
});

const emit = defineEmits(['close']);

const close = () => {
  emit('close');
};

</script>

<template>
  <BaseModal :isOpen="props.isOpen" @close="close">
    <div class="container">
      <KittenListItem :kitten="kitten" :read-only="true" />
      <CardWrapper class="booking-container">
        <span class="tip-text">Отличный выбор! Чтобы забронировать котёнка - оставьте заявку по форме ниже, или позвоните нам.</span>
        <div class="booking-cards">
          <CardWrapper class="form-container">
            <h2 style="margin-bottom: 0.875rem">
              Вы бронируете котёнка <span style="color: var(--color-context-blue)">{{kitten.name}}</span>
            </h2>
            <input v-model="form.name" placeholder="Имя" />
            <input v-model="form.phone" placeholder="Номер телефона" v-maska="'+7(###) ###-##-##'"/>
            <button style="width: max-content">Отправить</button>
            <p style="color: var(--color-text-context)">
              Нажимая кнопку <b>«Отправить»</b>,<br>Вы соглашаетесь с нашей <RouterLink to="/privacy">Политикой конфидециальности</RouterLink>
            </p>
          </CardWrapper>
          <CardWrapper class="form-container">
            <p class="contacts">
              <b>
                Номер телефона:<br>
                <a href="tel:+79166468510">+7(916)646-85-10</a> (Елена)
                <br>
                <br>
                Эл. почта:<br>
                <a href="mailto:ragdoll-bluebelldolls@mail.ru">ragdoll-bluebelldolls@mail.ru</a>
              </b>

            </p>
          </CardWrapper>
        </div>

      </CardWrapper>
    </div>
  </BaseModal>
</template>

<style scoped>

.container {
  max-width: 55rem;
  display: flex;
  flex-direction: column;
  gap: var(--padding-large);
}

.booking-container {
  display: flex;
  flex-direction: column;
  padding: var(--padding-large);
  gap: var(--padding-large);
}

.booking-cards {
  display: flex;
  flex-direction: row;
  justify-content: space-between;
  align-items: flex-start;
  gap: var(--padding-small);
}

.tip-text {
  font-weight: 525;
  color: var(--color-text-caption);
}

.form-container {
  display: flex;
  flex-direction: column;
  padding: var(--padding-large);
  gap: var(--padding-small);
}

.contacts {
  font-size: 1.125rem;
}

</style>