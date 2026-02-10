export function createPieChart(labelValues, backgroundColorValues, dataValues) {
  return {
    labels: labelValues,
    datasets: [
      {
        backgroundColor: backgroundColorValues,
        data: dataValues,
      },
    ],
  }
}
