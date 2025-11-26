<script setup>
import { computed, inject } from 'vue'
import { storeToRefs } from 'pinia'
import { useThoughtsStore } from '@/stores/thoughts.js'

// Dependencies
const thoughtsStore = useThoughtsStore()
const { focusedReactions } = storeToRefs(thoughtsStore)

// Provide and inject
const reactionId = inject('activeReactionId')

// Derived state
const usersReacted = computed(
  () => focusedReactions.value.find((r) => r.id === reactionId.value)?.users || [],
)
</script>

<template>
  <div v-if="!usersReacted.length">Nobody used this reaction</div>
  <div v-else>
    <div>Users reacted:</div>
    <ul>
      <li v-for="(user, index) in usersReacted" :key="index">
        {{ user.username }}
      </li>
    </ul>
  </div>
</template>

<style scoped></style>
