<script setup>
import { computed, watchEffect } from 'vue'
import { VueDatePicker } from '@vuepic/vue-datepicker'
import { Line } from 'vue-chartjs'
import {
  Chart as ChartJS,
  TimeScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
  Filler,
} from 'chart.js'
import { faker } from '@faker-js/faker'
import { dateViewTypes } from '@/types/chartView.types.js'
import { formatISO } from 'date-fns'
import { useTimeSeriesChart } from '@/composables/timeSeriesChart.js'
import '@vuepic/vue-datepicker/dist/main.css'

ChartJS.register(LinearScale, PointElement, LineElement, Title, Tooltip, Legend, TimeScale, Filler)

// Props and emits
const { apiData, yAxisLabel, tooltipText, dateRange, datasetOptions } = defineProps({
  apiData: Array,
  yAxisLabel: String,
  tooltipText: String,
  dateRange: Array,
  datasetOptions: Object,
})

const emits = defineEmits(['fetchApiData', 'dateChange', 'faker'])

// Derived state
const dates = computed({
  get() {
    return dateRange
  },

  set(newValue) {
    emits('dateChange', newValue)
  },
})

const rangeBetweenDates = computed(() => {
  const startDate = dates.value[0].getTime()
  const endDate = dates.value[1].getTime()
  return Math.floor((endDate - startDate) / (24 * 60 * 60 * 1000))
})

// Composables
const { selectedViewMode, chartData, chartOptions } = useTimeSeriesChart(
  computed(() => apiData),
  yAxisLabel,
  tooltipText,
  datasetOptions,
)

// todo | remove this when not in dev stage
function generate(days) {
  const data = []
  const today = new Date()

  // A loop visszafelé számol (pl. 30-tól 0-ig),
  // hogy a dátumok a múltból a jelen felé haladjanak (növekvő sorrend).
  for (let i = days - 1; i >= 0; i--) {
    // Dátum kiszámítása: Mai nap - i nap
    const date = new Date(today)
    date.setDate(today.getDate() - i)

    // (Opcionális) Idő nullázása, ha csak a nap számít
    date.setHours(0, 0, 0, 0)

    data.push({
      // ISO formátum a backend kompatibilitás miatt
      dateUtc: formatISO(date).split('+')[0],

      // Faker használata a véletlenszámhoz (pl. 0 és 150 között)
      count: faker.number.int({ min: 0, max: 50 }),
    })
  }

  emits('faker', data)
}

// Hooks and watchers
watchEffect(() => emits('fetchApiData', rangeBetweenDates.value))
</script>

<template>
  <div class="row justify-content-between pt-5">
    <!-- Date picker -->
    <div class="col-6">
      <p class="lead">Select date</p>

      <div class="date-picker">
        <VueDatePicker
          v-model="dates"
          :range="{
            fixedEnd: true,
            // todo | delete this later
            maxRange: 365,
          }"
          :clearable="false"
        />
      </div>

      <div class="buttons">
        <button type="button" @click="generate(91)">Generate random data</button>
      </div>
    </div>

    <div class="col-6 align-self-end text-end">
      <p class="lead">View</p>
      <select v-model="selectedViewMode">
        <option v-for="view in dateViewTypes" :key="view.value" :value="view.value">
          {{ view.displayName }}
        </option>
      </select>
    </div>
  </div>

  <div class="row pt-5">
    <div class="col-12">
      <Line v-if="apiData.length" :data="chartData" :options="chartOptions"></Line>

      <p v-else class="lead">No data could be found.</p>
    </div>
  </div>
</template>

<style>
.dp--clear-btn {
  display: none !important;
}
</style>
