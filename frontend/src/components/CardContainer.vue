<script setup>
import ThoughtCard from '@/components/ThoughtCard.vue'

const { loading, hasThoughts, thoughts } = defineProps({
  loading: { type: Boolean, default: false },
  hasThoughts: { type: Boolean, default: false },
  thoughts: Object,
})
</script>

<template>
  <div v-if="loading" class="d-flex align-items-center justify-content-center">
    <div class="loader"></div>
  </div>
  <div v-else-if="!hasThoughts">No data available.</div>
  <div v-else class="row row-cols-1 gap-3">
    <ThoughtCard v-for="thought in thoughts" :thought="thought" :key="thought.id"></ThoughtCard>
  </div>
</template>

<style scoped></style>
