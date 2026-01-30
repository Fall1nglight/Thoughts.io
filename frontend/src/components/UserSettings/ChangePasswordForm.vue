<script setup>
import { useAuthStore } from '@/stores/auth.js'
import { ref } from 'vue'

// Dependencies
const authStore = useAuthStore()

// Local state
const newSetting = ref({
  password: '',
  confirmPassword: '',
})

// Methods
function resetForm() {
  newSetting.value.password = ''
  newSetting.value.confirmPassword = ''
}

async function handleSubmit() {
  try {
    const payload = { password: newSetting.value.password }
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
      <label for="changePasswordInput" class="form-label">New password</label>
      <input
        type="password"
        class="form-control"
        id="changePasswordInput"
        v-model="newSetting.password"
      />
    </div>

    <div class="mb-3">
      <label for="changePasswordConfirmInput" class="form-label">Confirm new password</label>
      <input
        type="password"
        class="form-control"
        id="changePasswordConfirmInput"
        v-model="newSetting.confirmPassword"
      />
    </div>

    <div>
      <button type="submit" class="btn btn-primary">Submit changes</button>
    </div>
  </form>
</template>

<style scoped></style>
