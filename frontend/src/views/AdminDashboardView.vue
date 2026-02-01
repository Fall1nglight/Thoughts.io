<script setup>
import { shallowRef } from 'vue'
import ManageThoughts from '@/components/AdminDashboard/ManageThoughts.vue'
import ManageUsers from '@/components/AdminDashboard/ManageUsers.vue'
import Statistics from '@/components/AdminDashboard/Statistics.vue'

// Local state
const tabs = {
  thoughts: ManageThoughts,
  users: ManageUsers,
  statistics: Statistics,
}

const selectedTab = shallowRef(null)

// Methods
function selectTab(tab) {
  selectedTab.value = tab
}
</script>

<template>
  <section class="admin-dashboard p-5">
    <div class="row">
      <div class="col-12 text-center">
        <h1>Admin dashboard</h1>
      </div>

      <div class="col-md-5">
        <div class="dropdown">
          <a
            class="btn btn-secondary dropdown-toggle"
            href="#"
            role="button"
            data-bs-toggle="dropdown"
            aria-expanded="false"
          >
            Menu
          </a>

          <ul class="dropdown-menu">
            <li>
              <a class="dropdown-item" type="button" @click="selectTab(tabs.thoughts)">
                Manage Thoughts
              </a>
            </li>

            <li>
              <a class="dropdown-item" type="button" @click="selectTab(tabs.users)">
                Manage Users
              </a>
            </li>

            <li><hr class="dropdown-divider" /></li>

            <li>
              <a class="dropdown-item" type="button" @click="selectTab(tabs.statistics)">
                Open statistics
              </a>
            </li>
          </ul>
        </div>
      </div>
    </div>
  </section>

  <component v-if="selectedTab" :is="selectedTab"></component>
</template>

<style scoped>
* {
  color: white;
}

.dropdown-toggle,
.dropdown-item {
  color: black;
}
</style>
