<script setup>
import { useThoughtsStore } from '@/stores/thoughts.js'
import { storeToRefs } from 'pinia'
import { onMounted, ref } from 'vue'
import ThoughtComment from '@/components/ThoughtComment.vue'

const thoughtsStore = useThoughtsStore()
const { focusedThought } = storeToRefs(thoughtsStore)

const loading = ref(true)
const comments = ref([])

onMounted(async () => {
  loading.value = true
  const data = await thoughtsStore.fetchComments(focusedThought.value.id)
  comments.value = data.comments

  setTimeout(() => {
    loading.value = false
  }, 250)
})
</script>

<template>
  <div v-if="loading" class="d-flex align-items-center justify-content-center">
    <div class="loader"></div>
  </div>
  <div v-else-if="!comments.length">No comments found</div>
  <div v-else>
    <ThoughtComment
      v-for="comment in comments"
      :comment="comment"
      :key="comment.id"
    ></ThoughtComment>
  </div>
</template>

<style scoped></style>
