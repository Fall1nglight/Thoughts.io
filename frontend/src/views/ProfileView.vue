<script setup>
import { computed, ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import { storeToRefs } from 'pinia'
import { useThoughtsStore } from '@/stores/thoughts.js'
import CardContainer from '@/components/CardContainer.vue'
import { useUserStore } from '@/stores/user.js'
import { formatDistanceToNow } from 'date-fns'
import reactionTypes from '@/types/reaction.types.js'

// Dependencies
const thoughtsStore = useThoughtsStore()
const userStore = useUserStore()
const { thoughts } = storeToRefs(thoughtsStore)
const { user, hasLoadedUser } = storeToRefs(userStore)
const { fetchUserById } = userStore
const route = useRoute()

// Local state
const loading = ref(false)

// Derived state
const userThoughts = computed(() => thoughts.value.filter((x) => x.user.id === route.params.userId))

// Methods
async function fetchUserData() {
  loading.value = true

  await thoughtsStore.fetchUserThoughts(route.params.userId)
  await fetchUserById(route.params.userId)

  loading.value = false
}

// Hooks and watchers
watch(
  () => route.params.userId,
  async () => {
    await fetchUserData()
  },
  { immediate: true },
)
</script>

<template>
  <section class="welcome text-center p-5">
    <div class="row p-5">
      <h2 v-if="hasLoadedUser">{{ user.username }}'s profile</h2>
      <h2 v-else>Failed to load username</h2>
    </div>
  </section>

  <section v-if="hasLoadedUser" class="details px-5 pb-5">
    <div class="row p-5">
      <div class="col-12">
        <h2 class="text-center">Details</h2>
        <p class="lead">
          Joined: {{ formatDistanceToNow(user.createdAtUtc + 'Z', { addSuffix: true }) }}
        </p>

        <p class="lead">Thoughts: {{ user.stats.thoughts.count }}</p>

        <p class="lead">Comments: {{ user.stats.comments.count }}</p>

        <p class="lead">
          Reactions:
          <span v-for="reaction in user.stats.reactions" :key="reaction.id">
            <i :class="reactionTypes[reaction.id]"></i>
            <span>
              {{ reaction.count }}
            </span>
          </span>
        </p>
      </div>
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

.details > div {
  /* background-color: rgba(41, 44, 51, 0.5);*/
  background-color: rgba(255, 100, 21, 0.45);
  border-radius: 10px;
}

* {
  color: white;
}
</style>
