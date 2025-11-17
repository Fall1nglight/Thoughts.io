import { defineStore } from 'pinia'
import { computed, ref } from 'vue'

export const useErrorStore = defineStore('error', () => {
  // state
  const currentError = ref({
    type: '',
    message: '',
    details: null,
  })

  const modal = ref(null)

  // getters
  const hasError = computed(() => !!currentError.value.message)

  // action
  function addError(type, err) {
    currentError.value.type = type || 'Unknown error type'
    currentError.value.message = err.message || 'Unknown error'
    currentError.value.details = err
  }

  function clearError() {
    currentError.value.type = ''
    currentError.value.message = ''
    currentError.value.details = null
  }

  return { currentError, modal, hasError, addError, clearError }
})
