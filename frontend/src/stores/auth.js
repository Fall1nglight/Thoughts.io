import axios from 'axios'
import { jwtDecode } from 'jwt-decode'
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { defineStore } from 'pinia'
import { authUri, usersUri } from '@/config/api.config.js'
import { useErrorStore } from '@/stores/error.js'
import errorTypes from '@/types/error.types.js'
import claimTypes from '@/types/claim.types.js'
import roleTypes from '@/types/role.types.js'
export const useAuthStore = defineStore('auth', () => {
  // other stores
  const errorStore = useErrorStore()

  // router
  const router = useRouter()

  // axios
  const client = axios.create({ baseURL: authUri })
  const userClient = axios.create({ baseURL: usersUri })

  userClient.interceptors.request.use((config) => {
    const token = getAccessToken.value

    if (token) {
      config.headers.Authorization = 'Bearer ' + token
    }

    return config
  })

  // state
  const user = ref({
    id: '',
    email: '',
    username: '',
    createdAtUtc: 0,
    updatedAtUtc: 0,
    lastLoginAtUtc: 0,
    active: true,
    roles: [],
    iat: 0,
    exp: 0,
    nbf: 0,
  })

  const localTokensId = 'thoughts_io_tokens'
  const localTokens = JSON.parse(localStorage.getItem(localTokensId))
  const tokens = ref({
    accessToken: localTokens?.accessToken || '',
    refreshToken: localTokens?.refreshToken || '',
  })

  // getters
  const isLoggedIn = computed(() => !!user.value.id)
  const isAdmin = computed(() => user.value.roles.indexOf(roleTypes.admin) > 0)
  const getAccessToken = computed(() => tokens.value.accessToken)
  const getRefreshToken = computed(() => tokens.value.refreshToken)
  const getUserId = computed(() => user.value.id)

  // actions
  function initAuth() {
    if (!getAccessToken.value) return
    const payload = getUserDataFromJwt()

    if (new Date().getTime() >= payload.exp * 1000) {
      resetTokens()
      return
    }

    setUserData(payload)
  }

  function handleError(error) {
    errorStore.addError(errorTypes.apiError, error)
  }

  function setUserData(payload) {
    user.value.id = payload[claimTypes.id]
    user.value.username = payload[claimTypes.username]
    user.value.iat = payload.iat
    user.value.exp = payload.exp
    user.value.nbf = payload.nbf

    const roles = payload[claimTypes.roles]

    if (Array.isArray(roles)) {
      user.value.roles.push(...roles)
    } else {
      user.value.roles.push(roles)
    }
  }

  function setTokens(payload) {
    tokens.value.accessToken = payload.accessToken
    tokens.value.refreshToken = payload.refreshToken
    localStorage.setItem(localTokensId, JSON.stringify(payload))
  }

  function getUserDataFromJwt() {
    return jwtDecode(getAccessToken.value)
  }

  function resetTokens() {
    tokens.value.accessToken = ''
    tokens.value.refreshToken = ''
    localStorage.removeItem(localTokensId)
  }
  async function signup(user) {
    try {
      const { data } = await client.post('/signup', user)
      setTokens(data)

      const payload = getUserDataFromJwt()
      setUserData(payload)
    } catch (error) {
      error.message = 'Invalid login credentials! Please try again.'
      handleError(error)
    }
  }

  async function login(user) {
    try {
      const { data } = await client.post('/login', user)
      setTokens(data)

      const payload = getUserDataFromJwt()
      setUserData(payload)
    } catch (error) {
      error.message = 'Invalid login credentials! Please try again.'
      handleError(error)
    }
  }

  async function refreshToken() {
    try {
      const { data } = await client.post('/refresh-token', {
        token: getRefreshToken.value,
      })
      setTokens(data)

      const payload = getUserDataFromJwt()
      setUserData(payload)
    } catch (error) {
      error.message = 'Invalid refresh token!'
      handleError(error)
    }
  }

  async function updateUser(newSetting) {
    try {
      const updateResponse = await userClient.patch(`/${getUserId.value}`, newSetting)

      if (updateResponse.status !== 204) throw new Error('Failed to update user!')

      await refreshToken()
    } catch (error) {
      handleError(error)
    }
  }

  async function deleteUser() {
    try {
      const deleteResponse = await userClient.delete(`/${getUserId.value}`)

      if (deleteResponse.status !== 204) throw new Error('Failed to delete user!')

      await logout()
    } catch (error) {
      handleError(error)
    }
  }

  async function logout() {
    $reset()
    await router.push('/')
  }

  function $reset() {
    user.value.id = ''
    user.value.email = ''
    user.value.username = ''
    user.value.createdAtUtc = 0
    user.value.updatedAtUtc = 0
    user.value.lastLoginAtUtc = 0
    user.value.active = true
    user.value.roles = []
    user.value.iat = 0
    user.value.exp = 0
    user.value.nbf = 0

    resetTokens()
  }

  initAuth()

  return {
    user,
    tokens,
    getAccessToken,
    getRefreshToken,
    getUserId,
    isLoggedIn,
    isAdmin,
    signup,
    login,
    refreshToken,
    updateUser,
    deleteUser,
    logout,
    $reset,
  }
})
