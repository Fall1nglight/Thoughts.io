<script setup>
import { inject, ref, watchEffect } from 'vue'
import { useThoughtsStore } from '@/stores/thoughts.js'
import { storeToRefs } from 'pinia'

const reactionId = inject('activeReactionId')
const thoughtsStore = useThoughtsStore()
const { focusedThought } = storeToRefs(thoughtsStore)

const loading = ref(true)
const usersReacted = ref([])

watchEffect(async () => {
  loading.value = true

  const reactions = await thoughtsStore.fetchReactionsById(
    focusedThought.value.id,
    reactionId.value,
  )

  usersReacted.value = reactions.users

  setTimeout(() => {
    loading.value = false
  }, 250)
})
</script>

<template>
  <div v-if="loading" class="d-flex align-items-center justify-content-center">
    <div class="loader"></div>
  </div>
  <div v-else-if="!usersReacted.length">Nobody used this reaction</div>
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
