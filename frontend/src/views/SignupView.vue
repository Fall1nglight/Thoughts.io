<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { storeToRefs } from 'pinia'
import { useAuthStore } from '@/stores/auth.js'
import { useErrorStore } from '@/stores/error.js'
import schemas from '@/validation/index.js'
import errorTypes from '@/types/error.types.js'

const authStore = useAuthStore()
const errorStore = useErrorStore()
const { isLoggedIn } = storeToRefs(authStore)

const router = useRouter()

const user = ref({
  email: '',
  username: '',
  password: '',
  confirmPassword: '',
})

const handleSubmit = async () => {
  try {
    await schemas.auth.signup.validateAsync(user.value)
    await authStore.signup(user.value)

    if (!isLoggedIn.value) return

    user.value.email = ''
    user.value.username = ''
    user.value.password = ''
    user.value.confirmPassword = ''

    await router.push('/')
  } catch (error) {
    errorStore.addError(errorTypes.validationError, error)
  }
}
</script>

<template>
  <section class="login mt-5">
    <div class="row justify-content-center">
      <div class="col-sm-4 py-4">
        <form @submit.prevent="handleSubmit">
          <legend>Create an account</legend>

          <div>
            <label for="inputEmail" class="form-label">Email address</label>
            <input
              v-model="user.email"
              type="email"
              class="form-control"
              id="inputEmail"
              placeholder="Enter email"
            />
          </div>

          <div class="mt-4">
            <label for="inputUsername" class="form-label">Username</label>
            <input
              v-model="user.username"
              type="text"
              class="form-control"
              id="inputUsername"
              placeholder="Enter username"
            />
          </div>

          <div class="mt-4">
            <label for="inputPassword" class="form-label">Password</label>
            <input
              v-model="user.password"
              type="password"
              class="form-control"
              id="inputPassword"
              placeholder="Password"
              autocomplete="off"
            />
          </div>

          <div class="mt-4">
            <label for="inputConfirmPassword" class="form-label">Confirm password</label>
            <input
              v-model="user.confirmPassword"
              type="password"
              class="form-control"
              id="inputConfirmPassword"
              placeholder="Confirm password"
              autocomplete="off"
            />
          </div>

          <div class="mt-4 text-center">
            <small class="small">
              Already have an account?
              <RouterLink to="/login" class="login-link">Log in</RouterLink>
            </small>
          </div>

          <div class="mt-4 text-center">
            <button type="submit" class="btn btn-primary rounded">Submit</button>
          </div>
        </form>
      </div>
    </div>
  </section>
</template>

<style scoped>
small {
  color: #adb5bd !important;
}

.login-link {
  color: #469ae0;
}

legend {
  color: white;
  font-size: large;
  margin-bottom: 1.5rem;
}

label {
  color: #adb5bd !important;
}

.form-control {
  background-color: transparent !important;
  border: 2px solid rgba(173, 181, 189, 0.5) !important;
  border-radius: 0.25rem;
  padding: 0.35rem 0.5rem;
  color: white;
}

.col-sm-4 {
  background-color: #292c33 !important;
  padding: 0.5rem 1rem;
  border: 1px solid #469ae0;
  border-radius: 5px;
}

.btn-primary {
  background-color: #469ae0;
  min-width: 150px;
  padding: 0.3rem 1rem;
}
</style>
