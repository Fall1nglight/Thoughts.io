import { ref, shallowRef } from 'vue'
import { defineStore, storeToRefs } from 'pinia'
import axios from 'axios'
import { useErrorStore } from '@/stores/error.js'
import { useAuthStore } from '@/stores/auth.js'
import { statsUri } from '@/config/api.config.js'
import errorTypes from '@/types/error.types.js'
import { statSortTypes } from '@/types/sort.types.js'

export const useAdminStats = defineStore('adminStats', () => {
  // other stores
  const errorStore = useErrorStore()
  const authStore = useAuthStore()
  const { getAccessToken } = storeToRefs(authStore)

  // axios
  const client = axios.create({ baseURL: statsUri })

  client.interceptors.request.use((config) => {
    const token = getAccessToken.value

    if (token) {
      config.headers.Authorization = 'Bearer ' + token
    }

    return config
  })

  // state
  const distribution = ref({
    content: {
      thoughtCount: 0,
      commentCount: 0,
    },

    reactions: [],
  })

  const growth = ref({
    users: [],
    userDateRange: [],
    thoughts: [],
    thoughtDateRange: [],
  })

  const rankings = ref({
    users: [],
    userQuery: {
      sortBy: statSortTypes.sortByOptions[0],
      limit: statSortTypes.limitOptions[2],
    },

    thoughts: [],
    thoughtQuery: {
      sortBy: statSortTypes.sortByOptions[0],
      limit: statSortTypes.limitOptions[2],
    },
  })
  const selectedTab = shallowRef(null)

  // getters

  // helpers
  function handleError(error) {
    errorStore.addError(errorTypes.apiError, error)
  }

  // actions
  async function fetchContentDistribution() {
    try {
      const { data } = await client.get('/content/breakdown')
      distribution.value.content.thoughtCount = data.thoughtCount
      distribution.value.content.commentCount = data.commentCount
    } catch (error) {
      handleError(error)
    }
  }

  async function fetchReactionDistribution() {
    try {
      const { data } = await client.get('/reactions/breakdown')
      distribution.value.reactions = data.reactions
    } catch (error) {
      handleError(error)
    }
  }

  async function fetchDistribution() {
    await fetchContentDistribution()
    await fetchReactionDistribution()
  }

  async function fetchUserGrowth(days = 10) {
    try {
      const { data } = await client.get(`/users/growth?days=${days}`)
      growth.value.users = data.registrations
    } catch (error) {
      handleError(error)
    }
  }

  async function fetchThoughtActivity(days = 30) {
    try {
      const { data } = await client.get(`/thoughts/activity?days=${days}`)
      growth.value.thoughts = data.creations
    } catch (error) {
      handleError(error)
    }
  }

  async function fetchUserLeaderboard(limit = 10) {
    try {
      const { data } = await client.get(`/users/leaderboard?limit=${limit}`)
      rankings.value.users = data.users
    } catch (error) {
      handleError(error)
    }
  }

  async function fetchMostPopularThoughts(sortBy = 'Total', limit = 10) {
    try {
      const { data } = await client.get(`/thoughts/popular?sortBy=${sortBy}&limit=${limit}`)
      rankings.value.thoughts = data.thoughts
    } catch (error) {
      handleError(error)
    }
  }

  return {
    distribution,
    growth,
    rankings,
    selectedTab,
    fetchContentDistribution,
    fetchReactionDistribution,
    fetchDistribution,
    fetchUserGrowth,
    fetchThoughtActivity,
    fetchUserLeaderboard,
    fetchMostPopularThoughts,
  }
})
