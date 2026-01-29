<script setup>
import { computed, provide, ref } from 'vue'
import { useAuthStore } from '@/stores/auth.js'
import { formatDistanceToNow } from 'date-fns'
import { provideTypes } from '@/types/provide.types.js'
import ReactionContainer from '@/components/ReactionContainer.vue'
import ThoughtCommentButton from '@/components/ThoughtCommentButton.vue'
import { storeToRefs } from 'pinia'
import { useThoughtsStore } from '@/stores/thoughts.js'

// Props and emits
const { thought } = defineProps({
  thought: Object,
})

// Dependecies
const authStore = useAuthStore()
const thoughtsStore = useThoughtsStore()
const { getUserId } = storeToRefs(authStore)
const { deleteThoughtById, updateThoughtById } = thoughtsStore

// Local state
const editMode = ref(false)
const newThought = ref({
  title: thought.title,
  content: thought.content,
  isPublic: thought.isPublic,
})

// Derived state
const wasEverUpdated = computed(() => new Date(thought.updatedAtUtc).getUTCFullYear() > 1)
const ownedThought = computed(() => thought.user.id === getUserId.value)

// Methods
function toggleEdit() {
  editMode.value = !editMode.value
}

async function handleDelete() {
  if (!confirm('Are you sure you want to delete?')) return
  deleteThoughtById(thought.id).then(() => (editMode.value = false))
}

async function handleEdit() {
  const payload = {
    id: thought.id,
    title: newThought.value.title,
    content: newThought.value.content,
    isPublic: newThought.value.isPublic,
  }

  updateThoughtById(payload).then(() => (editMode.value = false))
}

// Provide
provide(provideTypes.thought, thought)
</script>

<template>
  <div
    class="col"
    :class="thought.isPublic ? 'border border-3 border-success' : 'border border-3 border-warning'"
  >
    <div class="card">
      <div class="card-header d-flex align-items-center justify-content-between">
        <div>
          <RouterLink :to="{ name: 'profile', params: { userId: thought.user.id } }">
            {{ thought.user.username }}</RouterLink
          >
        </div>

        <div class="d-flex align-items-center justify-content-between">
          <div>
            <span>
              {{ 'Posted ' + formatDistanceToNow(thought.createdAtUtc + 'Z', { addSuffix: true }) }}
            </span>
          </div>

          <div v-if="ownedThought">
            <button @click="toggleEdit" type="button" class="btn btn-primary">Edit</button>
            <button @click="handleDelete" type="button" class="btn btn-primary">Delete</button>
          </div>
        </div>
      </div>

      <div v-if="!editMode" class="card-body">
        <h5 class="card-title">{{ thought.title }}</h5>
        <p class="card-text">
          {{ thought.content }}
        </p>
      </div>

      <div v-else>
        <form @submit.prevent="handleEdit">
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
      </div>

      <div class="card-footer d-flex align-items-center justify-content-between">
        <div class="updated-at">
          <small v-if="wasEverUpdated" class="text-body-secondary"
            >{{ 'Updated ' + formatDistanceToNow(thought.updatedAtUtc + 'Z', { addSuffix: true }) }}
          </small>
        </div>

        <div class="d-flex">
          <ThoughtCommentButton></ThoughtCommentButton>
          <ReactionContainer></ReactionContainer>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped></style>
