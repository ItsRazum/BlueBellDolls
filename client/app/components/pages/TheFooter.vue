<script setup lang="ts">
import { vMaska } from "maska/vue";

const { t } = useI18n();
const developerModal = useModal();
const config = useAppConfig();

const form = reactive({
  name: "",
  phone: "",
});

const isLoading = ref(false);
const statusMessage = ref<string | null>(null);
const isSuccess = ref(false);

const isValid = computed(() => form.name.length > 2 && form.phone.length >= 17);

const reset = () => {
  statusMessage.value = null;

  if (isSuccess.value) {
    form.name = "";
    form.phone = "";
    isSuccess.value = false;
  }
};

const feebackApi = useFeedbackRequestApi();

const submitFeedback = async () => {
  if (!isValid.value || isLoading.value) return;
  isLoading.value = true;

  const payload = ref<CreateFeedbackRequestDto>({
    name: form.name,
    phoneNumber: form.phone,
  });
  try {
    await feebackApi.sendRequest(payload.value);

    isSuccess.value = true;
    statusMessage.value = t("common.form.success");
  } catch (error) {
    isSuccess.value = false;
    statusMessage.value = error.data.message ?? 'Неизвестная ошибка';
  } finally {
    isLoading.value = false;
  }
};
</script>

<template>
  <div class="container">
    <div class="contacts-container">

      <div class="form-container relative-container">

        <StatusOverlay
            :message="statusMessage"
            :is-success="isSuccess"
            @close="reset"
            class="rounded-(--border-radius-main) m-(--padding-small)"
        />

        <span class="mb-[0.875rem] text-2xl font-medium">{{ $t('footer.questionTitle') }}</span>

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
            @click="submitFeedback"
            :disabled="!isValid || isLoading"
        >
          {{ isLoading ? $t('common.form.sending') : $t('common.form.send') }}
        </button>

        <p style="color: var(--color-text-context); white-space: pre-line">
          {{ $t("common.form.privacyText") }}
          <NuxtLinkLocale to="/privacy">{{ $t("common.form.privacyLink") }}</NuxtLinkLocale>
        </p>
      </div>

      <div class="separator vertical h-[15rem] m-[2rem]" />

      <div class="form-container">
        <span class="mb-[0.875rem] text-2xl font-medium">{{ $t("footer.orContact") }}</span>
        <p class="contacts text-xl">
          <b>
            {{ $t("common.contacts.phoneLabel") }}<br />
            <a :href="`tel:${config.contacts.phoneLink}`">{{ config.contacts.phoneDisplay }}</a> ({{
              $t("common.contacts.name")
            }})
            <br />
            <br />
            {{ $t("common.contacts.emailLabel") }}<br />
            <a :href="`mailto:${config.contacts.email}`">{{ config.contacts.email }}</a>
            <br />
            <br />
            {{ $t("common.contacts.telegramLabel") }}<br />
            <a :href="config.contacts.telegram">@elena_ragdoll_BlueBellDolls</a>
          </b>
        </p>
      </div>
    </div>

    <div class="footer-container">
      <div class="links-container">
        <div class="block-container">
          <NuxtLinkLocale to="/" class="link primary">BlueBellDolls</NuxtLinkLocale>
          <NuxtLinkLocale to="/litters" class="link secondary">{{ $t("footer.nav.kittens") }}</NuxtLinkLocale>
          <NuxtLinkLocale to="/cats/females" class="link secondary">{{ $t("footer.nav.femalecats") }}</NuxtLinkLocale>
          <NuxtLinkLocale to="/cats/males" class="link secondary">{{ $t("footer.nav.malecats") }}</NuxtLinkLocale>
          <NuxtLinkLocale to="/gallery" class="link secondary">{{ $t("footer.nav.gallery") }}</NuxtLinkLocale>
        </div>
        <div class="block-container">
          <NuxtLinkLocale to="/about" class="link primary">{{ $t("footer.about.aboutUs") }}</NuxtLinkLocale>
          <NuxtLinkLocale to="/history" class="link secondary">{{ $t("footer.about.ourStory") }}</NuxtLinkLocale>
          <NuxtLinkLocale to="/achievements" class="link secondary">{{ $t("footer.about.achievements") }}</NuxtLinkLocale>
          <NuxtLinkLocale to="/route" class="link secondary">{{ $t("footer.about.howToGetToUs") }}</NuxtLinkLocale>
        </div>
        <div class="block-container">
          <NuxtLinkLocale to="/about-ragdoll" class="link primary">{{ $t("footer.aboutRagdoll.ragdollBreed") }}</NuxtLinkLocale>
          <NuxtLinkLocale to="/ragdoll-history" class="link secondary">{{ $t("footer.aboutRagdoll.ragdollHistory") }}</NuxtLinkLocale>
          <NuxtLinkLocale to="/ragdoll-stats" class="link secondary">{{ $t("footer.aboutRagdoll.ragdollCharacteristics") }}</NuxtLinkLocale>
        </div>
        <div class="block-container">
          <span class="link primary">{{ $t("footer.privacy.title") }}</span>
          <NuxtLinkLocale to="/privacy" class="link secondary">{{ $t("footer.privacy.privacyPolicy") }}</NuxtLinkLocale>
          <NuxtLinkLocale to="/cookies" class="link secondary">Файлы Cookie</NuxtLinkLocale>
        </div>
        <div class="block-container">
          <span class="link primary">{{ $t("footer.socials") }}</span>
          <a class="link secondary" :href="config.socials.telegram">Telegram</a>
          <a class="link secondary" :href="config.socials.instagram">Instagram</a>
          <a class="link secondary" :href="config.socials.facebook">Facebook</a>
          <a class="link secondary" :href="config.socials.vk">VK</a>
        </div>
      </div>
      <div class="author-container">
        <p class="dev">
          {{ $t("footer.developedBy") }}
          <button @click="developerModal.open" class="link-btn dev-link">Demid Krisov</button>
        </p>
      </div>
    </div>
  </div>

  <DeveloperModal @close="developerModal.close" :is-open="developerModal.isOpen.value" />
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

.relative-container {
  position: relative;
  overflow: hidden;
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
  color: #899dd5;
}
</style>