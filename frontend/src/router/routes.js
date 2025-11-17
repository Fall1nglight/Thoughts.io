import HomeView from '@/views/HomeView.vue'
import LoginView from '@/views/LoginView.vue'
import SignupView from '@/views/SignupView.vue'
import AboutView from '@/views/AboutView.vue'
import ProfileView from '@/views/ProfileView.vue'
import ResetPasswordView from '@/views/ResetPasswordView.vue'
import AdminDashboardView from '@/views/AdminDashboardView.vue'
import LogoutView from '@/views/LogoutView.vue'
import metaProperties from '@/router/metaProperties.js'

const routes = [
  {
    path: '/',
    name: 'home',
    component: HomeView,
  },

  {
    path: '/login',
    name: 'login',
    component: LoginView,
    meta: {
      [metaProperties.unAuthenticatedOnly]: true,
    },
  },

  {
    path: '/signup',
    name: 'signup',
    component: SignupView,
    meta: {
      [metaProperties.unAuthenticatedOnly]: true,
    },
  },

  {
    path: '/about',
    name: 'about',
    component: AboutView,
  },

  {
    path: '/profile',
    name: 'profile',
    component: ProfileView,
    meta: {
      [metaProperties.requiresAuth]: true,
    },
  },

  {
    path: '/reset-password',
    name: 'reset-password',
    component: ResetPasswordView,
    meta: {
      [metaProperties.unAuthenticatedOnly]: true,
    },
  },

  {
    path: '/admin-dashboard',
    name: 'admin-dashboard',
    component: AdminDashboardView,
    meta: {
      [metaProperties.requiresAuth]: true,
      [metaProperties.requiresAdminRole]: true,
    },
  },
  {
    path: '/logout',
    name: 'logout',
    component: LogoutView,
    meta: {
      [metaProperties.requiresAuth]: true,
    },
  },
]

export default routes
