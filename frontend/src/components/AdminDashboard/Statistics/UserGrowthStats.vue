<script setup>
import { computed, ref } from 'vue'
import { storeToRefs } from 'pinia'
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
import { useAdminStats } from '@/stores/adminStats.js'
import { faker } from '@faker-js/faker'
import { dateViewTypes } from '@/types/chartView.types.js'
import { formatISO } from 'date-fns'
import { useTimeSeriesChart } from '@/composables/timeSeriesChart.js'

ChartJS.register(LinearScale, PointElement, LineElement, Title, Tooltip, Legend, TimeScale, Filler)

// Dependencies
const adminStats = useAdminStats()
const { growth } = storeToRefs(adminStats)

// Local state
const defaultDayRange = 10
const currentDate = new Date()
const defaultPastDate = new Date(currentDate)
defaultPastDate.setDate(defaultPastDate.getDate() - defaultDayRange)
const dates = ref([defaultPastDate, currentDate])

const datasetOptions = {
  borderColor: '#41B883',
  backgroundColor: 'rgba(184,65,119,0.2)',
  borderWidth: 2,
  fill: true,
  tension: 0.3,
  pointRadius: 3,
  pointHitRadius: 20,
  pointHoverRadius: 4,
  pointHoverBackgroundColor: '#10B981',
  pointHoverBorderColor: '#ffffff',
  pointHoverBorderWidth: 2,
}

const { selectedViewMode, chartData, chartOptions } = useTimeSeriesChart(
  computed(() => growth.value.users),
  'Registrations',
  'New users',
  datasetOptions,
)

// Methods
function getPrevDays() {
  const startDate = dates.value[0].getTime()
  const endDate = dates.value[1].getTime()
  return Math.floor((endDate - startDate) / (24 * 60 * 60 * 1000))
}

async function fetchUserGrowthStats() {
  await adminStats.fetchUserGrowth(getPrevDays())
}

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

  growth.value.users = data
}
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
          :time-config="{ enableTimePicker: false }"
          :clearable="false"
        ></VueDatePicker>
      </div>

      <div class="buttons">
        <button type="button" @click="fetchUserGrowthStats">Load stats</button>
        <button type="button" @click="generate(365)">Generate random data</button>
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

  <div class="row">
    <div class="col-12">
      <Line v-if="growth.users.length" :data="chartData" :options="chartOptions"></Line>
    </div>
  </div>
</template>

<style scoped></style>
