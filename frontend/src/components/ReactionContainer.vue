<script setup>
import ReactionButton from '@/components/ReactionButton.vue'

const { reactions } = defineProps({
  reactions: Array,
  thoughtId: String,
})
</script>

<template>
  <div class="d-flex align-items-center">
    <ReactionButton
      v-for="reaction in reactions"
      :reaction="reaction"
      :thought-id="thoughtId"
      :key="reaction.id"
    ></ReactionButton>
  </div>
</template>

<style scoped></style>
