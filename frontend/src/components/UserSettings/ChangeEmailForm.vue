<script setup>
import { useAuthStore } from '@/stores/auth.js'
import { ref } from 'vue'

// Dependencies
const authStore = useAuthStore()

// Local state
const newSetting = ref({
  email: '',
  confirmEmail: '',
})

// Methods
function resetForm() {
  newSetting.value.email = ''
  newSetting.value.confirmEmail = ''
}

async function handleSubmit() {
  try {
    const payload = { email: newSetting.value.email }
    await authStore.updateUser(payload)
    resetForm()
  } catch (error) {
    console.error(error)
  }
}
</script>

<template>
  <form @submit.prevent="handleSubmit">
    <div class="mb-3">
      <label for="changeEmailInput" class="form-label">New email</label>
      <input type="email" class="form-control" id="changeEmailInput" v-model="newSetting.email" />
    </div>

    <div class="mb-3">
      <label for="changeEmailConfirmInput" class="form-label">Confirm new email</label>
      <input
        type="email"
        class="form-control"
        id="changeEmailConfirmInput"
        v-model="newSetting.confirmEmail"
      />
    </div>

    <div>
      <button type="submit" class="btn btn-primary">Submit changes</button>
    </div>
  </form>
</template>

<style scoped></style>
