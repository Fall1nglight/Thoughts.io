<script setup>
import { shallowRef } from 'vue'
import ManageThoughts from '@/components/AdminDashboard/Thoughts/ManageThoughts.vue'
import ManageUsers from '@/components/AdminDashboard/Users/ManageUsers.vue'
import Statistics from '@/components/AdminDashboard/Statistics/Statistics.vue'

// Local state
const tabs = {
  thoughts: ManageThoughts,
  users: ManageUsers,
  statistics: Statistics,
}

const selectedTab = shallowRef(tabs.thoughts)

// Methods
function selectTab(tab) {
  selectedTab.value = tab
}

function isActiveTab(tab) {
  return selectedTab.value === tab
}
</script>

<template>
  <section class="admin-dashboard p-5">
    <div class="row">
      <div class="col-12 text-center">
        <h1>Admin dashboard</h1>
      </div>

      <div class="col-md-10">
        <button
          @click="selectTab(tabs.thoughts)"
          type="button"
          class="btn btn-secondary"
          :class="{ active: isActiveTab(tabs.thoughts) }"
        >
          Manage thoughts
        </button>

        <button
          @click="selectTab(tabs.users)"
          type="button"
          class="btn btn-secondary"
          :class="{ active: isActiveTab(tabs.users) }"
        >
          Manage users
        </button>

        <button
          @click="selectTab(tabs.statistics)"
          type="button"
          class="btn btn-secondary"
          :class="{ active: isActiveTab(tabs.statistics) }"
        >
          View statistics
        </button>
      </div>
    </div>
  </section>

  <component v-if="selectedTab" :is="selectedTab"></component>
</template>

<style scoped>
h1 {
  color: white;
}
</style>
