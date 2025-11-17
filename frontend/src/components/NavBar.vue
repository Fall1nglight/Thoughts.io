<script setup>
import { RouterLink } from 'vue-router'
import { useAuthStore } from '@/stores/auth.js'
import { storeToRefs } from 'pinia'

const authStore = useAuthStore()
const { user, isLoggedIn, isAdmin } = storeToRefs(authStore)
</script>

<template>
  <nav class="navbar navbar-expand-lg bg-primary" data-bs-theme="dark">
    <div class="container-fluid">
      <RouterLink to="/" class="navbar-brand">Thoughts.io</RouterLink>

      <button
        class="navbar-toggler"
        type="button"
        data-bs-toggle="collapse"
        data-bs-target="#navbarColor01"
        aria-controls="navbarColor01"
        aria-expanded="false"
        aria-label="Toggle navigation"
      >
        <span class="navbar-toggler-icon"></span>
      </button>
      <div class="collapse navbar-collapse" id="navbarColor01">
        <ul class="navbar-nav ms-auto">
          <li class="nav-item">
            <RouterLink to="/" class="nav-link">Home</RouterLink>
          </li>

          <li v-show="!isLoggedIn" class="nav-item">
            <RouterLink to="/login" class="nav-link">Login</RouterLink>
          </li>

          <li v-show="!isLoggedIn" class="nav-item">
            <RouterLink to="/signup" class="nav-link">Signup</RouterLink>
          </li>

          <!--          <li class="nav-item">-->
          <!--            <RouterLink to="/about" class="nav-link">About</RouterLink>-->
          <!--          </li>-->

          <li v-show="isLoggedIn" class="nav-item">
            <RouterLink to="/profile" class="nav-link">{{ user.username }}</RouterLink>
          </li>

          <li v-show="isAdmin" class="nav-item">
            <RouterLink to="/admin-dashboard" class="nav-link">Admin Dashboard</RouterLink>
          </li>

          <li v-show="isLoggedIn" class="nav-item">
            <RouterLink to="/logout" class="nav-link">Logout</RouterLink>
          </li>
        </ul>
      </div>
    </div>
  </nav>
</template>

<style scoped>
.navbar-brand {
  border-bottom: 1px solid #469ae0;
}

.nav-link {
  border: 1px solid #469ae0;
  border-radius: 5px;
  font-weight: 300;
  padding: 0.35rem 1rem;
  min-width: 90px;
  text-align: center;
  color: white;
}

.nav-link:hover {
  background-color: #469ae0;
}

.navbar {
  background-color: #292c33 !important;
}
</style>
