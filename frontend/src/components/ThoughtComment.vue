<script setup>
import { computed, ref } from 'vue'
import { storeToRefs } from 'pinia'
import { useAuthStore } from '@/stores/auth.js'
import { formatDistanceToNow } from 'date-fns'
import { useThoughtsStore } from '@/stores/thoughts.js'

// Props and emits
const { comment } = defineProps({
  comment: Object,
})

// Dependencies
const authStore = useAuthStore()
const thoughtsStore = useThoughtsStore()
const { deleteComment, editComment } = useThoughtsStore()
const { getUserId } = storeToRefs(authStore)
const { focusedThought } = storeToRefs(thoughtsStore)

// Local state
const editMode = ref(false)
const newComment = ref({
  content: comment.content,
})

// Derived state
const ownedComment = computed(() => comment.user.id === getUserId.value)
const wasEverUpdated = computed(() => new Date(comment.updatedAtUtc).getUTCFullYear() > 1)

// Methods
function formatDate(date) {
  return formatDistanceToNow(new Date(date), { addSuffix: true })
}

function handleCommentDelete() {
  // if (!confirm('Are you sure you want to delete this comment?')) return
  deleteComment(focusedThought.value.id, comment.id)
}

function toggleEdit() {
  editMode.value = !editMode.value
}

function submitEdit() {
  // if (!confirm('Are you sure you want to edit this comment?')) return

  editComment(focusedThought.value.id, comment.id, newComment.value)

  editMode.value = false
}

// todo | implement option to sort comments (default sorting behaviour: user's comments)
</script>

<template>
  <div class="border rounded border-secondary mb-1">
    <div class="comment-edit text-end">
      <button v-if="ownedComment" @click="handleCommentDelete">
        <i class="fa-solid fa-xmark"></i>
      </button>
      <button v-if="ownedComment" @click="toggleEdit">
        <i class="fa-solid fa-pen-to-square pointer"></i>
      </button>
    </div>

    <div class="comment-data d-flex align-items-center justify-content-start p-2">
      <div class="me-2">
        <span class="fw-bold">[{{ comment.user.username }}]</span>
      </div>

      <div v-if="!editMode">
        <div class="text-break">{{ comment.content }}</div>
      </div>

      <div v-else>
        <form @submit.prevent="submitEdit">
          <input
            type="text"
            class="form-control"
            id="exampleFormControlInput1"
            v-model="newComment.content"
          />

          <button type="submit" class="btn btn-primary">submit edit</button>
        </form>
      </div>
    </div>

    <div class="comment-details d-flex align-items-center justify-content-between">
      <div>
        <span v-if="wasEverUpdated"> Updated {{ formatDate(comment.updatedAtUtc + 'Z') }} </span>
      </div>

      <div>Created {{ formatDate(comment.createdAtUtc + 'Z') }}</div>
    </div>
  </div>
</template>

<style scoped>
.pointer {
  cursor: pointer !important;
}
</style>
