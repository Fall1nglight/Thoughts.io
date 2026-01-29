<script setup>
import { computed, ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import { storeToRefs } from 'pinia'
import { useThoughtsStore } from '@/stores/thoughts.js'
import CardContainer from '@/components/CardContainer.vue'

// Dependencies
const thoughtsStore = useThoughtsStore()
const { thoughts } = storeToRefs(thoughtsStore)
const route = useRoute()

// Local state
const loading = ref(false)

// Derived state
const userThoughts = computed(() => thoughts.value.filter((x) => x.user.id === route.params.userId))

// Methods
async function fetchUserThoughts() {
  loading.value = true
  await thoughtsStore.fetchUserThoughts(route.params.userId)

  setTimeout(() => {
    loading.value = false
  }, 500)
}

watch(
  () => route.params.userId,
  async (oldId, newId) => {
    await fetchUserThoughts(newId)
  },
  { immediate: true },
)

// Lifecycle hooks & Watchers
</script>

<template>
  <section class="welcome text-center p-5">
    <div class="row p-5">
      <h2>{{ route.params.userId }}'s posts</h2>
    </div>
  </section>

  <CardContainer
    :loading="loading"
    :hasThoughts="userThoughts.length > 0"
    :thoughts="userThoughts"
  ></CardContainer>
</template>

<style scoped>
.welcome > h1,
h2 {
  font-family: 'Poppins', sans-serif;
  font-weight: 300;
  font-style: normal;
  padding: 0;
  margin: 0;
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
