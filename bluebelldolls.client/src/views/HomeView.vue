<script setup lang="ts">
  import TipBox from '@/components/ui/TipBox.vue'
  import ArticleComponent from '@/components/ui/ArticleComponent.vue'
</script>

<template>
  <div class="w-[100%]">
    <div class="p-[20px]">
      <ArticleComponent imagePath="src/assets/images/header-bg.png" header="Test" text="test text" />
    </div>
    <TipBox imagePath="src/assets/images/header-bg.png" text="Добро пожаловать в питомник BlueBellDolls — место, где рождаются чистокровные и ласковые Рэгдоллы. Мы с любовью подходим к каждому этапу выращивания, чтобы подарить вам здоровых, социализированных и невероятно красивых котят. Узнайте больше о нас и наших выпускниках!" />
  </div>
</template>
