<script setup lang="ts">
import { vMaska } from "maska/vue";

const props = defineProps<{
  kitten: KittenListDto;
  isOpen: boolean;
}>();

const config = useAppConfig();

const { t } = useI18n();

const form = reactive({
  name: "",
  phone: "",
});

const isLoading = ref(false);
const statusMessage = ref<string | null>(null);
const isSuccess = ref(false);

const isValid = computed(() => {
  return form.name.length > 2 && form.phone.length >= 17;
});

const kittenApi = useKittenApi();

const submitBooking = async () => {
  if (!isValid.value || isLoading.value) return;

  isLoading.value = true;
  statusMessage.value = null;
  isSuccess.value = false;

  const payload: CreateBookingRequestDto = {
    name: form.name,
    phoneNumber: form.phone,
    kittenId: props.kitten.id,
  };

  try {
    await new Promise((resolve) => setTimeout(resolve, 1500));
    //await kittenApi.bookKitten(payload);
    isSuccess.value = true;
    statusMessage.value = `${t("common.form.success")}\n${t("components.modals.kittenBooking.youCanCloseThisWindow")}`;

    form.name = "";
    form.phone = "";
  } catch (error: any) {
    isSuccess.value = false;

    statusMessage.value = error.data.message;
  } finally {
    isLoading.value = false;
  }
};

const emit = defineEmits(["close"]);

const close = () => {
  if (!isSuccess.value) reset();
  emit("close");
};

const reset = () => {
  if (!isSuccess.value) statusMessage.value = "";
};
</script>

<template>
  <BaseModal :isOpen="props.isOpen" @close="close">
    <div class="container">
      <KittenListItem :kitten="kitten" :read-only="true" />
      <CardWrapper class="booking-container">
        <span class="tip-text">{{ $t("components.modals.kittenBooking.requestInstruction") }}</span>
        <div class="booking-cards">
          <CardWrapper class="form-container relative-container" :show-border="false">
            <StatusOverlay
                :message="statusMessage"
                :is-success="isSuccess"
                @close="reset"
                style="border: none"
            />
            <h2 style="margin-bottom: 0.875rem">
              {{ $t("components.modals.kittenBooking.youBookingKitten") }}
              <span style="color: var(--color-context-blue)">{{ kitten.name }}</span>
            </h2>
            <input
              v-model="form.name"
              :placeholder="$t('common.form.namePlaceholder')"
              :disabled="isLoading"
            />
            <input
              v-model="form.phone"
              :placeholder="$t('common.form.phonePlaceholder')"
              v-maska="'+7(###) ###-##-##'"
              :disabled="isLoading"
            />

            <button
              style="width: max-content"
              @click="submitBooking"
              :disabled="!isValid || isLoading"
            >
              {{ isLoading ? $t("common.form.sending") : $t("common.form.send") }}
            </button>

            <p style="color: var(--color-text-context)">
              {{ $t("common.form.privacyText") }}
              <NuxtLinkLocale to="/privacy">{{ $t("common.form.privacyLink") }}</NuxtLinkLocale>
            </p>
          </CardWrapper>
          <CardWrapper class="form-container" :show-border="false">
            <p class="contacts">
              <b>
                {{ $t("common.contacts.phoneLabel") }}<br />
                <a :href="`tel:${config.contacts.phoneLink}`">{{ config.contacts.phoneDisplay }}</a>
                ({{ $t("common.contacts.name") }})
                <br />
                <br />
                {{ $t("common.contacts.emailLabel") }}<br />
                <a class="w-full" :href="`mailto:${config.contacts.email}`">{{
                  config.contacts.email
                }}</a>
                <br />
                <br />
                {{ $t("common.contacts.telegramLabel") }}<br />
                <a :href="`mailto:${config.contacts.telegram}`">@elena_ragdoll_BlueBellDolls</a>
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

.relative-container {
  position: relative;
  overflow: hidden;
}

.overlay-content h3 {
  margin: 0;
  font-size: 1.25rem;
}

</style>
