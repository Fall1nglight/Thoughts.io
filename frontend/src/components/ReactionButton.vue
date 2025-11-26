<script setup>
import { computed, inject } from 'vue'
import { storeToRefs } from 'pinia'
import { useAuthStore } from '@/stores/auth.js'
import { useThoughtsStore } from '@/stores/thoughts.js'
import { provideTypes } from '@/types/provide.types.js'
import { useReactionStyle } from '@/composables/reactionStyle.js'

// Props and emits
const { reaction } = defineProps({
  reaction: Object,
})

// Dependencies
const { toggleReaction } = useThoughtsStore()
const authStore = useAuthStore()
const { isLoggedIn } = storeToRefs(authStore)

// Derived state
const buttonStyle = computed(() => (isLoggedIn.value ? 'mx-1 reaction-button' : 'mx-1'))

// Composable
const { iconStyle } = useReactionStyle(reaction.id)

// Methods
function handleReactionButtonClick(thoughtId, reactionId) {
  if (!isLoggedIn.value) return

  toggleReaction(thoughtId, reactionId)
}

// Provide and inject
const thought = inject(provideTypes.thought)
</script>

<template>
  <span @click="handleReactionButtonClick(thought.id, reaction.id)" :class="buttonStyle">
    <i :class="iconStyle"></i>
    <span>{{ reaction.count }}</span>
  </span>
</template>

<style scoped>
.reaction-button {
  cursor: pointer;
}
</style>
