<script setup>
import { formatDistanceToNow } from 'date-fns'
import { computed } from 'vue'

const { comment } = defineProps({
  comment: Object,
})

const wasEverUpdated = computed(() => new Date(comment.updatedAtUtc).getUTCFullYear() > 1)

function formatDate(date) {
  return formatDistanceToNow(new Date(date), { addSuffix: true })
}
</script>

<template>
  <div class="border-bottom border-secondary py-3">
    <div class="d-flex justify-content-between align-items-baseline">
      <h6 class="fw-bold text-primary mb-1">{{ comment.user.username }}</h6>

      <small class="text-muted">{{ formatDate(comment.createdAtUtc) }}</small>
    </div>

    <p class="mb-1 text-break">{{ comment.content }}</p>

    <div v-if="wasEverUpdated" class="text-end">
      <small class="text-muted fst-italic" style="font-size: 0.75rem">
        (módosítva: {{ formatDate(comment.updatedAtUtc) }})
      </small>
    </div>
  </div>
</template>

<style scoped></style>
