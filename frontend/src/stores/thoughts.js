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

  function transformReactions(thought, reactionIds) {
    const count = thought.reactions.reduce((acc, reaction) => {
      acc[reaction.id] = reaction.count
      return acc
    }, {})

    thought.reactions = reactionIds.map((reactionId) => {
      return { id: reactionId, count: count[reactionId] ?? 0 }
    })

    return thought
  }

  async function fetchPublicThoughts() {
    try {
      const { data } = await client.get('/')

      // töröljük a publikus gondolatokat
      // a privátakat meghagyjuk, mert azok csak akkor fognak frissülni, ha a saját oldalunkra megyünk

      thoughts.value = thoughts.value.filter((x) => !x.isPublic)
      const reactionIds = Object.keys(reactionTypes).map(Number)

      const transformedThoughts = data.thoughts
        .map((thought) => transformReactions(thought, reactionIds))
        .sort((a, b) => {
          return new Date(b.createdAtUtc).getTime() - new Date(a.createdAtUtc).getTime()
        })

      thoughts.value.push(...transformedThoughts)
    } catch (error) {
      error.message = 'Failed to fetch data from the API. Please try again later.'
      handleError(error)
    }
  }

  async function deleteThoughtById(thoughtId) {
    try {
      const deleteResponse = await client.delete(thoughtId)

      if (deleteResponse.status !== 204)
        throw new Error('Failed to delete reaction. Please try again later.')

      thoughts.value = thoughts.value.filter((thought) => thought.id !== thoughtId)
    } catch (error) {
      handleError(error)
    }
  }

  async function updateThoughtById(newThought) {
    try {
      const updateResponse = await client.put(newThought.id, {
        title: newThought.title,
        content: newThought.content,
        isPublic: newThought.isPublic,
      })

      if (updateResponse.status !== 204)
        throw new Error('Failed to update thought. Please try again later.')

      if (!newThought.isPublic) {
        // nem kell újra lekérni az API-ból, hiszen ha privátra állítottuk, akkor
        // csak a saját oldalunkon tekinthetjük meg
        // de ha a saját oldalunkat megnyitjuk, akkor lekérjük alapból az API-ból => felesleges dupla fetch elkerülése
        thoughts.value = thoughts.value.map((x) => {
          if (x.id !== newThought.id) return x

          x.isPublic = false
          return x
        })
      } else {
        // fetch thought's new details
        await fetchThoughtById(newThought.id)
      }
    } catch (error) {
      handleError(error)
    }
  }

  async function fetchThoughtById(thoughtId) {
    try {
      const {
        data: { thought },
      } = await client.get(`/${thoughtId}`)

      // ellenőrizzük, hogy a lekért gondolat, már tárolva van-e -> frissíteni kell
      // vagy nincs tárolva -> át kell alakítani a reakciókat, majd hozzáadni a thoughts-hoz

      if (thoughts.value.some((x) => x.id === thoughtId)) {
        thoughts.value = thoughts.value.map((x) => {
          if (x.id !== thoughtId) return x

          x.title = thought.title
          x.content = thought.content
          x.isPublic = thought.isPublic
          x.updatedAtUtc = thought.updatedAtUtc
          return x
        })
      } else {
        const reactionIds = Object.keys(reactionTypes).map(Number)
        thoughts.value.unshift(transformReactions(thought, reactionIds))
      }
    } catch (error) {
      handleError(error)
    }
  }

  async function addThought(newThought) {
    try {
      const {
        data: { id: thoughtId },
      } = await client.post('/', newThought)

      await fetchThoughtById(thoughtId)
    } catch (error) {
      handleError(error)
    }
  }

  async function fetchUserThoughts(userId) {
    try {
      const {
        data: { thoughts: userThoughts },
      } = await client.get(`/user/${userId}`)

      thoughts.value = thoughts.value.filter((x) => x.user.id !== userId)
      // itt mindegy hogy unshift vagy push mert a /profile-ban úgyis rendezetten fogjuk megkapni a gondolatokat
      // és, ha visszamegyünk a főoldalra újra lekérjük majd a gondolatokat

      const reactionIds = Object.keys(reactionTypes).map(Number)
      const transformedUserThoughts = userThoughts.map((x) => transformReactions(x, reactionIds))

      thoughts.value.push(...transformedUserThoughts)
    } catch (error) {
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

      focusedComments.value = data.comments.sort((a, b) => {
        return new Date(b.createdAtUtc).getTime() - new Date(a.createdAtUtc).getTime()
      })
    } catch (error) {
      handleError(error)
    }
  }

  async function deleteComment(thoughtId, commentId) {
    try {
      const response = await client.delete(`/${thoughtId}/comments/${commentId}`)

      if (response.status !== 204)
        throw new Error('Failed to delete comment. Please try again later.')

      focusedComments.value = focusedComments.value.filter((comment) => comment.id !== commentId)

      thoughts.value = thoughts.value.map((thought) => {
        if (thought.id !== thoughtId) return thought

        thought.comments.count--
        return thought
      })
    } catch (error) {
      handleError(error)
    }
  }

  async function editComment(thoughtId, commentId, newComment) {
    try {
      const response = await client.put(`/${thoughtId}/comments/${commentId}`, newComment)

      if (response.status !== 200)
        throw new Error('Failed to edit comment. Please try again later.')

      focusedComments.value = focusedComments.value.map((comment) =>
        comment.id === commentId ? { ...comment, content: newComment.content } : comment,
      )

      await fetchComment(thoughtId, commentId)
    } catch (error) {
      handleError(error)
    }
  }

  async function addComment(thoughtId, newComment) {
    try {
      const {
        data: { commentId },
      } = await client.post(`${thoughtId}/comments`, newComment)

      // növeljük a thoughts-ban commentCount-ot a commentId alapján
      thoughts.value = thoughts.value.map((thought) => {
        if (thought.id !== thoughtId) return thought

        thought.comments.count++
        return thought
      })

      await fetchComment(thoughtId, commentId)
    } catch (error) {
      handleError(error)
    }
  }

  async function fetchComment(thoughtId, commentId) {
    try {
      const { data } = await client.get(`/${thoughtId}/comments/${commentId}`)
      // ha egy kommentet hozzáadunk a thought-hoz
      // akkor le kell kérjük majd az API-ból a fetchComment() metódussal
      // ezt hozzá kell adjuk majd a focusedComments-hez

      // ha egy kommentet editelünk, akkor ugyanúgy lekérjük a frissített verzióját az API-ból
      // és kicseréljük a focusedComments-ben lévő régi verzióját az újra

      // ezért a fetchComment-et két féle dolgot csinál majd
      // előszőr is ellenőrzi, hogy a komment létezik-e a focusedComments-ben
      //  ha igen => kicseréli az új API-ból kapott értékre
      //  ha nem => szimplán hozzáadja csak a focusedComments-hez

      // 1. ellenőrizzük, hogy szerepel-e a focusedComments-ben hozzáadott vagy módosított komment
      // 2. ha szerepel, akkor felülírjuk map-al
      // 4. ha NEM szerepel, szimplán hozzáadjuk a focusedThought-hoz

      if (focusedComments.value.some((comment) => comment.id === commentId)) {
        // szerepel => frissíteni kell
        focusedComments.value = focusedComments.value.map((comment) => {
          if (comment.id !== commentId) return comment

          return data.comment
        })
      } else {
        focusedComments.value.unshift(data.comment)
      }
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
    deleteThoughtById,
    updateThoughtById,
    addThought,
    fetchUserThoughts,
    toggleReaction,
    fetchReactionsById,
    fetchReactions,
    fetchComments,
    deleteComment,
    editComment,
    addComment,
    modal,
  }
})
