<script setup>
import { computed } from 'vue'
import { storeToRefs } from 'pinia'
import { useAuthStore } from '@/stores/auth.js'
import { formatDistanceToNow } from 'date-fns'

// Props and emits
const { comment } = defineProps({
  comment: Object,
})

// Dependencies
const authStore = useAuthStore()
const { getUserId } = storeToRefs(authStore)

// Derived state
const ownedComment = computed(() => comment.user.id === getUserId.value)
const wasEverUpdated = computed(() => new Date(comment.updatedAtUtc).getUTCFullYear() > 1)

// Methods
function formatDate(date) {
  return formatDistanceToNow(new Date(date), { addSuffix: true })
}

// todo | implement option to sort comments (default sorting behaviour: user's comments)
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
