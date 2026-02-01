<script setup>
import { shallowRef } from 'vue'
import ChangeUsernameForm from '@/components/UserSettings/ChangeUsernameForm.vue'
import ChangePasswordForm from '@/components/UserSettings/ChangePasswordForm.vue'
import ChangeEmailForm from '@/components/UserSettings/ChangeEmailForm.vue'
import DeleteUserForm from '@/components/UserSettings/DeleteUserForm.vue'

// Local state
const forms = {
  username: ChangeUsernameForm,
  password: ChangePasswordForm,
  email: ChangeEmailForm,
  delete: DeleteUserForm,
}

const selectedForm = shallowRef(forms.username)

// Methods
function toggleForm(form) {
  selectedForm.value = form
}

function isActiveForm(form) {
  return form === selectedForm.value
}
</script>

<template>
  <div class="row p-5">
    <h1>Settings</h1>
    <div class="col">
      <button
        @click="toggleForm(forms.username)"
        type="button"
        class="btn btn-secondary"
        :class="{ active: isActiveForm(forms.username) }"
      >
        Change username
      </button>

      <button
        @click="toggleForm(forms.email)"
        type="button"
        class="btn btn-secondary"
        :class="{ active: isActiveForm(forms.email) }"
      >
        Change email
      </button>

      <button
        @click="toggleForm(forms.password)"
        type="button"
        class="btn btn-secondary"
        :class="{ active: isActiveForm(forms.password) }"
      >
        Change password
      </button>

      <button
        @click="toggleForm(forms.delete)"
        type="button"
        class="btn btn-secondary"
        :class="{ active: isActiveForm(forms.delete) }"
      >
        Delete account
      </button>
    </div>
  </div>

  <div class="row p-5">
    <div class="col-md-5">
      <component :is="selectedForm"></component>
    </div>
  </div>
</template>

<style scoped>
.btn-secondary {
  color: black;
}

h1 {
  color: white;
}

* {
  color: white;
}
</style>
