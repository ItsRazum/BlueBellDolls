import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import LittersView from '../views/LittersView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView,
      meta: { buttonText: 'Наши помёты', title: 'Питомник Рэгдолл №1 в России.' }
    },
    {
      path: '/litters',
      name: 'litters',
      component: LittersView,
    }
  ],
})

export default router
