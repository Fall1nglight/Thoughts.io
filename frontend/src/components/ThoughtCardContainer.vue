<script setup>
import { onMounted, ref } from 'vue'
import { storeToRefs } from 'pinia'
import { useThoughtsStore } from '@/stores/thoughts.js'
import ThoughtCard from '@/components/ThoughtCard.vue'

// Dependencies
const thoughtsStore = useThoughtsStore()
const { publicThoughts, hasPublicThoughts } = storeToRefs(thoughtsStore)
const { fetchPublicThoughts } = thoughtsStore

// Local state
const loading = ref(true)

// Hooks and watchers
onMounted(async () => {
  loading.value = true
  await fetchPublicThoughts()

  setTimeout(() => {
    loading.value = false
  }, 500)
})
</script>

<template>
  <div v-if="loading" class="d-flex align-items-center justify-content-center">
    <div class="loader"></div>
  </div>
  <div v-else-if="!hasPublicThoughts">No data available.</div>
  <div v-else class="row row-cols-1 gap-3">
    <ThoughtCard
      v-for="thought in publicThoughts"
      :thought="thought"
      :key="thought.id"
    ></ThoughtCard>
  </div>
</template>

<style scoped></style>
