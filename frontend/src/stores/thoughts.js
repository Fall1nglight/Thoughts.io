import axios from 'axios'
import { computed, ref } from 'vue'
import { defineStore, storeToRefs } from 'pinia'
import { useAuthStore } from '@/stores/auth.js'
import { useErrorStore } from '@/stores/error.js'
import { thoughtsUri } from '@/config/api.config.js'
import errorTypes from '@/types/error.types.js'
import reactionTypes from '@/types/reaction.types.js'

export const useThoughtsStore = defineStore('thoughts', () => {
  // other stores
  const authStore = useAuthStore()
  const errorStore = useErrorStore()
  const { user, getAccessToken } = storeToRefs(authStore)

  // axios
  const client = axios.create({
    baseURL: thoughtsUri,
  })

  client.interceptors.request.use((config) => {
    const token = getAccessToken.value

    if (token) {
      config.headers.Authorization = 'Bearer ' + token
    }

    return config
  })

  // axios.interceptors.response.use(
  //   (response) => response,
  //   async (error) => {
  //     const originalRequest = error.config
  //
  //     if (error.response.status === 401 && !originalRequest._retry) {
  //       originalRequest._retry = true
  //
  //       try {
  //         // refresh token
  //         await authStore.refreshToken()
  //       } catch (err) {}
  //     }
  //   },
  // )

  // state
  const thoughts = ref([])
  const focusedThought = ref(null)
  const focusedComments = ref([])
  const focusedReactions = ref([])
  const modal = ref(null)

  // getters
  const publicThoughts = computed(() => thoughts.value.filter((x) => x.isPublic))
  const hasPublicThoughts = computed(() => publicThoughts.value.length > 0)

  // actions
  function handleError(error) {
    errorStore.addError(errorTypes.apiError, error)
  }

  // tervezés
  // jelenleg vannak a publikus gondolatok
  // ha a user megnyitja majd a saját gondolatait akkor betöltjük a userthoughts-ot
  // ezzel az a probléma, ha a homeview-ban frissítjük valamelyik gondolatot akkor csak a publikus gondolatok frissülnek
  // megoldás -> az összes gondolatot egy helyre gyújtjük, ha be van lépve a felh, a saját adatai adjuk vissza
  // a homeview-ba csak a publikusokat
  // az egyéni user felületen meg route.param.id (userId) alapján
  // ha vmi változik akkor csak egy helyen kell frissíteni

  async function fetchPublicThoughts() {
    try {
      const { data } = await client.get('/')

      // töröljük a publikus gondolatokat
      // a privátakat meghagyjuk, mert azok csak akkor fognak frissülni, ha a saját oldalunkra megyünk

      thoughts.value = thoughts.value.filter((x) => !x.isPublic)
      const reactionIds = Object.keys(reactionTypes).map(Number)

      const transformedThoughts = data.thoughts.map((thought) => {
        const count = thought.reactions.reduce((acc, reaction) => {
          acc[reaction.id] = reaction.count
          return acc
        }, {})

        thought.reactions = reactionIds.map((reactionId) => {
          return { id: reactionId, count: count[reactionId] ?? 0 }
        })

        return thought
      })

      thoughts.value.push(...transformedThoughts)
    } catch (error) {
      error.message = 'Failed to fetch data from the API. Please try again later.'
      handleError(error)
    }
  }

  async function deleteReaction(thoughtId, reactionId) {
    const deleteResponse = await client.delete(`/${thoughtId}/reactions/user`)

    if (deleteResponse.status !== 204)
      throw new Error('Failed to delete reaction. Please try again later.')

    thoughts.value = thoughts.value.map((thought) => {
      if (thought.id !== thoughtId) return thought

      thought.userReactionId = 0

      thought.reactions.map((reaction) => {
        if (reaction.id !== reactionId) return reaction

        reaction.count--
        return reaction
      })

      return thought
    })
  }

  async function upsertReaction(thoughtId, reactionId) {
    const upsertResponse = await client.put(`/${thoughtId}/reactions`, { reactionId })

    if (upsertResponse.status !== 200)
      throw new Error('Failed to submit reaction. Please try again later.')

    thoughts.value = thoughts.value.map((thought) => {
      if (thought.id !== thoughtId) return thought

      thought.reactions.map((reaction) => {
        if (reaction.id === thought.userReactionId) {
          reaction.count--
          return reaction
        }

        if (reaction.id !== reactionId) return reaction

        reaction.count++
        return reaction
      })

      thought.userReactionId = reactionId
      return thought
    })
  }

  async function toggleReaction(thoughtId, reactionId) {
    try {
      const thought = thoughts.value.find((x) => x.id === thoughtId)

      if (thought.userReactionId === reactionId) {
        await deleteReaction(thoughtId, reactionId)
      } else {
        await upsertReaction(thoughtId, reactionId)
      }
    } catch (error) {
      handleError(error)
    }
  }

  async function fetchReactionsById(thoughtId, reactionId) {
    try {
      const { data } = await client.get(`/${thoughtId}/reactions/${reactionId}`)
      return data
    } catch (error) {
      handleError(error)
    }
  }

  async function fetchReactions(thoughtId) {
    try {
      const { data } = await client.get(`/${thoughtId}/reactions`)
      focusedReactions.value = data.reactions
    } catch (error) {
      handleError(error)
    }
  }

  async function fetchComments(thoughtId) {
    try {
      const { data } = await client.get(`/${thoughtId}/comments`)

      focusedComments.value = data.comments
    } catch (error) {
      handleError(error)
    }
  }

  return {
    thoughts,
    focusedThought,
    focusedComments,
    focusedReactions,
    publicThoughts,
    hasPublicThoughts,
    fetchPublicThoughts,
    toggleReaction,
    fetchReactionsById,
    fetchReactions,
    fetchComments,
    modal,
  }
})
