<script setup>
import { useAuthStore } from '@/stores/auth.js'

// todo | delete this view and use a button instead which calls the "logout()" method

const { logout } = useAuthStore()
logout()
</script>

<template>
  <div></div>
</template>

<style scoped></style>
