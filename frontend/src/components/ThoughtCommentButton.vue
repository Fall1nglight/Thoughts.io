<script setup>
import { useAuthStore } from '@/stores/auth.js'
import { storeToRefs } from 'pinia'
import { computed } from 'vue'
import { useThoughtsStore } from '@/stores/thoughts.js'

const { thought, comments } = defineProps({
  thought: Object,
  comments: Object,
})

const authStore = useAuthStore()
const thoughtsStore = useThoughtsStore()
const { isLoggedIn } = storeToRefs(authStore)
const { focusedThought } = storeToRefs(thoughtsStore)

const buttonStyle = computed(() => (isLoggedIn.value ? 'comment-button' : ''))

function handleButtonClick() {
  if (!isLoggedIn.value) return

  focusedThought.value = thought
}
</script>

<template>
  <span @click="handleButtonClick" :class="buttonStyle">
    <i class="fa-solid fa-comment"></i>
    <span>{{ comments.count }}</span>
  </span>
</template>

<style scoped>
.comment-button {
  cursor: pointer;
}
</style>
