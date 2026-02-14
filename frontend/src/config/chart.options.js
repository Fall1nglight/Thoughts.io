import 'chartjs-adapter-date-fns'
import { enUS } from 'date-fns/locale'

export const commonOptions = {
  responsive: true,
  maintainAspectRatio: false,
}

export const timeScaleOptions = {
  responsive: true,
  maintainAspectRatio: false,
  scales: {
    x: {
      type: 'time',
      time: {
        tooltipFormat: 'yyyy. MM',
        displayFormats: {
          day: 'MMM d.',
          month: 'yyyy MMM',
          year: 'yyyy',
        },
        minUnit: 'day',
      },

      adapters: {
        date: {
          locale: enUS,
        },
      },

      ticks: {
        autoSkip: true,
        maxRotation: 0,
        source: 'auto',
      },
    },

    y: {
      beginAtZero: true,
      title: {
        display: true,
        text: 'Registered users',
      },
    },
  },

  plugins: {
    legend: {
      display: false,
    },

    tooltip: {
      intersect: false,
      mode: 'index',
    },
  },
}

export const colors = {
  red: '#ff0000',
  green: '#00ff00',
  blue: '#0000ff',
  black: '#000000',
}
