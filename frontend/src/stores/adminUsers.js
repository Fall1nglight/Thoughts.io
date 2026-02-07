import { defineStore, storeToRefs } from 'pinia'
import { useErrorStore } from '@/stores/error.js'
import errorTypes from '@/types/error.types.js'
import axios from 'axios'
import { baseUri } from '@/config/api.config.js'
import { computed, ref } from 'vue'
import { useAuthStore } from '@/stores/auth.js'

export const useAdminUserStore = defineStore('adminUser', () => {
  // other stores
  const errorStore = useErrorStore()
  const authStore = useAuthStore()
  const { getAccessToken } = storeToRefs(authStore)

  // axios
  const client = axios.create({
    baseURL: baseUri,
  })

  client.interceptors.request.use((config) => {
    const token = getAccessToken.value

    if (token) {
      config.headers.Authorization = 'Bearer ' + token
    }

    return config
  })

  // const
  const searchMethods = {
    username: 'username',
    userId: 'userId',
    all: 'all',
  }

  // state
  const users = ref([])
  const searchParams = ref({
    method: '',
    query: '',
  })

  // getters
  const getUsersBySearch = computed(() => {
    const method = searchParams.value.method
    const query = searchParams.value.query

    if (method === searchMethods.username) return users.value.filter((x) => x.username === query)
    if (method === searchMethods.userId) return users.value.filter((x) => x.id === query)

    return users.value
  })

  // functions
  function handleError(error) {
    errorStore.addError(errorTypes.apiError, error)
  }

  // actions
  async function fetch() {
    try {
      const method = searchParams.value.method
      const query = searchParams.value.query

      const newUsers = []

      switch (method) {
        case searchMethods.username: {
          const result = await fetchUserByName(query)
          newUsers.push(result)
          break
        }

        case searchMethods.userId: {
          const result = await fetchUserById(query)
          newUsers.push(result)
          break
        }

        default: {
          const result = await fetchUsers()
          newUsers.push(...result)
        }
      }

      const usersMap = new Map(users.value.map((x) => [x.id, x]))
      newUsers.forEach((x) => usersMap.set(x.id, x))
      users.value = Array.from(usersMap.values())
    } catch (error) {
      handleError(error)
    }
  }

  async function fetchUsers() {
    const { data } = await client.get('/admin/users')
    return data.users
  }

  async function fetchUserById(id) {
    const { data } = await client.get(`/users/${id}`)
    return data.user
  }

  async function fetchUserByName(username) {
    const { data } = await client.get(`/admin/users/username/${username}`)
    return data.user
  }

  async function deleteUser(id) {
    try {
      const deleteResponse = await client.delete(`/admin/users/${id}`)

      if (deleteResponse.status !== 204)
        throw new Error('Failed to delete user. Please try again later.')

      users.value = users.value.filter((x) => x.id !== id)
    } catch (error) {
      handleError(error)
    }
  }

  return {
    users,
    getUsersBySearch,
    searchParams,
    searchMethods,
    fetch,
    deleteUser,
  }
})
