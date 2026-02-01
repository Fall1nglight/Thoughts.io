<script setup>
import { useAuthStore } from '@/stores/auth.js'

// Dependencies
const authStore = useAuthStore()

// Methods
function handleDelete() {
  if (!confirm('Are you sure you want to delete your account?')) return
  authStore.deleteUser()
}
</script>

<template>
  <div>
    <p class="lead">Deleting your account cannot be reversed!</p>
    <button type="button" class="btn btn-primary" @click="handleDelete">Delete account</button>
  </div>
</template>

<style scoped></style>
