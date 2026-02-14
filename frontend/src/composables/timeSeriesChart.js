import { computed, ref } from 'vue'
import { dateViewTypes } from '@/types/chartView.types.js'
import { groupDates, determineDefaultView } from '@/utils/date.grouper.js'
import { createTimeSeriesChartData } from '@/utils/chart.factory.js'
import { timeScaleOptions } from '@/config/chart.options.js'

export function useTimeSeriesChart(apiData, yAxisLabel, tooltipText, datesetOptions) {
  const selectedViewMode = ref(dateViewTypes.auto.value)

  function getTooltipFormatByViewMode(viewMode) {
    const formats = {
      [dateViewTypes.days.value]: 'yyyy. MM. dd',
      [dateViewTypes.weeks.value]: 'yyyy. MM. dd',
      [dateViewTypes.months.value]: 'yyyy. MM',
      [dateViewTypes.years.value]: 'yyyy',
    }

    return formats[viewMode] || 'yyyy. MM. dd'
  }

  // if viewMode is set to 'auto', it calculates the optimal viewMode by determineDefaultView()
  const calculatedViewMode = computed(() => {
    return selectedViewMode.value === dateViewTypes.auto.value
      ? determineDefaultView(apiData.value.length)
      : selectedViewMode.value
  })

  const chartData = computed(() => {
    // returns {x|y} values from dates grouped by the viewMode
    const points = groupDates(apiData.value, calculatedViewMode.value).map((group) => ({
      x: group.date,
      y: group.count,
    }))

    const updatedOptions = { ...datesetOptions }
    if (points.length > 90) updatedOptions.pointRadius = 0

    return createTimeSeriesChartData(yAxisLabel, points, updatedOptions)
  })

  const chartOptions = computed(() => {
    const newOptions = { ...timeScaleOptions }

    newOptions.scales.x.time.tooltipFormat = getTooltipFormatByViewMode(calculatedViewMode.value)
    newOptions.scales.y.title.text = tooltipText

    return newOptions
  })

  return {
    selectedViewMode,
    chartData,
    chartOptions,
  }
}
