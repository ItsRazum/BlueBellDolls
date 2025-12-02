<script setup lang="ts">

import {KittenClass, KittenStatus, PhotosType} from "~~/enums/enums";
import type {PhotoDto} from "~~/types/api";
import Advertisement from "~/components/Advertisement.vue";

const mockCatBoy = ref<ParentCatListDto>({
  id: 1,
  name: 'Boy BlueBellDolls',
  birthDay: '07.11.2025',
  isMale: true,
  isEnabled: true,
  color: 'Seal Bicolor',
  description: 'Это описание кота. От себя рекомендую писать в описании хотя-бы 100 символов ;)',
  mainPhotoUrl: 'photo.png'
});

const mockCatGirl = ref<KittenListDto>({
  id: 1,
  name: 'Girl BlueBellDolls',
  birthDay: '07.11.2025',
  isMale: false,
  isEnabled: true,
  color: 'Seal Bicolor',
  description: 'Это описание котёнка. От себя рекомендую писать в описании хотя-бы 100 символов ;)',
  mainPhotoUrl: 'photo.png',
  class: KittenClass.Pet,
  status: KittenStatus.Available,
  litterId: 1,
  litterLetter: 'A',
});

const mockLitter = ref<LitterDetailDto>({
  id: 1,
  letter: 'A',
  birthDay: '07.11.2025',
  description: 'Это описание помёта. От себя рекомендую писать в описании хотя-бы 100 символов ;)',
  kittens: [
      mockCatGirl.value
  ],
  fatherCatId: 2,
  fatherCat: ref<ParentCatMinimalDto>({
    id: 2,
    name: 'Caesar BlueBellDolls',
  }),
  motherCatId: 3,
  motherCat: ref<ParentCatMinimalDto>({
    id: 3,
    name: 'Tess BlueBellDolls',
  }),
  photos: [ {
    id: 1,
    url: 'photo.png',
    type: PhotosType.Photos,
    isMain: true,
  } ],
})

</script>

<template>

  <main>

    <div style="display: flex; flex-direction: column; align-items: center; justify-content: center; gap: 40px; margin: 40px">
      <LitterListItem :litter="mockLitter"/>
      <Advertisement photo-url="photo.png" name="Статья" description="Это описание статьи. Лучше всего писать его на 3-4 строки, чтобы компонент не выглядел 'сплюснутым' сверху и снизу" redirect-url="/"/>
      <ParentCatListItem :parent-cat="mockCatBoy"/>
    </div>

  </main>
</template>
