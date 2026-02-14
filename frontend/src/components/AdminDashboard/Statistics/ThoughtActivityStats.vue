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
async function fetchThoughtActivity(range) {
  await adminStats.fetchThoughtActivity(range)
}

function handleDateChange(dates) {
  growth.value.thoughtDateRange = dates
}

function handleFaker(randomData) {
  growth.value.thoughts = randomData
}

// Hooks and watchers
onBeforeMount(() => {
  const currentDate = new Date()

  if (!growth.value.thoughtDateRange.length) {
    const defaultPastDate = new Date(currentDate)
    defaultPastDate.setDate(defaultPastDate.getDate() - defaultDayRange)
    growth.value.thoughtDateRange = [defaultPastDate, currentDate]
  }

  growth.value.thoughtDateRange[1] = currentDate
})
</script>

<template>
  <TimeSeriesChart
    :api-data="growth.thoughts"
    :date-range="growth.thoughtDateRange"
    y-axis-label="Posts"
    tooltip-text="New thoughts"
    :dataset-options="datasetOptions"
    @fetch-api-data="fetchThoughtActivity"
    @date-change="handleDateChange"
    @faker="handleFaker"
  ></TimeSeriesChart>
</template>

<style scoped></style>
