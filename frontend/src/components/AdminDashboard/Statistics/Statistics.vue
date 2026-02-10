<script setup>
import DistributionStats from '@/components/AdminDashboard/Statistics/DistributionStats.vue'
import ActivityStats from '@/components/AdminDashboard/Statistics/ActivityStats.vue'
import UserLeaderboard from '@/components/AdminDashboard/Statistics/UserLeaderboard.vue'
import ThoughtPopularity from '@/components/AdminDashboard/Statistics/ThoughtPopularity.vue'
import { onMounted } from 'vue'
import { storeToRefs } from 'pinia'
import { useAdminStats } from '@/stores/adminStats.js'

// Dependencies
const adminStats = useAdminStats()
const { selectedTab } = storeToRefs(adminStats)

// Local state
const tabs = {
  distribution: {
    displayName: 'Distribution',
    component: DistributionStats,
  },

  activity: {
    displayName: 'Activity',
    component: ActivityStats,
  },

  userLeaderboard: {
    displayName: 'User Leaderboard',
    component: UserLeaderboard,
  },

  thoughtLeaderboard: {
    displayName: 'Thought Popularity',
    component: ThoughtPopularity,
  },
}

// Hooks and watchers
onMounted(() => {
  if (!selectedTab.value) selectedTab.value = tabs.distribution.component
})
</script>

<template>
  <section class="statistics">
    <div class="row">
      <div class="col-12">
        <p class="lead">Statistics</p>
      </div>
    </div>

    <div class="row">
      <div class="col-6">
        <select v-model="selectedTab">
          <option v-for="tab in tabs" :key="tab.displayName" :value="tab.component">
            {{ tab.displayName }}
          </option>
        </select>
      </div>
    </div>

    <component :is="selectedTab" />
  </section>
</template>

<style scoped></style>
