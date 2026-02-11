<script setup>
import { computed, onMounted } from 'vue'
import { storeToRefs } from 'pinia'
import { Chart as ChartJS, ArcElement, Tooltip, Legend } from 'chart.js'
import { Pie } from 'vue-chartjs'
import { useAdminStats } from '@/stores/adminStats.js'
import { createPieChartData } from '@/utils/chart.factory.js'
import { colors, commonOptions } from '@/config/chart.options.js'

ChartJS.register(ArcElement, Tooltip, Legend)

// Dependencies
const adminStats = useAdminStats()
const { distribution } = storeToRefs(adminStats)

// Local state
const contentChartColors = [colors.green, colors.red]
const reactionChartColors = [colors.green, colors.red, colors.black]

// Derived state
const contentChartData = computed(() => {
  const labels = ['Thoughts', 'Comments']
  const values = [distribution.value.content.thoughtCount, distribution.value.content.commentCount]

  return createPieChartData(labels, contentChartColors, values)
})

const reactionChartData = computed(() => {
  const labels = distribution.value.reactions.map((x) => x.name)
  const values = distribution.value.reactions.map((x) => x.count)

  return createPieChartData(labels, reactionChartColors, values)
})

// Hooks and watchers
onMounted(async () => {
  try {
    await adminStats.fetchDistribution()
  } catch (error) {
    console.error(error)
  }
})
</script>

<template>
  <div class="row pt-5">
    <div class="col-12">
      <p class="lead">Content distribution</p>
    </div>

    <div class="col-12">
      <Pie :data="contentChartData" :options="commonOptions"></Pie>
    </div>
  </div>

  <div class="row pt-5">
    <div class="col-12">
      <p class="lead">Reaction distribution</p>
    </div>

    <div class="col-12">
      <Pie :data="reactionChartData" :options="commonOptions"></Pie>
    </div>
  </div>
</template>

<style scoped></style>
