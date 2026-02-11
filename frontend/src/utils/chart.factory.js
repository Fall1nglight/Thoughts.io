export function createPieChartData(label, backgroundColor, data) {
  return {
    labels: label,
    datasets: [
      {
        backgroundColor: backgroundColor,
        data: data,
      },
    ],
  }
}

export function createTimeSeriesChartData(yAxisLabel, points, datasetOptions) {
  return {
    datasets: [
      {
        label: yAxisLabel,
        data: points,
        ...datasetOptions,
      },
    ],
  }
}
