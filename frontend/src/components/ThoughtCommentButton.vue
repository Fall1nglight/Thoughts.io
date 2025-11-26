<script setup>
import { computed, inject } from 'vue'
import { storeToRefs } from 'pinia'
import { useAuthStore } from '@/stores/auth.js'
import { useThoughtsStore } from '@/stores/thoughts.js'
import { provideTypes } from '@/types/provide.types.js'

// Dependencies
const authStore = useAuthStore()
const thoughtsStore = useThoughtsStore()
const { isLoggedIn } = storeToRefs(authStore)
const { focusedThought } = storeToRefs(thoughtsStore)

// Derived state
const buttonStyle = computed(() => (isLoggedIn.value ? 'comment-button' : ''))

// Provide and inject
const thought = inject(provideTypes.thought)

// Methods
function handleButtonClick() {
  if (!isLoggedIn.value) return

  focusedThought.value = thought
}
</script>

<template>
  <span @click="handleButtonClick" :class="buttonStyle">
    <i class="fa-solid fa-comment"></i>
    <span>{{ thought.comments.count }}</span>
  </span>
</template>

<style scoped>
.comment-button {
  cursor: pointer;
}
</style>
