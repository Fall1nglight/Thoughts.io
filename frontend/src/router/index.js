import { createRouter, createWebHistory } from 'vue-router'
import routes from './routes'
import metaProperties from '@/router/metaProperties.js'
import { useAuthStore } from '@/stores/auth.js'
import { storeToRefs } from 'pinia'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
})

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  const { isLoggedIn, isAdmin } = storeToRefs(authStore)

  if (to.meta[metaProperties.unAuthenticatedOnly] && isLoggedIn.value) {
    return next({ name: from.name || 'home' })
  }

  if (to.meta[metaProperties.requiresAuth] && !isLoggedIn.value) {
    return next({
      name: from.name || 'login',
    })
  }

  if (to.meta[metaProperties.requiresAdminRole] && !isAdmin.value) {
    return next({
      name: 'home',
    })
  }

  next()
})

export default router
