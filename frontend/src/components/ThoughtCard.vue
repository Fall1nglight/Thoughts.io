<script setup>
import { computed, provide } from 'vue'
import { formatDistanceToNow } from 'date-fns'
import { provideTypes } from '@/types/provide.types.js'
import ReactionContainer from '@/components/ReactionContainer.vue'
import ThoughtCommentButton from '@/components/ThoughtCommentButton.vue'

// Props and emits
const { thought } = defineProps({
  thought: Object,
})

// Provide and inject
provide(provideTypes.thought, thought)

// Derived state
const wasEverUpdated = computed(() => new Date(thought.updatedAtUtc).getUTCFullYear() > 1)
</script>

<template>
  <div class="col">
    <div class="card">
      <div class="card-header d-flex align-items-center justify-content-between">
        <span>
          {{ thought.user.username }}
        </span>

        <span>
          {{ 'Posted ' + formatDistanceToNow(thought.createdAtUtc, { addSuffix: true }) }}
        </span>
      </div>

      <div class="card-body">
        <h5 class="card-title">{{ thought.title }}</h5>
        <p class="card-text">
          {{ thought.content }}
        </p>
      </div>

      <div class="card-footer d-flex align-items-center justify-content-between">
        <div class="updated-at">
          <small v-if="wasEverUpdated" class="text-body-secondary"
            >{{ 'Updated ' + formatDistanceToNow(thought.updatedAtUtc, { addSuffix: true }) }}
          </small>
        </div>

        <div class="d-flex">
          <ThoughtCommentButton></ThoughtCommentButton>

          <ReactionContainer
            :reactions="thought.reactions"
            :thought-id="thought.id"
          ></ReactionContainer>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped></style>
