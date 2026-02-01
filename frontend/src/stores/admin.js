import { defineStore } from 'pinia'
import { ref } from 'vue'
import errorTypes from '@/types/error.types.js'
import { useErrorStore } from '@/stores/error.js'
import axios from 'axios'

const useAdminStore = defineStore('admin', () => {
  // other stores
  const errorStore = useErrorStore()

  // axios
  // const client = axios.create({baseURL:})

  // state
  const thoughts = ref([])

  // getters

  // functions
  function handleError(error) {
    errorStore.addError(errorTypes.apiError, error)
  }

  // actions
  async function fetchThoughts() {
    try {
    } catch (error) {
      handleError(error)
    }
  }

  return {
    thoughts,
    fetchThoughts,
  }
})
