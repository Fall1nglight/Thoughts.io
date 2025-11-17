<script setup>
import { useErrorStore } from '@/stores/error.js'
import { onMounted, watch } from 'vue'
import { storeToRefs } from 'pinia'

const errorStore = useErrorStore()
const { currentError, modal, hasError } = storeToRefs(errorStore)

onMounted(() => {
  if (window.bootstrap) {
    if (!modal.value) {
      const modalEl = document.getElementById('errorModal')
      modal.value = new window.bootstrap.Modal(modalEl)

      modalEl.addEventListener('hide.bs.modal', () => {
        if (document.activeElement) document.activeElement.blur()
      })

      modalEl.addEventListener('hidden.bs.modal', () => {
        errorStore.clearError()
      })
    }
  } else {
    console.error('Failed to load bootstrap.')
  }
})

watch(hasError, (newHasError) => {
  if (newHasError) modal.value.show()
})
</script>

<template>
  <!-- Modal -->
  <div class="modal fade" id="errorModal" tabindex="-1">
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header">
          <h1 class="modal-title fs-5" id="errorModalLabel">{{ currentError.type }}</h1>
          <button
            type="button"
            class="btn-close"
            data-bs-dismiss="modal"
            aria-label="Close"
          ></button>
        </div>
        <div class="modal-body">
          {{ currentError.message }}
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped></style>
