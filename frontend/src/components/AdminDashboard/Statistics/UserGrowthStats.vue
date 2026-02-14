<script setup>
import TimeSeriesChart from '@/components/AdminDashboard/Statistics/TimeSeriesChart.vue'
import { useAdminStats } from '@/stores/adminStats.js'
import { storeToRefs } from 'pinia'
import { onBeforeMount } from 'vue'

// Dependencies
const adminStats = useAdminStats()
const { growth } = storeToRefs(adminStats)

// Local state
const defaultDayRange = 30
const datasetOptions = {
  borderColor: '#41B883',
  backgroundColor: 'rgba(184,65,119,0.2)',
  borderWidth: 2,
  fill: true,
  tension: 0.3,
  pointStyle: 'Circles',
  pointBackgroundColor: '#ffffff',
  pointRadius: 3,
  pointHitRadius: 20,
  pointHoverRadius: 4,
  pointHoverBackgroundColor: '#10B981',
  pointHoverBorderColor: '#ffffff',
  pointHoverBorderWidth: 2,
}

// Methods
async function fetchUserGrowth(range) {
  await adminStats.fetchUserGrowth(range)
}

function handleDateChange(dates) {
  growth.value.userDateRange = dates
}

function handleFaker(randomData) {
  growth.value.users = randomData
}

// Hooks and watchers
onBeforeMount(() => {
  const currentDate = new Date()

  if (!growth.value.userDateRange.length) {
    const defaultPastDate = new Date(currentDate)
    defaultPastDate.setDate(defaultPastDate.getDate() - defaultDayRange)
    growth.value.userDateRange = [defaultPastDate, currentDate]
  }

  growth.value.userDateRange[1] = currentDate
})
</script>

<template>
  <TimeSeriesChart
    :api-data="growth.users"
    :date-range="growth.userDateRange"
    y-axis-label="Registrations"
    tooltip-text="New users"
    :dataset-options="datasetOptions"
    @fetch-api-data="fetchUserGrowth"
    @date-change="handleDateChange"
    @faker="handleFaker"
  ></TimeSeriesChart>
</template>

<style scoped></style>
