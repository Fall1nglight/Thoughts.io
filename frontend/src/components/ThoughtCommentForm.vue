<script setup>
import { useThoughtsStore } from '@/stores/thoughts.js'
import { ref } from 'vue'
import { storeToRefs } from 'pinia'

// Dependencies
const thoughtsStore = useThoughtsStore()
const { addComment } = thoughtsStore
const { focusedThought } = storeToRefs(thoughtsStore)

// Local state
const newComment = ref({
  content: '',
})

// Methods
async function handleSubmit() {
  addComment(focusedThought.value.id, newComment.value).then(() => {
    newComment.value.content = ''
  })
}

// todo | validation
</script>

<template>
  <form @submit.prevent="handleSubmit" class="row row-cols-1 row-cols-2">
    <div class="mb-3">
      <input
        v-model="newComment.content"
        type="text"
        id="newCommentInput"
        class="form-control mb-2"
        placeholder="Enter new comment"
      />
    </div>

    <div>
      <button type="submit" class="btn btn-dark">Add</button>
    </div>
  </form>
</template>

<style scoped></style>
