<script setup>
import { storeToRefs } from 'pinia'
import { useAuthStore } from '@/stores/auth.js'

const authStore = useAuthStore()
const { user } = storeToRefs(authStore)
</script>

<template>
  <h1>Welcome {{ user.username }}</h1>
</template>

<style scoped></style>
