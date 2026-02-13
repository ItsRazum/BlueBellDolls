<script setup lang="ts">
definePageMeta({
  title: "pages.main.title",
  backgroundImage: "/header.jpg",
  buttonText: "pages.main.buttonText",
  buttonUrl: "/litters",
});

const kittenApi = useKittenApi();
const { data: kittensResponse, pending } = await kittenApi.getAvailableKittens();

const kittens = computed(() => kittensResponse.value || []);
</script>

<template>
  <div class="page-block primary flex-row articles-container">
    <Article
      :title="$t('pages.main.articles.aboutRagdoll.title')"
      photo-url="photo.png"
      :description="$t('pages.main.articles.aboutRagdoll.description')"
      redirect-url="/about-ragdoll"
    />
    <Article
      :title="$t('pages.main.articles.availableKittens.title')"
      photo-url="photo.png"
      :description="$t('pages.main.articles.availableKittens.description')"
      redirect-url="/litters"
    />
    <Article
      :title="$t('pages.main.articles.aboutUs.title')"
      photo-url="photo.png"
      :description="$t('pages.main.articles.aboutUs.description')"
      redirect-url="/about"
    />
  </div>
  <div class="page-block secondary">
    <MessageBox imageUrl="/photo.png" :text="$t('pages.main.welcomeMessage')" />
  </div>
  <div v-if="pending" class="kittens-container page-block primary">
    <h2 class="text-3xl">{{ $t("pages.main.kittensForSale") }}</h2>
    <div class="carousel-wrapper">
      <KittenListItemSkeleton variant="compact" />
      <KittenListItemSkeleton variant="compact" />
      <KittenListItemSkeleton variant="compact" />
    </div>
  </div>
  <div v-else-if="kittens && kittens.length && kittens.length === 0" class="kittens-container page-block primary">
    <div class="no-kittens-container">
      <span class="font-bold text-3xl text-white">{{ $t("pages.main.kittensWillBeSoon") }}</span>
      <span class="text-2xl text-white text-center">{{
        $t("pages.main.stayTunedForNewKittens")
      }}</span>
    </div>
  </div>
  <div v-else-if="kittens && kittens.length && kittens.length < 3" class="kittens-container">
    <h2 class="text-3xl">{{ $t("pages.main.kittensForSale") }}</h2>
    <div class="carousel-wrapper">
      <KittenListItem
        variant="compact"
        v-for="kitten in kittens"
        :key="kitten.id"
        :kitten="kitten"
      />
    </div>
    <NuxtLinkLocale to="litters" class="text-[1.25rem]">{{
      $t("pages.main.showMoreKittens")
    }}</NuxtLinkLocale>
  </div>
  <div v-else class="kittens-container page-block primary">
    <h2 class="kittens-title">{{ $t("pages.main.kittensForSale") }}</h2>
    <KittensCarousel :kittens="kittens" />
    <NuxtLinkLocale to="litters" class="text-[1.25rem]">{{
      $t("pages.main.showMoreKittens")
    }}</NuxtLinkLocale>
  </div>
  <div class="page-block secondary">
    <span class="kittens-title">{{ $t("pages.main.ourCatsDescription") }}</span>
    <div class="cats-articles-container">
      <Article
        :title="$t('pages.main.articles.ourFemaleCats.title')"
        photo-url="photo.png"
        :description="$t('pages.main.articles.ourFemaleCats.description')"
        redirect-url="/cats/females"
      />
      <Article
        :title="$t('pages.main.articles.ourMaleCats.title')"
        photo-url="photo.png"
        :description="$t('pages.main.articles.ourMaleCats.description')"
        redirect-url="/cats/males"
      />
    </div>
  </div>
</template>

<style scoped>
.articles-container {
  padding: var(--padding-pages);
}

.kittens-container {
  padding: var(--padding-large) var(--padding-extra-large);
}

.no-kittens-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: space-between;
  width: 48.75rem;
  height: 19rem;
  padding: var(--padding-large);
  border-radius: var(--border-radius-main);
  background-image: url("/photo.png");
  background-size: cover;
}

.carousel-wrapper {
  display: flex;
  align-items: center;
  gap: var(--padding-extra-large);
  justify-content: center;
  width: 100%;
  max-width: 1150px;
  margin: 0 auto;
}

.cats-articles-container {
  display: flex;
  justify-content: center;
  flex-wrap: wrap;
  gap: var(--padding-large);
}

.kittens-title {
  font-weight: 500;
  font-size: 1.875rem;
  max-width: 55rem;
  text-align: center;
}

@media (max-width: 720px) {
  .kittens-title {
    font-size: 1.5rem;
  }
}


</style>
