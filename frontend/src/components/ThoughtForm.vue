<script setup>
import { ref } from 'vue'
import { useThoughtsStore } from '@/stores/thoughts.js'

// Dependencies
const thoughtsStore = useThoughtsStore()
const { addThought } = thoughtsStore

// Local state
const newThought = ref({
  title: '',
  content: '',
  isPublic: true,
})

// Methods
function resetForm() {
  newThought.value.title = ''
  newThought.value.content = ''
  newThought.value.isPublic = true
}

async function handleSubmit() {
  await addThought(newThought.value).then(() => resetForm())
}
</script>

<template>
  <form @submit.prevent="handleSubmit">
    <div class="mb-3">
      <label for="thoughtTitleEditInput" class="form-label">Title</label>
      <input
        type="text"
        class="form-control"
        id="thoughtTitleEditInput"
        v-model="newThought.title"
      />
    </div>

    <div class="mb-3">
      <label for="thoughtContentEditInput" class="form-label">Content</label>
      <input
        type="text"
        class="form-control"
        id="thoughtContentEditInput"
        v-model="newThought.content"
      />
    </div>

    <div class="mb-3">
      <div class="form-check">
        <input
          class="form-check-input"
          type="radio"
          name="thoughtIsPublicRadio"
          id="thoughtIsPublicRadio1"
          :value="true"
          v-model="newThought.isPublic"
        />
        <label class="form-check-label" for="radioDefault1">Public</label>
      </div>

      <div class="form-check">
        <input
          class="form-check-input"
          type="radio"
          name="thoughtIsPublicRadio"
          id="thoughtIsPublicRadio2"
          :value="false"
          v-model="newThought.isPublic"
        />
        <label class="form-check-label" for="radioDefault2">Private</label>
      </div>
    </div>

    <div>
      <button type="submit" class="btn btn-primary">Submit</button>
    </div>
  </form>
</template>

<style scoped></style>
