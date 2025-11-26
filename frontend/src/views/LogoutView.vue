<script setup>
import { onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth.js'

// todo | delete this view and use a button instead which calls the "logout()" method

// Dependencies
const { logout } = useAuthStore()

// Hooks and watchers
onMounted(() => logout())
</script>

<template>
  <div></div>
</template>

<style scoped></style>
