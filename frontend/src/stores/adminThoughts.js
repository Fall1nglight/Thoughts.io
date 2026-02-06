import { defineStore, storeToRefs } from 'pinia'
import { computed, ref } from 'vue'
import errorTypes from '@/types/error.types.js'
import { useErrorStore } from '@/stores/error.js'
import axios from 'axios'
import { adminUri } from '@/config/api.config.js'
import { useAuthStore } from '@/stores/auth.js'

export const useAdminThoughtsStore = defineStore('adminThoughts', () => {
  // other stores
  const errorStore = useErrorStore()
  const authStore = useAuthStore()
  const { getAccessToken } = storeToRefs(authStore)

  // axios
  const client = axios.create({ baseURL: adminUri + '/thoughts' })

  client.interceptors.request.use((config) => {
    const token = getAccessToken.value

    if (token) {
      config.headers.Authorization = 'Bearer ' + token
    }

    return config
  })

  // const
  const searchMethods = {
    thoughtId: 'thoughtId',
    thoughtTitle: 'thoughtTitle',
    userId: 'userId',
    username: 'username',
    all: 'all',
  }

  // state
  const modal = ref(null)
  const thoughts = ref([])
  const focusedThought = ref(null)
  const focusedComments = ref([])
  const searchParams = ref({
    method: '',
    query: '',
  })

  // getters
  const getThoughtsBySearch = computed(() => {
    const method = searchParams.value.method
    const query = searchParams.value.query

    switch (method) {
      case searchMethods.thoughtId:
        return thoughts.value.filter((x) => x.id === query)

      case searchMethods.thoughtTitle:
        return thoughts.value.filter((x) => x.title.toLowerCase().includes(query))

      case searchMethods.userId:
        return thoughts.value.filter((x) => x.user.id === query)

      case searchMethods.username:
        return thoughts.value.filter((x) => x.user.username === query)

      default:
        return thoughts.value
    }
  })

  // functions
  function handleError(error) {
    errorStore.addError(errorTypes.apiError, error)
  }

  // actions

  // fetch
  async function fetchThoughtsV2() {
    try {
      const method = searchParams.value.method
      const query = searchParams.value.query

      const newThoughts = []

      switch (method) {
        case searchMethods.thoughtId: {
          const result = await fetchThoughtById(query)

          if (result) newThoughts.push(result)
          break
        }

        case searchMethods.thoughtTitle: {
          const result = await fetchThoughtByTitle(query)
          newThoughts.push(...(result || []))
          break
        }

        case searchMethods.userId: {
          const result = await fetchThoughtsByUserId(query)
          newThoughts.push(...(result || []))
          break
        }

        case searchMethods.username: {
          const result = await fetchThoughtsByUsername(query)
          newThoughts.push(...(result || []))
          break
        }

        default: {
          const result = await fetchThoughts()
          newThoughts.push(...(result || []))
        }
      }

      const thoughtsMap = new Map(thoughts.value.map((x) => [x.id, x]))
      newThoughts.forEach((x) => thoughtsMap.set(x.id, x))
      thoughts.value = Array.from(thoughtsMap.values())
    } catch (error) {
      handleError(error)
    }
  }

  async function fetchThoughts() {
    try {
      const { data } = await client.get('/')
      return data.thoughts
    } catch (error) {
      handleError(error)
    }
  }

  async function fetchThoughtById(id) {
    try {
      const { data } = await client.get(id)
      return data.thought
    } catch (error) {
      handleError(error)
    }
  }

  async function fetchThoughtByTitle(title) {
    try {
      const { data } = await client.get(`/title/${title}`)
      return data.thoughts
    } catch (error) {
      handleError(error)
    }
  }
  async function fetchThoughtsByUserId(userId) {
    try {
      const { data } = await client.get(`/user/${userId}`)
      return data.thoughts
    } catch (error) {
      handleError(error)
    }
  }

  async function fetchThoughtsByUsername(username) {
    try {
      const { data } = await client.get(`/username/${username}`)
      return data.thoughts
    } catch (error) {
      handleError(error)
    }
  }

  // Update
  async function updateThought(id, isPublic) {
    try {
      const updateResponse = await client.put(id, { isPublic })

      if (updateResponse.status !== 204)
        throw new Error('Failed to toggle visibility. Please try again later.')

      thoughts.value = thoughts.value.map((x) => {
        if (x.id !== id) return x

        x.isPublic = isPublic
        return x
      })
    } catch (error) {
      handleError(error)
    }
  }

  // Delete
  async function deleteThought(id) {
    try {
      const deleteResponse = await client.delete(id)

      if (deleteResponse.status !== 204)
        throw new Error('Failed to delete thought. Please try again later.')

      thoughts.value = thoughts.value.filter((x) => x.id !== id)
    } catch (error) {
      handleError(error)
    }
  }

  // comments
  async function fetchComments(thoughtId) {
    try {
      const { data } = await client.get(`/${thoughtId}/comments`)

      focusedComments.value = data.comments.sort((a, b) => {
        return new Date(b.createdAtUtc).getTime() - new Date(a.createdAtUtc).getTime()
      })
    } catch (error) {
      handleError(error)
    }
  }

  async function deleteComment(thoughtId, commentId) {
    try {
      const deleteResponse = await client.delete(`/${thoughtId}/comments/${commentId}`)

      if (deleteResponse.status !== 204)
        throw new Error('Failed to delete comment. Please try again later.')

      focusedComments.value = focusedComments.value.filter((x) => x.id !== commentId)
      thoughts.value = thoughts.value.map((x) => {
        if (x.id !== thoughtId) return x

        x.comments.count--
        return x
      })
    } catch (error) {
      handleError(error)
    }
  }

  return {
    thoughts,
    focusedThought,
    focusedComments,
    modal,
    fetchThoughtsV2,
    updateThought,
    deleteThought,
    fetchComments,
    deleteComment,
    searchMethods,
    searchParams,
    getThoughtsBySearch,
  }
})
