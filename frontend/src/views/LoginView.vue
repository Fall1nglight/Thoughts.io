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
  password: '',
})

const handleSubmit = async () => {
  try {
    await schemas.auth.login.validateAsync(user.value)
    await authStore.login(user.value)

    if (!isLoggedIn.value) return

    user.value.email = ''
    user.value.password = ''

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
          <legend>Log in to your account</legend>

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

          <div class="mt-4 text-center">
            <button type="submit" class="btn btn-primary rounded">Submit</button>
          </div>

          <div class="mt-1 text-center">
            <small class="small">
              <RouterLink to="/reset-password" class="reset-link">Forgot your password?</RouterLink>
            </small>
          </div>

          <div class="mt-4 text-center">
            <small class="small">
              Don't have an account?
              <RouterLink to="/signup" class="signup-link">Sign up</RouterLink>
            </small>
          </div>
        </form>
      </div>
    </div>
  </section>
</template>

<style scoped>
small {
  color: #adb5bd;
}

.signup-link,
.reset-link {
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
