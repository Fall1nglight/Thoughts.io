<script setup>
import { computed } from 'vue'
import { storeToRefs } from 'pinia'
import { useAuthStore } from '@/stores/auth.js'
import { useThoughtsStore } from '@/stores/thoughts.js'
import reactionTypes from '@/types/reaction.types.js'

const { reaction } = defineProps({
  reaction: Object,
  thoughtId: String,
})

const { toggleReaction } = useThoughtsStore()
const authStore = useAuthStore()
const { isLoggedIn } = storeToRefs(authStore)

const buttonStyle = computed(() => (isLoggedIn.value ? 'mx-1 reaction-button' : 'mx-1'))
const iconStyle = computed(() => reactionTypes[reaction.id])

function handleReactionButtonClick(thoughtId, reactionId) {
  if (!isLoggedIn.value) return

  toggleReaction(thoughtId, reactionId)
}
</script>

<template>
  <span @click="handleReactionButtonClick(thoughtId, reaction.id)" :class="buttonStyle">
    <i :class="iconStyle"></i>
    <span>{{ reaction.count }}</span>
  </span>
</template>

<style scoped>
.reaction-button {
  cursor: pointer;
}
</style>
