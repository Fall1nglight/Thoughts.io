<script setup>
import { formatDistanceToNow } from 'date-fns'
import { computed } from 'vue'
import ReactionContainer from '@/components/ReactionContainer.vue'
import ThoughtCommentButton from '@/components/ThoughtCommentButton.vue'

const { thought } = defineProps({
  thought: Object,
})

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
          <ThoughtCommentButton
            :thought="thought"
            :comments="thought.comments"
          ></ThoughtCommentButton>

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
