<script setup>
import { storeToRefs } from 'pinia'
import { useAdminThoughtsStore } from '@/stores/adminThoughts.js'
import { ref } from 'vue'

// Dependencies
const adminThoughts = useAdminThoughtsStore()
const { searchMethods } = adminThoughts
const { searchParams } = storeToRefs(adminThoughts)

// Local state
const searchOption = ref(searchParams.value.method || searchMethods.all)
const searchQuery = ref(searchParams.value.query || '')

// Methods
async function handleSubmit() {
  try {
    searchParams.value.method = searchOption.value
    searchParams.value.query = searchQuery.value
    await adminThoughts.fetchThoughtsV2()
  } catch (error) {
    console.error(error)
  }
}
</script>

<template>
  <div class="row">
    <div class="col-12">
      <form @submit.prevent="handleSubmit">
        <!-- Select -->
        <div class="mb-3 col-6">
          <select class="form-select" v-model="searchOption">
            <option :value="searchMethods.all">Load all</option>
            <option :value="searchMethods.thoughtId">Load by thought_id</option>
            <option :value="searchMethods.thoughtTitle">Load by thought_title</option>
            <option :value="searchMethods.userId">Load by user_id</option>
            <option :value="searchMethods.username">Load by user_name</option>
          </select>
        </div>

        <!-- Input field -->
        <div class="mb-3 col-6" v-if="searchOption !== searchMethods.all">
          <label for="thoughtSearchInput" class="form-label">Value </label>

          <input type="text" class="form-control" id="thoughtSearchInput" v-model="searchQuery" />
        </div>

        <div>
          <button type="submit" class="btn btn-primary">Load</button>
        </div>
      </form>
    </div>
  </div>
</template>

<style scoped></style>
