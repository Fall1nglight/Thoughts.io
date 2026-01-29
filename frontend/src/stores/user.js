import { defineStore, storeToRefs } from 'pinia'
import axios from 'axios'
import { usersUri } from '@/config/api.config.js'
import { useAuthStore } from '@/stores/auth.js'
import { computed, ref } from 'vue'
import { useErrorStore } from '@/stores/error.js'
import errorTypes from '@/types/error.types.js'
import reactionTypes from '@/types/reaction.types.js'

export const useUserStore = defineStore('user', () => {
  // other stores
  const errorStore = useErrorStore()
  const authStore = useAuthStore()
  const { getAccessToken } = storeToRefs(authStore)

  // axios
  const client = axios.create({
    baseURL: usersUri,
  })

  // todo | ezt központosítani kell (DRY)
  client.interceptors.request.use((config) => {
    const token = getAccessToken.value

    if (token) {
      config.headers.Authorization = 'Bearer ' + token
    }

    return config
  })

  // state
  const user = ref({
    id: '',
    username: '',
    createdAtUtc: null,
    stats: {
      thoughts: {
        count: 0,
      },
      comments: {
        count: 0,
      },
      reactions: [],
    },
  })

  // getters
  const hasLoadedUser = computed(() => !!user.value.id)

  // methods
  function handleError(error) {
    errorStore.addError(errorTypes.apiError, error)
  }

  function setUser(newUser) {
    user.value.id = newUser.id
    user.value.username = newUser.username
    user.value.createdAtUtc = newUser.createdAtUtc
    user.value.stats.thoughts.count = newUser.stats.thoughts.count
    user.value.stats.comments.count = newUser.stats.comments.count
    user.value.stats.reactions = newUser.stats.reactions
  }

  // actions

  async function fetchUserById(userId) {
    try {
      const { data } = await client.get(`/${userId}`)
      setUser(data.user)
    } catch (error) {
      handleError(error)
    }
  }

  return {
    user,
    hasLoadedUser,
    fetchUserById,
  }
})
