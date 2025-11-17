<script setup>
import { useAuthStore } from '@/stores/auth.js'
import { useRouter } from 'vue-router'

const { logout } = useAuthStore()
const router = useRouter()

logout()
router.push('/')
</script>

<template>
  <div></div>
</template>

<style scoped></style>
