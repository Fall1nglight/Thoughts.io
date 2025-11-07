import HomeView from '@/views/HomeView.vue'
import LoginView from '@/views/LoginView.vue'
import SignupView from '@/views/SignupView.vue'
import AboutView from '@/views/AboutView.vue'

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
  },

  {
    path: '/signup',
    name: 'signup',
    component: SignupView,
  },

  {
    path: '/about',
    name: 'about',
    component: AboutView,
  },
]

export default routes
