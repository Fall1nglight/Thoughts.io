<script setup>
import { useAuthStore } from '@/stores/auth.js'
import { ref } from 'vue'

// Dependencies
const authStore = useAuthStore()

// Local state
const newSetting = ref({
  username: '',
  confirmUsername: '',
})

// Methods
function resetForm() {
  newSetting.value.username = ''
  newSetting.value.confirmUsername = ''
}

async function handleSubmit() {
  try {
    const payload = { username: newSetting.value.username }
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
      <label for="changeUsernameInput" class="form-label">New username</label>
      <input
        type="text"
        class="form-control"
        id="changeUsernameInput"
        v-model="newSetting.username"
      />
    </div>

    <div class="mb-3">
      <label for="changeUsernameConfirmInput" class="form-label">Confirm new username</label>
      <input
        type="text"
        class="form-control"
        id="changeUsernameConfirmInput"
        v-model="newSetting.confirmUsername"
      />
    </div>

    <div>
      <button type="submit" class="btn btn-primary">Submit changes</button>
    </div>
  </form>
</template>

<style scoped></style>
