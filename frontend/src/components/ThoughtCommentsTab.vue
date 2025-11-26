<script setup>
import { storeToRefs } from 'pinia'
import { useThoughtsStore } from '@/stores/thoughts.js'
import ThoughtComment from '@/components/ThoughtComment.vue'

// Dependencies
const thoughtsStore = useThoughtsStore()
const { focusedComments } = storeToRefs(thoughtsStore)
</script>

<template>
  <div v-if="!focusedComments.length">No comments found</div>
  <div v-else>
    <ThoughtComment
      v-for="comment in focusedComments"
      :comment="comment"
      :key="comment.id"
    ></ThoughtComment>
  </div>
</template>

<style scoped></style>
