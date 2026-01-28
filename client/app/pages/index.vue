<script setup lang="ts">
definePageMeta({
  title: "pages.main.title",
  backgroundImage: "/header.jpg",
  buttonText: "pages.main.buttonText",
  buttonUrl: "/litters",
});

const kittenApi = useKittenApi();

const { data: kittens, pending } = await kittenApi.getAvailableKittens();
</script>

<template>
  <div class="articles-container">
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
  <div class="first-message-container">
    <MessageBox imageUrl="/photo.png" :text="$t('pages.main.welcomeMessage')" />
  </div>
  <div v-if="pending" class="kittens-container">
    <h2 class="text-3xl">{{ $t("pages.main.kittensForSale") }}</h2>
    <div class="carousel-wrapper">
      <KittenListItemSkeleton variant="compact" />
      <KittenListItemSkeleton variant="compact" />
      <KittenListItemSkeleton variant="compact" />
    </div>
  </div>
  <div v-else-if="kittens.length === 0" class="kittens-container">
    <div class="no-kittens-container">
      <span class="font-bold text-3xl text-white">{{ $t("pages.main.kittensWillBeSoon") }}</span>
      <span class="text-2xl text-white text-center">{{
        $t("pages.main.stayTunedForNewKittens")
      }}</span>
    </div>
  </div>
  <div v-else-if="kittens.length < 4" class="kittens-container">
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
  <div v-else class="kittens-container">
    <h2 class="text-3xl">{{ $t("pages.main.kittensForSale") }}</h2>
    <KittensCarousel :kittens="kittens" />
    <NuxtLinkLocale to="litters" class="text-[1.25rem]">{{
      $t("pages.main.showMoreKittens")
    }}</NuxtLinkLocale>
  </div>
  <div class="cats-container">
    <span class="kittens-title">{{ $t("pages.main.ourCatsDescription") }}</span>
    <div class="cats-articles-container">
      <Article
        :title="$t('pages.main.articles.ourFemaleCats.title')"
        photo-url="photo.png"
        :description="$t('pages.main.articles.ourFemaleCats.description')"
        redirect-url="/femalecats"
      />
      <Article
        :title="$t('pages.main.articles.ourMaleCats.title')"
        photo-url="photo.png"
        :description="$t('pages.main.articles.ourMaleCats.description')"
        redirect-url="/malecats"
      />
    </div>
  </div>
</template>

<style scoped>
.articles-container {
  display: flex;
  justify-content: center;
  background-color: var(--color-pages-primary-background);
  gap: var(--padding-extra-large);
  padding: var(--padding-pages);
}

.first-message-container {
  display: flex;
  background-color: var(--color-pages-secondary-background);
  padding: var(--padding-large) 5.375rem;
}

.kittens-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  background-color: var(--color-pages-primary-background);
  padding: var(--padding-large) var(--padding-extra-large);
  gap: var(--padding-extra-large);
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

.cats-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  background-color: var(--color-pages-secondary-background);
  gap: var(--padding-extra-large);
  padding: var(--padding-large);
}

.cats-articles-container {
  display: flex;
  justify-content: center;
  gap: var(--padding-large);
}

.kittens-title {
  font-weight: 500;
  font-size: 1.875rem;
  max-width: 55rem;
  text-align: center;
}
</style>
