<script setup>
import { inject } from 'vue'
import { provideTypes } from '@/types/provide.types.js'
import ReactionButton from '@/components/ReactionButton.vue'

// provide and inject
const thought = inject(provideTypes.thought)
</script>

<template>
  <div class="d-flex align-items-center">
    <ReactionButton
      v-for="reaction in thought.reactions"
      :reaction="reaction"
      :key="reaction.id"
    ></ReactionButton>
  </div>
</template>

<style scoped></style>
