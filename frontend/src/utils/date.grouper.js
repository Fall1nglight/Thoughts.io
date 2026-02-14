import { startOfWeek, startOfMonth, startOfYear } from 'date-fns'
import { dateViewTypes } from '@/types/chartView.types.js'

export function groupDates(apiData, viewMode) {
  if (viewMode === dateViewTypes.days.value)
    return apiData.map((item) => ({
      date: new Date(item.dateUtc),
      count: item.count,
    }))

  const groups = apiData.reduce((acc, dataPoint) => {
    const dataPointDate = new Date(dataPoint.dateUtc)
    const key = getGroupKeyByViewMode(dataPointDate, viewMode)

    if (!acc[key]) acc[key] = 0
    acc[key] += dataPoint.count

    return acc
  }, {})

  return Object.entries(groups).map(([timestamp, count]) => ({
    date: new Date(Number(timestamp)),
    count,
  }))
}

// creates a key to group by with based on the viewMode
function getGroupKeyByViewMode(date, viewMode) {
  switch (viewMode) {
    case dateViewTypes.weeks.value:
      return startOfWeek(date, { weekStartsOn: 1 }).getTime()

    case dateViewTypes.months.value:
      return startOfMonth(date).getTime()

    case dateViewTypes.years.value:
      return startOfYear(date).getTime()

    default:
      return date.getTime()
  }
}

export function determineDefaultView(dataLength) {
  if (dataLength > 365 * 2) return dateViewTypes.years.value
  if (dataLength > 180) return dateViewTypes.months.value
  if (dataLength > 60) return dateViewTypes.weeks.value
  return dateViewTypes.days.value
}
