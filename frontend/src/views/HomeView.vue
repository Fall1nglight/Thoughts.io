<script setup>
import ThoughtForm from '@/components/ThoughtForm.vue'
import { useAuthStore } from '@/stores/auth.js'
import { storeToRefs } from 'pinia'
import { useThoughtsStore } from '@/stores/thoughts.js'
import { onMounted, ref } from 'vue'
import CardContainer from '@/components/CardContainer.vue'

// Dependencies
const authStore = useAuthStore()
const thoughtsStore = useThoughtsStore()
const { isLoggedIn } = storeToRefs(authStore)
const { hasPublicThoughts, publicThoughts } = storeToRefs(thoughtsStore)
const { fetchPublicThoughts } = thoughtsStore

// Local state
const loading = ref(false)

// Hooks and watchers
onMounted(async () => {
  loading.value = true

  await fetchPublicThoughts()

  setTimeout(() => {
    loading.value = false
  }, 500)
})
</script>

<template>
  <section class="welcome text-center p-5">
    <div class="row p-5">
      <h1>Welcome to Thoughts.io</h1>
      <h2>Share your public thoughts with the world or keep them private!</h2>
    </div>
  </section>

  <section v-if="isLoggedIn" class="thoughtForm p-5">
    <ThoughtForm></ThoughtForm>
  </section>

  <section class="publicThoughts p-5">
    <CardContainer
      :loading="loading"
      :hasThoughts="hasPublicThoughts"
      :thoughts="publicThoughts"
    ></CardContainer>
  </section>
</template>

<style scoped>
.welcome > h1,
h2 {
  font-family: 'Poppins', sans-serif;
  font-weight: 300;
  font-style: normal;
}

.welcome > div {
  /* background-color: rgba(41, 44, 51, 0.5);*/
  background-color: rgba(255, 100, 21, 0.45);
  border-radius: 10px;
}

* {
  color: white;
}
</style>
